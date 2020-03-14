using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_4
{
    interface IPricingUtilities
    {
        float ComputePricingRule(string pricingRule, float height, float width);

        void ComputePricingUpdateRule(float z, IList<Item> items, IList<Bin<Tuple>> bins, string priceUpdatingRule);

    }
}
