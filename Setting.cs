using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;

namespace CityTreasury
{
    /// <summary>
    /// Options page for City Treasury. Everything the mod does is controlled
    /// from here, so v1 needs no custom UI panel.
    ///
    /// The two "trigger" switches (AddCashNow, SetCashNow) act as buttons:
    /// flip one on in the options menu, the TreasurySystem applies it on its
    /// next tick and flips it back off.
    /// </summary>
    [FileLocation(nameof(CityTreasury))]
    [SettingsUIGroupOrder(kSafetyGroup, kCashGroup, kAutopilotGroup)]
    [SettingsUIShowTemplate]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kSafetyGroup = "Safety Net";
        public const string kCashGroup = "Cash Control";
        public const string kAutopilotGroup = "Budget Autopilot";

        public Setting(IMod mod) : base(mod)
        {
            // Defaults: safety net ON with a modest floor, everything else off.
            SafetyNetEnabled = true;
            FloorAmount = 100000;
            CashAmount = 100000;
            TargetBalance = 1000000;
            AutopilotEnabled = false;
            AutopilotAggressiveness = 1;
        }

        // ---------------- Safety net ----------------

        [SettingsUISection(kSection, kSafetyGroup)]
        public bool SafetyNetEnabled { get; set; }

        [SettingsUISection(kSection, kSafetyGroup)]
        [SettingsUISlider(min = 0, max = 10000000, step = 10000, scalarMultiplier = 1, unit = Unit.kMoney)]
        public int FloorAmount { get; set; }

        // ---------------- Cash control ----------------

        [SettingsUISection(kSection, kCashGroup)]
        [SettingsUISlider(min = 1000, max = 100000000, step = 1000, scalarMultiplier = 1, unit = Unit.kMoney)]
        public int CashAmount { get; set; }

        [SettingsUISection(kSection, kCashGroup)]
        public bool AddCashNow { get; set; }

        [SettingsUISection(kSection, kCashGroup)]
        [SettingsUISlider(min = 0, max = 100000000, step = 10000, scalarMultiplier = 1, unit = Unit.kMoney)]
        public int TargetBalance { get; set; }

        [SettingsUISection(kSection, kCashGroup)]
        public bool SetCashNow { get; set; }

        // ---------------- Budget autopilot ----------------

        [SettingsUISection(kSection, kAutopilotGroup)]
        public bool AutopilotEnabled { get; set; }

        /// <summary>Percent per adjustment step. Kept small on purpose.</summary>
        [SettingsUISection(kSection, kAutopilotGroup)]
        [SettingsUISlider(min = 1, max = 3, step = 1, scalarMultiplier = 1, unit = Unit.kPercentageSingleFraction)]
        public int AutopilotAggressiveness { get; set; }

        public override void SetDefaults()
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>English labels for the options page.</summary>
    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "City Treasury" },
                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "City Treasury" },

                { m_Setting.GetOptionGroupLocaleID(Setting.kSafetyGroup), "Safety Net" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCashGroup), "Cash Control" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kAutopilotGroup), "Budget Autopilot" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SafetyNetEnabled)), "Safety net on/off" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SafetyNetEnabled)),
                    "When the treasury drops below the floor, the mod tops it back up to the floor automatically." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.FloorAmount)), "Treasury floor" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.FloorAmount)),
                    "The minimum balance the safety net maintains." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.CashAmount)), "Cash amount" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.CashAmount)),
                    "How much the Add Cash switch adds." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AddCashNow)), "Add cash now" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AddCashNow)),
                    "Flip this on to add the cash amount once. It turns itself back off." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TargetBalance)), "Target balance" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TargetBalance)),
                    "The exact balance the Set Balance switch applies." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SetCashNow)), "Set balance now" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SetCashNow)),
                    "Flip this on to set the treasury to the target balance once. It turns itself back off." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AutopilotEnabled)), "Budget autopilot on/off" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AutopilotEnabled)),
                    "Watches the monthly trend. Sustained deficits nudge taxes up a step, sustained surpluses nudge them down. Small steps, with caps." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AutopilotAggressiveness)), "Autopilot step size (percent)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AutopilotAggressiveness)),
                    "How many percent taxes move per adjustment. 1 is gentle, 3 is assertive." },
            };
        }

        public void Unload()
        {
        }
    }
}
