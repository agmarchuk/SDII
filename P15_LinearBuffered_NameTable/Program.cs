using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SDII;
using UniversalIndex;

namespace P15_LinearBuffered_NameTable
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "../../";
            if (!Directory.Exists(path + "../Databases/name table universal")) Directory.CreateDirectory(path + "../Databases/name table universal");
            NametableLinearBuffered nameTable = new NametableLinearBuffered(path + "../Databases/name table universal", 1000000) ;
            Console.WriteLine("Start P15_LinearBuffered_NameTable");
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            TextWriter res = new StreamWriter(new FileStream(path + "res.txt", FileMode.Append, FileAccess.Write));
            XElement xcnf = XElement.Load(path + "tests.xml");
            XElement xcommon = XElement.Load(path + "../common.xml");
            xcommon.Add(xcnf);
            Random rnd;

            foreach (XElement xprobe in xcnf.Elements())
            {
                ProbeFrame probe = new ProbeFrame(xprobe.AncestorsAndSelf().Attributes());

                if (probe.sol == "LoadAll")
                {
                    // Загрузка
                    sw.Restart();
                    int siz = (int)probe.siz;
                    nameTable.Load(siz, Enumerable.Range(0, siz).Select(i => i.ToString()));
                    sw.Stop();
                    probe.lod = sw.ElapsedMilliseconds;
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("Загрузка OK");
                }
                else if (probe.sol == "TestCompositions")
                {
                    int siz = (int) probe.siz;
                    foreach (var key in nameTable.Keys.Take(siz))
                    {
                        if (key == nameTable.GetString(nameTable.GetCode(key))) continue;
                        throw new Exception(key + " code:" + nameTable.GetCode(key) + " key:" +
                                            nameTable.GetString(nameTable.GetCode(key)));
                    }
                    foreach (var code in nameTable.Codes.Take(siz))
                    {
                        if (code == nameTable.GetCode(nameTable.GetString(code))) continue;
                        throw new Exception(code + " key:" + nameTable.GetString(code) + " code:" +
                                            nameTable.GetCode(nameTable.GetString(code)));
                    }
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("TestCompositions OK");
                }
                else if (probe.sol == "GetStringTime")
                {
                    rnd = new Random(777777777);
                    int nte = (int)probe.nte;

                    // выберем nte случайных кодов из таблицы имён
                    int[] codes =
                        Enumerable.Range(0, nte)
                            .Select(i => rnd.Next((int)nameTable.Count))
                            //.Select(i => nameTable.Codes.ElementAt(i))
                            .ToArray();
                    sw.Restart();
                    long sum = 0L;

                    foreach (var code in codes)
                        sum += nameTable.GetString(code).Length;

                    sw.Stop();
                    probe.sum = sum;
                    probe.tim = sw.ElapsedMilliseconds;
                    probe.tsk = "int2str";
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("GetStringTime OK");
                }
                else if (probe.sol == "GetCodeTime")
                {
                    int nte = (int)probe.nte;
                    rnd = new Random(777777777);
                    // выберем nte случайных строк из таблицы имён
                    string[] keys =
                        Enumerable.Range(0, nte)
                            .Select(i => rnd.Next((int)nameTable.Count))
                            .Select(i => nameTable.Keys.ElementAt(i))
                            .ToArray();
                    long sum = 0L;
                    sw.Restart();
                    foreach (var key in keys)
                    {
                        sum += nameTable.GetCode(key);
                    }
                    sw.Stop();
                    probe.sum = sum;
                    probe.tsk = "str2int";
                    probe.tim = sw.ElapsedMilliseconds;
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("GetCodeTime OK");
                }
                else if (probe.sol == "InseartPortion")
                {
                    int nte = (int) probe.nte;
                    int existingCount = Math.Min(nte/2, (int) nameTable.Count);

                    string[] newKeys =
                        Enumerable.Range((int) nameTable.Count - existingCount, nte)
                            .Select(i => i.ToString())
                            .ToArray();
                    long sum = 0L;
                    sw.Restart();
                  var  portionCoded = nameTable.InsertPortion(newKeys);
                   var fff= nameTable.GetString(nameTable.GetCode("1001"));
                    Console.WriteLine(fff);
                    if (portionCoded.Keys.Distinct().Count()!=nte) 
                        throw new Exception("portion count" + portionCoded.Values.Distinct().Count());
                    if (portionCoded.Values.Distinct().Count() < nte)
                        throw new Exception("portion count" + portionCoded.Values.Distinct().Count());
                    foreach (var newKey in newKeys.Where(newKey => newKey != nameTable.GetString(nameTable.GetCode(newKey))))
                        throw new Exception("portion " + newKey);
                    foreach (var newKey in newKeys.Where(newKey => portionCoded[newKey] != nameTable.GetCode(newKey)))
                        throw new Exception("portion " + newKey);
                    foreach (var newKey in portionCoded.Keys.Where(newKey => newKey != nameTable.GetString(nameTable.GetCode(newKey))))
                            throw new Exception("portion " + newKey);
                    foreach (var newKey in portionCoded.Values.Where(newcode => newcode != nameTable.GetCode(nameTable.GetString(newcode))))
                        throw new Exception("portion " + newKey);
                    sum = nameTable.Count;
                    sw.Stop();
                    probe.sum = sum;
                    probe.tsk = "InseartPortion";
                    probe.tim = sw.ElapsedMilliseconds;
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("InseartPortion OK");
                }
            }
            res.Close();
        }
    }
}
