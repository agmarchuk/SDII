using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Polar.Data
{
    public class Phototeka
    {
        // Одна персона дает 3 + 2*2 + 6*3 = 25 триплетов
        private int npersons, nphotos, nreflections;
        private int seed;
        private Random rnd;
        public Phototeka(int npersons, int seed)
        {
            this.npersons = npersons;
            this.nphotos = npersons * 2;
            this.nreflections = npersons * 6;
            this.seed = seed;
            this.rnd = new Random(seed);
        }
        public IEnumerable<XElement> Generate1of3()
        {
            for (int i = 0; i < npersons; i++)
            {
                yield return new XElement("person", new XAttribute("id", i),
                    new XElement("name", "Пупкин" + i + "_" + rnd.Next(npersons)),
                    new XElement("age", 20 + rnd.Next(80)));
            }
        }
        public IEnumerable<XElement> Generate2of3()
        {
            for (int i = 0; i < nphotos; i++)
            {
                yield return new XElement("photo_doc", new XAttribute("id", i),
                    new XElement("name", "DSP" + i));
            }
        }
        public IEnumerable<XElement> Generate3of3()
        {
            for (int i = 0; i < nreflections; i++)
            {
                yield return new XElement("reflection", new XAttribute("id", i),
                    new XElement("reflected", new XAttribute("ref", rnd.Next(npersons - 1))),
                    new XElement("in_doc", new XAttribute("ref", rnd.Next(nphotos - 1))));
            }
        }

        public IEnumerable<Tuple<int, string, string>> GenerateRDF()
        {
            for (int i = 0; i < npersons; i++)
            {
                yield return Tuple.Create(i, "a", "person");
                yield return Tuple.Create(i, "name", "Пупкин" + i + "_" + rnd.Next(npersons));
                yield return Tuple.Create(i, "age", (20 + rnd.Next(80)).ToString());
            }
            for (int i = 0; i < nphotos; i++)
            {
                yield return Tuple.Create(i, "a", "photo_doc");
                yield return Tuple.Create(i, "name", "DSP" + i);
            }
            for (int i = 0; i < nreflections; i++)
            {
                yield return Tuple.Create(i, "a", "reflection");
                yield return Tuple.Create(i, "reflected", rnd.Next(npersons - 1).ToString());
                yield return Tuple.Create(i, "in_doc", rnd.Next(nphotos - 1).ToString());
            }
        }
    }
}
