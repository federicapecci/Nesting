using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_4
{
    public class OutputUtilities : IOutputUtilities
    {

        public double CurrentBestWidth { get; set; } = double.MaxValue;

        public double CurrentBestArea { get; set; } = double.MaxValue;

        /// <summary>
        /// questo metodo calcola in un bin la massima lunghezza occupata dagli item
        /// </summary>
        /// <param name="bins"></param>
        /// <returns></returns>
        public bool IsNewBestWidthFound(IList<Item> nestedItems) 
        {
            double currentWidth = double.MinValue;

            foreach (var nestedItem in nestedItems)
            {
                if (nestedItem.BRpPosition > currentWidth)
                {
                    currentWidth = nestedItem.BRpPosition;
                }
            }

            if (currentWidth < CurrentBestWidth)
            {
                CurrentBestWidth = currentWidth;
                return true;
            }
            return false;

        }

        public double GetBestWidthFound() 
        {
            return CurrentBestWidth;
        }

        public double ComputeUsedAreaAbsoluteValue(IList<Item> nestedItems) 
        {
            double usedArea = 0;
            foreach (var nestedItem in nestedItems)
            {
                usedArea += nestedItem.Height * nestedItem.Width;
            }
            return usedArea;
        }

        public double ComputeUsedAreaPercentageValue(IList<Item> nestedItems, double binHeight, double binWidth) 
        {
            double usedArea = 0;
            double percentage;
            foreach (var nestedItem in nestedItems)
            {
                usedArea += nestedItem.Height * nestedItem.Width;
            }

            //x : 100 = area usata: area totale
            percentage = usedArea * 100 / (binHeight * binWidth);
            percentage = Math.Round(percentage, 2, MidpointRounding.AwayFromZero);

            return percentage;
        }

        public bool IsNewBestAreaFound(IList<Item> nestedItems)
        {
            double currentArea = 0;

            foreach (var nestedItem in nestedItems)
            {
                currentArea += nestedItem.Height * nestedItem.Width;
            }

            if (currentArea < CurrentBestArea)
            {
                CurrentBestArea = currentArea;
                return true;
            }
            return false;

        }

        public double GetBestAreaFound()
        {
            return CurrentBestArea;
        }

        public double ComputeWidthLastBin(IList<Item> nestedItems)
        {
            double currentWidth = double.MinValue;

            foreach (var nestedItem in nestedItems)
            {
                if (nestedItem.BRpPosition > currentWidth)
                {
                    currentWidth = nestedItem.BRpPosition;
                }
            }

            return currentWidth;
        }

    }
}
