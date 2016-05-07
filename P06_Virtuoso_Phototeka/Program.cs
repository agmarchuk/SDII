using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenLink.Data.Virtuoso;
using Polar.Data;
using RDFCommon.OVns;
using sema2012m;
using SDII;
using VirtuosoTest;

namespace P06_Virtuoso_Phototeka
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "../../";
            Console.WriteLine("Start TestGenerator");
            TextWriter res = new StreamWriter(new FileStream(path + "res.txt", FileMode.Append, FileAccess.Write));
            XElement xcnf = XElement.Load(path + "tests.xml");
            XElement xcommon = XElement.Load(path + "../common.xml");
            xcommon.Add(xcnf);
            Random rnd;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            AdapterVirtuoso engine = new AdapterVirtuoso("HOST=localhost:1550;UID=dba;PWD=dba;Charset=UTF-8;Connection Timeout=500", "g");

            foreach (XElement xprobe in xcnf.Elements())
            {
                ProbeFrame probe = new ProbeFrame(xprobe.AncestorsAndSelf().Attributes());
                int npersons = (int)probe.siz;
                if (probe.sol == "virtuoso7_load")
                {
                    sw.Restart();
                    Reload(engine, npersons);
                    sw.Stop();
                    Console.WriteLine("Load ok. Duration={0}", sw.ElapsedMilliseconds); // 10000: 14.9 сек.
                    probe.ndx = sw.ElapsedMilliseconds;
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == "virtuoso7_SelectById")
                {
                    rnd = new Random(777777777);
                    long sum = 0;
                    sw.Restart();

                    for (int i = 0; i < probe.nte; i++)
                    {
                        string sid = "person"+ rnd.Next(0, (int)probe.siz - 1);
                        var v = engine.Query("sparql select * { <" + sid + "> ?p ?o }");
                        sum +=(int) v.First(paramValues => paramValues[0].ToString() == "age")[1];
                    }
                    sw.Stop();
                    probe.tim = sw.ElapsedMilliseconds;
                    probe.sum = sum;
                    Console.WriteLine("SelectById ok. Duration={0}", sw.ElapsedMilliseconds); // 7
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == "virtuoso7_SearchByName")
                {
                    Console.WriteLine((string)engine.Query("sparql select Count(?p) { ?p a <person>}").First()[0].ToString());
                    Console.WriteLine((string)engine.Query("sparql select Count(?p) { ?p a <photo_doc>}").First()[0].ToString());
                    Console.WriteLine((string)engine.Query("sparql select Count(?p) { ?p a <reflection>}").First()[0].ToString());
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
                        //        if (o is SqlRdfBox)
                        //            string oname = (string)engine.Query("sparql select ?name { <" + o + "> <name> ?name}").First()[0].ToString();
                        //        if (!o.ToString().StartsWith(namePrefix))
                        //            Console.WriteLine("ERROR!");
                        //    }
                        //}
                        sum += (int)enumerable.Count();
                   }
                    sw.Stop();
                    probe.tim = sw.ElapsedMilliseconds;
                    probe.sum = sum;
                    Console.WriteLine("SearchByName ok. Duration={0}", sw.ElapsedMilliseconds); // 7
                    res.WriteLine(probe.ToCSV());
                }
                else if (probe.sol == "virtuoso7_GetRelationByPerson")
                {
                    rnd = new Random(777777777);
                    sw.Restart();
                    long sum = 0;
                    for (int i = 0; i < probe.nte; i++)
                    {
                        string persId = "person"+ rnd.Next(0, (int)probe.siz - 1);
                        sum += engine.Query(string.Format("sparql select ?phname {{?refl <reflected> <{0}> . ?refl <in_doc> ?ph . ?ph <name> ?phname}}", persId))
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
        private static void Reload(AdapterVirtuoso store, int npersons)
        {
            Polar.Data.Phototeka generator = new Polar.Data.Phototeka(npersons, 777777);
            Func<XElement,IEnumerable<Tuple<string, string, ObjectVariants>>> f= (XElement ele) =>
            {
                string id = ele.Name + ele.Attribute("id").Value;
                var seq = Enumerable.Repeat(Tuple.Create(id, "http://www.w3.org/1999/02/22-rdf-syntax-ns#type", (ObjectVariants)new OV_iri(ele.Name.LocalName)), 1)
                    .Concat(ele.Elements().Select(subele =>
                    {
                        XAttribute ratt = subele.Attribute("ref");
                        Tuple<string, string, ObjectVariants> triple = null;
                        if (ratt != null)
                        {
                            string r = (subele.Name == "reflected" ? "person" : "photo_doc") +
                                       ratt.Value;
                            triple = Tuple.Create(id, subele.Name.LocalName, (ObjectVariants)new OV_iri(r));
                        }
                        else
                        {
                            string value = subele.Value; // Нужны языки и другие варианты!
                            bool possiblenumber = !string.IsNullOrEmpty(value);
                            if (possiblenumber)
                            {
                                char c = value[0];
                                if (char.IsDigit(c) || c == '-')
                                {
                                }
                                else possiblenumber = false;
                            }
                            triple = Tuple.Create(id, subele.Name.LocalName,possiblenumber
                                    ? (ObjectVariants)new OV_int(int.Parse(value))
                                    : (ObjectVariants)new OV_string(value));
                        }
                        return triple;
                    }));
                return seq;
            };
            store.Load(generator.Generate1of3().SelectMany(f).Concat(generator.Generate2of3().SelectMany(f)).Concat(generator.Generate3of3().SelectMany(f)));
        }
    }
}
