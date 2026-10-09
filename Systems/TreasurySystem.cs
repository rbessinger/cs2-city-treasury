using Game;
using Game.City;
using Unity.Entities;

namespace CityTreasury.Systems
{
    /// <summary>
    /// Core money logic. Runs inside GameSimulation, throttled to a few real
    /// seconds per tick so it never taxes performance.
    ///
    /// The city treasury lives in the PlayerMoney component on the city
    /// entity. VERIFY IN VM (see CLAUDE.md): confirm the PlayerMoney field
    /// name (expected: money) and CitySystem.City against the installed
    /// Game.dll before first real use.
    ///
    /// Monthly sampling for the autopilot uses the simulation frame counter:
    /// CS2 advances one in game day per 262144 frames at 1x in the default
    /// time model this mod was scaffolded against. VERIFY IN VM: autopilot
    /// also works if sampling is approximate, it only changes how often the
    /// trend is evaluated. The adapter binding for tax rates lives in
    /// TaxAdapter.cs.
    /// </summary>
    public partial class TreasurySystem : GameSystemBase
    {
        private CitySystem m_CitySystem;
        private Setting m_Setting;
        private float m_TickTimer;
        private int m_LastSampledMonth = -1;
        private int m_PreviousMonthBalance;
        private int m_ConsecutiveDeficits;
        private int m_ConsecutiveSurpluses;
        private bool m_HasPreviousSample;

        private const float kTickSeconds = 2f;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_CitySystem = World.GetOrCreateSystemManaged<CitySystem>();
        }

        protected override void OnUpdate()
        {
            m_TickTimer += SystemAPI.Time.DeltaTime;
            if (m_TickTimer < kTickSeconds)
                return;
            m_TickTimer = 0f;

            if (m_Setting == null)
            {
                // The mod registers its Setting in OnLoad (Mod.ActiveSetting).
                m_Setting = Mod.ActiveSetting;
                if (m_Setting == null)
                    return;
            }

            Entity city = m_CitySystem.City;
            if (city == Entity.Null)
                return;
            if (!EntityManager.TryGetComponent(city, out PlayerMoney playerMoney))
                return;

            bool dirty = false;
            int balance = playerMoney.money;

            // ---- Cash control triggers (one shot) ----
            if (m_Setting.AddCashNow)
            {
                balance += m_Setting.CashAmount;
                m_Setting.AddCashNow = false;
                dirty = true;
                Mod.log.Info($"City Treasury: added {m_Setting.CashAmount}, balance now {balance}");
            }

            if (m_Setting.SetCashNow)
            {
                balance = m_Setting.TargetBalance;
                m_Setting.SetCashNow = false;
                dirty = true;
                Mod.log.Info($"City Treasury: balance set to {balance}");
            }

            // ---- Safety net ----
            if (m_Setting.SafetyNetEnabled && balance < m_Setting.FloorAmount)
            {
                Mod.log.Info($"City Treasury: safety net top up from {balance} to {m_Setting.FloorAmount}");
                balance = m_Setting.FloorAmount;
                dirty = true;
            }

            if (dirty)
            {
                playerMoney.money = balance;
                EntityManager.SetComponentData(city, playerMoney);
                m_Setting.ApplyAndSave();
            }

            // ---- Budget autopilot (monthly trend) ----
            if (m_Setting.AutopilotEnabled)
                EvaluateAutopilot(balance);
        }

        /// <summary>
        /// Samples the balance once per in game month. After 2 consecutive
        /// deficit months, taxes step up; after 3 consecutive surplus months
        /// above the safety floor, taxes step down. Steps are small and the
        /// adapter enforces hard caps, so this can never spiral.
        /// </summary>
        private void EvaluateAutopilot(int balance)
        {
            int month = CurrentGameMonth();
            if (month == m_LastSampledMonth)
                return;
            m_LastSampledMonth = month;

            if (!m_HasPreviousSample)
            {
                m_PreviousMonthBalance = balance;
                m_HasPreviousSample = true;
                return;
            }

            int delta = balance - m_PreviousMonthBalance;
            m_PreviousMonthBalance = balance;

            if (delta < 0)
            {
                m_ConsecutiveDeficits++;
                m_ConsecutiveSurpluses = 0;
            }
            else if (delta > 0)
            {
                m_ConsecutiveSurpluses++;
                m_ConsecutiveDeficits = 0;
            }

            if (m_ConsecutiveDeficits >= 2)
            {
                TaxAdapter.AdjustTaxes(+m_Setting.AutopilotAggressiveness);
                m_ConsecutiveDeficits = 0;
            }
            else if (m_ConsecutiveSurpluses >= 3 && balance > m_Setting.FloorAmount * 2)
            {
                TaxAdapter.AdjustTaxes(-m_Setting.AutopilotAggressiveness);
                m_ConsecutiveSurpluses = 0;
            }
        }

        /// <summary>
        /// Approximate current in game month counter. Based on the simulation
        /// frame count; only used to detect month boundaries, so small error
        /// is harmless. VERIFY IN VM if the time model constants differ.
        /// </summary>
        private int CurrentGameMonth()
        {
            // One in game day is 262144 simulation frames in the default model,
            // months are 30 days in the treasury trend sense used here.
            long frame = World.GetOrCreateSystemManaged<Game.Simulation.SimulationSystem>().frameIndex;
            long day = frame / 262144L;
            return (int)(day / 30L);
        }
    }
}
