using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SDII;

namespace P06_Virtuoso
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Start P06_Virtuoso");
            string path = "../../";
            TextWriter res = new StreamWriter(new FileStream(path + "res.txt", FileMode.Append, FileAccess.Write));
            XElement xcnf = XElement.Load(path + "tests.xml");
            XElement xcommon = XElement.Load(path + "../common.xml");
            xcommon.Add(xcnf);

            Random rnd;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            AdapterVirtuosoSimple engine = new AdapterVirtuosoSimple("HOST=localhost:1550;UID=dba;PWD=dba;Charset=UTF-8;Connection Timeout=500", "g");

            foreach (XElement xprobe in xcnf.Elements())
            {
                ProbeFrame probe = new ProbeFrame(xprobe.AncestorsAndSelf().Attributes());
                int npersons = (int)probe.siz;
                if (probe.sol == "virtuoso_load")
                {
                    engine.PrepareToLoad();
                    Polar.Data.Phototeka generator = new Polar.Data.Phototeka(npersons, 777777);
                    sw.Restart();
                    engine.Load(generator.Generate1of3());
                    engine.Load(generator.Generate2of3());
                    engine.Load(generator.Generate3of3());
                    sw.Stop();
                    Console.WriteLine("Load ok. Duration={0}", sw.ElapsedMilliseconds); // 10000: 14.9 сек.
                    probe.ndx = sw.ElapsedMilliseconds;
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == "virtuoso_SelectById")
                {
                    rnd = new Random(777777777);
                    long sum = 0;
                    sw.Restart();
                    //var rcommand = engine.RunStart();
                    for (int i = 0; i < probe.nte; i++)
                    {
                        string sid = "person" + rnd.Next(0, (int)probe.siz - 1);
                        var v = engine.Query("sparql select * { <" + sid + "> ?p ?o }")
                            .First(po => po[0].ToString() == "age")
                            .ToArray();
                        //    .First(po => (string)po[0] == "age");
                        string s = v[1].ToString();
                        sum += Int32.Parse(s);
                    }
                    //engine.RunStop(rcommand);
                    sw.Stop();
                    probe.tim = sw.ElapsedMilliseconds;
                    probe.sum = sum;
                    Console.WriteLine("SelectById ok. Duration={0}", sw.ElapsedMilliseconds); // 7
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == "virtuoso_SearchByName")
                {
                    rnd = new Random(777777777);
                    sw.Restart();
                    long sum = 0;
                    for (int i = 0; i < probe.nte; i++)
                    {
                        var intId = rnd.Next(0, (int)probe.siz - 1);
                        string namePrefix = "Pupkin" + intId / 10;
                        //sum += (int)engine.Query(string.Format("sparql select ?s {{ ?s <name> ?o . Filter(strStarts(str(?o), \"{0}\")) }}", namePrefix)).Count();
                        var enumerable = engine.Query(string.Format("sparql select ?s {{ ?s <name> ?o . Filter(strStarts(str(?o), \"{0}\")) }}", namePrefix));
                        //foreach (var objectse in enumerable)
                        //{
                        //    foreach (var o in objectse)
                        //    {
                        //        Console.WriteLine(o);
                        //    }
                        //    Console.WriteLine();
                        //}
                        sum += (int)enumerable.Count();
                    }
                    sw.Stop();
                    probe.tim = sw.ElapsedMilliseconds;
                    probe.sum = sum;
                    Console.WriteLine("SearchByName ok. Duration={0}", sw.ElapsedMilliseconds); // 7
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == "virtuoso_GetRelationByPerson")
                {
                    rnd = new Random(777777777);
                    sw.Restart();
                    long sum = 0;
                    for (int i = 0; i < probe.nte; i++)
                    {
                        string persId = "person" + rnd.Next(0, (int)probe.siz - 1);
                        sum += engine.Query(
                            "sparql select ?phname {?refl <reflected> <"+persId+"> . ?refl <in_doc> ?ph . ?ph <name> ?phname}")
                            //"sparql select ?refl {?refl <reflected> <"+persId+"> . }")
                            .Count();
                    }
                    sw.Stop();
                    probe.tim = sw.ElapsedMilliseconds;
                    probe.sum = sum;
                    Console.WriteLine("GetRelationByPerson ok. Duration={0}", sw.ElapsedMilliseconds); // 7
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == "unused")
                {
                }
            }
            res.Close();

        }
    }
}
