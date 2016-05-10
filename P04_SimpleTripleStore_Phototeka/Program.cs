using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SDII;
using SimpleTripleStore;
using UniversalIndex.Text_index;

namespace P04_SimpleTripleStore_Phototeka
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            string path = "../../";
            if (!Directory.Exists(path+ "../Databases/simple triple store")) Directory.CreateDirectory(path+ "../Databases/simple triple store");
            Console.WriteLine("Start TestGenerator");
            TextWriter res = new StreamWriter(new FileStream(path + "res.txt", FileMode.Append, FileAccess.Write));
            XElement xcnf = XElement.Load(path + "tests.xml");
            XElement xcommon = XElement.Load(path + "../common.xml");
            xcommon.Add(xcnf);
            Random rnd;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            SimpleTripleStore.SimpleTripleStore simpleTripleStore =
                new SimpleTripleStore.SimpleTripleStore(path + "../Databases/simple triple store/",
                    1000*1000);
            foreach (XElement xprobe in xcnf.Elements())
            {
                ProbeFrame probe = new ProbeFrame(xprobe.AncestorsAndSelf().Attributes());
                int npersons = (int) probe.siz;
                if (probe.sol == "simpleTripleStore_load")
                {
                    // Directory.Delete(path + "../Databases/simple triple store", true);
                    sw.Restart();
                    Polar.Data.Phototeka generator = new Polar.Data.Phototeka(npersons, 777777);
                    simpleTripleStore.Build(generator.GenerateRDF());
                    sw.Stop();
                    Console.WriteLine("Load ok. Duration={0}", sw.ElapsedMilliseconds); // 10000: 14.9 сек.
                    probe.ndx = sw.ElapsedMilliseconds;
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == "simpleTripleStore_SelectById")
                {
                    rnd = new Random(777777777);
                    sw.Restart();
                    long sum = 0;
                    for (int i = 0; i < probe.nte; i++)
                    {
                        int id = rnd.Next(0, (int) probe.siz - 1);
                        sum += Convert.ToInt32(simpleTripleStore.GetDirects(id).FirstOrDefault(tuple => tuple.Item1=="age" ).Item2);
                    }
                    sw.Stop();
                    probe.tim = sw.ElapsedMilliseconds;
                    probe.sum = sum;
                    Console.WriteLine("SelectById ok. Duration={0}", sw.ElapsedMilliseconds); // 7
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == "simpleTripleStore_SearchByName")
                {

                    rnd = new Random(777777777);
                    sw.Restart();

                    WordIndex index = new WordIndex();
                    
                    for (int i = 0; i < probe.siz; i++)
                    {
                        var w = simpleTripleStore.GetObject(i, "name").First();
                        index.Insert(w, i);
                    }
                    Console.WriteLine("build words trigrams index " + sw.ElapsedMilliseconds);
                    Console.WriteLine("RAM used {0} mb.", GC.GetTotalMemory(false) / 1024 / 1024);
                    sw.Restart();
                    long sum = 0, sum2=0;
                    for (int i = 0; i < probe.nte; i++)
                    {
                        int id = rnd.Next(0, (int)probe.siz - 1);
                        string namePrefix = "Pupkin" + id / 10;
                        sum += (int)index.FindBySubWord(namePrefix).Count();
                    }
                    Console.WriteLine(sum);
                    Console.WriteLine(sum2);
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
                        int persId = rnd.Next(0, (int) probe.siz - 1);
                        sum += simpleTripleStore.GetSubjects("reflected", persId.ToString())
                            .Select(refid=>simpleTripleStore.GetObject(refid, "in_doc").First())
                            .Select(int.Parse)
                            .Select(photoId=>simpleTripleStore.GetDirects(photoId))
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
