using System.Collections.Generic;
using System.Linq;

namespace Nesting
{
    /// <summary>
    /// classe che contiene delle utilities 
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
            else if (temporaryBin.Points.Count == 1 && //se il bin contiene 1 solo punto e quel punto è (0,0)
                temporaryBin.Points.ElementAt(0).Pposition == 0 &&
                temporaryBin.Points.ElementAt(0).Qposition == 0)
            {

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

                HandleOperationsPostNestedItem(temporaryBin, temporaryItem, temporaryBin.Points.ElementAt(0));
                return true;
            }
            else if (temporaryBin.Points.Count > 1)//se il bin contiene n punti
            {
                foreach (var feasiblePoint in temporaryBin.Points)
                {
                    if (!feasiblePoint.IsUsed)
                    {
                        //assegno le coordinate di partenza al nuovo item da nestare, poi inzio a muoverlo
                        NestedItem newNestedItem = new NestedItem(temporaryItem)
                        {
                            BLpPosition = feasiblePoint.Pposition,
                            BLqPosition = feasiblePoint.Qposition,
                            BRpPosition = feasiblePoint.Pposition + temporaryItem.Width,
                            BRqPosition = feasiblePoint.Qposition,
                            TLpPosition = feasiblePoint.Pposition,
                            TLqPosition = feasiblePoint.Qposition + temporaryItem.Height
                        };
                        feasiblePoint.HatchedRegion = GetHatchedRegion(feasiblePoint, newNestedItem, temporaryBin, temporaryItem);
                    }
                }

                //trovo la tupla con lo scarto minore tra quelle ancora disponibili
                //(Where(x => x.HatchedRegion >= 0) filtra solo le tuple con hatched region = 0)
                Tuple minHatchedRegionTuple = temporaryBin.Points.Where(x => x.IsUsed == false && x.HatchedRegion >= 0)
                                                                 .OrderBy(x => x.HatchedRegion) 
                                                                 .First();

                //controllo se ho più tuple che hanno lo stesso scarto (il minore)
                IList<Tuple> minHatchedRegionPoints = new List<Tuple>();
                foreach (var point in temporaryBin.Points)
                {
                    if (point.HatchedRegion == minHatchedRegionTuple.HatchedRegion && !point.IsUsed)
                    {
                        minHatchedRegionPoints.Add(point);
                    }
                }

                if (minHatchedRegionPoints.Count == 1)
                {
                    var minHatchedRegionPoint = minHatchedRegionPoints.ElementAt(0);

                    temporaryBin.NestedItems.Add(new NestedItem(temporaryItem)
                    {
                        BLpPosition = minHatchedRegionPoint.PfinalPosition,
                        BLqPosition = minHatchedRegionPoint.QfinalPosition,
                        BRpPosition = minHatchedRegionPoint.PfinalPosition + temporaryItem.Width,
                        BRqPosition = minHatchedRegionPoint.QfinalPosition,
                        TLpPosition = minHatchedRegionPoint.PfinalPosition,
                        TLqPosition = minHatchedRegionPoint.QfinalPosition + temporaryItem.Height
                    });

                    HandleOperationsPostNestedItem(temporaryBin, temporaryItem, minHatchedRegionPoint);
                    return true;
                }
                else if (minHatchedRegionPoints.Count > 1)
                {
                    Tuple minCoordinatePoint = ApplyRule(minHatchedRegionPoints);

                    temporaryBin.NestedItems.Add(new NestedItem(temporaryItem)
                    {
                        BLpPosition = minCoordinatePoint.PfinalPosition,
                        BLqPosition = minCoordinatePoint.QfinalPosition,
                        BRpPosition = minCoordinatePoint.PfinalPosition + temporaryItem.Width,
                        BRqPosition = minCoordinatePoint.QfinalPosition,
                        TLpPosition = minCoordinatePoint.PfinalPosition,
                        TLqPosition = minCoordinatePoint.QfinalPosition + temporaryItem.Height
                    });

                    HandleOperationsPostNestedItem(temporaryBin, temporaryItem, minCoordinatePoint);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// gestisco le operazione post item nestato
        /// </summary>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <param name="point"></param>
        private void HandleOperationsPostNestedItem(Bin<Tuple> temporaryBin, Item temporaryItem, Tuple point)
        {
            //setto il punto ad usato, per recuparare il punto dalla lista uso l'id
            var matchingPoint = temporaryBin.Points.Where(x => x.Pposition == point.Pposition &&
                                                       x.Qposition == point.Qposition)
                                           .First();
            matchingPoint.IsUsed = true;

            //aggiungo 2 nuovi punti ma prima controllo se sono già presenti nella lista temporaryBin.Points
            Tuple pointFound = temporaryBin.Points.Where(x => x.Pposition == point.PfinalPosition &&
                                           x.Qposition == point.QfinalPosition + temporaryItem.Height).FirstOrDefault();

            if (pointFound == null)
            {
                temporaryBin.Points.Add(new Tuple()
                {
                    Pposition = point.PfinalPosition,
                    Qposition = point.QfinalPosition + temporaryItem.Height,
                    IsUsed = false

                });
            }
            else
            {
                pointFound = null;
            }

            pointFound = temporaryBin.Points.Where(x => x.Pposition == point.PfinalPosition + temporaryItem.Width &&
                                           x.Qposition == point.QfinalPosition).FirstOrDefault();

            if (pointFound == null)
            {
                temporaryBin.Points.Add(new Tuple()
                {
                    Pposition = point.PfinalPosition + temporaryItem.Width,
                    Qposition = point.QfinalPosition,
                    IsUsed = false
                });
            }

            //setto item a nestato
            temporaryItem.IsRemoved = true;
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
            foreach (var point in temporaryBin.Points)
            {
                //il punto è esterno al bin
                if ((point.Pposition == 0 && point.Qposition >= temporaryBin.Height) ||
                    (point.Pposition >= temporaryBin.Width && point.Qposition == 0) ||
                    (point.Pposition >= temporaryBin.Width && point.Qposition >= temporaryBin.Height))
                {
                    point.IsUsed = true;
                }
            }
        }

        /// <summary>
        /// questo metodo, data la posizione inziale di un item,
        /// prova a spingerlo in basso e verso sinistra quanto
        /// più possibile.
        /// Poi, calcola l'hatched region in basso e a sinistra 
        /// dell'item
        /// </summary>
        /// <param name="feasiblePoint"></param>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <returns></returns>
        private float GetHatchedRegion(Tuple feasiblePoint, NestedItem newNestedItem, Bin<Tuple> temporaryBin, Item temporaryItem)
        {
                PushItemDown(feasiblePoint, temporaryBin, temporaryItem, newNestedItem);
                PushItemLeft(feasiblePoint, temporaryBin, temporaryItem, newNestedItem);

                //controllo se l'oggettto, anche essendo stato spostato in basso a sintra, sborda
                if (IsBorderObserved(newNestedItem, temporaryBin.Height, temporaryBin.Width))
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
        }

        private void PushItemDown(Tuple feasiblePoint, Bin<Tuple> temporaryBin, Item temporaryItem, NestedItem newNestedItem)
        {

            //lista delle intersezioni tra item nuovo e item già in soluzione
            IList<NestedItem> intersectedItems = new List<NestedItem>();

            foreach (var nestedItem in temporaryBin.NestedItems)
            {
                if (feasiblePoint.Pposition >= nestedItem.TLpPosition &&
                   feasiblePoint.Pposition < nestedItem.BRpPosition &&
                   feasiblePoint.Qposition >= nestedItem.TLqPosition)
                {
                    intersectedItems.Add(nestedItem);
                }
            }

            //3 possibili risultati
            if (intersectedItems.Count == 0) //non ho intersezioni
            {
                newNestedItem.BLqPosition = 0;
                newNestedItem.TLqPosition = temporaryItem.Height;
                newNestedItem.BRqPosition = 0;
            }
            else if (intersectedItems.Count == 1) //1 sola intersezione
            {
                float delta = feasiblePoint.Qposition - intersectedItems.ElementAt(0).Height;
                newNestedItem.BLqPosition -= delta;
                newNestedItem.TLqPosition -= delta;
                newNestedItem.BRqPosition -= delta;
            }
            else if (intersectedItems.Count > 1) //N intersezioni
            {
                float heightSum = 0;
                foreach (var intersectionItem in intersectedItems)
                {
                    heightSum += intersectionItem.Height;
                }
                float delta = feasiblePoint.Qposition - heightSum;
                newNestedItem.BLqPosition -= delta;
                newNestedItem.TLqPosition -= delta;
                newNestedItem.BRqPosition -= delta;
            }

            feasiblePoint.QfinalPosition = newNestedItem.BLqPosition;

        }

        private void PushItemLeft(Tuple feasiblePoint, Bin<Tuple> temporaryBin, Item temporaryItem, NestedItem newNestedItem)
        {

            //lista delle intersezioni tra item nuovo e item già in soluzione
            IList<NestedItem> intersectedItems = new List<NestedItem>();

            foreach (var nestedItem in temporaryBin.NestedItems)
            {
                if (feasiblePoint.Qposition < nestedItem.TLqPosition &&
                   feasiblePoint.Qposition >= nestedItem.BRqPosition &&
                   feasiblePoint.Pposition >= nestedItem.BRpPosition)
                {
                    intersectedItems.Add(nestedItem);
                }
            }

            //3 possibili risultati
            if (intersectedItems.Count == 0) //non ho intersezioni
            {
                newNestedItem.BLpPosition = 0;
                newNestedItem.TLpPosition = 0;
                newNestedItem.BRpPosition = temporaryItem.Width;
            }
            else if (intersectedItems.Count == 1) //1 sola intersezione
            {
                float delta = feasiblePoint.Pposition - intersectedItems.ElementAt(0).Width;
                newNestedItem.BLpPosition -= delta;
                newNestedItem.TLpPosition -= delta;
                newNestedItem.BRpPosition -= delta;
            }
            else if (intersectedItems.Count > 1) //N intersezioni
            {
                float widthSum = 0;
                foreach (var intersectedItem in intersectedItems)
                {
                    widthSum += intersectedItem.Width;
                }
                float delta = feasiblePoint.Pposition - widthSum;
                newNestedItem.BLpPosition -= delta;
                newNestedItem.TLpPosition -= delta;
                newNestedItem.BRpPosition -= delta;
            }

            feasiblePoint.PfinalPosition = newNestedItem.BLpPosition;


        }

        /// <summary>
        /// Questo metodo controlla se le dimensione dell'item eccedono quelle del bin
        /// </summary>
        /// <param name="newNestedItem"></param>
        /// <param name="temporaryBinHeight"></param>
        /// <param name="temporaryBinWidth"></param>
        /// <returns></returns>
        bool IsBorderObserved(NestedItem newNestedItem, float temporaryBinHeight, float temporaryBinWidth)
        {
            return newNestedItem.TLqPosition < temporaryBinHeight && newNestedItem.BRpPosition < temporaryBinWidth;
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
        private Tuple ApplyRule(IList<Tuple> minHatchedRegionTuples)
        {
            Tuple result = null;
            IList<Tuple> pMinTuples = new List<Tuple>();

            float minFinalP = minHatchedRegionTuples.OrderBy(x => x.PfinalPosition).First().PfinalPosition;

            foreach (var minHatchedRegionTuple in minHatchedRegionTuples)
            {
                if (minHatchedRegionTuple.PfinalPosition == minFinalP)
                {
                    pMinTuples.Add(minHatchedRegionTuple);
                }
            }

            if (pMinTuples.Count == 1)
            {
                result = pMinTuples.ElementAt(0);
            }
            else
            {
                IList<Tuple> qMinTuples = new List<Tuple>();
                float minFinalQ = pMinTuples.OrderBy(x => x.QfinalPosition).First().QfinalPosition;
                foreach (var qMinTuple in pMinTuples)
                {
                    if (qMinTuple.QfinalPosition == minFinalQ)
                    {
                        qMinTuples.Add(qMinTuple);
                    }
                }
                //if (qMinTuples.Count == 1)
                //{
                    result = qMinTuples.ElementAt(0);
                //}
            }
            return result;
        }
    }
}

