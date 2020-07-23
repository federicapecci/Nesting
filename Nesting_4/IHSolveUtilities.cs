using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_4
{
    interface IHSolveUtilities
    {

        void Initialize(Configuration configuration, string pricingRule, IPricingUtilities PricingUtilities);

        void EraseCurrentSolution(Configuration configuration);

        void FillUpBinI(string itemAllocationMethod, IUtilities Utilities);

        string CheckIfAllItemsAreAllocated(Configuration configuration, IUtilities Utilities, IPricingUtilities PricingUtilities,
           string itemAllocationMethod, IOutputUtilities OutputUtilities, string pricingRule, string priceUpdatingRule);

        void UpdateBestSolution(IOutputUtilities OutputUtilities, string itemAllocationMethod, string pricingRule, string priceUpdatingRule);

        void UpdateItemPrices(Configuration configuration, IPricingUtilities PricingUtilities, string priceUpdatingRule);

        IList<Sequence> GetSequences();

        int GetIter();

        int GetMaxIter();

    }
}
