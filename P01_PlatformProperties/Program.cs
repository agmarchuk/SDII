using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using SDII;

namespace P01_PlatformProperties
{
    public class Program
    {
        public static void Main()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            string path = @"../../";
            TextWriter res = new StreamWriter(new FileStream(path + "res.txt", FileMode.Append, FileAccess.Write));
            if (true || !System.IO.File.Exists(path + "tests.xml")) System.IO.File.Copy(path + "tests0.xml", path + "tests.xml", true);
            XElement xcnf = XElement.Load(path + "tests.xml");
            if (true || !System.IO.File.Exists(path + "../common.xml")) System.IO.File.Copy(path + "../common0.xml", path + "../common.xml", true);
            XElement xcommon = XElement.Load(path + "../common.xml");
            xcommon.Add(xcnf);
            Random rnd;


            foreach (XElement xprobe in xcnf.Elements())
            {
                ProbeFrame probe = new ProbeFrame(xprobe.AncestorsAndSelf().Attributes());
                // измеряются задачи: FlowIO, DirectRand
                if (probe.sol == "Arr")
                {
                    // Загрузка
                    sw.Restart();
                    int siz = (int)probe.siz;
                    long[] arr = Enumerable.Range(0, siz)
                        .Select(i => (long)(siz-i-1))
                        //.Select(i => (long)(i))
                        .ToArray();
                    sw.Stop();
                    probe.lod = sw.ElapsedMilliseconds;
                    Console.WriteLine("Загрузка OK");

                    // Сортировка
                    sw.Restart();
                    Array.Sort(arr);
                    sw.Stop();
                    probe.ndx = sw.ElapsedMilliseconds;

                    Console.WriteLine("Сортировка OK");

                    long ssum = 0;
                    sw.Restart();
                    for (long ii = 0; ii < probe.siz; ii++)
                    {
                        ssum += arr[ii];
                    }
                    sw.Stop();
                    probe.scn = sw.ElapsedMilliseconds;
                    probe.sum = ssum;

                    Console.WriteLine("Сканирование OK");

                    ssum = 0;
                    rnd = new Random(777777777);
                    int[] indexes = Enumerable.Range(0, probe.nte)
                        .Select(i => rnd.Next((int)probe.siz - 1))
                        .ToArray();
                    sw.Restart();
                    for (long ii = 0; ii < probe.nte; ii++)
                    {
                        ssum += arr[indexes[ii]];
                    }
                    sw.Stop();
                    probe.tim = sw.ElapsedMilliseconds - probe.scn * probe.nte / probe.siz;
                    probe.sum = ssum;

                    Console.WriteLine("Доступ OK");

                    res.WriteLine(probe.ToCSV());
                }
            }
            res.Close();    
        }

    }
}
