using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDII
{
   public static class ProjectDirectory
    {
       public static string GetProjectDirectory()
       {
          var baseBase= new DirectoryInfo("../../");
           if (baseBase.Name == "bin")
               return baseBase.Parent.FullName+"/";
           return baseBase.FullName;
       }
    }
}
