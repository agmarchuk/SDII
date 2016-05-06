using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SDII;

namespace P03_SQL_Phototeka
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Start TestGenerator");
            string path = "../../";
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            TextWriter res = new StreamWriter(new FileStream(path + "res.txt", FileMode.Append, FileAccess.Write));
            XElement xcnf = XElement.Load(path + "tests.xml");
            XElement xcommon = XElement.Load(path + "../common.xml");
            xcommon.Add(xcnf);
            Random rnd;

            //MySQL db = new MySQL("server=localhost;uid=root;port=3306;password=fetnaggi;");
            SQLite db = new SQLite("Data Source=" + path + "../databases/test.db3");
            //SQLdatabase db = new SQLdatabase(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\Home\dev2012\SDII\Databases\Test_Phototeka.mdf;Integrated Security=True;Connect Timeout=30");

            string dbname = db.GetType().Name;

            foreach (XElement xprobe in xcnf.Elements())
            {
                ProbeFrame probe = new ProbeFrame(xprobe.AncestorsAndSelf().Attributes());
                int npersons = (int)probe.siz;
                if (probe.sol == dbname + "_load")
                {
                    db.PrepareToLoad();
                    sw.Restart();
                    Polar.Data.Phototeka generator = new Polar.Data.Phototeka(npersons, 777777);
                    db.LoadElementFlow(generator.Generate1of3());
                    db.LoadElementFlow(generator.Generate2of3());
                    db.LoadElementFlow(generator.Generate3of3());
                    sw.Stop();
                    probe.lod = sw.ElapsedMilliseconds;

                    sw.Restart();
                    db.MakeIndexes();
                    sw.Stop();
                    probe.ndx = sw.ElapsedMilliseconds;
                    Console.WriteLine("Load ok."); // 10000: 14.9 сек.
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == dbname + "_SelectById")
                {
                    rnd = new Random(777777777);
                    sw.Restart();
                    long sum = 0;
                    for (int i = 0; i < probe.nte; i++)
                    {
                        int id = rnd.Next(0, (int)probe.siz - 1);
                        sum += (int)(db.GetById(id, "person")[2]);
                    }
                    sw.Stop();
                    probe.tim = sw.ElapsedMilliseconds;
                    probe.sum = sum;
                    Console.WriteLine("SelectById ok. Duration={0}", sw.ElapsedMilliseconds); // 7
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == dbname + "_SearchByName")
                {
                    rnd = new Random(777777777);
                    sw.Restart();
                    long sum = 0;
                    for (int i = 0; i < probe.nte; i++)
                    {
                        int id = rnd.Next(0, (int)probe.siz - 1);
                        sum += db.SearchByName("Pupkin" + id/10, "person").Count();
                    }
                    sw.Stop();
                    probe.tim = sw.ElapsedMilliseconds;
                    probe.sum = sum;
                    Console.WriteLine("SearchByName ok. Duration={0}", sw.ElapsedMilliseconds); // 7
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == dbname + "_GetRelationByPerson")
                {
                    rnd = new Random(777777777);
                    sw.Restart();
                    long sum = 0;
                    for (int i = 0; i < probe.nte; i++)
                    {
                        int id = rnd.Next(0, (int)probe.siz - 1);
                        sum += db.GetPhotosOfPersonUsingRelation(id).Count();
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
