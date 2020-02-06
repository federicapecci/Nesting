﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    class Utilities : IUtilities
    {
        /// <summary>
        /// this method computes the continuos lower bound L0
        /// (ref. pag 29 part I of the paper)
        /// </summary>
        /// <param name="items"></param>
        /// <param name="binWidth"></param>
        /// <param name="binHeight"></param>
        /// <returns></returns>
        public float ComputeLowerBound(IList<Item> items, int binWidth, int binHeight)
        {
            float result = 0F;

            foreach (var item in items)
            {
                result += item.Price;
            }

            result /= (binWidth * binHeight);

            return result;
        }

        public bool IsBestPositionFound(Bin<Tuple> temporaryBin, Item temporaryItem)
        {
            IList<EnrichedTuple> feasibleTriples = SetFeasibleTriples(temporaryBin, temporaryItem);

            //se il bin non contiene triple
            if (temporaryBin.Points.Count == 0)
            {
                return false;
            }
            else if (temporaryBin.Points.Count == 1) //se il bin contiene 1 sola tripla
            {
                temporaryBin.OrientedItems = new List<Nestedtem>
                {
                    new Nestedtem(temporaryItem)
                    {
                        Pposition = 0,
                        Qposition = 0
                    }
                };
                temporaryBin.Points.ElementAt(0).IsUsed = true;

                temporaryBin.Points.Add(new Tuple()
                {
                    Pposition = 0,
                    Qposition = temporaryItem.Height,
                    IsUsed = false
                });

                temporaryBin.Points.Add(new Tuple()
                {
                    Pposition = temporaryItem.Width,
                    Qposition = 0,
                    IsUsed = false
                });

                temporaryItem.IsRemoved = true;
                return true;
            }
            else //se il bin contiene n triple
            {
                foreach (var feasibleTriple in feasibleTriples)
                {
                    //calcolo lo scarto potenziale di tale tripla
                    feasibleTriple.HatchedRegion = ComputeHatchedRegion(feasibleTriple, temporaryBin, temporaryItem);
                }

                //trovo la tripla con lo scarto minore
                //https://stackoverflow.com/questions/914109/how-to-use-linq-to-select-object-with-minimum-or-maximum-property-value
                EnrichedTuple minHatchedRegionTriple = feasibleTriples.OrderBy(x => x.HatchedRegion).First();

                //controllo se ho più triple che hanno lo stesso scarto (il minore)
                IList<EnrichedTuple> minHatchedRegionTriples = new List<EnrichedTuple>();
                foreach (var feasibleTriple in feasibleTriples)
                {
                    if (feasibleTriple.HatchedRegion == minHatchedRegionTriple.HatchedRegion)
                    {
                        minHatchedRegionTriples.Add(feasibleTriple);
                    }
                }

                if (minHatchedRegionTriples.Count == 1)
                {
                    return true;
                }
                else if (minHatchedRegionTriples.Count > 1)
                {
                    Tuple triple = ApplyRule(minHatchedRegionTriples);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// this method updates each item price (v) 
        /// </summary>
        /// <param name="z"></param>
        /// <param name="items"></param>
        /// <param name="temporaryBin"></param>
        public void UpdatePrice(int z, IList<Item> items, Bin<Tuple> temporaryBin)
        {
            float alpha = 0.9F;
            float beta = 1.1F;

            foreach (var item in items)
            {
                /*if(temporaryBin.OrientedItemNumber <= (0.5*z))
                {
                    item.Price = alpha * item.Price;
                }
                if(temporaryBin.OrientedItemNumber > (0.5*z))
                {
                    item.Price = beta * item.Price;
                }    */
            }
        }

        /// <summary>
        /// this method selects all of the feasible triples 
        /// for a specific item j 
        /// </summary>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <returns></returns>
        private IList<EnrichedTuple> SetFeasibleTriples(Bin<Tuple> temporaryBin, Item temporaryItem)
        {
            IList<EnrichedTuple> result = new List<EnrichedTuple>();
            foreach(var point in temporaryBin.Points)
            {
                //se la tripla non è ancora stata usata &&
                //la dimensione dell'item non va oltre la dimensione del bin se considero le coordinate p e q della tripla ||
                //la tripla non rappresenta un punto che coincide con gli angoli del bin
                if (!point.IsUsed &&
                    ((temporaryItem.Width < point.Pposition && temporaryItem.Height < point.Qposition) ||
                    (point.Pposition != 0 && point.Qposition != temporaryItem.Height) ||
                    (point.Pposition != temporaryItem.Width && point.Qposition != temporaryItem.Height) ||
                    (point.Pposition != temporaryItem.Width && point.Qposition != 0)))
                {
                    result.Add(new EnrichedTuple(point)
                    {
                        IsFeasible = true
                    });       
                }               
            }
            return result;
        }

        /// <summary>
        /// TO DO
        /// </summary>
        /// <returns></returns>
        private float ComputeHatchedRegion(EnrichedTuple feasibleTriple, Bin<Tuple> temporaryBin, Item temporaryItem)
        {
            return 0;
        }

        /// <summary>
        /// Questo metodo stabilisce un criterio per decidere quale
        /// tripla scegliere se ho più triple che hanno lo stesso scarto potenziale minimo.
        /// Il criterio implementato fa in modo che gli item stiano il più a sinistra possibile.
        /// In particolare:
        /// -tra le triple sceglie quella con la p minima
        /// -a parità di p minima sceglie la tripla con la q minima
        /// -a parità di q minima sceglie la tripla con la r minima
        /// (ref criterio: rules AC1 pag 141 paper part II)
        /// </summary>
        /// <returns></returns>
        private Tuple ApplyRule(IList<EnrichedTuple> minHatchedRegionTriples)
        {
            Tuple result = null;
            IList<EnrichedTuple> pMinTriples = new List<EnrichedTuple>();
            float pMin = minHatchedRegionTriples.OrderBy(x => x.Pposition).First().Pposition;

            foreach (var minHatchedRegionTriple in minHatchedRegionTriples)
            {
                if(minHatchedRegionTriple.Pposition == pMin)
                {
                    pMinTriples.Add(minHatchedRegionTriple);
                }
            }

            if(pMinTriples.Count == 1)
            {
                result = pMinTriples.ElementAt(0);
            }
            else
            {
                IList<EnrichedTuple> qMinTriples = pMinTriples;
                float qMin = qMinTriples.OrderBy(x => x.Qposition).First().Qposition;
                foreach (var qMinTriple in qMinTriples)
                {
                    if (qMinTriple.Qposition == qMin)
                    {
                        qMinTriples.Add(qMinTriple);
                    }
                }
                if (qMinTriples.Count == 1)
                {
                    result = qMinTriples.ElementAt(0);
                   
                }
                /*else
                {
                    IList<EnrichedTuple> rMinTriples = qMinTriples;
                    bool rMin = rMinTriples.OrderBy(x => x.Rotation).First().Rotation;
                    foreach (var rMinTriple in rMinTriples)
                    {
                        if(rMinTriple.Rotation == rMin)
                        {
                            rMinTriples.Add(rMinTriple);
                        }
                    }
                    result = rMinTriples.ElementAt(0);
                }*/
            }
            return result;
        }
    }
}
