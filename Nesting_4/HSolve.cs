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

        public HSolve()
        {
            Utilities = new Utilities();
            PricingUtilities = new PricingUtilities();
        }
       
        /// <summary>
        /// metodo che computa l'euristica di hsolve 
        /// </summary>
        public IList<Sequence> ComputeHeuristic(Configuration configuration, string itemAllocationMethod,
                            string pricingRule, string priceUpdatingRule)
        {

            IList<Sequence> sequences = new List<Sequence>();
            //================ STEP 1 - INITIALIZATION ================

            //inizializzo il prezzo v associato ad ogni item j
            IList<Item> items = new List<Item>();
            int counter = 0;

            foreach (var dimension in configuration.Dimensions) {
                var item = new Item()
                {
                    Height = dimension.Height,
                    Width = dimension.Width,
                    Id = counter,
                    Price = PricingUtilities.ComputePricingRule(pricingRule, dimension.Height, dimension.Width)
                };
                items.Add(item);
                counter += 1;
            }

            IList<Bin<Tuple>> bins = new List<Bin<Tuple>>();
            counter = 0;

            //inserisco ogni item prezzato e i nuovi punti disponibili 
            //in un bin diverso
            foreach(var item in items)
            {
                var bin = new Bin<Tuple>()
                {
                    Id = counter,
                    Height = configuration.BinHeight,
                    Width = configuration.BinWidth,
                    NestedItems = new List<Item>()
                    {
                        items.ElementAt(counter)
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

            int itemNumber = counter + 1;
            counter = 0;
            //inizializzo il costo della soluzione con il numero degli elementi
            int zStar = itemNumber;
            
            //inizializzo il numero di iterazioni
            int iter = 0;

            //calcolo il lower bound ed il relativo intervallo
            double lowerBound = Utilities.ComputeLowerBound(items, configuration.BinWidth, configuration.BinHeight);
            double lowerBoundMin = lowerBound - configuration.LowerBoundDelta;
            double lowerBoundMax = lowerBound + configuration.LowerBoundDelta;
            int maxIter = configuration.MaxIter;

        //================ STEP 2 - ERASE THE CURRENT SOLUTION ================

        l3:
            Sequence sequence = new Sequence()
            {
                Bins = new List<Bin<Tuple>>()
            };

            //creo una lista temporanea J' di item 
            IList<Item> temporaryItems = new List<Item>();

            //assegno la lista di item J a J'
            temporaryItems = items;

            //setto il bin che considero al momento
            int i = 0;

            //creo tanti bin temporanei quanti sono gli item
            IList<Bin<Tuple>> temporaryBins = new List<Bin<Tuple>>();
            foreach (var item in items)
            {
                var temporaryBin = new Bin<Tuple>()
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

            //1) ogni volta che inizio una nuova iterazione iter devo riordinare gli item per price decrescente
            //dato che i price sono stati aggiornati;
            //2) riattribuisco gli id agli item, così l'item col prezzo maggiore è quello che ha l'id 0 e così via
            var sortedTemporaryItems = new List<Item>();
            counter = 0;
            foreach(var temporaryItem in temporaryItems.OrderByDescending(x => x.Price))
            {
                sortedTemporaryItems.Add(temporaryItem);
                temporaryItem.Id = counter;
                counter += 1;
            }

        //================ STEP 3 - FILLING UP BIN i ================
        l1: //Console.WriteLine("ciao");

            //cerco la posizione migliore per ogni item j'
            foreach (var sortedTemporaryItem in sortedTemporaryItems)
            {                
                if (!sortedTemporaryItem.IsRemoved)
                {                   
                    Utilities.IsBestPositionFound(temporaryBins.ElementAt(i), sortedTemporaryItem, itemAllocationMethod);
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


            //================ STEP 4 - CHECK IF ALL ITEMS HAVE BEEN ALLOCATED ================
            int z = i;
            bool isSortedTemporaryItemsEmpty = true;

            //controllo se tutta la lista è stata svuotata
            foreach (var sortedTemporaryItem in sortedTemporaryItems)
            {
                if (sortedTemporaryItem.IsRemoved == false)
                {
                    isSortedTemporaryItemsEmpty = false;
                    break;
                }
            }

            if (isSortedTemporaryItemsEmpty)
            {
                goto l0;
            }
            if (!isSortedTemporaryItemsEmpty && (i < (zStar)))
            {
                i += 1;
                goto l1;
            }
            goto l2;

        //================ STEP 5 - UPDATE THE BEST SOLUTION ================

        l0: zStar = z;
            bins = temporaryBins;

            //aggiungo la sequenza di un certa iterazione
            Sequence sequence1 = new Sequence()
            {
                Zstar = zStar,
                Bins = new List<Bin<Tuple>>(),
                IteratioNumber = iter,
                Criterias = new List<string>
                {
                    itemAllocationMethod,
                    pricingRule,
                    priceUpdatingRule
                }
            };
            sequence1.Bins = bins;
            sequences.Add(sequence1);
            Utilities.CheckSolution(items, bins, iter);
            //============================================

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
                PricingUtilities.ComputePricingUpdateRule(z, items, temporaryBins, priceUpdatingRule);
                iter += 1;
                //rimetto tutti gli item come isRemoved = false perché cominicio una nuova iterazione
                foreach (var item in items)
                {
                    item.IsRemoved = false;
                }
                goto l3;

            }

        end:
            //aggiungo la sequenza dell'ultima iterazione
            /*Sequence sequence2 = new Sequence()
            {
                Zstar = zStar,
                Bins = new List<Bin<Tuple>>(),
                IteratioNumber = iter,
                Criterias = new List<string>
                 {
                    itemAllocationMethod,
                    pricingRule,
                    priceUpdatingRule
                 }
            };
            sequence2.Bins = temporaryBins;
            sequences.Add(sequence2);*/
            //===========================================

            //Utilities.CheckSolution(items, temporaryBins, iter);

            //Sequence firstSequence = sequences.FirstOrDefault();
            Sequence lastSequence = sequences.LastOrDefault();
            sequences.Clear();
            //sequences.Add(firstSequence);
            sequences.Add(lastSequence);
        
            return sequences;
        }
    }
}
