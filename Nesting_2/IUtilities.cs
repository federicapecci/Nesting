using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting_2
{
    interface IUtilities
    {
        float ComputeLowerBound(IList<Item> items, int binWidth, int binHeight);

        bool IsBestPositionFound(Bin<Tuple> temporaryBin, Item temporaryItem);

        void UpdatePrice(float z, IList<Item> items, IList<Bin<Tuple>> bins);

    }
}
