using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PolarDB;

namespace F01_Dataflow_polar
{
    public class Program
    {
        public static void Main()
        {
            string path = "../../../Databases/";
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            System.IO.TextWriter writer = new System.IO.StreamWriter(path + "res.txt", true); 
            Console.WriteLine("Start F01_Dataflow_polar.");
            PaCell cell = new PaCell(new PTypeSequence(new PType(PTypeEnumeration.longinteger)), path + "test.pac", false);
            cell.Clear();
            cell.Fill(new object[0]);
            sw.Restart();
            long nd = 100000000L;
            for (long i = 0; i < nd; i++)
            {
                cell.Root.AppendElement(nd - i);
            }
            sw.Stop();
            cell.Flush();
            writer.WriteLine("{0}, Запись потока длиных целых, {1}, {2}, {3}", "F01", "polar", nd, sw.ElapsedMilliseconds);
            writer.Close();
        }
    }
}
