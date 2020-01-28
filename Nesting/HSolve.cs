using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    internal class HSolve : IHSolve
    {
        public void ComputeHeuristic()
        {
            Console.WriteLine("Algorithm started");

            //================ STEP 1 - INITIALIZATION ================

            
            IList<Item> items = new List<Item>();
            int itemNumber = 5;
          
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

            IList<Bin> bins = new List<Bin>();
            //alloco ogni item j in un bin diverso
            for (int k = 0; k < itemNumber; k++)
            {
                var bin = new Bin()
                {
                    Id = k,
                    Height = 6,
                    Width = 8,
                    OrientedItems = new List<OrientedItem>()
                    {
                        new OrientedItem(items.ElementAt(k)){
                            Pposition = 0,
                            Qposition = 0,
                            Rotation = false,
                        }
                    }
                };
                bins.Add(bin);
            }

            int zStar = itemNumber;
            int iter = 0;
            float lowerBound = ComputeLowerBound(items, bins[0].Width, bins[0].Height);
            int maxIter = 100;

            //================ STEP 2 - ERASE THE CURRENT SOLUTION ================

      l3:   //creo una lista temporanea di item (che sarebbe J')
            IList<Item> temporaryItems = new List<Item>();
            //assegno la lista di item J a J'
            temporaryItems = items;

            //i = current empty bin index
            int i = 0;

            //creo tanti bin temporanei quanti sono gli item
            IList<Bin> temporaryBins = new List<Bin>();

            for (int k = 0; k < itemNumber; k++)
            {
                var temporaryBin = new Bin()
                {
                    Id = k,
                    Height = 6,
                    Width = 8,
                    OrientedItemNumber = itemNumber,
                    OrientedItems = null,
                    AvailableTriples = new List<Triple>()
                    {
                        new Triple()
                        {
                            Pposition = 0,
                            Qposition = 0,
                            Rotation = false,
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
                bool isBestPositionFound = IsBestPositionFound(temporaryBins.ElementAt(i), temporaryItem);
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
            if (!isTemporaryItemsEmpty && (i < zStar - 1))
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
                UpdatePrice(z, items);
                iter += 1;
                goto l3;
            }

        end: Console.WriteLine("Algorithm ended");
        }

        private bool IsBestPositionFound(Bin temporaryBin, Item temporaryItem)
        {
            bool result = false;
            //trovo la posizione per il primo elemento del bin
            if (temporaryBin.OrientedItems == null)
            {
                temporaryBin.OrientedItems.Add(new OrientedItem(temporaryItem)
                {
                    Pposition = 0,
                    Qposition = 0
                });
                temporaryBin.AvailableTriples.ElementAt(0).IsUsed = true;

                temporaryBin.AvailableTriples.Add(new Triple()
                {
                    Pposition = 0,
                    Qposition = temporaryItem.Height
                });

                temporaryBin.AvailableTriples.Add(new Triple()
                {
                    Pposition = temporaryItem.Width,
                    Qposition = 0
                });
                temporaryItem.IsRemoved = true;
                result = true;
                return result;
            }
            else
            {
                //trovo la posizione per l'n-esimo elemento del bin

                //calcolo p min tra le triple che non sono ancora state usate
                    // (-2.0 verifico che le dim del nuovo elemento non escano dal bin
                    //se escono cerco q min
                    //verifico che le dim del nuovo elemento non escano dal bin
                    //se escono, return false)
                var triple = temporaryBin.AvailableTriples
                                .Where(x => x.IsUsed == false) // tra tutte le tuple non ancora usate 
                                .OrderBy(x => x.Pposition).FirstOrDefault(); //le ordine per p crescente e prendo la prima p

                //aggiungo un nuovo item al bin 
                temporaryBin.OrientedItems.Add(new OrientedItem(temporaryItem)
                {
                    Pposition = triple.Pposition,
                    Qposition = triple.Qposition
                });

                //marco cancellata la tripla usata per posizioare l'elemento
                var tripleUsed = temporaryBin.AvailableTriples.Where(x => x == triple).FirstOrDefault();
                tripleUsed.IsUsed = true;

                //aggiorno le triple disponibili (aggiungo le due del nuovo elemento agganciato)
                temporaryBin.AvailableTriples.Add(new Triple()
                {
                    Pposition = triple.Pposition + temporaryItem.Width,
                    Qposition = triple.Qposition
                });

                temporaryBin.AvailableTriples.Add(new Triple()
                {
                    Pposition = triple.Pposition,
                    Qposition = triple.Qposition + temporaryItem.Height
                });
                temporaryItem.IsRemoved = true;
                result = true;
                return result;
            }
        }

        /// <summary>
        /// this method computes the continuos lower bound L0
        /// (ref. pag 29 part I of the paper)
        /// </summary>
        /// <param name="items"></param>
        /// <param name="binWidth"></param>
        /// <param name="binHeight"></param>
        /// <returns></returns>
        private float ComputeLowerBound(IList<Item> items, int binWidth, int binHeight)
        {
            float result = 0F;

            foreach (var item in items)
            {
                result += item.Price;
            }

            result /= (binWidth * binHeight);

            return result;
        }

        private void UpdatePrice(int z, IList<Item> items)
        {
            float alpha = 0.9F;
            float beta = 1.1F;

            foreach(var item in items)
            {
                item.Price = alpha * item.Price;
            }

        }
    }
}
