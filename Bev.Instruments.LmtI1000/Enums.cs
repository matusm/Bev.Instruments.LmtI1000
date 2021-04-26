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
        Range0, //  20 mA
        Range1, //   2 mA
        Range2, // 200 uA
        Range3, //  20 uA
        Range4, //   2 uA
        Range5, // 200 nA
        Range6, //  20 nA
        Range7  //   2 nA
    }

}
