using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SDII;

namespace P13_NametableTry
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Start P13_NametableTry");
            string path = "../../";
            string dpath = path + "../Databases/p13nametabletry";
            if (!Directory.Exists(dpath)) Directory.CreateDirectory(dpath);
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            TextWriter res = new StreamWriter(new FileStream(path + "res.txt", FileMode.Append, FileAccess.Write));
            XElement xcnf = XElement.Load(path + "tests.xml");
            XElement xcommon = XElement.Load(path + "../common.xml");
            xcommon.Add(xcnf);
            Random rnd;
            int[] testingcodes = null;

            NametableTry nametable = new NametableTry(dpath + "/");
            //Stan3TabsInt tabs = new Stan3TabsInt(dpath + "/");


            foreach (XElement xprobe in xcnf.Elements())
            {
                ProbeFrame probe = new ProbeFrame(xprobe.AncestorsAndSelf().Attributes());

                if (probe.sol == "LoadAll")
                {
                    // Загрузка
                    int siz = (int)probe.siz;

                    nametable.Clear();
                    sw.Restart();
                    nametable.BuildTable(
                        Enumerable.Range(0, siz).Select(i =>
                            new XElement("pair",
                                new XAttribute("id", i),
                                new XElement("name", i.ToString()))));
                    sw.Stop();
                    probe.lod = sw.ElapsedMilliseconds;
                    sw.Restart();
                    nametable.BuildIndexes();
                    sw.Stop();
                    probe.ndx = sw.ElapsedMilliseconds;

                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("Загрузка OK");
                }
                else if (probe.sol == "Code2String")
                {
                    rnd = new Random(777777777);
                    int nte = (int)probe.nte;

                    // выберем nte случайных кодов из таблицы имён
                    if (testingcodes == null)
                        testingcodes =
                            Enumerable.Range(0, nte)
                                .Select(i => rnd.Next((int)probe.siz - 1))
                                .ToArray();
                    nametable.Warmup();
                    sw.Restart();
                    long sum = 0L;
                    //nametable.Clear();
                    foreach (var code in testingcodes)
                        sum += nametable.GetNameByCode(code).Length;

                    sw.Stop();
                    probe.sum = sum;
                    probe.tim = sw.ElapsedMilliseconds;
                    probe.tsk = "int2str";
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("Code2String OK");
                }
                else if (probe.sol == "String2Code")
                {
                    int nte = (int)probe.nte;
                    //rnd = new Random(777777777);
                    rnd = new Random(111111111);
                    // выберем nte случайных строк из таблицы имён
                    if (testingcodes == null)
                        testingcodes =
                            Enumerable.Range(0, nte)
                                .Select(i => rnd.Next((int)probe.siz - 1))
                            //.Select(i => str2Int.Values.ElementAt(i))
                                .ToArray();
                    string[] keys =
                        testingcodes
                            .Select(i => i.ToString())
                            .ToArray();
                    nametable.Warmup();
                    long sum = 0L;
                    sw.Restart();
                    int cnt = 0;
                    foreach (var key in keys)
                    {
                        int code = nametable.GetPersonsByName(key);
                        sum += code;
                        cnt++;
                    }
                    sw.Stop();
                    probe.sum = sum;
                    probe.tsk = "str2int";
                    probe.tim = sw.ElapsedMilliseconds;
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("String2Code OK cnt={0}", cnt);
                }
            }
            res.Close();
        }
    }
}
