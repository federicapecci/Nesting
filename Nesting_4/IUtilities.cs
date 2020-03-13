using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting_4
{
    interface IUtilities
    {
        float ComputeLowerBound(IList<PricedItem> pricedItems, int binWidth, int binHeight);

        Bin<Tuple> IsBestPositionFound(Bin<Tuple> temporaryBin, PricedItem temporaryPricedItem);

        void UpdatePrice(float z, IList<PricedItem> items, IList<Bin<Tuple>> bins);

    }
}
