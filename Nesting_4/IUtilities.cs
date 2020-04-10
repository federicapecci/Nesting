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

        Bin<Tuple> IsBestPositionFound(Bin<Tuple> temporaryBin, Item temporaryPricedItem, string itemAllocationMethod);

        bool IsSolutionCorrect(IList<Item> items, IList<Bin<Tuple>> bins, int iter);

        bool IsNewBestWidthFound(IList<Item> nestedItems);

        double GetBestWidthFound();

        double ComputeUsedAreaAbsoluteValue(IList<Item> nestedItems);

        double ComputeUsedAreaPercentageValue(IList<Item> nestedItems, double binHeight, double binWidth);
    }
}
