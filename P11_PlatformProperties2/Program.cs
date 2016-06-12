using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            string path = ProjectDirectory.GetProjectDirectory();
            TextWriter res = new StreamWriter(new FileStream(path + "res.txt", FileMode.Append, FileAccess.Write));
            XElement xcnf = XElement.Load(path + "tests.xml");
            XElement xcommon = XElement.Load(path + "../common.xml");
            xcommon.Add(xcnf);
            Random rnd;
            Dictionary<string, int> str2Int = new Dictionary<string, int>();
            List<string> int2Str = new List<string>();
            int[] testingcodes = null;
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
                        string s = i.ToString();
                        str2Int.Add(s, i);
                        int2Str.Add(s.ToString());
                    }
                    sw.Stop();
                    probe.lod = sw.ElapsedMilliseconds;
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("Загрузка OK");
                }
                else if (probe.sol == "TestCompositions")
                {
                    int nte = (int)probe.nte;
                    foreach (var key in str2Int.Keys.Take(nte))
                    {
                        if (key == int2Str[str2Int[key]]) continue;
                        throw new Exception(key + " " + str2Int[key] + " " + int2Str[str2Int[key]]);
                    }
                    foreach (var code in str2Int.Values.Take(nte))
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
                    if (testingcodes == null)
                    testingcodes =
                        Enumerable.Range(0, nte)
                            .Select(i => rnd.Next(str2Int.Values.Count - 1))
                            //.Select(i => str2Int.Values.ElementAt(i))
                            .ToArray();
                    long sum = 0L;
                    sw.Restart();
                    foreach (int key in testingcodes)
                    {
                        string s = int2Str[key];
                        sum += s.Length;
                    }
                    //sum += testingcodes.Select(k => int2Str[k].Length).Aggregate((sm, v) => sm + v);
                    sw.Stop();
                    probe.sum = sum;
                    probe.tim = sw.ElapsedMilliseconds;
                    probe.tsk = "int2str";
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("GetStringTime OK");
                }
                else if (probe.sol == "GetCodeTime")
                {
                    int nte = (int) probe.nte;
                    rnd = new Random(777777777);
                    // выберем nte случайных строк из таблицы имён
                    if (testingcodes == null || nte != testingcodes.Length)
                        testingcodes =
                            Enumerable.Range(0, nte)
                                .Select(i => rnd.Next(str2Int.Values.Count - 1))
                            //.Select(i => str2Int.Values.ElementAt(i))
                                .ToArray();
                    string[] keys =
                        testingcodes
                            .Select(i => i.ToString())
                            .ToArray();
                    long sum = 0L;
                    sw.Restart();
                    foreach (string key in keys)
                    {
                        sum += str2Int[key];
                    }
                    sw.Stop();
                    probe.sum = sum;
                    probe.tsk = "str2int";
                    probe.tim = sw.ElapsedMilliseconds;
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("GetCodeTime OK");
                }
            }
            res.Close();
        }
    }
}
