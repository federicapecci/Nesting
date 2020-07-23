using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_4
{
    interface IOutputUtilities
    {

         bool IsNewBestWidthFound(Bin bin);

         double GetBestWidthFound();

         double ComputeUsedAreaAbsoluteValue(IList<Item> nestedItems);

         double ComputeUsedAreaPercentageValue(double binHeight, double binWidth);

         bool IsNewBestAreaFound(Bin bin);

         double GetBestAreaFound();

        double ComputeWidthLastBin(IList<Item> nestedItems);
    }
}
