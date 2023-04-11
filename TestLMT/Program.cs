using Bev.IO.Gpib;
using Bev.IO.Gpib.Keithley500Serial;
using Bev.Instruments.LmtI1000;
using System;
using System.Globalization;
using System.Threading;

namespace TestLMT
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            int lmtAddress = 13;
            int keithleyAddress = 18;
            string port = "COM1";

            IGpibHandler gpib = new Keithley500Serial(port);

            LmtI1000 lmt = new LmtI1000(lmtAddress, gpib);

            Console.WriteLine(lmt.InstrumentID);
            //Console.WriteLine();

            //for (int i = 0; i < 5; i++)
            //{
            //    lmt.FetchInstrumentState();
            //    Console.WriteLine(lmt.State.CurrentA);
            //    Console.WriteLine();
            //}

            gpib.Remote(keithleyAddress);
            Thread.Sleep(200);

            gpib.Output(keithleyAddress, "SYSTEM:VERSION?");
            Console.WriteLine( gpib.Enter(keithleyAddress));

            gpib.Output(keithleyAddress, "local lockout");

            gpib.Output(keithleyAddress, "*IDN?");
            Console.WriteLine(gpib.Enter(keithleyAddress));

            //gpib.Output(keithleyAddress, "SYSTEM:SENSE:VOLTAGE:DC:RANGE:UPPER 20");
            //Console.WriteLine(gpib.Enter(keithleyAddress));

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(lmt.GetDetectorCurrent());
                gpib.Output(keithleyAddress, "FETCH?");
                Thread.Sleep(200);
                string str = $"{i} - {gpib.Enter(keithleyAddress)}";
                Console.WriteLine(str);
            }
            gpib.Local(keithleyAddress);

            lmt.Disconnect();

        }
    }
}
