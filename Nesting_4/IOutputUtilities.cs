using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_4
{
    interface IOutputUtilities
    {

         bool IsNewBestWidthFound(IList<Item> nestedItems);

         double GetBestWidthFound();

         double ComputeUsedAreaAbsoluteValue(IList<Item> nestedItems);

         double ComputeUsedAreaPercentageValue(IList<Item> nestedItems, double binHeight, double binWidth);

         bool IsNewBestAreaFound(IList<Item> nestedItems);

         double GetBestAreaFound();
    }
}
