using Bev.IO.Gpib;
using Bev.IO.Gpib.Keithley500Serial;
using Bev.Instruments.LmtI1000;
using System;
using System.Globalization;

namespace TestLMT
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            int gpibAddress = 16;
            string port = "COM1";

            IGpibHandler gpib = new Keithley500Serial(port);

            LmtI1000 lmt = new LmtI1000(gpibAddress, gpib);

            Console.WriteLine(lmt.InstrumentID);
            Console.WriteLine();

            for (int i = 0; i < 10; i++)
            {
                lmt.FetchInstrumentState();
                Console.WriteLine(lmt.State);
                Console.WriteLine();
            }

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(lmt.GetDetectorCurrent());
            }

            lmt.Disconnect();
        }
    }
}
