using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using SDII;

namespace P01_PlatformProperties
{
    public class Program
    {
        public static void Main()
        {
            string path = @"..\..\..\";
            if (!System.IO.File.Exists(path + "cnf.xml")) System.IO.File.Copy(path + "cnf0.xml", path + "cnf.xml");
            XElement xcnf = XElement.Load(path + "cnf.xml");
                
            ProbeFrame pf = new ProbeFrame(xcnf);
        }

    }
}
