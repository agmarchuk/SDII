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
        public Phototeka(int npersons, int seed)
        {
            this.npersons = npersons;
            this.nphotos = npersons * 2;
            this.nreflections = npersons * 6;
            this.seed = seed;
        }
        public IEnumerable<XElement> Generate()
        {
            Random rnd = new Random(seed);
            for (int i = 0; i < npersons; i++)
            {
                yield return new XElement("person", new XAttribute("id", i),
                    new XElement("name", "Пупкин" + i + "_" + rnd.Next(npersons)),
                    new XElement("age", 20 + rnd.Next(80)));
            }
            for (int i = 0; i < nphotos; i++)
            {
                yield return new XElement("photo_doc", new XAttribute("id", i),
                    new XElement("name", "DSP" + i));
            }
            for (int i = 0; i < nreflections; i++)
            {
                yield return new XElement("reflection", new XAttribute("id", i),
                    new XElement("reflected", new XAttribute("ref", rnd.Next(npersons - 1))),
                    new XElement("in_doc", new XAttribute("ref", rnd.Next(nphotos - 1))));

            }
        }

        public IEnumerable<Person> GeneratePersons()
        {
            Random rnd = new Random(seed);

            for (int i = 0; i < npersons; i++)
            {
                yield return new Person()
                {
                    Id = i.ToString(),
                    Name = "Пупкин" + i + "_" + rnd.Next(npersons),
                    Age = 20 + rnd.Next(80)
                };
            }
        }
        public IEnumerable<Photo> GeneratePhotos()
        {
            Random rnd = new Random(seed);

            for (int i = 0; i < nphotos; i++)
            {
                yield return new Photo()
                {
                    Id = i.ToString(),
                    Name = "DSP" + i
                };
            }
        }
        public IEnumerable<Reflection> GenerateReflections()
        {
            Random rnd = new Random(seed);

            for (int i = 0; i < npersons; i++)
            {
                yield return new Reflection()
                {
                    Id = i.ToString(),
                    PersonId = rnd.Next(npersons - 1).ToString(),
                    PhotoId = rnd.Next(nphotos - 1).ToString()
                };
            }
        }

        public IEnumerable<Tuple<string, string, object>> GenerateTriples()
        {
            Random rnd = new Random(seed);
            for (int i = 0; i < npersons; i++)
            {
                var id = i.ToString();
                yield return Tuple.Create(id, "a", (object)"person");
                yield return Tuple.Create(id, "name", (object)("Пупкин" + i + "_" + rnd.Next(npersons)));
                    //new XElement("age", 20 + rnd.Next(80)));
            }
            throw new NotImplementedException();
        }
    public class Person
{
    public string Id { get; set; }
    public int Age { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Photo> Photos { get; set; }
}

public class Photo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Person> Persons { get; set; }
}

public class Reflection
{
    public string Id { get; set; }
    public string PersonId { get; set; }
    public string PhotoId { get; set; }
}
    }
}
