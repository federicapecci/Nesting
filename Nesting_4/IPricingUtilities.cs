using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_4
{
    interface IPricingUtilities
    {
        double ComputePricingRule(string pricingRule, double height, double width);

        void ComputePricingUpdateRule(double z, IList<Item> items, IList<Bin> bins, string priceUpdatingRule, string partitionType);

    }
}
