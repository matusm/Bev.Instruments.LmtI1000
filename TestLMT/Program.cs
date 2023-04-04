using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bev.IO.Keithley500S;
using Bev.Instruments.LmtI1000;
using System.Globalization;


namespace TestLMT
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            int gpibAddress = 16;
            string port = "COM8";

            K500S k500S = new K500S(port);

            LmtI1000 lmt = new LmtI1000(gpibAddress);

            lmt.GpibHandler = k500S;

            lmt.Initialize();

            Console.WriteLine(lmt.InstrumentID);

            for (int i = 0; i < 10; i++)
            {
                lmt.FetchInstrumentState();
                Console.WriteLine(lmt.State);
                Console.WriteLine();
            }

            k500S.Local(gpibAddress);

        }
    }
}
