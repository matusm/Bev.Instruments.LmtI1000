namespace Bev.Instruments.LmtI1000
{
    public enum LmtMode
    {
        Unknown,
        BlueRed,        // I 1000 SD with blue/red switch activated
        TwoChannels,    // I 1000 SD with blue/red switch deactivated
        SingleChannel   // I 1000
    }

    public enum MeasurementRange
    {
        Unknown,
        RangeOverflow, // >20 mA
        Range02, // 19.999 mA -   2.000 ma
        Range03, // 1.9999 mA -  0.2000 mA
        Range04, // 199.99 uA -   20.00 uA
        Range05, // 19.999 uA -   2.000 uA
        Range06, // 1.9999 uA -  0.2000 uA
        Range07, // 199.99 nA -   20.00 nA
        Range08, // 19.999 nA -   2.000 nA
        Range09  // 1.9999 nA -  0.0000 nA
    }
}

