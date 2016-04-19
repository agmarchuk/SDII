﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SDII
{
    public class TestFrame
    {
        public string tsk = "", sol = "", reg = "", com = "";
        public long siz = 0L;
        public int nte = 0;
        public long lod = -1, ndx = -1, tim = -1, vol = -1, ram = -1, sum = 0;
        DateTime dte = DateTime.Now;

        // Инициализация загрузкой XML-элементом
        public TestFrame(XElement el)
        {
            XAttribute a;
            a = el.Attribute("tsk"); if (a != null) tsk = a.Value;
            a = el.Attribute("sol"); if (a != null) sol = a.Value;
            a = el.Attribute("reg"); if (a != null) reg = a.Value;
            a = el.Attribute("com"); if (a != null) com = a.Value;
            a = el.Attribute("siz"); if (a != null) siz = Int64.Parse(a.Value);
            a = el.Attribute("nte"); if (a != null) nte = Int32.Parse(a.Value);
        }
        // Сохранение теста в CSV-строке
        public string ToCSV()
        {
            return 
                tsk + "," +
                sol + "," +
                reg + "," +
                com + "," +
                siz + "," +
                nte + "," +
                lod + "," +
                ndx + "," +
                tim + "," +
                vol + "," +
                ram + "," +
                sum + "," +
                dte.ToString("s");
        }
    }
}
