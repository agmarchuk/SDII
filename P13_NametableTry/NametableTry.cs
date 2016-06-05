using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PolarDB;
using UniversalIndex;

namespace P13_NametableTry
{
    public class NametableTry
    {
        private TableView tab_person;
        private IndexKeyImmutable<int> ind_arr_person;
        private IndexDynamic<int, IndexKeyImmutable<int>> index_person;
        private IndexViewImmutable<string> ind_arr_person_name;
        private IndexDynamic<string, IndexViewImmutable<string>> index_person_name;
        public NametableTry(string path)
        {
            PType tp_person = new PTypeRecord(
                new NamedType("code", new PType(PTypeEnumeration.integer)),
                new NamedType("name", new PType(PTypeEnumeration.sstring)));
            tab_person = new TableView(path + "nametable", tp_person);
            // Индексы: Персона
            Func<object, int> person_code_keyproducer = v => (int)((object[])((object[])v)[1])[0];
            ind_arr_person = new IndexKeyImmutable<int>(path + "code_ind")
            {
                Table = tab_person,
                KeyProducer = person_code_keyproducer,
                Scale = null
            };
            ind_arr_person.Scale = new ScaleCell(path + "code_ind") { IndexCell = ind_arr_person.IndexCell };
            index_person = new IndexDynamic<int, IndexKeyImmutable<int>>(true)
            {
                Table = tab_person,
                IndexArray = ind_arr_person,
                KeyProducer = person_code_keyproducer
            };
            Func<object, string> name_keyproducer = v => (string)((object[])((object[])v)[1])[1];
            ind_arr_person_name = new IndexViewImmutable<string>(path + "name_ind")
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
        }
        public void Clear() { tab_person.Clear(); }
        public void Warmup() { tab_person.Warmup(); index_person.Warmup(); index_person_name.Warmup(); }
        public void BuildTable(IEnumerable<XElement> records)
        {
            tab_person.Clear();
            //tab_person.Fill(new object[0]);
            IEnumerable<object> flow = records
                .Select(rec =>
                {
                    int code = Int32.Parse(rec.Attribute("id").Value);
                    string name = rec.Element("name").Value;
                    return new object[] { code, name };
                }).ToArray();
            tab_person.Fill(flow);
        }
        // Построение индексов
        public void BuildIndexes()
        {
            ind_arr_person.Build();
            ind_arr_person_name.Build();
        }
        public string GetNameByCode(int code)
        {
            var ob = index_person.GetAllByKey(code)
                .Select(ent => ((object[])ent.Get())[1])
                .Cast<object[]>()
                .FirstOrDefault();
            return (string)ob[1];
        }
        public int GetPersonsByName(string name)
        {
            PaEntry entry = tab_person.Element(0);
            int code = index_person_name.GetAllByKey(name)
                .Select(ent =>
                {
                    object[] obs = (object[])ent.Get();
                    return obs;
                })
                .Where(obs => !(bool)obs[0])
                .Select(obs => (int)((object[])obs[1])[0])
                .FirstOrDefault();
            if (code.ToString() != name) Console.WriteLine("code={0} name={1}", code, name);
            return code;
        }
    }
}
