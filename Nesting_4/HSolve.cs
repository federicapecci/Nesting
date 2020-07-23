using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nesting_4
{
    /// <summary>
    /// classe che implementa l'euristica 
    /// </summary>
    class HSolve : IHSolve
    {
        public IUtilities Utilities { get; set; } = null;

        public IPricingUtilities PricingUtilities { get; set; } = null;

        public IOutputUtilities OutputUtilities { get; set; } = null;

        public IHSolveUtilities HSolveUtilities { get; set; } = null;

        public HSolve()
        {
            Utilities = new Utilities();
            PricingUtilities = new PricingUtilities();
            OutputUtilities = new OutputUtilities();
            HSolveUtilities = new HSolveUtilities();
        }

        /// <summary>
        /// metodo che computa l'euristica di hsolve 
        /// </summary>
        public IList<Sequence> ComputeHeuristic(Configuration configuration, string itemAllocationMethod,
                            string pricingRule, string priceUpdatingRule)
        {
            //================ STEP 1 - INITIALIZATION ================

            HSolveUtilities.Initialize(configuration, pricingRule, PricingUtilities);

            //================ STEP 2 - ERASE THE CURRENT SOLUTION ================

            while (HSolveUtilities.GetIter() < HSolveUtilities.GetMaxIter())
            {

                HSolveUtilities.EraseCurrentSolution(configuration);

                //================ STEP 3 - FILLING UP BIN i ================

                HSolveUtilities.FillUpBinI(itemAllocationMethod, Utilities);

                string check = HSolveUtilities.CheckIfAllItemsAreAllocated(configuration, Utilities, PricingUtilities, itemAllocationMethod,
                    OutputUtilities, pricingRule, priceUpdatingRule);

                while (check == "FillUpBinI") { 

                    HSolveUtilities.FillUpBinI(itemAllocationMethod, Utilities);

                    //================ STEP 4 - CHECK IF ALL ITEMS HAVE BEEN ALLOCATED ================

                    check = HSolveUtilities.CheckIfAllItemsAreAllocated(configuration, Utilities, PricingUtilities, itemAllocationMethod,
                       OutputUtilities, pricingRule, priceUpdatingRule);

                }

                if(check == "UpdateItemPrices")
                {
                    HSolveUtilities.UpdateItemPrices(configuration, PricingUtilities, priceUpdatingRule);
                    
                }else if(check == "UpdateBestSolution")
                {
                    HSolveUtilities.UpdateBestSolution(OutputUtilities, itemAllocationMethod, pricingRule, priceUpdatingRule);
                    HSolveUtilities.UpdateItemPrices(configuration, PricingUtilities, priceUpdatingRule);
                }
               
            }

            return HSolveUtilities.GetSequences();

        }
    }
}
