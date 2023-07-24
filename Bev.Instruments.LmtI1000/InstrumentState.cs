using System;
using System.Globalization;

namespace Bev.Instruments.LmtI1000
{
    public class InstrumentState
    {
        public InstrumentState()
        {
            ResetState();
        }

        public LmtMode Mode { get; private set; }
        public double CurrentA { get; private set; }
        public double CurrentB { get; private set; }
        public bool OverflowA { get; private set; } // true if overflow
        public bool OverflowB { get; private set; } // true if overflow
        public MeasurementRange RangeA { get; private set; }
        public MeasurementRange RangeB { get; private set; }
        public double Ratio => CurrentA / CurrentB;
        public DateTime TimeStamp { get; private set; }
        public string ResponseLine { get; private set; }

        public void ParseString(string line)
        {
            line = line.TrimEnd('\r', '\n'); // return value of LmtI1000.Read() is trimmed already. InstrumentState.ParseString() can be used with other libs, too.
            ResetState();
            ResponseLine = line;
            if (line.Length == 31)
            {
                CurrentA = ParseDoubleFrom(line, 1);
                CurrentB = ParseDoubleFrom(line, 17);
                int statusByteA = ParseDigitFrom(line, 14);
                OverflowA = CheckOverflow(statusByteA) || CheckOverflow(line.Substring(2, 6));
                OverflowB = CheckOverflow(ParseDigitFrom(line, 30)) || CheckOverflow(line.Substring(18, 6));
                if (statusByteA == 7) Mode = LmtMode.TwoChannels;
                if (statusByteA == 6) Mode = LmtMode.BlueRed;
                if (statusByteA == 3) Mode = LmtMode.TwoChannels;
                if (statusByteA == 2) Mode = LmtMode.BlueRed;
                RangeA = GetRange(ParseDigitFrom(line, 11));
                RangeB = GetRange(ParseDigitFrom(line, 27));
            }
            if (line.Length == 14)
            {
                CurrentA = ParseDoubleFrom(line, 0);
                OverflowA = CheckOverflow(ParseDigitFrom(line, 13)) || CheckOverflow(line.Substring(1, 6));
                Mode = LmtMode.SingleChannel;
                RangeA = GetRange(ParseDigitFrom(line, 10));
            }
            if (OverflowA) CurrentA = double.NaN;
            if (OverflowB) CurrentB = double.NaN;
        }

        public override string ToString()
        {
            string toStr = $"[InstrumentState Mode:{Mode}]";
            string toStrA = $"[Channel A - Current:{CurrentA} Range:{RangeA} Overflow:{OverflowA}]";
            string toStrB = $"[Channel B - Current:{CurrentB} Range:{RangeB} Overflow:{OverflowB}]";
            return $"{toStr}\n{toStrA}\n{toStrB}";
        }

        private MeasurementRange GetRange(int exp)
        {
            switch (exp)
            {
                case 2:
                    return MeasurementRange.Range02;
                case 3:
                    return MeasurementRange.Range03;
                case 4:
                    return MeasurementRange.Range04;
                case 5:
                    return MeasurementRange.Range05;
                case 6:
                    return MeasurementRange.Range06;
                case 7:
                    return MeasurementRange.Range07;
                case 8:
                    return MeasurementRange.Range08;
                case 9:
                    return MeasurementRange.Range09;
                default:
                    return MeasurementRange.Unknown;
            }
        }

        private bool CheckOverflow(int statusByte)
        {
            if (statusByte == 7) return false;
            if (statusByte == 6) return false;
            return true;
        }

        private bool CheckOverflow(string substr)
        {
            if (substr == "6.0000")
                return true;
            return false;
        }

        private int ParseDigitFrom(string token, int atStartIndex)
        {
            string subToken = token.Substring(atStartIndex, 1);
            return int.TryParse(subToken, out int value) ? value : -1; // good old C++ error return value
        }

        private double ParseDoubleFrom(string token, int atStartIndex)
        {
            string subToken = token.Substring(atStartIndex, 11);
            return double.TryParse(subToken, NumberStyles.Any, CultureInfo.InvariantCulture, out double value) ? value : double.NaN;
        }

        private void ResetState()
        {
            TimeStamp = DateTime.UtcNow;
            Mode = LmtMode.Unknown;
            OverflowA = false;
            OverflowB = false;
            CurrentA = double.NaN;
            CurrentB = double.NaN;
            RangeA = MeasurementRange.Unknown;
            RangeB = MeasurementRange.Unknown;
            ResponseLine = string.Empty;
        }
    }
}
