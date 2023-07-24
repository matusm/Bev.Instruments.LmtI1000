using System;
using System.Globalization;
using System.Threading;
using Bev.IO.RemoteInterface;
using Bev.IO.Gpib.Keithley500Serial;
using Bev.IO.Gpib.NiVisa;
using Bev.Instruments.LmtI1000;

namespace TestLMT
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            int lmtAddress = 13;
            string port = "COM1";

            //IRemoteInterface gpib = new Keithley500Serial(port);
            IRemoteInterface gpib = new NiVisa(lmtAddress);

            LmtI1000 lmt = new LmtI1000(lmtAddress, gpib);

            Console.WriteLine(lmt.InstrumentID);
            Console.WriteLine();

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(lmt.GetDetectorCurrent());
            }

            for (int i = 0; i < 10; i++)
            {
                lmt.FetchInstrumentState();
                Console.WriteLine(lmt.State.ResponseLine);
            }

            //gpib.Remote(keithleyAddress);
            //Thread.Sleep(200);

            //gpib.Output(keithleyAddress, "SYSTEM:VERSION?");
            //Console.WriteLine( gpib.Enter(keithleyAddress));

            //gpib.Output(keithleyAddress, "local lockout");

            //gpib.Output(keithleyAddress, "*IDN?");
            //Console.WriteLine(gpib.Enter(keithleyAddress));

            //gpib.Output(keithleyAddress, "SYSTEM:SENSE:VOLTAGE:DC:RANGE:UPPER 20");
            //Console.WriteLine(gpib.Enter(keithleyAddress));

            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine(lmt.GetDetectorCurrent());
            //    gpib.Output(keithleyAddress, "FETCH?");
            //    Thread.Sleep(200);
            //    string str = $"{i} - {gpib.Enter(keithleyAddress)}";
            //    Console.WriteLine(str);
            //}

            //gpib.Local(keithleyAddress);

            lmt.Disconnect();

        }
    }
}
