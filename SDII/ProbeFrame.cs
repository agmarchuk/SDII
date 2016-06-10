using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SDII
{
    public class ProbeFrame
    {
        public string tsk = "", sol = "", reg = "", cnf = "", com = "";
        public long siz = 0L;
        public int nte = 0;
        public long lod = -1, ndx = -1, scn = -1, tim = -1, vol = -1, ram = -1, sum = 0;
        DateTime dte = DateTime.Now;

        // Инициализация загрузкой XML-элементом
        public ProbeFrame(XElement el)
        {
            XAttribute a;
            a = el.Attribute("tsk"); if (a != null) tsk = a.Value;
            a = el.Attribute("sol"); if (a != null) sol = a.Value;
            a = el.Attribute("reg"); if (a != null) reg = a.Value;
            a = el.Attribute("cnf"); if (a != null) cnf = a.Value;
            a = el.Attribute("siz"); if (a != null) siz = Int64.Parse(a.Value);
            a = el.Attribute("nte"); if (a != null) nte = Int32.Parse(a.Value);
            a = el.Attribute("com"); if (a != null) com = a.Value;
        }
        // Инициализация множестовом XML-атрибутов
        public ProbeFrame(IEnumerable<XAttribute> atts)
        {
            foreach (XAttribute att in atts)
            {
                if (att.Name == "tsk") tsk = att.Value;
                if (att.Name == "sol") sol = att.Value;
                if (att.Name == "reg") reg = att.Value;
                if (att.Name == "cnf") cnf = att.Value;
                if (att.Name == "tsk") tsk = att.Value;
                if (att.Name == "siz") siz = Int64.Parse(att.Value);
                if (att.Name == "nte") nte = Int32.Parse(att.Value);
                if (att.Name == "com") com = att.Value;
            }
        }
        // Сохранение теста в CSV-строке
        public string ToCSV()
        {
            return 
                tsk + "," +
                sol + "," +
                reg + "," +
                cnf + "," +
                siz + "," +
                nte + "," +
                lod + "," +
                ndx + "," +
                scn + "," +
                tim + "," +
                vol + "," +
                ram + "," +
                sum + "," +
                dte.ToString("s") + "," +
                "\"" + com + "\"";
        }
    }
}
