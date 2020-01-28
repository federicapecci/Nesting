using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    class Program
    {
        static void Main(string[] args)
        {
            IHSolve hsolve = new HSolve();
            hsolve.ComputeHeuristic();
        }
    }
}
