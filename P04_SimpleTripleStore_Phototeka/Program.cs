using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P04_SimpleTripleStore_Phototeka
{
    public static class Program
    {
        static void Main(string[] args)
        {
            string path = "../../../Databases/simple triple store";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            Console.WriteLine("Start TestGenerator");
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            SimpleTripleStore.SimpleTripleStore simpleTripleStore=new SimpleTripleStore.SimpleTripleStore(path, 1000*1000);
        
            int npersons = 400000; // 40 тыс. персон соответствует 1 млн. триплетов 
            bool toload = true;
            if (toload)
            {
                Directory.Delete(path);
                sw.Restart();
                Polar.Data.Phototeka generator = new Polar.Data.Phototeka(npersons, 777777);
                simpleTripleStore.Build(generator.GenerateRDF());
                sw.Stop();
                Console.WriteLine("Load ok. Duration={0}", sw.ElapsedMilliseconds); // 10000: 14.9 сек.
            }

            sw.Restart();
            simpleTripleStore.GetSubjects("a","person").Count();
            sw.Stop();
            Console.WriteLine("Count ok. Duration={0}", sw.ElapsedMilliseconds); // 1200

            sw.Restart();
            simpleTripleStore.GetDirects(2870).Count();
            //db.SelectById(1309, "person");
            sw.Stop();
            Console.WriteLine("SelectById ok. Duration={0}", sw.ElapsedMilliseconds); // 7

            sw.Restart();
            simpleTripleStore.GetSubjects("name", "Pupkin999").Count();
            sw.Stop();
            Console.WriteLine("SearchByName ok. Duration={0}", sw.ElapsedMilliseconds); // 7

            sw.Restart();
            simpleTripleStore.GetSubjects("reflected", 2870.ToString());
            //db.GetRelation(1309, "reflected");
            sw.Stop();
            Console.WriteLine("GetRelation ok. Duration={0}", sw.ElapsedMilliseconds); // 91

            // Проверка серии портретов
            Random rnd = new Random(999999); int i = 0;
            sw.Restart();
            for (; i < 100; i++) simpleTripleStore.GetSubjects("reflected", rnd.Next(10000).ToString());
            sw.Stop();
            System.Console.WriteLine("GetPortraitById OK. times={0} duration={1}", i, sw.ElapsedMilliseconds); // 1227 ms. 

            // Проверка серии портретов (2) по тем же идентификаторам
            rnd = new Random(999999); i = 0;
            sw.Restart();
            for (; i < 100; i++) simpleTripleStore.GetSubjects("reflected", (rnd.Next(10000)).ToString());
            sw.Stop();
            System.Console.WriteLine("GetPortraitById OK. times={0} duration={1}", i, sw.ElapsedMilliseconds); // 43 ms. 

            // 
            Console.WriteLine("==== Динамика естественного разогрева ====");
            Random rnd2 = new Random();
            for (int j = 0; j < 100; j++)
            {
                sw.Restart();
                for (int ii = 0; ii < 100; ii++) simpleTripleStore.GetSubjects("reflected", rnd2.Next(npersons - 1).ToString());
                sw.Stop();
                Console.Write("{0} ", sw.ElapsedMilliseconds);
            }
            System.Console.WriteLine(); // 
        }
    }
}
