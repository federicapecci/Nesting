using System;
using System.Collections.Generic;
using System.Linq;

namespace Nesting_2
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
                var nestedItem = new NestedItem(temporaryItem)
                {
                    BLpPosition = 0,
                    BLqPosition = 0,
                    BRpPosition = temporaryItem.Width,
                    BRqPosition = 0,
                    TLpPosition = 0,
                    TLqPosition = temporaryItem.Height
                };

                temporaryBin.NestedItems = new List<NestedItem>();
                temporaryBin.NestedItems.Add(nestedItem);

                HandleOperationsPostNestedItem(temporaryBin, temporaryItem, temporaryBin.Points.ElementAt(0), nestedItem);
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
                        feasiblePoint.HatchedArea = GetHatchedArea(feasiblePoint, newNestedItem, temporaryBin, temporaryItem);
                    }
                }

                //trovo la tupla con lo scarto minore tra quelle ancora disponibili
                //(Where(x => x.HatchedArea >= 0) filtra solo le tuple con hatched area = 0)
                Tuple minHatchedAreaTuple = temporaryBin.Points.Where(x => x.IsUsed == false && x.HatchedArea >= 0)
                                                               .OrderBy(x => x.HatchedArea)
                                                               .First();

                //controllo se ho più tuple che hanno lo stesso scarto (il minore)
                IList<Tuple> minHatchedAreaPoints = new List<Tuple>();
                foreach (var point in temporaryBin.Points)
                {
                    if (point.HatchedArea == minHatchedAreaTuple.HatchedArea && !point.IsUsed)
                    {
                        minHatchedAreaPoints.Add(point);
                    }
                }

                if (minHatchedAreaPoints.Count == 1)
                {
                    var minHatchedAreaPoint = minHatchedAreaPoints.ElementAt(0);

                    var nestedItem = new NestedItem(temporaryItem)
                    {
                        BLpPosition = minHatchedAreaPoint.PfinalPosition,
                        BLqPosition = minHatchedAreaPoint.QfinalPosition,
                        BRpPosition = minHatchedAreaPoint.PfinalPosition + temporaryItem.Width,
                        BRqPosition = minHatchedAreaPoint.QfinalPosition,
                        TLpPosition = minHatchedAreaPoint.PfinalPosition,
                        TLqPosition = minHatchedAreaPoint.QfinalPosition + temporaryItem.Height
                    };

                    temporaryBin.NestedItems.Add(nestedItem);

                    HandleOperationsPostNestedItem(temporaryBin, temporaryItem, minHatchedAreaPoint, nestedItem);
                    return true;
                }
                else if (minHatchedAreaPoints.Count > 1)
                {
                    Tuple minCoordinatePoint = ApplyRule(minHatchedAreaPoints);

                    var nestedItem = new NestedItem(temporaryItem)
                    {
                        BLpPosition = minCoordinatePoint.PfinalPosition,
                        BLqPosition = minCoordinatePoint.QfinalPosition,
                        BRpPosition = minCoordinatePoint.PfinalPosition + temporaryItem.Width,
                        BRqPosition = minCoordinatePoint.QfinalPosition,
                        TLpPosition = minCoordinatePoint.PfinalPosition,
                        TLqPosition = minCoordinatePoint.QfinalPosition + temporaryItem.Height
                    };

                    temporaryBin.NestedItems.Add(nestedItem);

                    HandleOperationsPostNestedItem(temporaryBin, temporaryItem, minCoordinatePoint, nestedItem);
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
        private void HandleOperationsPostNestedItem(Bin<Tuple> temporaryBin, Item temporaryItem, Tuple point, NestedItem nestedItem)
        {
            //setto il punto ad usato, per recuparare il punto dalla lista uso l'id
            var matchingPoint = temporaryBin.Points.Where(x => x.Pposition == point.Pposition &&
                                                       x.Qposition == point.Qposition)
                                           .First();
            matchingPoint.IsUsed = true;

            //controllo se non è più possibile usare dei punti in quanto sono stati "coperti" dal nuovo item nestato
            foreach (var p in temporaryBin.Points)
            {
                if (p.Pposition >= nestedItem.BLpPosition && p.Pposition < nestedItem.BRpPosition &&
                   p.Qposition >= nestedItem.BLqPosition && p.Qposition < nestedItem.TLqPosition)
                {
                    p.IsUsed = true;
                }
            }

            //controllo se il primo nuovo punto (TL) da aggiungere è già presente nella lista temporaryBin.Points
            Tuple pointFound = temporaryBin.Points.Where(x => x.Pposition == point.PfinalPosition &&
                                                   x.Qposition == point.QfinalPosition + temporaryItem.Height &&
                                                   x.IsUsed == false).FirstOrDefault();

            //definisco il primo nuovo punto
            var firstPoint = new Tuple()
            {
                Pposition = point.PfinalPosition,
                Qposition = point.QfinalPosition + temporaryItem.Height,
                IsUsed = false

            };

            //controllo se il primo nuovo punto è idoneo ad essere aggiunto perché
            //potrebbe essere erroneaemente creato sul lato di un item già esistente
            bool isPointLyingOnItemSide = false;
            foreach (var ni in temporaryBin.NestedItems) {
                if (firstPoint.Qposition == ni.BLqPosition &&
                    firstPoint.Pposition >= ni.BLpPosition &&
                    firstPoint.Pposition <= ni.BRpPosition)
                {
                    isPointLyingOnItemSide = true;
                    break;
                }
            }

            //aggiungo il primo nuovo punto
            if (pointFound == null && !isPointLyingOnItemSide)
            {
                temporaryBin.Points.Add(firstPoint);
            }
            else
            {
                pointFound = null;
            }

            isPointLyingOnItemSide = false;

            //controllo se il secondo nuovo punto (BR) da aggiungere è già presente nella lista temporaryBin.Points
            pointFound = temporaryBin.Points.Where(x => x.Pposition == point.PfinalPosition + temporaryItem.Width &&
                                                        x.Qposition == point.QfinalPosition).FirstOrDefault();

            //definisco il secondo nuovo punto
            var secondPoint = new Tuple()
            {
                Pposition = point.PfinalPosition + temporaryItem.Width,
                Qposition = point.QfinalPosition,
                IsUsed = false
            };

            //controllo se il secondo nuovo punto è idoneo ad essere aggiunto perché
            //potrebbe essere erroneaemente creato sul lato di un item già esistente
            foreach (var ni in temporaryBin.NestedItems)
            {
                if (secondPoint.Pposition == ni.BLpPosition &&
                    secondPoint.Qposition >= ni.BLqPosition &&
                    secondPoint.Qposition <= ni.TLqPosition)
                {
                    isPointLyingOnItemSide = true;
                    break;
                }
            }

            //aggiungo il secondo nuovo punto
            if (pointFound == null && !isPointLyingOnItemSide)
            {
                temporaryBin.Points.Add(secondPoint);
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
        /// sono uguali alle dimensioni del bin o le eccedono
        /// </summary>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <returns></returns>
        private void SetFeasiblePoints(Bin<Tuple> temporaryBin, Item temporaryItem)
        {
            foreach (var point in temporaryBin.Points)
            {
                if (point.Pposition >= temporaryBin.Width || point.Qposition >= temporaryBin.Height)
                {
                    point.IsUsed = true;
                }
            }
        }

        /// <summary>
        /// questo metodo calcola l'hatched area in basso e a sinistra dell'item.
        /// Infine, ritorna la somma delle due. 
        /// Ritorna -1 se l'item, per come è posizionato, eccede le dimensioni del bin
        /// </summary>
        /// <param name="feasiblePoint"></param>
        /// <param name="newNestedItem"></param>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <returns></returns>
        private float GetHatchedArea(Tuple feasiblePoint, NestedItem newNestedItem, Bin<Tuple> temporaryBin, Item temporaryItem)
        {
            IList<NestedItem> downIntersectedNestedItems = PushItemDown(feasiblePoint, temporaryBin, temporaryItem, newNestedItem);
            IList<NestedItem> leftIntersectedNestedItems = PushItemLeft(feasiblePoint, temporaryBin, temporaryItem, newNestedItem);

            //controllo se l'oggettto, anche essendo stato spostato in basso a sintra, sborda
            if (IsBorderObserved(newNestedItem, temporaryBin.Height, temporaryBin.Width))
            {
                ComputeHatchedArea(feasiblePoint, newNestedItem, downIntersectedNestedItems, leftIntersectedNestedItems);
                Console.WriteLine("(" + feasiblePoint.Pposition + ", " + feasiblePoint.Qposition + "), HR = " + feasiblePoint.HatchedArea +
                ", new coord(" + feasiblePoint.PfinalPosition + ", " + feasiblePoint.QfinalPosition + ")");
                return feasiblePoint.HatchedArea;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// questo metodo calcolo l'hatched area prima sotto 
        /// e poi a sinistra del nuovo item
        /// </summary>
        /// <param name="downIntersectedNestedItems"></param>
        /// <param name="leftIntersectedNestedItems"></param>
        /// <returns></returns>
        private void ComputeHatchedArea(Tuple feasiblePoint, NestedItem newNestedItem, IList<NestedItem> downIntersectedNestedItems, IList<NestedItem> leftIntersectedNestedItems)
        {
            float totalHatchedArea = 0;
            //variabile per l'hatched area che eventualmente rimane sotto e a sinitra 
            float partialHatchedArea; 

            if (downIntersectedNestedItems.Count > 0)
            {
                //definiso la green area sotto il nuovo item
                GreenZone greenZone = new GreenZone()
                {
                    BRpPosition = feasiblePoint.PfinalPosition + newNestedItem.Width,
                    BRqPosition = 0,
                    TRpPosition = feasiblePoint.PfinalPosition + newNestedItem.Width,
                    TRqPosition = feasiblePoint.QfinalPosition,
                    BLpPosition = feasiblePoint.PfinalPosition,
                    BLqPosition = 0,
                    TLpPosition = feasiblePoint.PfinalPosition,
                    TLqPosition = feasiblePoint.QfinalPosition,
                    Height = newNestedItem.BRqPosition,
                    Width = newNestedItem.Width,
                    Area = newNestedItem.BRqPosition * newNestedItem.Width
                };

                //per ogni item già in posizione, che è stato intersecato, 
                //calcolo quanta parte di area di esso rientra nella green area e 
                //infine sommo tale area all'area totale degli item intersecati
                float itemsInSolutionArea = 0;
                float intersectedWidth;

                foreach (var intersectedItem in downIntersectedNestedItems)
                {
                    //if else per scegliere la width dell'item in soluzione 
                    //di cui devo calcolare l'area 
                    //per fare valore assoluto Math.Abs( ... );
                    if(newNestedItem.BLpPosition < intersectedItem.BLpPosition && //new item + a sx di interesect item
                       newNestedItem.BRpPosition < intersectedItem.BRpPosition) 
                    { 
                        intersectedWidth = newNestedItem.BRpPosition - intersectedItem.BLpPosition;
                    }
                    else if(intersectedItem.BLpPosition < newNestedItem.BLpPosition && //new item è + a  dx di intersected item
                       intersectedItem.BRpPosition < newNestedItem.BRpPosition)
                    {
                        intersectedWidth = intersectedItem.BRpPosition - newNestedItem.BLpPosition;
                    }
                    else if ((newNestedItem.BLpPosition == intersectedItem.BLpPosition && //new item inizia come insertected item ma termina prima
                    newNestedItem.BRpPosition < intersectedItem.BRpPosition) ||
                    (intersectedItem.BLpPosition < newNestedItem.BLpPosition && // new item inizia dopo di insertected item ma terminano uguali
                    intersectedItem.BRpPosition == newNestedItem.BRpPosition) || 
                    (intersectedItem.BLpPosition < newNestedItem.BLpPosition &&  //le coordinate p del new item cadono dentro quelle p dell'intersected item
                    intersectedItem.BRpPosition > newNestedItem.BRpPosition))
                    {
                        intersectedWidth = greenZone.Width;
                    }
                    else
                    {
                        intersectedWidth = intersectedItem.Width;
                    }

                    itemsInSolutionArea += (intersectedItem.Height * intersectedWidth);
                }

                partialHatchedArea = greenZone.Area - itemsInSolutionArea;
                totalHatchedArea += partialHatchedArea;
            }

            if (leftIntersectedNestedItems.Count > 0)
            {
                //definiso la green area a sintra del nuovo item
                GreenZone greenZone = new GreenZone()
                {
                    BRpPosition = feasiblePoint.PfinalPosition,
                    BRqPosition = feasiblePoint.QfinalPosition,
                    TRpPosition = feasiblePoint.PfinalPosition,
                    TRqPosition = feasiblePoint.QfinalPosition + newNestedItem.Height,
                    BLpPosition = 0,
                    BLqPosition = feasiblePoint.QfinalPosition,
                    TLpPosition = 0,
                    TLqPosition = feasiblePoint.QfinalPosition + newNestedItem.Height,
                    Height = newNestedItem.Height,
                    Width = newNestedItem.BLpPosition,
                    Area = newNestedItem.Height * newNestedItem.BLpPosition
                };

                //per ogni item già in posizione, che è stato intersecato, 
                //calcolo quanta parte di area di esso rientra nella green area e 
                //infine sommo tale area all'area totale degli item intersecati
                float itemsInSolutionArea = 0;
                float intersectedHeight;
                foreach (var intersectedItem in leftIntersectedNestedItems)
                {
                    //if else per scegliere la height dell'item in soluzione 
                    //di cui devo calcolare l'area 
                    //per fare valore assoluto Math.Abs( ... );
                    if (intersectedItem.BLqPosition < newNestedItem.BLqPosition && //new item + in alto di intersected item
                       intersectedItem.TLqPosition < newNestedItem.TLqPosition)
                    {
                        intersectedHeight = intersectedItem.TLqPosition - newNestedItem.BLqPosition;
                    }else if (newNestedItem.BLqPosition < intersectedItem.BLqPosition && //new item + in basso di interesect item
                       newNestedItem.TLqPosition < intersectedItem.TLqPosition)
                    {
                        intersectedHeight = newNestedItem.TLqPosition - intersectedItem.BLqPosition;
                    }
                    else if ((newNestedItem.BLqPosition == intersectedItem.BLqPosition && //new item inizia come insertected item ma termina prima
                       newNestedItem.TLqPosition < intersectedItem.TLqPosition) ||
                       (intersectedItem.BLqPosition < newNestedItem.BLqPosition && // new item inizia dopo di insertected item ma terminano uguali
                       intersectedItem.TLqPosition == newNestedItem.TLqPosition) ||
                       (intersectedItem.BLqPosition < newNestedItem.BLqPosition && //le coordinate q del new item cadono dentro quelle dell'intersected item 
                       intersectedItem.TLqPosition > newNestedItem.TLqPosition))
                    {
                        intersectedHeight = greenZone.Height;
                    }
                    else
                    {
                        intersectedHeight = intersectedItem.Height;
                    }

                    itemsInSolutionArea += (intersectedHeight * intersectedItem.Width);
                }

                partialHatchedArea = greenZone.Area - itemsInSolutionArea;
                totalHatchedArea += partialHatchedArea;
            }

            feasiblePoint.HatchedArea = totalHatchedArea;
        }


        /// <summary>
        /// questo metodo spinge in basso un nuovo item e
        /// calcolo l'hatched area sotto il nuovo iten
        /// </summary>
        /// <param name="feasiblePoint"></param>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <param name="newNestedItem"></param>
        /// <returns></returns>
        private IList<NestedItem> PushItemDown(Tuple feasiblePoint, Bin<Tuple> temporaryBin, Item temporaryItem, NestedItem newNestedItem)
        {
            //lista delle intersezioni tra item nuovo e item già in soluzione
            IList<NestedItem> intersectedItems = new List<NestedItem>();

            foreach (var nestedItem in temporaryBin.NestedItems)
            {
                //cerco intersezioni verticali tra nuovo item e item già in soluzione (HO TOLTO UGUALE DESTRA -> CHECK)
                if (((newNestedItem.BLpPosition >= nestedItem.BLpPosition && newNestedItem.BLpPosition < nestedItem.BRpPosition) ||
                   (newNestedItem.BRpPosition >= nestedItem.BLpPosition && newNestedItem.BRpPosition < nestedItem.BRpPosition)) &&
                    newNestedItem.BLqPosition >= nestedItem.TLqPosition)
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
            else
            {
                if (intersectedItems.Count == 1) //1 sola intersezione
                {
                    float delta = feasiblePoint.Qposition - intersectedItems.ElementAt(0).Height;
                    newNestedItem.BLqPosition -= delta;
                    newNestedItem.TLqPosition -= delta;
                    newNestedItem.BRqPosition -= delta;
                }
                else if (intersectedItems.Count > 1) //N intersezioni
                {
                    float heightSum = intersectedItems.OrderBy(x => x.TLqPosition).Last().TLqPosition;
                    float delta = feasiblePoint.Qposition - heightSum;
                    newNestedItem.BLqPosition -= delta;
                    newNestedItem.TLqPosition -= delta;
                    newNestedItem.BRqPosition -= delta;
                }
            }

            feasiblePoint.QfinalPosition = newNestedItem.BLqPosition;
            return intersectedItems;

        }

        /// <summary>
        /// questo metodo spinge a sinistra un nuovo item e
        /// calcolo l'hatched area a sinistra del nuovo iten
        /// </summary>
        /// <param name="feasiblePoint"></param>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <param name="newNestedItem"></param>
        /// <returns></returns>
        private IList<NestedItem> PushItemLeft(Tuple feasiblePoint, Bin<Tuple> temporaryBin, Item temporaryItem, NestedItem newNestedItem)
        {
            //lista delle intersezioni tra item nuovo e item già in soluzione
            IList<NestedItem> intersectedItems = new List<NestedItem>();

            foreach (var nestedItem in temporaryBin.NestedItems)
            {
                //cerco interesezioni orizzontali tra nuovo item e item già in soluzione (HO TOLTO UGUALE DESTRA -> CHECK)
                if(((newNestedItem.BLqPosition >= nestedItem.BLqPosition && newNestedItem.BLqPosition < nestedItem.TLqPosition) ||
                    (newNestedItem.TLqPosition >= nestedItem.BLqPosition && newNestedItem.TLqPosition < nestedItem.TLqPosition)) &&
                    newNestedItem.BLpPosition >= nestedItem.BRpPosition) { 
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
            else
            {
                if (intersectedItems.Count == 1) //1 sola intersezione
                {
                    float delta = feasiblePoint.Pposition - intersectedItems.ElementAt(0).Width;
                    newNestedItem.BLpPosition -= delta;
                    newNestedItem.TLpPosition -= delta;
                    newNestedItem.BRpPosition -= delta;
                }
                else if (intersectedItems.Count > 1) //N intersezioni
                {
                    float widthSum = intersectedItems.OrderBy(x => x.BRpPosition).Last().BRpPosition;
                    float delta = feasiblePoint.Pposition - widthSum;
                    newNestedItem.BLpPosition -= delta;
                    newNestedItem.TLpPosition -= delta;
                    newNestedItem.BRpPosition -= delta;
                }
            }

            feasiblePoint.PfinalPosition = newNestedItem.BLpPosition;
            return intersectedItems;

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
            return newNestedItem.TLqPosition <= temporaryBinHeight && newNestedItem.BRpPosition <= temporaryBinWidth;
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
        private Tuple ApplyRule(IList<Tuple> minHatchedAreaTuples)
        {
            Tuple result = null;
            IList<Tuple> pMinTuples = new List<Tuple>();

            float minFinalP = minHatchedAreaTuples.OrderBy(x => x.PfinalPosition).First().PfinalPosition;

            foreach (var minHatchedAreaTuple in minHatchedAreaTuples)
            {
                if (minHatchedAreaTuple.PfinalPosition == minFinalP)
                {
                    pMinTuples.Add(minHatchedAreaTuple);
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

