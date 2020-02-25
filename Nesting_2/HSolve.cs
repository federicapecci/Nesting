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
        public IList<Bin<Tuple>> ComputeHeuristic()
        {
            Console.WriteLine("Algorithm started");
            IUtilities utilities = new Utilities();

            //================ STEP 1 - INITIALIZATION ================

            //configuro gli item (e inizializzo il prezzo associato ad ogni item j)
            IList<Item> items = new List<Item>();
            items = Configuration.Items;

            int itemNumber = Configuration.Items.Count;

            //creo i bin pi greco e alloco ogni item in un bin diverso
            IList<Bin<Tuple>> bins = new List<Bin<Tuple>>();
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

            //inizializzo il costo della soluzione
            int zStar = itemNumber;
            //inizializzo il numero di iterazioni
            int iter = 0;
            float lowerBound = utilities.ComputeLowerBound(items, Configuration.BinWidth, Configuration.BinHeight);
            float lowerBoundMin = lowerBound - Configuration.LowerBoundDelta;
            float lowerBoundMax = lowerBound + Configuration.LowerBoundDelta;
            int maxIter = Configuration.MaxIter;

        //================ STEP 2 - ERASE THE CURRENT SOLUTION ================

        l3: //creo una lista temporanea J' di item 
            IList<Item> temporaryItems = new List<Item>();

            //assegno la lista di item J a J'
            temporaryItems = items;

            //setto il bin che considero al momento
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

            var sortedTemporaryItems = temporaryItems.OrderByDescending(x => x.Price);

        //================ STEP 3 - FILLING UP BIN i ================
        l1://cerco la posizione migliore per ogni item j'
            foreach (var sortedTemporaryItem in sortedTemporaryItems)
            {
                if (!sortedTemporaryItem.IsRemoved)
                {
                    utilities.IsBestPositionFound(temporaryBins.ElementAt(i), sortedTemporaryItem);
                }
            }

            //================ STEP 4 - CHECK IF ALL ITEMS HAVE BEEN ALLOCATED ================
            int z = i;
            bool isTemporaryItemsEmpty = true;
            
            //controllo se tutta la lista è stata svuotata
            foreach(var temporaryItem in sortedTemporaryItems)
            {
                if (temporaryItem.IsRemoved == false)
                {
                    isTemporaryItemsEmpty = false;
                    break;
                }
            }

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
            bins = temporaryBins;

        //================ STEP 6 - CHECK OPTIMALITY ================
        //guardo se il costo della soluzione è compreso nell'intervallo del lower bound 
            if (zStar > lowerBoundMin && zStar < lowerBoundMax) 
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
                //rimetto tutti gli item a non removed perché cominicio una nuova iterazione
                foreach (var item in items)
                {
                    item.IsRemoved = false;
                }
                goto l3;
            }

        end:
            Console.WriteLine("Algorithm ended");
            return bins;
        }
    }  
}
