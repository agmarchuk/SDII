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
            string path = @"..\..\";
            TextWriter res = new StreamWriter(new FileStream(path + "res.txt", FileMode.Append, FileAccess.Write));
            if (true || !System.IO.File.Exists(path + "tests.xml")) System.IO.File.Copy(path + "tests0.xml", path + "tests.xml", true);
            XElement xcnf = XElement.Load(path + "tests.xml");

            foreach (XElement xprobe in xcnf.Elements())
            {
                ProbeFrame probe = new ProbeFrame(xprobe);
                // измеряются задачи: FlowIO, DirectRand
                if (probe.sol == "Arr")
                {
                    sw.Restart();
                    long[] arr = Enumerable.Range(1, (int)probe.siz)
                        .Select(i => (long)i)
                        .ToArray();
                    sw.Stop();
                    probe.lod = sw.ElapsedMilliseconds;
                    res.WriteLine(probe.ToCSV());
                }
            }
            res.Close();    
        }

    }
}
