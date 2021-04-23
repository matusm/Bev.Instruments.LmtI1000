using System;
using System.Threading;
using System.Globalization;

namespace Bev.Instruments.LmtI1000
{
    public class LmtI1000
    {
        public LmtI1000(int deviceAddress)
        {
            if (deviceAddress < 0) deviceAddress = 0;
            if (deviceAddress > 31) deviceAddress = 31;
            DeviceAddress = deviceAddress;
        }

        public string InstrumentManufacturer => "Lichtmesstechnik Berlin";
        public string InstrumentType => GetInstrumentType();
        public string InstrumentSerialNumber => $"";
        public string InstrumentFirmwareVersion => $"";
        public string InstrumentID => $"{InstrumentType} {InstrumentFirmwareVersion} SN:{InstrumentSerialNumber}";
        public int DeviceAddress { get; }

        public double GetDetectorCurrent()
        {
            return GetDetectorCurrent(1);
        }

        private double GetDetectorCurrent(int channel)
        {
            ReadLMT();
            if (channel == 1)
                return channel1;
            if (channel == 2)
                return channel2;
            return double.NaN;
        }

        private string GetInstrumentType()
        {
            LmtMode mode = ParseAnswer(Query());
            switch (mode)
            {
                case LmtMode.Unknown:
                    return "unkown";
                case LmtMode.BlueRed:
                    return "I 1000 SD";
                case LmtMode.TwoChannels:
                    return "I 1000 SD";
                case LmtMode.SingleChannel:
                    return "I 1000";
            }
            return "";
        }

        private string Query()
        {
            string getCommand = $"REN UNL LISTEN {DeviceAddress} GET";
            Transmit(getCommand);
            string answer = Read();
            return answer;
        }

        private void ReadLMT()
        {
            LmtMode mode = ParseAnswer(Query());
        }

        private LmtMode ParseAnswer(string answer)
        {
            channel1 = double.NaN;
            channel2 = double.NaN;

            if(answer.Length == 31)
            {

            }
            if (answer.Length == 14)
            {

            }

                return LmtMode.Unknown;
        }


        #region IEEE 488 specific methods
        private string Read()
        {
            string example = "B+1.2345E-04A 6,R+0.1234E-09A 6"; // 1998 lf
            //string example = "A+1.2345E-04A 7,B+0.1234E-09A 7"; // 1992 lf
            //string example = "+0.3193E-08A 7"; // 2017 cr lf
            // clean from newlines
            return example.TrimEnd('\r', '\n');
        }

        private void Transmit(string getCommand)
        {
            throw new NotImplementedException();
        }
        #endregion

        private double channel1;
        private double channel2;


    }

}
