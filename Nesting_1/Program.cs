using System;

namespace Nesting_1
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
