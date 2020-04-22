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

        public HSolve()
        {
            Utilities = new Utilities();
            PricingUtilities = new PricingUtilities();
            OutputUtilities = new OutputUtilities();
        }

        /// <summary>
        /// metodo che computa l'euristica di hsolve 
        /// </summary>
        public IList<Sequence> ComputeHeuristic(Configuration configuration, string itemAllocationMethod,
                            string pricingRule, string priceUpdatingRule)
        {

            IList<Sequence> sequences = new List<Sequence>
            {
                new Sequence(),
                new Sequence()
            };

            //================ STEP 1 - INITIALIZATION ================

            //inizializzo il prezzo v associato ad ogni item j
            IList<Item> items = new List<Item>();
            IList<Bin<Tuple>> bins = new List<Bin<Tuple>>();
            int counter = 0;

            foreach (Dimension dimension in configuration.Dimensions) {
                Item item = new Item()
                {
                    Height = dimension.Height,
                    Width = dimension.Width,
                    Id = counter,
                    Price = PricingUtilities.ComputePricingRule(pricingRule, dimension.Height, dimension.Width)
                };
                items.Add(item);

                //inserisco ogni item prezzato e i nuovi punti disponibili 
                //in un bin diverso
                Bin<Tuple> bin = new Bin<Tuple>()
                {
                    Id = counter,
                    Height = configuration.BinHeight,
                    Width = configuration.BinWidth,
                    NestedItems = new List<Item>()
                    {
                        item
                    },
                    Points = new List<Tuple>()
                    {
                        new Tuple()
                        {
                            Pposition = 0,
                            Qposition = item.Height,
                            Rposition = 0,
                            IsUsed = false
                        },
                        new Tuple()
                        {
                            Pposition = 0,
                            Qposition = item.Height,
                            Rposition = 1,
                            IsUsed = false
                        },
                        new Tuple()
                        {
                            Pposition = item.Width,
                            Qposition = 0,
                            Rposition = 0,
                            IsUsed = false
                        },
                        new Tuple()
                        {
                            Pposition = item.Width,
                            Qposition = 0,
                            Rposition = 1,
                            IsUsed = false
                        }
                    }
                };

                bins.Add(bin);
                counter += 1;
            }
                  
            //inizializzo il costo della soluzione con il numero degli elementi
            int zStar = counter;
            
            //inizializzo il numero di iterazioni
            int iter = 0;

            //calcolo il lower bound ed il relativo intervallo
            double lowerBound;
            int maxIter = configuration.MaxIter;

        //================ STEP 2 - ERASE THE CURRENT SOLUTION ================

        l3: counter = 0;

            //creo una lista temporanea J' di item 
            IList<Item> temporaryItems = new List<Item>();

            //assegno la lista di item J a J'
            temporaryItems = items;

            //setto il bin che considero al momento
            int i = 0;

            //creo tanti bin temporanei quanti sono gli item
            IList<Bin<Tuple>> temporaryBins = new List<Bin<Tuple>>();
            foreach (Item item in items)
            {
                Bin<Tuple> temporaryBin = new Bin<Tuple>()
                {
                    Id = counter,
                    Height = configuration.BinHeight,
                    Width = configuration.BinWidth,
                    NestedItems = null,
                    Points = new List<Tuple>()
                    {
                        new Tuple()
                        {
                            Pposition = 0,
                            Qposition = 0,
                            Rposition = 0,
                            IsUsed = false
                        }
                    }
                };
                temporaryBins.Add(temporaryBin);
                counter += 1;
            }

            temporaryItems = temporaryItems.OrderByDescending(x => x.Price).ToList();


        //================ STEP 3 - FILLING UP BIN i ================
        l1:
            IList<Item> notRemovedItems = new List<Item>();
            //cerco la posizione migliore per ogni item j'
            foreach (Item temporaryItem in temporaryItems)
            {                
                if (!temporaryItem.IsRemoved)
                {                   
                    Utilities.IsBestPositionFound(temporaryBins.ElementAt(i), temporaryItem, itemAllocationMethod);
                    //salvo un bin nuovo ogni volta che  viene aggiunto un elemento
                    /*var tempBin = temporaryBins[i];
                    Bin<Tuple> b;
                    
                    if (tempBin.NestedItems != null)
                    {
                        b = new Bin<Tuple>
                        {
                            Id = tempBin.Id,
                            Height = tempBin.Height,
                            Width = tempBin.Width,
                            Points = new List<Tuple>(tempBin.Points),
                            NestedItems = new List<Item>(tempBin.NestedItems)
                        };
                        sequence.Bins.Add(b);
                    }
                    else
                    {
                        b = new Bin<Tuple>
                        {
                            Id = tempBin.Id,
                            Height = tempBin.Height,
                            Width = tempBin.Width,
                            Points = new List<Tuple>(tempBin.Points),
                            NestedItems = new List<Item>()
                        }
                    }*/
                }
               
            }

            notRemovedItems = temporaryItems.Where(x => x.IsRemoved == false).ToList();

            //================ STEP 4 - CHECK IF ALL ITEMS HAVE BEEN ALLOCATED ================

            int z = i; // K

            //============ TEST ==============
            lowerBound = Utilities.ComputeLowerBound(notRemovedItems, configuration.BinWidth, configuration.BinHeight);
            notRemovedItems.Clear();

            if((z + lowerBound) > zStar)
            {
                goto l2;
            }
            //================================

            bool isSortedTemporaryItemsEmpty = true;

            //controllo se tutta la lista è stata svuotata
            foreach (Item temporaryItem in temporaryItems)
            {
                if (temporaryItem.IsRemoved == false)
                {
                    isSortedTemporaryItemsEmpty = false;
                    break;
                }
            }

            if (isSortedTemporaryItemsEmpty)
            {
                goto l0;
            }
            if (!isSortedTemporaryItemsEmpty)
            {
                i += 1;
                goto l1;
            }

        //================ STEP 5 - UPDATE THE BEST SOLUTION ================

        l0: zStar = z;
            bins = temporaryBins;

            //Utilities.IsSolutionCorrect(items, bins, iter);

            if (OutputUtilities.IsNewBestWidthFound(bins[i])) 
            {
                //aggiungo la sequenza di un certa iterazione
                Sequence s = new Sequence()
                {
                    Zstar = zStar,
                    Bins = new List<Bin<Tuple>>(),
                    IteratioNumber = iter,
                    Criterias = new List<string>
                    {
                        itemAllocationMethod,
                        pricingRule,
                        priceUpdatingRule
                    },
                    WidthCovered = OutputUtilities.GetBestWidthFound(),
                    UsedAreaAbsoluteValue = OutputUtilities.ComputeUsedAreaAbsoluteValue(bins[i].NestedItems),
                    UsedAreaPercentageValue = OutputUtilities.ComputeUsedAreaPercentageValue(bins[i].Height, bins[i].Width)
                };

                //per mettere in sequence solo i bin che hanno elementi e non quelli dove nestedItems = null
                s.Bins = bins.Where(x => x.NestedItems != null).ToList();

                sequences.RemoveAt(0);
                sequences.Insert(0, s);
            }

            if (OutputUtilities.IsNewBestAreaFound(bins[i]))
            {
                //aggiungo la sequenza di un certa iterazione
                Sequence s = new Sequence()
                {
                    Zstar = zStar,
                    Bins = new List<Bin<Tuple>>(),
                    IteratioNumber = iter,
                    Criterias = new List<string>
                    {
                        itemAllocationMethod,
                        pricingRule,
                        priceUpdatingRule
                    },
                    WidthCovered = OutputUtilities.ComputeWidthLastBin(bins[i].NestedItems),
                    AreaCovered = OutputUtilities.GetBestAreaFound(),
                    UsedAreaAbsoluteValue = OutputUtilities.ComputeUsedAreaAbsoluteValue(bins[i].NestedItems),
                    UsedAreaPercentageValue = OutputUtilities.ComputeUsedAreaPercentageValue(bins[i].Height, bins[i].Width)
                };

                //per mettere in sequence solo i bin che hanno elementi e non quelli dove nestedItems = null
                s.Bins = bins.Where(x => x.NestedItems != null).ToList();

                sequences.RemoveAt(1);
                sequences.Insert(1, s);
            }

        //================ STEP 7 - UPDATE THE ITEM PRICES ================

        l2: if (iter == maxIter)
            {
                goto end;
            }
            else
            {
                if (z > (zStar / 2)) 
                {
                    PricingUtilities.ComputePricingUpdateRule(z, items, temporaryBins, priceUpdatingRule, "REGPART");
                }

                if (z <= (zStar / 2))
                {
                    //Console.WriteLine("CIAO 1 ");
                    PricingUtilities.ComputePricingUpdateRule(z, items, temporaryBins, priceUpdatingRule, "EXTRAPART");
                    //Console.WriteLine("CIAO 2 ");
                }

                iter += 1;
                
                //rimetto tutti gli item come isRemoved = false perché cominicio una nuova iterazione
                foreach (Item item in items)
                {
                    item.IsRemoved = false;
                }
                goto l3;

            }

        end:
                  
            return sequences;
        }
    }
}
