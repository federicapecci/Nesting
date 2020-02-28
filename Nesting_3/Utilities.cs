using System;
using System.Collections.Generic;
using System.Linq;

namespace Nesting_3
{
    /// <summary>
    /// classe che contiene le utilities 
    /// per implementare l'algoritmo hsolve
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
        public float ComputeLowerBound(IList<PricedItem> pricedItems, int binWidth, int binHeight)
        {
            float result = 0;

            foreach (var pricedItem in pricedItems)
            {
                result += pricedItem.Price;
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
        public bool IsBestPositionFound(Bin<Tuple> temporaryBin, PricedItem temporaryPricedItem)
        {
            SetFeasiblePoints(temporaryBin);

            //se il bin non contiene punti
            if (temporaryBin.Points.Count == 0)
            {
                return false;
            }
            else if (temporaryBin.Points.Count == 1 && //se il bin contiene 1 solo punto e quel punto è (0,0)
                    temporaryBin.Points.ElementAt(0).Pposition == 0 &&
                    temporaryBin.Points.ElementAt(0).Qposition == 0)
            {
                 temporaryBin.PricedItems = new List<PricedItem>(){
                     new PricedItem()
                     {
                         Height = temporaryPricedItem.Height,
                         Width = temporaryPricedItem.Width,
                         Id = temporaryPricedItem.Id,
                         Price = temporaryPricedItem.Price,
                         TLqPosition = temporaryPricedItem.Height,
                         BRpPosition =  temporaryPricedItem.Width,
                         TRpPosition =  temporaryPricedItem.Width,
                         TRqPosition = temporaryPricedItem.Height
                     }
                 };
                 HandleOperationsPostNestedItem(temporaryBin, temporaryPricedItem, temporaryBin.Points.ElementAt(0));
                 return true;
            }
            else if (temporaryBin.Points.Count > 1)//se il bin contiene n punti
            {
                foreach (var feasiblePoint in temporaryBin.Points)
                {
                    if (!feasiblePoint.IsUsed)
                    {
                        //assegno le coordinate di partenza al nuovo item da nestare, poi inzio a muoverlo
                        PricedItem newPricedItem = new PricedItem()
                        {
                            Height = temporaryPricedItem.Height,
                            Width = temporaryPricedItem.Width,
                            Id = temporaryPricedItem.Id,
                            Price = temporaryPricedItem.Price,
                            BLpPosition = feasiblePoint.Pposition,
                            BLqPosition = feasiblePoint.Qposition,
                            BRpPosition = feasiblePoint.Pposition + temporaryPricedItem.Width,
                            BRqPosition = feasiblePoint.Qposition,
                            TLpPosition = feasiblePoint.Pposition,
                            TLqPosition = feasiblePoint.Qposition + temporaryPricedItem.Height,
                            TRpPosition = feasiblePoint.Pposition + temporaryPricedItem.Width,
                            TRqPosition = feasiblePoint.Qposition + temporaryPricedItem.Height
                        };
                        feasiblePoint.HatchedArea = GetHatchedArea(temporaryBin, newPricedItem, feasiblePoint);
                    }
                }

                //trovo la tupla con lo scarto minore tra quelle ancora disponibili
                //(Where(x => x.HatchedArea >= 0) filtra solo le tuple con hatched area = 0)
                Tuple minHatchedAreaTuple = temporaryBin.Points.Where(x => x.IsUsed == false && x.HatchedArea >= 0)
                                                               .OrderBy(x => x.HatchedArea)
                                                               .FirstOrDefault();

                //se non riesco a trovare la tupla, vuol dire che le tuple sono finite 
                if (minHatchedAreaTuple == null)
                {
                    return false;
                }

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

                    var pricedItem = new PricedItem()
                    {
                        Height = temporaryPricedItem.Height,
                        Width = temporaryPricedItem.Width,
                        Id = temporaryPricedItem.Id,
                        Price = temporaryPricedItem.Price,
                        BLpPosition = minHatchedAreaPoint.PfinalPosition,
                        BLqPosition = minHatchedAreaPoint.QfinalPosition,
                        BRpPosition = minHatchedAreaPoint.PfinalPosition + temporaryPricedItem.Width,
                        BRqPosition = minHatchedAreaPoint.QfinalPosition,
                        TLpPosition = minHatchedAreaPoint.PfinalPosition,
                        TLqPosition = minHatchedAreaPoint.QfinalPosition + temporaryPricedItem.Height,
                        TRpPosition = minHatchedAreaPoint.PfinalPosition + temporaryPricedItem.Width,
                        TRqPosition = minHatchedAreaPoint.QfinalPosition + temporaryPricedItem.Height
                    };

                    temporaryBin.PricedItems.Add(pricedItem);
                    Console.WriteLine("item ID = " + pricedItem.Id + " nested in " + "(" + pricedItem.BLpPosition + ", " + pricedItem.BLqPosition + ")");
                    HandleOperationsPostNestedItem(temporaryBin, temporaryPricedItem, minHatchedAreaPoint);
                    return true;
                }
                else if (minHatchedAreaPoints.Count > 1)
                {
                    Tuple minCoordinatePoint = ApplyRule(minHatchedAreaPoints);

                    var pricedItem = new PricedItem()
                    {
                        Height = temporaryPricedItem.Height,
                        Width = temporaryPricedItem.Width,
                        Id = temporaryPricedItem.Id,
                        Price = temporaryPricedItem.Price,
                        BLpPosition = minCoordinatePoint.PfinalPosition,
                        BLqPosition = minCoordinatePoint.QfinalPosition,
                        BRpPosition = minCoordinatePoint.PfinalPosition + temporaryPricedItem.Width,
                        BRqPosition = minCoordinatePoint.QfinalPosition,
                        TLpPosition = minCoordinatePoint.PfinalPosition,
                        TLqPosition = minCoordinatePoint.QfinalPosition + temporaryPricedItem.Height,
                        TRpPosition = minCoordinatePoint.PfinalPosition + temporaryPricedItem.Width,
                        TRqPosition = minCoordinatePoint.QfinalPosition + temporaryPricedItem.Height
                    };

                    temporaryBin.PricedItems.Add(pricedItem);
                    Console.WriteLine("item ID = " + pricedItem.Id + " nested in " + "(" + pricedItem.BLpPosition + ", " + pricedItem.BLqPosition + ")");
                    HandleOperationsPostNestedItem(temporaryBin, temporaryPricedItem, minCoordinatePoint);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// metodo per gestire le operazioni post item nestato
        /// </summary>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <param name="point"></param>
        private void HandleOperationsPostNestedItem(Bin<Tuple> temporaryBin, PricedItem sortedTemporaryPricedItem, Tuple point)
        {
            //setto il punto ad usato, per recuparare il punto dalla lista uso l'id
            var matchingPoint = temporaryBin.Points.Where(x => x.Pposition == point.Pposition &&
                                                       x.Qposition == point.Qposition)
                                           .First();
            matchingPoint.IsUsed = true;

            //controllo se non è più possibile usare dei punti 
            foreach (var p in temporaryBin.Points)
            {
                //cerco punti "coperti" dal nuovo item nestato
                if ((p.Pposition >= sortedTemporaryPricedItem.BLpPosition && p.Pposition < sortedTemporaryPricedItem.BRpPosition &&
                   p.Qposition >= sortedTemporaryPricedItem.BLqPosition && p.Qposition < sortedTemporaryPricedItem.TLqPosition) ||
                   
                   (p.PfinalPosition == matchingPoint.PfinalPosition && //cerco punti che, dopo push down and left portavano alle stesse coordinate finali
                   p.QfinalPosition == matchingPoint.QfinalPosition))
                   
                {
                    p.IsUsed = true;
                }
            }

            //controllo se il primo nuovo punto (TL) da aggiungere è già presente nella lista temporaryBin.Points
            Tuple pointFound = temporaryBin.Points.Where(x => x.Pposition == point.PfinalPosition &&
                                                   x.Qposition == point.QfinalPosition + sortedTemporaryPricedItem.Height &&
                                                   x.IsUsed == false).FirstOrDefault();

            //definisco il primo nuovo punto
            var firstPoint = new Tuple()
            {
                Pposition = point.PfinalPosition,
                Qposition = point.QfinalPosition + sortedTemporaryPricedItem.Height,
                IsUsed = false

            };

            //controllo se il primo nuovo punto è idoneo ad essere aggiunto perché
            //potrebbe essere erroneaemente creato sul lato di un item già esistente
            bool isPointLyingOnItemSide = false;
            foreach (var ni in temporaryBin.PricedItems)
            {
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
            pointFound = temporaryBin.Points.Where(x => x.Pposition == point.PfinalPosition + sortedTemporaryPricedItem.Width &&
                                                        x.Qposition == point.QfinalPosition).FirstOrDefault();

            //definisco il secondo nuovo punto
            var secondPoint = new Tuple()
            {
                Pposition = point.PfinalPosition + sortedTemporaryPricedItem.Width,
                Qposition = point.QfinalPosition,
                IsUsed = false
            };

            //controllo se il secondo nuovo punto è idoneo ad essere aggiunto perché
            //potrebbe essere erroneaemente creato sul lato di un item già esistente
            foreach (var ni in temporaryBin.PricedItems)
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
            sortedTemporaryPricedItem.IsRemoved = true;
        }

        /// <summary>
        /// questo metodo aggiorna il prezzo di ogni item 
        /// </summary>
        /// <param name="z"></param>
        /// <param name="items"></param>
        /// <param name="temporaryBin"></param>
        public void UpdatePrice(float z, IList<PricedItem> items, IList<Bin<Tuple>> bins)
        {
            float alpha = 0.9F;
            float beta = 1.1F;
            bool isItemFound = false;

            //dato un certo item
            foreach (var item in items)
            {
                foreach (var bin in bins) //scorro tutti i bins
                {
                    if (bin.PricedItems != null)
                    {
                        foreach (var nestedItem in bin.PricedItems) //e scorro tutti i nested items di ogni bin
                        {
                            if (nestedItem.Id == item.Id) //se trovo l'id di un nested item che corrisponde all'id dell'item dato
                            {
                                //aggiorno il prezzo dell'item dato in base a se il nested item con id corrispondente si trova nella prima o nella seconda metà dei bin
                                if (bin.Id <= (0.5 * z))
                                {
                                    item.Price = alpha * item.Price;
                                }
                                else if (bin.Id > (0.5 * z))
                                {
                                    item.Price = beta * item.Price;
                                }
                                isItemFound = true;
                                break;
                            }
                        }
                        if (isItemFound)
                        {
                            isItemFound = false;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// metodo che setta come usate le tuple generate che 
        /// sono uguali alle dimensioni del bin o le eccedono
        /// </summary>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <returns></returns>
        private void SetFeasiblePoints(Bin<Tuple> temporaryBin)
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
        /// metodo che calcola l'hatched area totale relativa 
        /// al posizionamento di un nuovo item e la ritorna.
        /// il metodo ritorna -1 se l'item, per come è posizionato (cioè dopo averlo spinto 
        /// in basso a sinistra), eccede le dimensioni del bin
        /// </summary>
        /// <param name="feasiblePoint"></param>
        /// <param name="newNestedItem"></param>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <returns></returns>
        private float GetHatchedArea(Bin<Tuple> temporaryBin, PricedItem newPricedItem, Tuple feasiblePoint)
        {
            IList<PricedItem> downIntersectedPricedItems = PushItemDown(temporaryBin, newPricedItem, feasiblePoint);
            IList<PricedItem> leftIntersectedPricedItems = PushItemLeft(temporaryBin, newPricedItem, feasiblePoint);


            //controllo se l'oggetto, anche essendo stato spostato in basso a sintra, sborderebbe e
            //se la posizione in cui deve essere nestato il nuovo item comporterebbe delle sovrapposizioni con item già in soluzione
            if (IsBorderObserved(newPricedItem, temporaryBin.Height, temporaryBin.Width) && IsOverlappingOk(newPricedItem, temporaryBin)) 
            {
                ComputeHatchedArea(feasiblePoint, newPricedItem, downIntersectedPricedItems, leftIntersectedPricedItems);
                //Console.WriteLine("(" + feasiblePoint.Pposition + ", " + feasiblePoint.Qposition + "), HR = " + feasiblePoint.HatchedArea +
                //", new coord(" + feasiblePoint.PfinalPosition + ", " + feasiblePoint.QfinalPosition + ")");
                return feasiblePoint.HatchedArea;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// metodo che calcola l'hatched area prima sotto 
        /// e poi a sinistra del nuovo item
        /// </summary>
        /// <param name="downIntersectedNestedItems"></param>
        /// <param name="leftIntersectedNestedItems"></param>
        /// <returns></returns>
        /// 
        private void ComputeHatchedArea(Tuple feasiblePoint, PricedItem newPricedItem, IList<PricedItem> downIntersectedNestedItems, IList<PricedItem> leftIntersectedNestedItems)
        {
            float totalHatchedArea = 0;
            //variabile per l'hatched area che eventualmente rimane sotto e a sinitra 
            float partialHatchedArea;

            if (downIntersectedNestedItems.Count > 0)
            {
                //definiso la green area sotto il nuovo item
                GreenZone greenZone = new GreenZone()
                {
                    BRpPosition = feasiblePoint.PfinalPosition + newPricedItem.Width,
                    BRqPosition = 0,
                    TRpPosition = feasiblePoint.PfinalPosition + newPricedItem.Width,
                    TRqPosition = feasiblePoint.QfinalPosition,
                    BLpPosition = feasiblePoint.PfinalPosition,
                    BLqPosition = 0,
                    TLpPosition = feasiblePoint.PfinalPosition,
                    TLqPosition = feasiblePoint.QfinalPosition,
                    Height = newPricedItem.BRqPosition,
                    Width = newPricedItem.Width,
                    Area = newPricedItem.BRqPosition * newPricedItem.Width
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
                    if (newPricedItem.BLpPosition < intersectedItem.BLpPosition && //new item + a sx di interesect item
                       newPricedItem.BRpPosition < intersectedItem.BRpPosition)
                    {
                        intersectedWidth = newPricedItem.BRpPosition - intersectedItem.BLpPosition;
                    }
                    else if (intersectedItem.BLpPosition < newPricedItem.BLpPosition && //new item è + a  dx di intersected item
                       intersectedItem.BRpPosition < newPricedItem.BRpPosition)
                    {
                        intersectedWidth = intersectedItem.BRpPosition - newPricedItem.BLpPosition;
                    }
                    else if ((newPricedItem.BLpPosition == intersectedItem.BLpPosition && //new item inizia come insertected item ma termina prima
                    newPricedItem.BRpPosition < intersectedItem.BRpPosition) ||
                    (intersectedItem.BLpPosition < newPricedItem.BLpPosition && // new item inizia dopo di insertected item ma terminano uguali
                    intersectedItem.BRpPosition == newPricedItem.BRpPosition) ||
                    (intersectedItem.BLpPosition < newPricedItem.BLpPosition &&  //le coordinate p del new item cadono dentro quelle p dell'intersected item
                    intersectedItem.BRpPosition > newPricedItem.BRpPosition))
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
                    TRqPosition = feasiblePoint.QfinalPosition + newPricedItem.Height,
                    BLpPosition = 0,
                    BLqPosition = feasiblePoint.QfinalPosition,
                    TLpPosition = 0,
                    TLqPosition = feasiblePoint.QfinalPosition + newPricedItem.Height,
                    Height = newPricedItem.Height,
                    Width = newPricedItem.BLpPosition,
                    Area = newPricedItem.Height * newPricedItem.BLpPosition
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
                    if (intersectedItem.BLqPosition < newPricedItem.BLqPosition && //new item + in alto di intersected item
                       intersectedItem.TLqPosition < newPricedItem.TLqPosition)
                    {
                        intersectedHeight = intersectedItem.TLqPosition - newPricedItem.BLqPosition;
                    }
                    else if (newPricedItem.BLqPosition < intersectedItem.BLqPosition && //new item + in basso di interesect item
                      newPricedItem.TLqPosition < intersectedItem.TLqPosition)
                    {
                        intersectedHeight = newPricedItem.TLqPosition - intersectedItem.BLqPosition;
                    }
                    else if ((newPricedItem.BLqPosition == intersectedItem.BLqPosition && //new item inizia come insertected item ma termina prima
                       newPricedItem.TLqPosition < intersectedItem.TLqPosition) ||
                       (intersectedItem.BLqPosition < newPricedItem.BLqPosition && // new item inizia dopo di insertected item ma terminano uguali
                       intersectedItem.TLqPosition == newPricedItem.TLqPosition) ||
                       (intersectedItem.BLqPosition < newPricedItem.BLqPosition && //le coordinate q del new item cadono dentro quelle dell'intersected item 
                       intersectedItem.TLqPosition > newPricedItem.TLqPosition))
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
        /// metodo che spinge il più in basso possibile
        /// il nuovo item da nestare
        /// </summary>
        /// <param name="feasiblePoint"></param>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <param name="newNestedItem"></param>
        /// <returns></returns>
        private IList<PricedItem> PushItemDown( Bin<Tuple> temporaryBin, PricedItem newPricedItem, Tuple feasiblePoint)
        {
            //lista delle intersezioni tra item nuovo e item già in soluzione
            IList<PricedItem> intersectedPricedItems = new List<PricedItem>();

            foreach (var pricedItem in temporaryBin.PricedItems)
            {
                //cerco intersezioni verticali tra nuovo item e item già in soluzione (HO TOLTO UGUALE DESTRA -> CHECK)
                if (((newPricedItem.BLpPosition >= pricedItem.BLpPosition && newPricedItem.BLpPosition < pricedItem.BRpPosition) ||
                   (newPricedItem.BRpPosition > pricedItem.BLpPosition && newPricedItem.BRpPosition <= pricedItem.BRpPosition)) &&
                    newPricedItem.BLqPosition >= pricedItem.TLqPosition)
                {
                    intersectedPricedItems.Add(pricedItem);
                }
            }

            //3 possibili risultati
            if (intersectedPricedItems.Count == 0) //non ho intersezioni
            {
                newPricedItem.BLqPosition = 0;
                newPricedItem.TLqPosition = newPricedItem.Height;
                newPricedItem.BRqPosition = 0;
            }
            else
            {
                if (intersectedPricedItems.Count == 1) //1 sola intersezione
                {
                    float delta = feasiblePoint.Qposition - intersectedPricedItems.ElementAt(0).Height;
                    newPricedItem.BLqPosition -= delta;
                    newPricedItem.TLqPosition -= delta;
                    newPricedItem.BRqPosition -= delta;
                }
                else if (intersectedPricedItems.Count > 1) //N intersezioni
                {
                    float heightSum = intersectedPricedItems.OrderBy(x => x.TLqPosition).Last().TLqPosition;
                    float delta = feasiblePoint.Qposition - heightSum;
                    newPricedItem.BLqPosition -= delta;
                    newPricedItem.TLqPosition -= delta;
                    newPricedItem.BRqPosition -= delta;
                }
            }

            feasiblePoint.QfinalPosition = newPricedItem.BLqPosition;
            return intersectedPricedItems;

        }

        /// <summary>
        /// il metodo che spinge il più a sinistra possibile
        /// il nuovo item da nestare
        /// </summary>
        /// <param name="feasiblePoint"></param>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <param name="newNestedItem"></param>
        /// <returns></returns>
        private IList<PricedItem> PushItemLeft(Bin<Tuple> temporaryBin, PricedItem newPricedItem, Tuple feasiblePoint)
        {
            //lista delle intersezioni tra item nuovo e item già in soluzione
            IList<PricedItem> intersectedPricedItems = new List<PricedItem>();

            foreach (var pricedItem in temporaryBin.PricedItems)
            {
                //cerco interesezioni orizzontali tra nuovo item e item già in soluzione (HO TOLTO UGUALE DESTRA -> CHECK)
                if (((newPricedItem.BLqPosition >= pricedItem.BLqPosition && newPricedItem.BLqPosition < pricedItem.TLqPosition) ||
                    (newPricedItem.TLqPosition > pricedItem.BLqPosition && newPricedItem.TLqPosition <= pricedItem.TLqPosition)) &&
                    newPricedItem.BLpPosition >= pricedItem.BRpPosition)
                {
                    intersectedPricedItems.Add(pricedItem);
                }
            }

            //3 possibili risultati
            if (intersectedPricedItems.Count == 0) //non ho intersezioni
            {
                newPricedItem.BLpPosition = 0;
                newPricedItem.TLpPosition = 0;
                newPricedItem.BRpPosition = newPricedItem.Width;
            }
            else
            {
                if (intersectedPricedItems.Count == 1) //1 sola intersezione
                {
                    float delta = feasiblePoint.Pposition - intersectedPricedItems.ElementAt(0).Width;
                    newPricedItem.BLpPosition -= delta;
                    newPricedItem.TLpPosition -= delta;
                    newPricedItem.BRpPosition -= delta;
                }
                else if (intersectedPricedItems.Count > 1) //N intersezioni
                {
                    float widthSum = intersectedPricedItems.OrderBy(x => x.BRpPosition).Last().BRpPosition;
                    float delta = feasiblePoint.Pposition - widthSum;
                    newPricedItem.BLpPosition -= delta;
                    newPricedItem.TLpPosition -= delta;
                    newPricedItem.BRpPosition -= delta;
                }
            }

            feasiblePoint.PfinalPosition = newPricedItem.BLpPosition;
            return intersectedPricedItems;

        }

        /// <summary>
        /// metodo che controlla se, a fronte del posizione del nuovo item da nestare, 
        /// le dimensioni di tale item sforano rispetto alle dimensioni del bin
        /// </summary>
        /// <param name="newNestedItem"></param>
        /// <param name="temporaryBinHeight"></param>
        /// <param name="temporaryBinWidth"></param>
        /// <returns></returns>
        bool IsBorderObserved(PricedItem newPricedItem, float temporaryBinHeight, float temporaryBinWidth)
        {
            return newPricedItem.TLqPosition <= temporaryBinHeight && newPricedItem.BRpPosition <= temporaryBinWidth;
        }
        
        /// <summary>
        /// metodo per controllare se nestare un nuovo item in un certo punto
        /// comporta delle sovrapposizioni tra nuovo item e item già in soluzione
        /// </summary>
        /// <param name="newPricedItem"></param>
        /// <param name="temporaryBin"></param>
        /// <returns></returns>
        bool IsOverlappingOk(PricedItem newPricedItem, Bin<Tuple> temporaryBin)
        {
            foreach (var pricedItem in temporaryBin.PricedItems)
            {
                //se la coordinata in alto a destra del nuovo item interseca item presistenti 
                //significa che vi sono sovrapposizioni
                if ((newPricedItem.TRpPosition > pricedItem.BLpPosition && newPricedItem.TRpPosition <= pricedItem.BRpPosition) &&
                    (newPricedItem.TRqPosition > pricedItem.BLqPosition && newPricedItem.TRqPosition <= pricedItem.TLqPosition))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Metodo che stabilisce un criterio per decidere quale
        /// tripla scegliere se ho più triple che hanno lo stesso scarto potenziale minimo.
        /// Il criterio implementato fa in modo che gli item stiano il più a sinistra possibile.
        /// In particolare:
        /// -tra le triple sceglie quella con la p minima
        /// -a parità di p minima sceglie la tripla con la q minima
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
                result = qMinTuples.ElementAt(0);
            }
            return result;
        }
    }
}

