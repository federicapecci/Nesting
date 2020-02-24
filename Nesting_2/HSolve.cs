using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nesting_2
{
    /// <summary>
    /// classe che implemnenta l'euristica 
    /// </summary>
    class HSolve : IHSolve
    {
        /// <summary>
        /// la configurazione conente i parametri con 
        /// cui lanciare l'algortimo hsolve
        /// </summary>
        public Configuration Configuration { get; set; } = null;

        /// <summary>
        /// costruttore in cui setto la configuration di hsolve
        /// </summary>
        /// <param name="configuration"></param>
        public HSolve(Configuration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// metodo che computa l'euristica di hsolve 
        /// </summary>
        public Bin<Tuple> ComputeHeuristic()
        {
            Console.WriteLine("Algorithm started");
            IUtilities utilities = new Utilities();

            //================ STEP 1 - INITIALIZATION ================

            IList<Item> items = new List<Item>();
            int itemNumber = Configuration.Items.Count;

            //configuro gli item
            items = Configuration.Items;

            IList<Bin<Tuple>> bins = new List<Bin<Tuple>>();
            //alloco ogni item j in un bin diverso
            for (int k = 0; k < itemNumber; k++)
            {
                var bin = new Bin<Tuple>()
                {
                    Id = k,
                    Height = Configuration.BinHeight,
                    Width = Configuration.BinWidth,
                    NestedItems = new List<NestedItem>()
                    {
                        new NestedItem(items.ElementAt(k)) {
                            BLpPosition = 0,
                            BLqPosition = 0,
                            BRpPosition = items.ElementAt(k).Width,
                            BRqPosition = 0,
                            TLpPosition = 0,
                            TLqPosition = items.ElementAt(k).Height
                        }
                    },
                    Points = new List<Tuple>()
                    {
                        new Tuple()
                        {
                            Pposition = 0,
                            Qposition = items.ElementAt(k).Height,
                            IsUsed = false
                        },
                        new Tuple()
                        {
                            Pposition = items.ElementAt(k).Width,
                            Qposition = 0,
                            IsUsed = false
                        }
                    }
                };
                bins.Add(bin);
            }

            int zStar = itemNumber;
            int iter = 0;
            float lowerBound = 0;
            int maxIter = Configuration.MaxIter;

        //================ STEP 2 - ERASE THE CURRENT SOLUTION ================

        l3: //creo una lista temporanea di item (che sarebbe J')
            IList<Item> temporaryItems = new List<Item>();
            //assegno la lista di item J a J'
            temporaryItems = items;

            //i = current empty bin index
            int i = 0;

            //creo tanti bin temporanei quanti sono gli item
            IList<Bin<Tuple>> temporaryBins = new List<Bin<Tuple>>();

            for (int k = 0; k < itemNumber; k++)
            {
                var temporaryBin = new Bin<Tuple>()
                {
                    Id = k,
                    Height = Configuration.BinHeight,
                    Width = Configuration.BinWidth,
                    NestedItems = null,
                    Points = new List<Tuple>()
                    {
                        new Tuple()
                        {
                            Pposition = 0,
                            Qposition = 0,
                            IsUsed = false
                        }
                    }
                };
                temporaryBins.Add(temporaryBin);
            }

        //================ STEP 3 - FILLING UP BIN i ================
        l1:
            //int counter = 1;
            foreach (var temporaryItem in temporaryItems)
            {
                //Console.WriteLine("X INS " + counter);
                //TO DO -> FIND THE TRIPLE (p,q,r) FOR PLACING ITEM j IN BIN i 
                bool isBestPositionFound = utilities.IsBestPositionFound(temporaryBins.ElementAt(i), temporaryItem);
                if (!isBestPositionFound)
                {
                    break;
                }
                //counter += 1;
                //Console.WriteLine("\n");
            }

        //================ STEP 4 - CHECK IF ALL ITEMS HAVE BEEN ALLOCATED ================

            int z = i;
            bool isTemporaryItemsEmpty = temporaryItems.Any(x => x.IsRemoved == true);
            if (isTemporaryItemsEmpty)
            {
                goto l0;
            }
            if (!isTemporaryItemsEmpty && (i < (zStar - 1)))
            {
                i += 1;
                goto l1;
            }
            goto l2;

        //================ STEP 5 - UPDATE THE BEST SOLUTION ================

        l0: zStar = z;
            bins[i] = temporaryBins[i];

        //================ STEP 6 - CHECK OPTIMALITY ================

            lowerBound = utilities.ComputeLowerBound(items, bins[i].Width, bins[i].Height);

            if (zStar == lowerBound)
            {
                goto end;
            }

        //================ STEP 7 - UPDATE THE ITEM PRICES ================

        l2: if (iter == maxIter)
            {
                goto end;
            }
            else
            {
                utilities.UpdatePrice(z, items, bins);
                iter += 1;
                goto l3;
            }

        end:
            Console.WriteLine("Algorithm ended");
            return bins[i];
        }
    }  
}
