﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SDII;
using UniversalIndex;

namespace P14_Universal_NameTable
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "../../";
            if (!Directory.Exists(path + "../Databases/name table universal")) Directory.CreateDirectory(path + "../Databases/name table universal");
            NameTableUniversal nameTable=new NameTableUniversal(path + "../Databases/name table universal");
            Console.WriteLine("Start P14_Universal_NameTable");
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
                    nameTable.Load(siz, Enumerable.Range(0, siz).Select(i=> i.ToString()));
                    nameTable.BuildIndexes();
                    nameTable.BuildScale();
                    sw.Stop();
                    probe.lod = sw.ElapsedMilliseconds;
                    res.WriteLine(probe.ToCSV());
                    Console.WriteLine("Загрузка OK");
                }
                else if (probe.sol == "TestCompositions")
                {
                    int siz = (int)probe.siz;
                    foreach (var key in nameTable.Keys.Take(siz))
                    {
                        if (key == nameTable.GetString(nameTable.GetCode(key))) continue;
                        throw new Exception(key + " code:" + nameTable.GetCode(key) + " key:" + nameTable.GetString(nameTable.GetCode(key)));
                    }
                    foreach (var code in nameTable.Codes.Take(siz))
                    {
                        if (code == nameTable.GetCode(nameTable.GetString(code))) continue;
                        throw new Exception(code + " key:" + nameTable.GetString(code) + " code:" + nameTable.GetCode(nameTable.GetString(code)));
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
                            .Select(i => rnd.Next((int) nameTable.Count))
                            //.Select(i => nameTable.Codes.ElementAt(i))
                            .ToArray();
                    sw.Restart();
                    codes.Select(code => nameTable.GetString(code)).ToList();
                    sw.Stop();
                    probe.lod = sw.ElapsedMilliseconds;
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
                    sw.Restart();
                    keys.Select(key => nameTable.GetCode(key)).ToList();
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
