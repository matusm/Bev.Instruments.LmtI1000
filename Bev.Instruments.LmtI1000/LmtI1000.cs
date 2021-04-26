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

        public string InstrumentManufacturer => "Lichtmesstechnik Berlin";
        public string InstrumentType => GetInstrumentType();
        public string InstrumentSerialNumber => "---";      // no documented way to access
        public string InstrumentFirmwareVersion => "---";   // no documented way to access
        public string InstrumentID => $"{InstrumentType} @ {DeviceAddress:D2}";
        public int DeviceAddress { get; }
        public InstrumentState State { get; }

        // legacy function for single channel instruments
        public double GetDetectorCurrent()
        {
            Fetch();
            return State.CurrentA;
        }

        // TODO : update only after some delay time ?
        public void Fetch()
        {
            string getCommand = $"REN UNL LISTEN {DeviceAddress} GET";
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
                case MeasurementRange.Range0:
                    error = 0.001 * current + 1.0E-6;
                    break;
                case MeasurementRange.Range1:
                    error = 0.001 * current + 1.0E-7;
                    break;
                case MeasurementRange.Range2:
                    error = 0.001 * current + 1.0E-8;
                    break;
                case MeasurementRange.Range3:
                    error = 0.001 * current + 1.0E-9;
                    break;
                case MeasurementRange.Range4:
                    error = 0.001 * current + 1.0E-10;
                    break;
                case MeasurementRange.Range5:
                    error = 0.0015 * current + 1.0E-11;
                    break;
                case MeasurementRange.Range6:
                    error = 0.002 * current + 1.0E-12;
                    break;
                case MeasurementRange.Range7:
                    error = 0.003 * current + 2.0E-13;
                    break;
                default:
                    break;
            }
            // divide by Sqrt(3) for standard uncertainty
            return error * 0.57735;
        }

        public MeasurementRange EstimateMeasurementRange(double current)
        {
            if (double.IsNaN(current)) return MeasurementRange.Unknown;
            current = Math.Abs(current);
            if (current > 2.0E-3) return MeasurementRange.Range0;
            if (current > 2.0E-4) return MeasurementRange.Range1;
            if (current > 2.0E-5) return MeasurementRange.Range2;
            if (current > 2.0E-6) return MeasurementRange.Range3;
            if (current > 2.0E-7) return MeasurementRange.Range4;
            if (current > 2.0E-8) return MeasurementRange.Range5;
            if (current > 2.0E-9) return MeasurementRange.Range6;
            return MeasurementRange.Range7;
        }

        private string GetInstrumentType()
        {
            Fetch();
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
