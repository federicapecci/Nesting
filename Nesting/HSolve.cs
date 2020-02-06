using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nesting
{
    class HSolve : IHSolve
    {
        public void ComputeHeuristic()
        {
            Console.WriteLine("Algorithm started");
            IUtilities utilities = new Utilities();

            //================ STEP 1 - INITIALIZATION ================

            IList<Item> items = new List<Item>();
            int itemNumber = 12;

            for (int k = 0; k < itemNumber; k++)
            {
                items.Add(new Item()
                {
                    Id = k,
                    Height = 2,
                    Width = 2,
                    Price = 4,
                    IsRemoved = false
                });
            }

            IList<Bin<Tuple>> bins = new List<Bin<Tuple>>();
            //alloco ogni item j in un bin diverso
            for (int k = 0; k < itemNumber; k++)
            {
                var bin = new Bin<Tuple>()
                {
                    Id = k,
                    Height = 5,
                    Width = 7,
                    OrientedItems = new List<OrientedItem>()
                    {
                        new OrientedItem(items.ElementAt(k)){
                            Pposition = 0,
                            Qposition = 0,
                            Rotation = false,
                        }
                    },
                    Points = new List<Tuple>()
                    {
                        new Tuple()
                        {
                            Pposition = 0,
                            Qposition = 2,
                            IsUsed = false
                        },
                        new Tuple()
                        {
                            Pposition = 2,
                            Qposition = 0,
                            IsUsed = false
                        }
                    }
                };
                bins.Add(bin);
            }

            int zStar = itemNumber;
            int iter = 0;
            float lowerBound = utilities.ComputeLowerBound(items, bins[0].Width, bins[0].Height);
            int maxIter = 100;

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
                    Height = 5,
                    Width = 7,
                    OrientedItems = null,
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

        l1: foreach (var temporaryItem in temporaryItems)
            {
                //TO DO -> FIND THE TRIPLE (p,q,r) FOR PLACING ITEM j IN BIN i 
                bool isBestPositionFound = utilities.IsBestPositionFound(temporaryBins.ElementAt(i), temporaryItem);
                if (!isBestPositionFound)
                {
                    break;
                }
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
                utilities.UpdatePrice(z, items, temporaryBins[i]);
                iter += 1;
                goto l3;
            }

        end: Console.WriteLine("Algorithm ended");
        }
    }  
}
