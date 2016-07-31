using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_Indexing
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            int n_elements = 100000000;
            int[] vals = Enumerable.Range(0, n_elements)
                .Select(x => n_elements - x)
                .Select(x => rnd.Next())
                .ToArray();

            int[] ii = Enumerable.Range(0, n_elements).ToArray();
            sw.Restart();
            Array.Sort(vals, ii);
            sw.Stop();
            Console.WriteLine("{0} ms. for {1} elements", sw.ElapsedMilliseconds, n_elements);
        }
    }
}
