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
            //SQLdatabase db = new SQLdatabase(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\home\dev\AdapterRDB\bin\Debug\test.mdf;Integrated Security=True;Connect Timeout=30");

            foreach (XElement xprobe in xcnf.Elements())
            {
                ProbeFrame probe = new ProbeFrame(xprobe.AncestorsAndSelf().Attributes());
                int npersons = (int)probe.siz;
                if (probe.sol == "sqlite_load")
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
                else if (probe.sol == "unused")
                {
                    sw.Restart();
                    db.SelectById(2870, "person");
                    //db.SelectById(1309, "person");
                    sw.Stop();
                    Console.WriteLine("SelectById ok. Duration={0}", sw.ElapsedMilliseconds); // 7

                    sw.Restart();
                    int sum = db.SearchByName("Pupkin999", "person");
                    sw.Stop();
                    Console.WriteLine("SearchByName ok. sum={1} Duration={1}", sum, sw.ElapsedMilliseconds); // 7

                    sw.Restart();
                    db.GetRelationByPerson(2870);
                    //db.GetRelation(1309, "reflected");
                    sw.Stop();
                    Console.WriteLine("GetRelation ok. Duration={0}", sw.ElapsedMilliseconds); // 91

                    // Проверка серии портретов
                    rnd = new Random(999999); int i = 0;
                    sw.Restart();
                    for (; i < 100; i++) db.GetRelationByPerson(rnd.Next(10000));
                    sw.Stop();
                    System.Console.WriteLine("GetPortraitById OK. times={0} duration={1}", i, sw.ElapsedMilliseconds); // 1227 ms. 

                    // Проверка серии портретов (2) по тем же идентификаторам
                    rnd = new Random(999999); i = 0;
                    sw.Restart();
                    for (; i < 100; i++) db.GetRelationByPerson(rnd.Next(10000));
                    sw.Stop();
                    System.Console.WriteLine("GetPortraitById OK. times={0} duration={1}", i, sw.ElapsedMilliseconds); // 43 ms. 

                    //// 
                    //Console.WriteLine("==== Динамика естественного разогрева ====");
                    //Random rnd2 = new Random();
                    //for (int j = 0; j < 100; j++)
                    //{
                    //    sw.Restart();
                    //    for (int ii = 0; ii < 100; ii++) db.GetRelationByPerson(rnd2.Next(npersons - 1));
                    //    sw.Stop();
                    //    Console.Write("{0} ", sw.ElapsedMilliseconds);
                    //}
                    //System.Console.WriteLine(); // 
                }
            }
            res.Close();
        }
    }
}
