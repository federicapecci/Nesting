using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting_4
{
    public interface IUtilities
    {
        double ComputeLowerBound(IList<Item> pricedItems, int binWidth, int binHeight);

        Bin IsBestPositionFound(Bin temporaryBin, Item temporaryPricedItem, string itemAllocationMethod);

        bool IsSolutionCorrect(IList<Item> items, IList<Bin> bins, int iter);

    }
}
