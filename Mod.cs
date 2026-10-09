using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using CityTreasury.Systems;

namespace CityTreasury
{
    /// <summary>
    /// Entry point for the City Treasury mod.
    /// Three features, all driven from the Options page (see Setting.cs):
    ///   1. Safety net: auto top up when the treasury falls below a floor.
    ///   2. Cash control: add a chosen amount, or set the balance exactly.
    ///   3. Budget autopilot: rule based tax nudges when the city runs
    ///      sustained deficits or surpluses (binding lives in TaxAdapter.cs).
    /// </summary>
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(CityTreasury)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        public Setting m_Setting;

        /// <summary>Set in OnLoad so systems can reach the live settings.</summary>
        public static Setting ActiveSetting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            m_Setting = new Setting(this);
            ActiveSetting = m_Setting;
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));
            AssetManager.instance.LoadLocalSettingsFile(m_Setting);

            updateSystem.UpdateAt<TreasurySystem>(SystemUpdatePhase.GameSimulation);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
                ActiveSetting = null;
            }
        }
    }
}
