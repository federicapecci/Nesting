using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Nesting_4
{
    class HSolveUtilities : IHSolveUtilities
    {

        private IList<Sequence> Sequences = null;

        private int Counter { get; set; } = 0;

        private IList<Item> Items { get; set; } = null;

        private IList<Bin> Bins { get; set; } = null;

        private int Zstar { get; set; } = 0;

        private int Iter { get; set; } = 0;

        private int MaxIter { get; set; } = 0;

        private IList<Item> TemporaryItems { get; set; } = null;

        private IList<Bin> TemporaryBins { get; set; } = null;


        private double LowerBound = 0;

        private IList<Item> NotRemovedItems { get; set; } = null;

        private int i { get; set; } = 0;

        private int z { get; set; } = 0;


        public void Initialize(Configuration configuration, string pricingRule, IPricingUtilities PricingUtilities)
        {
            Sequences = new List<Sequence>
            {
                new Sequence(),
                new Sequence()
            };

            //inizializzo il prezzo v associato ad ogni item j
            Items = new List<Item>();
            Bins = new List<Bin>();
            Counter = 0;

            foreach (Dimension dimension in configuration.Dimensions)
            {
                Item item = new Item()
                {
                    Height = dimension.Height,
                    Width = dimension.Width,
                    Id = Counter,
                    Price = PricingUtilities.ComputePricingRule(pricingRule, dimension.Height, dimension.Width)
                };
                Items.Add(item);

                //inserisco ogni item prezzato e i nuovi punti disponibili 
                //in un bin diverso
                Bin bin = new Bin()
                {
                    Id = Counter,
                    Height = configuration.BinHeight,
                    Width = configuration.BinWidth,
                    NestedItems = new List<Item>()
                    {
                        item
                    },
                    Points = new List<Position>()
                    {
                        new Position()
                        {
                            Pposition = 0,
                            Qposition = item.Height,
                            Rposition = 0,
                            IsUsed = false
                        },
                        new Position()
                        {
                            Pposition = 0,
                            Qposition = item.Height,
                            Rposition = 1,
                            IsUsed = false
                        },
                        new Position()
                        {
                            Pposition = item.Width,
                            Qposition = 0,
                            Rposition = 0,
                            IsUsed = false
                        },
                        new Position()
                        {
                            Pposition = item.Width,
                            Qposition = 0,
                            Rposition = 1,
                            IsUsed = false
                        }
                    }
                };

                Bins.Add(bin);
                Counter += 1;
            }

            //inizializzo il costo della soluzione con il numero degli elementi
            Zstar = Counter;

            //inizializzo il numero di iterazioni
            Iter = 0;

            //calcolo il lower bound ed il relativo intervallo
            //double lowerBound;
            MaxIter = configuration.MaxIter;



        }


        //l3
        public void EraseCurrentSolution(Configuration configuration)
        {
            Counter = 0;

            //creo una lista temporanea J' di item 
            TemporaryItems = new List<Item>();

            //assegno la lista di item J a J'
            TemporaryItems = Items;









            //setto il bin che considero al momento
            i = 0;

            //creo tanti bin temporanei quanti sono gli item
            TemporaryBins = new List<Bin>();
            foreach (Item item in Items)
            {
                Bin temporaryBin = new Bin()
                {
                    Id = Counter,
                    Height = configuration.BinHeight,
                    Width = configuration.BinWidth,
                    NestedItems = null,
                    Points = new List<Position>()
                    {
                        new Position()
                        {
                            Pposition = 0,
                            Qposition = 0,
                            Rposition = 0,
                            IsUsed = false
                        }
                    }
                };
                TemporaryBins.Add(temporaryBin);
                Counter += 1;
            }

            TemporaryItems = TemporaryItems.OrderByDescending(x => x.Price).ToList();
        }


        //l1
        public  void FillUpBinI(string itemAllocationMethod, IUtilities Utilities)
        {
            NotRemovedItems = new List<Item>();
            //cerco la posizione migliore per ogni item j'
            foreach (Item temporaryItem in TemporaryItems)
            {
                if (!temporaryItem.IsRemoved)
                {
                    Utilities.IsBestPositionFound(TemporaryBins.ElementAt(i), temporaryItem, itemAllocationMethod);
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

            NotRemovedItems = TemporaryItems.Where(x => x.IsRemoved == false).ToList();
     

        }

        public string CheckIfAllItemsAreAllocated(Configuration configuration, IUtilities Utilities, IPricingUtilities PricingUtilities,
            string itemAllocationMethod, IOutputUtilities OutputUtilities, string pricingRule, string priceUpdatingRule)
        {
            string res = "";
            z = i; // K

            //============ TEST ==============
            LowerBound = Utilities.ComputeLowerBound(NotRemovedItems, configuration.BinWidth, configuration.BinHeight);
            NotRemovedItems.Clear();

            if ((z + LowerBound) > Zstar)
            {
                //goto l2;
                //UpdateItemPrices(configuration, PricingUtilities, priceUpdatingRule);
                res = "UpdateItemPrices";
            }
            //================================

            bool isSortedTemporaryItemsEmpty = true;

            //controllo se tutta la lista è stata svuotata
            foreach (Item temporaryItem in TemporaryItems)
            {
                if (temporaryItem.IsRemoved == false)
                {
                    isSortedTemporaryItemsEmpty = false;
                    break;
                }
            }

            if (isSortedTemporaryItemsEmpty)
            {
                //goto l0;
                //UpdateBestSolution(OutputUtilities, itemAllocationMethod, pricingRule, priceUpdatingRule);
                res = "UpdateBestSolution";
            }
            if (!isSortedTemporaryItemsEmpty)
            {
                i += 1;
                //l1
                //FillUpBinI(itemAllocationMethod, Utilities);
                res = "FillUpBinI";
            }
            return res;
        }

        //l0

        public void UpdateBestSolution(IOutputUtilities OutputUtilities, string itemAllocationMethod,
                            string pricingRule, string priceUpdatingRule)
        {

            Zstar = z;
            Bins = TemporaryBins;

            //Utilities.IsSolutionCorrect(items, bins, iter);

            if (OutputUtilities.IsNewBestWidthFound(Bins[i]))
            {
                //aggiungo la sequenza di un certa iterazione
                Sequence s = new Sequence()
                {
                    Zstar = Zstar,
                    Bins = new List<Bin>(),
                    IteratioNumber = Iter,
                    Criterias = new List<string>
                        {
                            itemAllocationMethod,
                            pricingRule,
                            priceUpdatingRule
                        },
                    WidthCovered = OutputUtilities.GetBestWidthFound(),
                    UsedAreaAbsoluteValue = OutputUtilities.ComputeUsedAreaAbsoluteValue(Bins[i].NestedItems),
                    UsedAreaPercentageValue = OutputUtilities.ComputeUsedAreaPercentageValue(Bins[i].Height, Bins[i].Width)
                };

                //per mettere in sequence solo i bin che hanno elementi e non quelli dove nestedItems = null
                s.Bins = Bins.Where(x => x.NestedItems != null).ToList();

                Sequences.RemoveAt(0);
                Sequences.Insert(0, s);
             
            }

            if (OutputUtilities.IsNewBestAreaFound(Bins[i]))
            {
                //aggiungo la sequenza di un certa iterazione
                Sequence s = new Sequence()
                {
                    Zstar = Zstar,
                    Bins = new List<Bin>(),
                    IteratioNumber = Iter,
                    Criterias = new List<string>
                        {
                            itemAllocationMethod,
                            pricingRule,
                            priceUpdatingRule
                        },
                    WidthCovered = OutputUtilities.ComputeWidthLastBin(Bins[i].NestedItems),
                    AreaCovered = OutputUtilities.GetBestAreaFound(),
                    UsedAreaAbsoluteValue = OutputUtilities.ComputeUsedAreaAbsoluteValue(Bins[i].NestedItems),
                    UsedAreaPercentageValue = OutputUtilities.ComputeUsedAreaPercentageValue(Bins[i].Height, Bins[i].Width)
                };

                //per mettere in sequence solo i bin che hanno elementi e non quelli dove nestedItems = null
                s.Bins = Bins.Where(x => x.NestedItems != null).ToList();

                Sequences.RemoveAt(1);
                Sequences.Insert(1, s);
               
            }


        }

        //l2
        public void UpdateItemPrices(Configuration configuration, IPricingUtilities PricingUtilities, string priceUpdatingRule)
        {

            /*if (Iter == MaxIter)
            {
                //goto end
       
            }
            else
            {*/
            if (z > (Zstar / 2))
            {
                PricingUtilities.ComputePricingUpdateRule(z, Items, TemporaryBins, priceUpdatingRule, "REGPART");
            }

            if (z <= (Zstar / 2))
            {
               
                PricingUtilities.ComputePricingUpdateRule(z, Items, TemporaryBins, priceUpdatingRule, "EXTRAPART");
       

            }

            Iter += 1;

            //rimetto tutti gli item come isRemoved = false perché cominicio una nuova iterazione
            foreach (Item item in Items)
            {
                item.IsRemoved = false;
            }
            //goto l3
            //EraseCurrentSolution(configuration);
        }


        public IList<Sequence> GetSequences()
        {
            return Sequences;
        }


        public int GetIter()
        {
            return Iter;
        }

        public int GetMaxIter()
        {
            return MaxIter;
        }















    }
}
