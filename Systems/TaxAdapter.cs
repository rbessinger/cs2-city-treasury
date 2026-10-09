namespace CityTreasury.Systems
{
    /// <summary>
    /// ISOLATED GAME BINDING: the only file that touches tax rates.
    ///
    /// Why this is a stub: CS2 has no official modding API docs, and the tax
    /// rate storage (Game.Economy.TaxSystem and its per resource rate array)
    /// is the one surface in this mod that must be confirmed against the
    /// actually installed Game.dll. Everything else in the mod (treasury
    /// reads and writes, settings, scheduling) is standard template API.
    ///
    /// JOB FOR CLAUDE CODE IN THE VM (see CLAUDE.md, stage 3):
    ///   1. Decompile or Object Browse Game.dll, find TaxSystem.
    ///   2. Implement GetAverageTaxRate and ApplyStep below against the real
    ///      members, keeping the safety contract:
    ///        - never move any single rate by more than the step percent
    ///        - clamp every rate into [kMinRatePercent, kMaxRatePercent]
    ///        - residential, commercial, industrial move together for v1
    ///   3. Until bound, AdjustTaxes must log and do nothing (current state),
    ///      so the rest of the mod is safe to use with autopilot off.
    /// </summary>
    public static class TaxAdapter
    {
        public const int kMinRatePercent = 0;
        public const int kMaxRatePercent = 30;

        private static bool s_Bound = false;

        /// <summary>
        /// Move all main tax rates by stepPercent (positive raises taxes).
        /// Returns true if rates actually changed.
        /// </summary>
        public static bool AdjustTaxes(int stepPercent)
        {
            if (!s_Bound)
            {
                Mod.log.Info(
                    $"City Treasury: autopilot wanted a {(stepPercent > 0 ? "+" : "")}{stepPercent}% tax step, " +
                    "but the tax binding is not implemented yet. No change made.");
                return false;
            }

            // TODO (stage 3, in VM): read current rates, apply stepPercent
            // with clamping to [kMinRatePercent, kMaxRatePercent], write back.
            return false;
        }
    }
}
