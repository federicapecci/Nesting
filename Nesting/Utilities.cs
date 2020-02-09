using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    /// <summary>
    /// classe che contiene dei metodi utili 
    /// per implementare l'euristica
    /// </summary>
    class Utilities : IUtilities
    {
        /// <summary>
        /// metodo per calcolare il lower bound
        /// </summary>
        /// <param name="items"></param>
        /// <param name="binWidth"></param>
        /// <param name="binHeight"></param>
        /// <returns></returns>
        public float ComputeLowerBound(IList<Item> items, int binWidth, int binHeight)
        {
            float result = 0;

            foreach (var item in items)
            {
                result += item.Price;
            }

            result /= (binWidth * binHeight);

            return result;
        }

        /// <summary>
        /// metodo per stabilire se è possibile 
        /// trovare una posizione in cui nestare
        /// un nuovo item
        /// </summary>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <returns></returns>
        public bool IsBestPositionFound(Bin<Tuple> temporaryBin, Item temporaryItem)
        {
            SetFeasiblePoints(temporaryBin, temporaryItem);

            //se il bin non contiene punti
            if (temporaryBin.Points.Count == 0)
            {
                return false;
            }
            else if (temporaryBin.Points.Count == 1 && 
                temporaryBin.Points.ElementAt(0).Pposition == 0 && //se il bin contiene 1 solo punto
                temporaryBin.Points.ElementAt(0).Qposition == 0 ) {
                temporaryBin.NestedItems = new List<NestedItem>
                {
                    new NestedItem(temporaryItem)
                    {
                        BLpPosition = 0,
                        BLqPosition = 0,
                        BRpPosition = temporaryItem.Width,
                        BRqPosition = 0,
                        TLpPosition = 0,
                        TLqPosition = temporaryItem.Height
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
            else if(temporaryBin.Points.Count > 1)//se il bin contiene n punti
            {
                foreach (var point in temporaryBin.Points)
                {
                    if (!point.IsUsed)
                    {
                        point.HatchedRegion = GetHatchedRegion(point, temporaryBin, temporaryItem);
                     
                    }
                }

                //trovo la tupla con lo scarto minore tra quelle ancora disponibili
                //https://stackoverflow.com/questions/914109/how-to-use-linq-to-select-object-with-minimum-or-maximum-property-value
                Tuple minHatchedRegionTuple = temporaryBin.Points.Where(x => x.IsUsed == false).OrderBy(x => x.HatchedRegion).First();

                //controllo se ho più triple che hanno lo stesso scarto (il minore)
                IList<Tuple> minHatchedRegionPoints = new List<Tuple>();
                foreach (var point in temporaryBin.Points)
                {
                    if (point.HatchedRegion == minHatchedRegionTriple.HatchedRegion)
                    {
                        minHatchedRegionPoints.Add(point);
                    }
                }

                if (minHatchedRegionPoints.Count == 1)
                {
                    return true;
                }
                else if (minHatchedRegionPoints.Count > 1)
                {
                    Tuple point = ApplyRule(minHatchedRegionPoints);
                    return true;
                }
                return false;
            }
            return false;
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
        /// questo metodo setta come usate le tuple generate che 
        /// sono uguali o eccedono le dimensioni del bin
        /// </summary>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <returns></returns>
        private void SetFeasiblePoints(Bin<Tuple> temporaryBin, Item temporaryItem)
        {
            foreach(var point in temporaryBin.Points)
            {
                //il punto è esterno al bin
                if ((point.Pposition == 0 && point.Qposition >= temporaryBin.Height) ||
                    (point.Pposition >= temporaryBin.Width && point.Qposition == 0) ||
                    (point.Pposition >= temporaryItem.Width && point.Qposition >= temporaryItem.Height))
                {
                    point.IsUsed = true;
                }               
            }
        }

        /// <summary>
        /// Given the item starting position, this method tries 
        /// to push an item down and towards the left
        /// as much as possible (compatting items). 
        /// Then, according to its final position, 
        /// it computes the hatched region (bottom and 
        /// left area left empty) of the item.
        /// </summary>
        private float GetHatchedRegion(Tuple feasiblePoint, Bin<Tuple> temporaryBin, Item temporaryItem)
        {
            //PushItemDown() algorithm
            NestedItem newNestedItem = new NestedItem(temporaryItem)
            {
                BLpPosition = feasiblePoint.Pposition,
                BLqPosition = feasiblePoint.Qposition,
                BRpPosition = feasiblePoint.Pposition + temporaryItem.Width,
                BRqPosition = feasiblePoint.Qposition,
                TLpPosition = feasiblePoint.Pposition,
                TLqPosition = feasiblePoint.Qposition + temporaryItem.Height
            };

            //lista delle possibili intersezioni tra item nuovo e item già in soluzione
            IList<NestedItem> possibleIntersectionItems = new List<NestedItem>();

            foreach (var nestedItem in temporaryBin.NestedItems)
            {
                if(feasiblePoint.Pposition > nestedItem.TLpPosition &&
                   feasiblePoint.Pposition < nestedItem.BRpPosition)
                {
                    possibleIntersectionItems.Add(nestedItem);
                }
            }

            //lista delle intersezioni tra item nuovo e item già in soluzione
            IList<NestedItem> intersectionItems = new List<NestedItem>();

            foreach (var possibleIntersectionItem in possibleIntersectionItems)
            {
                if(feasiblePoint.Qposition > possibleIntersectionItem.TLqPosition)
                {
                    intersectionItems.Add(possibleIntersectionItem);
                }
            }

            //3 possibili risultati
            
            //non ho intersezioni
            if(intersectionItems == null)
            {
                newNestedItem.BLqPosition = 0;
                newNestedItem.TLqPosition = temporaryItem.Height;
                newNestedItem.BRqPosition = 0;
            }else if(intersectionItems.Count == 1)
            {
                float waste = feasiblePoint.Qposition - intersectionItems.ElementAt(0).Height;
                newNestedItem.BLqPosition -= waste;
                newNestedItem.TLqPosition -= waste;
                newNestedItem.BRqPosition -= waste;
            }else if (intersectionItems.Count > 1)
            {
                float heightSum = 0;
                foreach (var intersectionItem in intersectionItems)
                {
                    heightSum += intersectionItem.Height;
                }
                float waste = feasiblePoint.Qposition - heightSum;
                newNestedItem.BLqPosition -= waste;
                newNestedItem.TLqPosition -= waste;
                newNestedItem.BRqPosition -= waste;
            }

            feasiblePoint.QfinalPosition = newNestedItem.BLqPosition;

            //PushItemLeft() algorithm
            //feasiblePoint.PfinalPosition = newNestedItem.BLpPosition;

            //computeHatchedRegion() algorithm
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
        private Tuple ApplyRule(IList<Tuple> minHatchedRegionTriples)
        {
            Tuple result = null;
            IList<Tuple> pMinTriples = new List<Tuple>();
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
                IList<Tuple> qMinTriples = pMinTriples;
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
