using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PolarDB;
using UniversalIndex;

namespace P05_Phototeka3TabsInt
{
    public class Stan3TabsInt
    {
        //private string path;
        private TableView tab_person, tab_photo_doc, tab_reflection;
        private IndexKeyImmutable<int> ind_arr_person, ind_arr_photo_doc, ind_arr_reflected, ind_arr_in_doc;
        private IndexDynamic<int, IndexKeyImmutable<int>> index_person, index_photo_doc, index_reflected, index_in_doc;
        private IndexViewImmutable<string> ind_arr_person_name;
        private IndexDynamic<string, IndexViewImmutable<string>> index_person_name;
        public Stan3TabsInt(string path)
        {
            PType tp_person = new PTypeRecord(
                new NamedType("code", new PType(PTypeEnumeration.integer)),
                new NamedType("name", new PType(PTypeEnumeration.sstring)),
                new NamedType("age", new PType(PTypeEnumeration.integer)));
            PType tp_photo_doc = new PTypeRecord(
                new NamedType("code", new PType(PTypeEnumeration.integer)),
                new NamedType("name", new PType(PTypeEnumeration.sstring)));
            PType tp_reflection = new PTypeRecord(
                new NamedType("code", new PType(PTypeEnumeration.integer)), // Может не быть
                new NamedType("reflected", new PType(PTypeEnumeration.integer)), // ссылки на коды
                new NamedType("in_doc", new PType(PTypeEnumeration.integer)));
            tab_person = new TableView(path + "person", tp_person);
            tab_photo_doc = new TableView(path + "photo_doc", tp_photo_doc);
            tab_reflection = new TableView(path + "reflection", tp_reflection);
            // Индексы: Персона
            Func<object, int> person_code_keyproducer = v => (int)((object[])((object[])v)[1])[0];
            ind_arr_person = new IndexKeyImmutable<int>(path + "person_ind")
            {
                Table = tab_person,
                KeyProducer = person_code_keyproducer,
                Scale = null
            };
            ind_arr_person.Scale = new ScaleCell(path + "person_ind") { IndexCell = ind_arr_person.IndexCell };
            index_person = new IndexDynamic<int, IndexKeyImmutable<int>>(true)
            {
                Table = tab_person,
                IndexArray = ind_arr_person,
                KeyProducer = person_code_keyproducer
            };
            // Индексы - документ
            Func<object, int> photo_doc_code_keyproducer = v => (int)((object[])((object[])v)[1])[0];
            ind_arr_photo_doc = new IndexKeyImmutable<int>(path + "photo_doc_ind")
            {
                Table = tab_photo_doc,
                KeyProducer = photo_doc_code_keyproducer,
                Scale = null
            };
            ind_arr_photo_doc.Scale = new ScaleCell(path + "photo_doc_ind") { IndexCell = ind_arr_photo_doc.IndexCell };
            index_photo_doc = new IndexDynamic<int, IndexKeyImmutable<int>>(true)
            {
                Table = tab_photo_doc,
                IndexArray = ind_arr_photo_doc,
                KeyProducer = photo_doc_code_keyproducer
            };
            // Индекс - reflection-reflected
            Func<object, int> reflected_keyproducer = v => (int)((object[])((object[])v)[1])[1];
            ind_arr_reflected = new IndexKeyImmutable<int>(path + "reflected_ind")
            {
                Table = tab_reflection,
                KeyProducer = reflected_keyproducer,
                Scale = null
            };
            ind_arr_reflected.Scale = new ScaleCell(path + "reflected_ind") { IndexCell = ind_arr_reflected.IndexCell };
            index_reflected = new IndexDynamic<int, IndexKeyImmutable<int>>(false)
            {
                Table = tab_reflection,
                IndexArray = ind_arr_reflected,
                KeyProducer = reflected_keyproducer
            };
            // Индекс - reflection-in_doc
            Func<object, int> in_doc_keyproducer = v => (int)((object[])((object[])v)[1])[2];
            ind_arr_in_doc = new IndexKeyImmutable<int>(path + "in_doc_ind")
            {
                Table = tab_reflection,
                KeyProducer = in_doc_keyproducer,
                Scale = null
            };
            ind_arr_in_doc.Scale = new ScaleCell(path + "in_doc_ind") { IndexCell = ind_arr_in_doc.IndexCell };
            index_in_doc = new IndexDynamic<int, IndexKeyImmutable<int>>(false)
            {
                Table = tab_reflection,
                IndexArray = ind_arr_in_doc,
                KeyProducer = in_doc_keyproducer
            };
            Func<object, string> name_keyproducer = v => (string)((object[])((object[])v)[1])[1];
            ind_arr_person_name = new IndexViewImmutable<string>(path + "personname_ind")
            {
                Table = tab_person,
                KeyProducer = name_keyproducer,
                Scale = null
            };
            index_person_name = new IndexDynamic<string, IndexViewImmutable<string>>(false)
            {
                Table = tab_person,
                IndexArray = ind_arr_person_name,
                KeyProducer = name_keyproducer
            };
            tab_person.RegisterIndex(index_person);
            tab_person.RegisterIndex(index_person_name);
            tab_photo_doc.RegisterIndex(index_photo_doc);
            tab_reflection.RegisterIndex(index_reflected);
            tab_reflection.RegisterIndex(index_in_doc);
        }
        public void Clear() { tab_person.Clear(); tab_photo_doc.Clear(); tab_reflection.Clear(); }
        public void BuildPersons(IEnumerable<XElement> records)
        {
            IEnumerable<object> flow = records
                .Select(rec =>
                {
                    int code = Int32.Parse(rec.Attribute("id").Value);
                    string name = rec.Element("name").Value;
                    int age = Int32.Parse(rec.Element("age").Value);
                    return new object[] { code, name, age };
                });
            tab_person.Fill(flow);

            index_person.FillFinish();
            index_person_name.FillFinish();
        }
        public void BuildPhoto_docs(IEnumerable<XElement> records)
        {
            index_photo_doc.FillInit();
            IEnumerable<object> flow = records
                .Select(rec =>
                {
                    int code = Int32.Parse(rec.Attribute("id").Value);
                    string name = rec.Element("name").Value;
                    return new object[] { code, name };
                });
            tab_photo_doc.Fill(flow);
            index_photo_doc.FillFinish();
        }
        public void BuildReflections(IEnumerable<XElement> records)
        {
            index_reflected.FillInit();
            index_in_doc.FillInit();

            IEnumerable<object> flow = records
                .Select(rec =>
                {
                    int code = Int32.Parse(rec.Attribute("id").Value);
                    int reflected = Int32.Parse(rec.Element("reflected").Attribute("ref").Value);
                    int in_doc = Int32.Parse(rec.Element("in_doc").Attribute("ref").Value);
                    return new object[] { code, reflected, in_doc };
                });
            tab_reflection.Fill(flow);

            index_reflected.FillFinish();
            index_in_doc.FillFinish();
        }
        public void Build(IEnumerable<XElement> records)
        {
            this.Clear();
            
            tab_person.Fill(new object[0]);
            tab_photo_doc.Fill(new object[0]);
            tab_reflection.Fill(new object[0]);
            foreach (XElement rec in records)
            {
                int code = Int32.Parse(rec.Attribute("id").Value);
                if (rec.Name == "person")
                {
                    string name = rec.Element("name").Value;
                    int age = Int32.Parse(rec.Element("age").Value);
                    tab_person.AppendValue(new object[] { code, name, age });
                }
                else if (rec.Name == "photo_doc")
                {
                    string name = rec.Element("name").Value;
                    tab_photo_doc.AppendValue(new object[] { code, name });
                }
                else if (rec.Name == "reflection")
                {
                    int reflected = Int32.Parse(rec.Element("reflected").Attribute("ref").Value);
                    int in_doc = Int32.Parse(rec.Element("in_doc").Attribute("ref").Value);
                    tab_reflection.AppendValue(new object[] { code, reflected, in_doc });
                }
            }
            index_in_doc.FillFinish();
            index_person.FillFinish();
            index_person_name.FillFinish();
            index_photo_doc.FillFinish();
            index_reflected.FillFinish();
        }
        // Построение индексов
        public void BuildIndexes()
        {
            index_person.Build();
            index_photo_doc.Build();
            index_reflected.Build();
            index_in_doc.Build();
            index_person_name.Build();
        }
        public object[] GetPersonByCode(int code)
        {
            var ob = index_person.GetAllByKey(code)
                .Select(ent => ((object[])ent.Get())[1])
                .FirstOrDefault();
            return (object[])ob;
        }
        public object[] GetPhoto_docByCode(int code)
        {
            var ob = index_photo_doc.GetAllByKey(code)
                .Select(ent => ((object[])ent.Get())[1])
                .FirstOrDefault();
            return (object[])ob;
        }
        /// <summary>
        /// По заданному коду персоны, выдет множество отношений, в которых код присутствует на соответстующем месте. 
        /// </summary>
        /// <param name="code"></param>
        public IEnumerable<object> GetReflectionsByReflected(int code)
        {
            var query = index_reflected.GetAllByKey(code)
                .Select(ent => ((object[])ent.Get())[1])
                .Select(re => (int)((object[])re)[2])
                .Select(c => this.GetPhoto_docByCode(c))
                .Select(ee => (object)ee)
                ;
            return query;
        }
        public IEnumerable<object> GetPersonsByName(string firstpartofname)
        {
            firstpartofname = firstpartofname.ToLower();
            PaEntry entry = tab_person.Element(0);
            var query = index_person_name.GetAllByLevel(s =>
            {
                string ss = s.ToLower();
                if (ss.StartsWith(firstpartofname)) return 0;
                return ss.CompareTo(firstpartofname);
            });
            return query
                .Select(ent =>
            {
                //long off = (long)ent.Get();
                //entry.offset = off;
                return entry.Get();
            })
            .Cast<object[]>()
            .Where(pair => !((bool)pair[0]))
            .Select(pair => pair[1]);
        }

        public void TestCounts()
        {
            Console.WriteLine("person "+tab_person.Count());
            Console.WriteLine("photos "+tab_photo_doc.Count());
            Console.WriteLine("reflections "+ tab_reflection.Count());
            Console.WriteLine("index_in_doc " + index_in_doc.Count());
            Console.WriteLine("index_person " + index_person.Count());
            Console.WriteLine("index_person_name " + index_person_name.Count());
            Console.WriteLine("index_photo_doc " + index_photo_doc.Count());
            Console.WriteLine("index_reflected " + index_reflected.Count());
            Console.WriteLine(index_person_name.GetAllByKey(index_person_name.KeyProducer( tab_person.TableCell.Root.Element(0).Get())).Count());
        }

        
    }
}
