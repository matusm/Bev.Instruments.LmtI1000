using System;
using System.Threading;

namespace Bev.Instruments.LmtI1000
{
    public class LmtI1000
    {
        public LmtI1000(int deviceAddress)
        {
            DeviceAddress = deviceAddress;
            State = new InstrumentState();
            Initialize();
        }

        public int DeviceAddress { get; }
        public string InstrumentManufacturer => "Lichtmesstechnik Berlin";
        public string InstrumentType => GetInstrumentType();
        public string InstrumentSerialNumber => GetDeviceSerialNumber();
        public string InstrumentID => $"{InstrumentType} SN:{InstrumentSerialNumber} @ {DeviceAddress:D2}";
        public InstrumentState State { get; }

        // legacy function for single channel instruments
        public double GetDetectorCurrent()
        {
            FetchInstrumentState();
            return State.CurrentA;
        }

        // legacy function for single channel instruments
        public MeasurementRange GetMeasurementRange()
        {
            if (State.RangeA == MeasurementRange.Unknown)
                FetchInstrumentState();
            return State.RangeA;
        }

        public void FetchInstrumentState()
        {
            string getCommand = $"REN UNL LISTEN {DeviceAddress:D2} GET";
            Send(getCommand);
            string answer = Read();
            State.ParseString(answer);
        }

        public double GetMeasurementUncertainty(double current)
        {
            var range = EstimateMeasurementRange(current);
            return GetMeasurementUncertainty(current, range);
        }

        public double GetMeasurementUncertainty(double current, MeasurementRange range)
        {
            double error = 0;
            current = Math.Abs(current);
            switch (range)
            {
                case MeasurementRange.Unknown:
                    error = double.NaN;
                    break;
                case MeasurementRange.Range02:
                    error = 0.001 * current + 1.0E-6;
                    break;
                case MeasurementRange.Range03:
                    error = 0.001 * current + 1.0E-7;
                    break;
                case MeasurementRange.Range04:
                    error = 0.001 * current + 1.0E-8;
                    break;
                case MeasurementRange.Range05:
                    error = 0.001 * current + 1.0E-9;
                    break;
                case MeasurementRange.Range06:
                    error = 0.001 * current + 1.0E-10;
                    break;
                case MeasurementRange.Range07:
                    error = 0.0015 * current + 1.0E-11;
                    break;
                case MeasurementRange.Range08:
                    error = 0.002 * current + 1.0E-12;
                    break;
                case MeasurementRange.Range09:
                    error = 0.003 * current + 2.0E-13;
                    break;
                default:
                    break;
            }
            // divide by Sqrt(3) for standard uncertainty
            return error * 0.577350269;
        }

        public MeasurementRange EstimateMeasurementRange(double current)
        {
            if (double.IsNaN(current)) return MeasurementRange.Unknown;
            current = Math.Abs(current);
            if (current > 1.9999E-2) return MeasurementRange.RangeOverflow;
            if (current > 1.9999E-3) return MeasurementRange.Range02;
            if (current > 1.9999E-4) return MeasurementRange.Range03;
            if (current > 1.9999E-5) return MeasurementRange.Range04;
            if (current > 1.9999E-6) return MeasurementRange.Range05;
            if (current > 1.9999E-7) return MeasurementRange.Range06;
            if (current > 1.9999E-8) return MeasurementRange.Range07;
            if (current > 1.9999E-9) return MeasurementRange.Range08;
            return MeasurementRange.Range09;
        }

        private string GetInstrumentType()
        {
            FetchInstrumentState();
            switch (State.Mode)
            {
                case LmtMode.Unknown:
                    return "---";
                case LmtMode.BlueRed:
                    return "I 1000 SD";
                case LmtMode.TwoChannels:
                    return "I 1000 SD";
                case LmtMode.SingleChannel:
                    return "I 1000";
            }
            return "---";
        }

        private string GetDeviceSerialNumber()
        {
            // there is no documented way to obtain the serial number
            // TODO implement dictonary DeviceAddress-Ser.Nr. ?
            return "---";
        }

        #region IEEE 488 specific methods

        private void Initialize()
        {
            if (DeviceAddress < 0) throw new Exception();
            if (DeviceAddress > 31) throw new Exception();
            // if necessary do some init here
        }

        private string Read()
        {
            //string[] examples = {
            //    "B+1.2345E-04A 6,R+0.1234E-09A 6", // 1998 lf
            //    "A+1.2345E-02A 7,B+0.1234E-08A 7", // 1992 lf
            //    "+0.3193E-07A 3" // 2017 cr lf
            //};

            string replyString = "B+1.2345E-04A 6,R+0.1234E-09A 6";
            return replyString.TrimEnd('\r', '\n');
        }

        private void Send(string command)
        {
            //throw new NotImplementedException();
            Thread.Sleep(delayAfterSend);
        }

        private const int delayAfterSend = 200;

        #endregion

    }

}
