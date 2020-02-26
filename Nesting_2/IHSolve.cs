using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting_2
{
    interface IHSolve
    {
        IList<Sequence> ComputeHeuristic();
    }
}
