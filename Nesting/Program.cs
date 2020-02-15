using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    class Program
    {
        /// <summary>
        /// metodo entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            IHSolve hsolve = new HSolve();
            Bin<Tuple> bin = hsolve.ComputeHeuristic();

            IDrawer drawer = new DxfDrawer(bin);
            drawer.WriteDxfDocument();

        }
    }
}
