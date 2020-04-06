using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting_4
{
    interface IHSolve
    {
        Sequence ComputeHeuristic(Configuration configuration, string itemAllocationMethod,
                            string pricingRule, string priceUpdatingRule);
    }
}
