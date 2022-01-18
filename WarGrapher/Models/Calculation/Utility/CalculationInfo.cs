namespace WarGrapher.Models.Calculation.Utility
{
    /// <summary>
    /// Represents additional information about the calculation
    /// </summary>
    public struct CalculationInfo
    {
        public string XAxisName { get; set; }
        public string XAxisUnit { get; set; }

        public string YAxisName { get; set; }
        public string YAxisUnit { get; set; }

        public string ChartName { get; set; }
    }
}
