using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Polar.Data;
using SDII;

namespace P05_Phototeka3TabsInt
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Start P05_Phototeka3TabsInt");
            string path = "../../";
            if (!Directory.Exists(path + "../Databases/P05_Phototeka3TabsInt")) Directory.CreateDirectory(path + "../Databases/P05_Phototeka3TabsInt");
            TextWriter res = new StreamWriter(new FileStream(path + "res.txt", FileMode.Append, FileAccess.Write));
            XElement xcnf = XElement.Load(path + "tests.xml");
            XElement xcommon = XElement.Load(path + "../common.xml");
            xcommon.Add(xcnf);
            Random rnd = new Random(777777777);
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            string dbpath = path + "../Databases/P05_Phototeka3TabsInt/";

            Stan3TabsInt tabs = new Stan3TabsInt(dbpath);

            foreach (XElement xprobe in xcnf.Elements())
            {
                ProbeFrame probe = new ProbeFrame(xprobe.AncestorsAndSelf().Attributes());
                int npersons = (int)probe.siz;
                if (probe.sol == "Stan3TabsInt_load")
                {
                    sw.Restart();
                    Polar.Data.Phototeka generator = new Polar.Data.Phototeka(npersons, 777777);
                    tabs.Build(
                        generator.Generate1of3()
                        .Concat(generator.Generate2of3()
                        .Concat(generator.Generate3of3())));
                    sw.Stop();
                    probe.lod = sw.ElapsedMilliseconds;
                    Console.WriteLine("Load ok. Duration={0}", sw.ElapsedMilliseconds); // 10000: 14.9 сек.
                    sw.Restart();
                    tabs.BuildIndexes();
                    sw.Stop();
                    probe.ndx = sw.ElapsedMilliseconds;
                    Console.WriteLine("BuildIndexes ok. Duration={0}", sw.ElapsedMilliseconds); // 10000: 14.9 сек.
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == "Stan3TabsInt_SelectById")
                {
                    rnd = new Random(777777777);
                    sw.Restart();
                    long sum = 0;
                    for (int i = 0; i < probe.nte; i++)
                    {
                        int id = rnd.Next(0, (int)probe.siz - 1);
                        var p = tabs.GetPersonByCode(id);
                        sum += (int)p[2];
                    }
                    sw.Stop();
                    probe.tim = sw.ElapsedMilliseconds;
                    probe.sum = sum;
                    Console.WriteLine("SelectById ok. Duration={0}", sw.ElapsedMilliseconds); // 46 (1000)
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == "simpleTripleStore_SearchByName")
                {
                    rnd = new Random(777777777);
                    sw.Restart();
                    long sum = 0;
                    for (int i = 0; i < probe.nte; i++)
                    {
                        int id = rnd.Next(0, (int)probe.siz - 1);
                        string namePrefix = "Pupkin" + id / 10;
                        //sum += (int)simpleTripleStore.GetSubjects("a", "person").Select(personId => simpleTripleStore.GetObject(personId, "name").FirstOrDefault()).Count(name => name.StartsWith(namePrefix));
                    }
                    sw.Stop();
                    probe.tim = sw.ElapsedMilliseconds;
                    probe.sum = sum;
                    Console.WriteLine("SearchByName ok. Duration={0}", sw.ElapsedMilliseconds); // 7
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == "simpleTripleStore_GetRelationByPerson")
                {
                    rnd = new Random(777777777);
                    sw.Restart();
                    long sum = 0;
                    for (int i = 0; i < probe.nte; i++)
                    {
                        int id = rnd.Next(0, (int)probe.siz - 1);
                        //sum += simpleTripleStore.GetSubjects("reflected", id.ToString()).Count();
                    }
                    sw.Stop();
                    probe.tim = sw.ElapsedMilliseconds;
                    probe.sum = sum;
                    Console.WriteLine("GetRelationByPerson ok. Duration={0}", sw.ElapsedMilliseconds); // 7
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == "unused")
                {
                    int cnt = 0;
                    bool toload = false;
                    toload = true;
                    if (toload)
                    {
                        sw.Restart();
                        Phototeka generator = new Phototeka(npersons, 2378459);
                        tabs.Build(
                            generator.Generate1of3()
                            .Concat(generator.Generate2of3()
                            .Concat(generator.Generate3of3())));

                        sw.Stop();
                        Console.WriteLine("Load ok. duration={0}", sw.ElapsedMilliseconds);
                    }

                    sw.Restart();
                    for (int i = 0; i < 10000; i++)
                    {
                        int code = rnd.Next(npersons - 1);
                        object[] v = tabs.GetPersonByCode(code);
                    }
                    sw.Stop();
                    Console.WriteLine("10000 persons ok. duration={0}", sw.ElapsedMilliseconds);

                    sw.Restart();
                    for (int i = 0; i < 10000; i++)
                    {
                        int code = rnd.Next(2 * npersons - 1);
                        object[] v = tabs.GetPhoto_docByCode(code);
                        if (i == 200)
                        {
                            Console.WriteLine("photo_doc record: {0} {1}", v[0], v[1]);
                        }
                    }
                    sw.Stop();
                    Console.WriteLine("10000 photo_docs ok. duration={0}", sw.ElapsedMilliseconds);

                    sw.Restart();
                    for (int i = 0; i < 10000; i++)
                    {
                        int code = rnd.Next(2 * npersons - 1);
                        cnt = tabs.GetReflectionsByReflected(code).Count();
                    }
                    sw.Stop();
                    Console.WriteLine("10000 portraits ok. duration={0}", sw.ElapsedMilliseconds);
                }
            }
            res.Close();
        }
    }
}
