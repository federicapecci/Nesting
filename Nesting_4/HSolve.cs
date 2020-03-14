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
        /// <summary>
        /// la configurazione contenwnte i parametri con 
        /// cui lanciare l'algortimo hsolve
        /// </summary>
        public Configuration Configuration { get; set; } = null;

        /// <summary>
        /// ogni container appartente a containers contiene
        /// gli n bin di una certa iterazione
        /// </summary>
        public IList<Sequence> Sequences { get; set; } = null;

        /// <summary>
        /// costruttore in cui setto la configuration di hsolve
        /// </summary>
        /// <param name="configuration"></param>
        public HSolve(Configuration configuration)
        {
            Configuration = configuration;
            Sequences = new List<Sequence>();
        }

        /// <summary>
        /// metodo che computa l'euristica di hsolve 
        /// </summary>
        public IList<Sequence> ComputeHeuristic()
        {
            IUtilities utilities = new Utilities();

            //================ STEP 1 - INITIALIZATION ================

            //inizializzo il prezzo v associato ad ogni item j
            IList<Item> items = new List<Item>();
            int counter = 0;

            foreach (var dimension in Configuration.Dimensions) { 
                var item = new Item()
                {
                    Height = dimension.Height,
                    Width = dimension.Width,
                    Id = counter,
                    Price = dimension.Height * dimension.Width,
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
                    Height = Configuration.BinHeight,
                    Width = Configuration.BinWidth,
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
                            IsUsed = false
                        },
                        new Tuple()
                        {
                            Pposition = item.Width,
                            Qposition = 0,
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
            float lowerBound = utilities.ComputeLowerBound(items, Configuration.BinWidth, Configuration.BinHeight);
            float lowerBoundMin = lowerBound - Configuration.LowerBoundDelta;
            float lowerBoundMax = lowerBound + Configuration.LowerBoundDelta;
            int maxIter = Configuration.MaxIter;

        //================ STEP 2 - ERASE THE CURRENT SOLUTION ================

        l3:
            /*Sequence sequence = new Sequence()
            {
                Bins = new List<Bin<Tuple>>()
            };*/

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
        l1://cerco la posizione migliore per ogni item j'
            foreach (var sortedTemporaryItem in sortedTemporaryItems)
            {                
                if (!sortedTemporaryItem.IsRemoved)
                {
                    utilities.IsBestPositionFound(temporaryBins.ElementAt(i), sortedTemporaryItem);
                    //salvo un bin nuovo ogni volta che  viene aggiunto un elemento
                    /*var tempItem = temporaryBins[i];
                    Bin<Tuple> b;
                    
                    if (tempItem.PricedItems != null)
                    {

                        b = new Bin<Tuple>
                        {
                            Id = tempItem.Id,
                            Height = tempItem.Height,
                            Width = tempItem.Width,
                            Points = new List<Tuple>(tempItem.Points),
                            PricedItems = new List<PricedItem>(tempItem.PricedItems)
                        };
                    }
                    else
                    {
                        b = new Bin<Tuple>
                        {
                            Id = tempItem.Id,
                            Height = tempItem.Height,
                            Width = tempItem.Width,
                            Points = new List<Tuple>(tempItem.Points),
                            PricedItems = new List<PricedItem>()
                        };
                    }
                    
                    sequence.Bins.Add(b);*/
                    
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
                //Sequences.Add(sequence);
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

            Sequence sequence1 = new Sequence()
            {
                Bins = new List<Bin<Tuple>>()
            };
            sequence1.Bins = bins;
            Sequences.Add(sequence1);
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
                //rimetto tutti gli item come isRemoved = false perché cominicio una nuova iterazione
                foreach (var item in items)
                {
                    item.IsRemoved = false;
                }
                goto l3;

            }

        end:
                Sequence sequence2 = new Sequence()
            {
                Bins = new List<Bin<Tuple>>()
            };
            sequence2.Bins = bins;
            Sequences.Add(sequence2);

            Sequence firstSequence = Sequences.FirstOrDefault();
            Sequence lastSequence = Sequences.LastOrDefault();

            Sequences.Clear();
            Sequences.Add(firstSequence);
            Sequences.Add(lastSequence);

            return Sequences;
        }
    }
}
