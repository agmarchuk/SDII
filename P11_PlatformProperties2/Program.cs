using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SDII;

namespace P11_PlatformProperties2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start P11_PlatformPreperties2");
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            string path = @"../../";
            TextWriter res = new StreamWriter(new FileStream(path + "res.txt", FileMode.Append, FileAccess.Write));
            XElement xcnf = XElement.Load(path + "tests.xml");
            XElement xcommon = XElement.Load(path + "../common.xml");
            xcommon.Add(xcnf);
            Random rnd;
            Dictionary<string, int> str2Int = new Dictionary<string, int>();
            List<string> int2Str = new List<string>();
            foreach (XElement xprobe in xcnf.Elements())
            {
                ProbeFrame probe = new ProbeFrame(xprobe.AncestorsAndSelf().Attributes());
               
                if (probe.sol == "LoadAll")
                {
                    // Загрузка
                    sw.Restart();
                    int siz = (int)probe.siz;
                    for (int i = 0; i < siz; i++)
                    {
                        str2Int.Add(i.ToString(), i);
                        int2Str.Add(i.ToString());
                    }
                    sw.Stop();
                    probe.lod = sw.ElapsedMilliseconds;
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("Загрузка OK");
                }
                else if (probe.sol == "TestCompositions")
                {
                    int siz = (int)probe.siz;
                    foreach (var key in str2Int.Keys.Take(siz))
                    {
                        if (key == int2Str[str2Int[key]]) continue;
                        throw new Exception(key + " " + str2Int[key] + " " + int2Str[str2Int[key]]);
                    }
                    foreach (var code in str2Int.Values.Take(siz))
                    {
                        if (code == str2Int[int2Str[code]]) continue;
                        throw new Exception(code + " " + int2Str[code] + " " + str2Int[int2Str[code]]);
                    }
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("TestCompositions OK");
                }
                else if (probe.sol == "GetStringTime")
                {
                    rnd = new Random(777777777);
                    int nte = (int) probe.nte;

                    // выберем nte случайных кодов из таблицы имён
                    int[] codes =
                        Enumerable.Range(0, nte)
                            .Select(i => rnd.Next(str2Int.Values.Count))
                            .Select(i => str2Int.Values.ElementAt(i))
                            .ToArray();
                    sw.Restart();
                    codes.Select(key => int2Str[key]).ToList();
                    sw.Stop();
                    probe.lod = sw.ElapsedMilliseconds;
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("GetStringTime OK");
                }
                else if (probe.sol == "GetCodeTime")
                {
                    int nte = (int) probe.nte;
                    rnd = new Random(777777777);
                    // выберем nte случайных строк из таблицы имён
                    string[] keys =
                        Enumerable.Range(0, nte)
                            .Select(i => rnd.Next(str2Int.Keys.Count))
                            .Select(i => str2Int.Keys.ElementAt(i))
                            .ToArray();
                    sw.Restart();
                    keys.Select(key => str2Int[key]).ToList();
                    sw.Stop();
                    probe.lod = sw.ElapsedMilliseconds;
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("GetCodeTime OK");
                }
            }
            res.Close();
        }
    }
}
