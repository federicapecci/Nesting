using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    interface IUtilities
    {
        float ComputeLowerBound(IList<Item> items, int binWidth, int binHeight);

        bool IsBestPositionFound(Bin temporaryBin, Item temporaryItem);

        void UpdatePrice(int z, IList<Item> items, Bin temporaryBin);

    }
}
