﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Nesting_4
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
        public double ComputeLowerBound(IList<Item> pricedItems, int binWidth, int binHeight)
        {
            double result = 0;

            foreach (Item pricedItem in pricedItems)
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
        public Bin IsBestPositionFound(Bin temporaryBin, Item temporaryItem, string itemAllocationMethod)
        {


            //se l'item non è nestabile (ovvero le sue dimensioni eccedano quelle del bin)
            //setto comunque l'item come removed, altrimenti la lista di item non sarà mai empty e hsolve non va avanti
            /*if(temporaryItem.Height > temporaryBin.Height || temporaryItem.Width > temporaryBin.Width)
            {
                temporaryItem.IsRemoved = true;
                return temporaryBin;
            }*/

            //item nestabile, procedo
            SetFeasiblePoints(temporaryBin);

            //se il bin non contiene punti
            if (temporaryBin.Points.Count == 0)
            {
                return temporaryBin;
            }
            else if (temporaryBin.Points.Count == 1 && //se il bin contiene 1 solo punto e quel punto è (0,0)
                    temporaryBin.Points.ElementAt(0).Pposition == 0 &&
                    temporaryBin.Points.ElementAt(0).Qposition == 0)
            {
                Item pricedItem = new Item()
                {
                    Height = temporaryItem.Height,
                    Width = temporaryItem.Width,
                    Id = temporaryItem.Id,
                    Price = temporaryItem.Price,
                    TLqPosition = temporaryItem.Height,
                    BRpPosition = temporaryItem.Width,
                    TRpPosition = temporaryItem.Width,
                    TRqPosition = temporaryItem.Height
                };
                if (IsBorderObserved(pricedItem, temporaryBin.Height, temporaryBin.Width))
                {
                    temporaryBin.NestedItems = new List<Item>
                    {
                        pricedItem
                    };
                    HandleOperationsPostNestedItem(temporaryBin, temporaryItem, temporaryBin.Points.ElementAt(0), pricedItem);
                }
                else
                {
                    pricedItem = new Item()
                    {
                        Height = temporaryItem.Width,
                        Width = temporaryItem.Height,
                        Id = temporaryItem.Id,
                        Price = temporaryItem.Price,
                        TLqPosition = temporaryItem.Width,
                        BRpPosition = temporaryItem.Height,
                        TRpPosition = temporaryItem.Height,
                        TRqPosition = temporaryItem.Width
                    };
                    if (IsBorderObserved(pricedItem, temporaryBin.Height, temporaryBin.Width))
                    {
                        temporaryBin.NestedItems = new List<Item>
                         {
                             pricedItem
                         };
                        HandleOperationsPostNestedItem(temporaryBin, temporaryItem, temporaryBin.Points.ElementAt(0), pricedItem);
                    }
                }


                //Console.WriteLine("item " + pricedItem.Id + "(" + pricedItem.Height + ", " + pricedItem.Width + ") nested in "
                //      + temporaryBin.Points.ElementAt(0).PfinalPosition + ", " +
                //  temporaryBin.Points.ElementAt(0).QfinalPosition + ", " + temporaryBin.Points.ElementAt(0).Rposition + " BIN " + temporaryBin.Id);
                return temporaryBin;
            }
            else if (temporaryBin.Points.Count > 1)//se il bin contiene n punti
            {
                foreach (Position feasiblePoint in temporaryBin.Points)
                {
                    if (!feasiblePoint.IsUsed)
                    {
                        Item newPricedItem = null;
                        if (feasiblePoint.Rposition == 0)
                        {
                            //assegno le coordinate di partenza al nuovo item da nestare, poi inzio a muoverlo
                            newPricedItem = new Item()
                            {
                                Height = temporaryItem.Height,
                                Width = temporaryItem.Width,
                                Id = temporaryItem.Id,
                                Price = temporaryItem.Price,
                                BLpPosition = feasiblePoint.Pposition,
                                BLqPosition = feasiblePoint.Qposition,
                                BRpPosition = feasiblePoint.Pposition + temporaryItem.Width,
                                BRqPosition = feasiblePoint.Qposition,
                                TLpPosition = feasiblePoint.Pposition,
                                TLqPosition = feasiblePoint.Qposition + temporaryItem.Height,
                                TRpPosition = feasiblePoint.Pposition + temporaryItem.Width,
                                TRqPosition = feasiblePoint.Qposition + temporaryItem.Height
                            };
                        }
                        else if (feasiblePoint.Rposition == 1)
                        {
                            newPricedItem = new Item()
                            {
                                Height = temporaryItem.Width,
                                Width = temporaryItem.Height,
                                Id = temporaryItem.Id,
                                Price = temporaryItem.Price,
                                BLpPosition = feasiblePoint.Pposition,
                                BLqPosition = feasiblePoint.Qposition,
                                BRpPosition = feasiblePoint.Pposition + temporaryItem.Height,
                                BRqPosition = feasiblePoint.Qposition,
                                TLpPosition = feasiblePoint.Pposition,
                                TLqPosition = feasiblePoint.Qposition + temporaryItem.Width,
                                TRpPosition = feasiblePoint.Pposition + temporaryItem.Height,
                                TRqPosition = feasiblePoint.Qposition + temporaryItem.Width
                            };
                        }
                        feasiblePoint.HatchedArea = GetHatchedArea(temporaryBin, newPricedItem, feasiblePoint);
                    }
                }

                //trovo la tupla con lo scarto minore tra quelle ancora disponibili
                //(Where(x => x.HatchedArea >= 0) filtra solo le tuple con hatched area = 0)
                Position minHatchedAreaTuple = temporaryBin.Points.Where(x => x.IsUsed == false && x.HatchedArea >= 0)
                                                               .OrderBy(x => x.HatchedArea)
                                                               .FirstOrDefault();

                //se non riesco a trovare la tupla, vuol dire che le tuple sono finite 

                if (minHatchedAreaTuple == null)
                {
                    return temporaryBin;
                }

                //controllo se ho più tuple che hanno lo stesso scarto (il minore)
                IList<Position> minHatchedAreaPoints = new List<Position>();
                foreach (Position point in temporaryBin.Points)
                {
                    if (point.HatchedArea == minHatchedAreaTuple.HatchedArea && !point.IsUsed)
                    {
                        minHatchedAreaPoints.Add(point);
                    }
                }

                if (minHatchedAreaPoints.Count == 1)
                {
                    Position minHatchedAreaPoint = minHatchedAreaPoints.ElementAt(0);
                    Item pricedItem = null;
                    if (minHatchedAreaPoint.Rposition == 0)
                    {
                        pricedItem = new Item()
                        {
                            Height = temporaryItem.Height,
                            Width = temporaryItem.Width,
                            Id = temporaryItem.Id,
                            Price = temporaryItem.Price,
                            BLpPosition = minHatchedAreaPoint.PfinalPosition,
                            BLqPosition = minHatchedAreaPoint.QfinalPosition,
                            BRpPosition = minHatchedAreaPoint.PfinalPosition + temporaryItem.Width,
                            BRqPosition = minHatchedAreaPoint.QfinalPosition,
                            TLpPosition = minHatchedAreaPoint.PfinalPosition,
                            TLqPosition = minHatchedAreaPoint.QfinalPosition + temporaryItem.Height,
                            TRpPosition = minHatchedAreaPoint.PfinalPosition + temporaryItem.Width,
                            TRqPosition = minHatchedAreaPoint.QfinalPosition + temporaryItem.Height
                        };
                    }
                    else if (minHatchedAreaPoint.Rposition == 1)
                    {
                        pricedItem = new Item()
                        {
                            Height = temporaryItem.Width,
                            Width = temporaryItem.Height,
                            Id = temporaryItem.Id,
                            Price = temporaryItem.Price,
                            BLpPosition = minHatchedAreaPoint.PfinalPosition,
                            BLqPosition = minHatchedAreaPoint.QfinalPosition,
                            BRpPosition = minHatchedAreaPoint.PfinalPosition + temporaryItem.Height,
                            BRqPosition = minHatchedAreaPoint.QfinalPosition,
                            TLpPosition = minHatchedAreaPoint.PfinalPosition,
                            TLqPosition = minHatchedAreaPoint.QfinalPosition + temporaryItem.Width,
                            TRpPosition = minHatchedAreaPoint.PfinalPosition + temporaryItem.Height,
                            TRqPosition = minHatchedAreaPoint.QfinalPosition + temporaryItem.Width
                        };
                    }
                    //Console.WriteLine("item " + pricedItem.Id + "(" + pricedItem.Height + ", " + pricedItem.Width + ") nested in "
                    //  + minHatchedAreaPoint.PfinalPosition + ", " +
                    //minHatchedAreaPoint.QfinalPosition + ", " + minHatchedAreaPoint.Rposition + " BIN " + temporaryBin.Id);

                    temporaryBin.NestedItems.Add(pricedItem);
                    HandleOperationsPostNestedItem(temporaryBin, temporaryItem, minHatchedAreaPoint, pricedItem);
                    return temporaryBin;
                }
                else if (minHatchedAreaPoints.Count > 1)
                {
                    Position minCoordinatePoint = ApplyRule(minHatchedAreaPoints, itemAllocationMethod);
                    Item pricedItem = null;
                    if (minCoordinatePoint.Rposition == 0)
                    {
                        pricedItem = new Item()
                        {
                            Height = temporaryItem.Height,
                            Width = temporaryItem.Width,
                            Id = temporaryItem.Id,
                            Price = temporaryItem.Price,
                            BLpPosition = minCoordinatePoint.PfinalPosition,
                            BLqPosition = minCoordinatePoint.QfinalPosition,
                            BRpPosition = minCoordinatePoint.PfinalPosition + temporaryItem.Width,
                            BRqPosition = minCoordinatePoint.QfinalPosition,
                            TLpPosition = minCoordinatePoint.PfinalPosition,
                            TLqPosition = minCoordinatePoint.QfinalPosition + temporaryItem.Height,
                            TRpPosition = minCoordinatePoint.PfinalPosition + temporaryItem.Width,
                            TRqPosition = minCoordinatePoint.QfinalPosition + temporaryItem.Height
                        };
                    }
                    else if (minCoordinatePoint.Rposition == 1)
                    {
                        pricedItem = new Item()
                        {
                            Height = temporaryItem.Width,
                            Width = temporaryItem.Height,
                            Id = temporaryItem.Id,
                            Price = temporaryItem.Price,
                            BLpPosition = minCoordinatePoint.PfinalPosition,
                            BLqPosition = minCoordinatePoint.QfinalPosition,
                            BRpPosition = minCoordinatePoint.PfinalPosition + temporaryItem.Height,
                            BRqPosition = minCoordinatePoint.QfinalPosition,
                            TLpPosition = minCoordinatePoint.PfinalPosition,
                            TLqPosition = minCoordinatePoint.QfinalPosition + temporaryItem.Width,
                            TRpPosition = minCoordinatePoint.PfinalPosition + temporaryItem.Height,
                            TRqPosition = minCoordinatePoint.QfinalPosition + temporaryItem.Width
                        };
                    }

                    //Console.WriteLine("item " + pricedItem.Id + "(" + pricedItem.Height + ", " + pricedItem.Width + ") nested in " 
                    //  + minCoordinatePoint.PfinalPosition + ", " +
                    //minCoordinatePoint.QfinalPosition + ", " + minCoordinatePoint.Rposition + " BIN " + temporaryBin.Id);

                    temporaryBin.NestedItems.Add(pricedItem);
                    HandleOperationsPostNestedItem(temporaryBin, temporaryItem, minCoordinatePoint, pricedItem);

                    return temporaryBin;
                }
            }
            return temporaryBin;
        }

        /// <summary>
        /// metodo per gestire le operazioni post item nestato
        /// </summary>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <param name="point"></param>
        private void HandleOperationsPostNestedItem(Bin temporaryBin, Item sortedTemporaryItem, Position point, Item pricedItem)
        {
            //recupero il punto dalla lista usando l'id. tale punto lo setterò poi ad usato
            var matchingPoints = temporaryBin.Points.Where(x => x.Pposition == point.PfinalPosition &&
                                                       x.Qposition == point.QfinalPosition &&
                                                       x.PfinalPosition == point.PfinalPosition &&
                                                       x.QfinalPosition == point.QfinalPosition);
            //.FirstOrDefault();

            //gestisco il fatto che ci possano essere più punti che portano alle stesse coordinate finali
            if (matchingPoints != null)
            {
                foreach (Position matchingPoint in matchingPoints)
                {
                    matchingPoint.IsUsed = true;
                }
            }
            else
            {
                matchingPoints = temporaryBin.Points.Where(x => x.PfinalPosition == point.PfinalPosition &&
                                                       x.QfinalPosition == point.QfinalPosition);
                //.FirstOrDefault();
                foreach (Position matchingPoint in matchingPoints)
                {
                    matchingPoint.IsUsed = true;
                }
            }
            //===============================================

            //controllo se non è più possibile usare dei punti 
            foreach (Position p in temporaryBin.Points)
            {
                foreach (Position matchingPoint in matchingPoints)
                {
                    //cerco punti "coperti" dal nuovo item nestato
                    if ((p.Pposition >= pricedItem.BLpPosition && p.Pposition < pricedItem.BRpPosition &&
                    p.Qposition >= pricedItem.BLqPosition && p.Qposition < pricedItem.TLqPosition) ||

                    (p.PfinalPosition == matchingPoint.PfinalPosition && //cerco punti che, dopo push down and left portavano alle stesse coordinate finali
                    p.QfinalPosition == matchingPoint.QfinalPosition &&
                    pricedItem.BRpPosition >= p.Pposition &&
                    pricedItem.TLqPosition >= p.Qposition))
                    {
                        p.IsUsed = true;
                    }
                }
            }

            //controllo se il primo nuovo punto (TL) da aggiungere è già presente nella lista temporaryBin.Points e non è statao usato
            Position pointFound = temporaryBin.Points.Where(x => x.Pposition == point.PfinalPosition &&
                                                   x.Qposition == point.QfinalPosition + pricedItem.Height &&
                                                   x.IsUsed == false).FirstOrDefault();

            //definisco il primo nuovo punto
            Position firstPoint = new Position()
            {
                Pposition = point.PfinalPosition,
                Qposition = point.QfinalPosition + pricedItem.Height,
                Rposition = 0,
                IsUsed = false

            };

            //controllo se il primo nuovo punto è idoneo ad essere aggiunto perché
            //potrebbe essere erroneaemente creato sul lato di un item già in soluzione
            bool isPointLyingOnItemSide = false;
            foreach (Item ni in temporaryBin.NestedItems)
            {
                if (firstPoint.Qposition == ni.BLqPosition &&
                    firstPoint.Pposition > ni.BLpPosition &&
                    firstPoint.Pposition < ni.BRpPosition)
                {
                    isPointLyingOnItemSide = true;
                    break;
                }
            }

            //aggiungo il primo nuovo punto
            if (pointFound == null && !isPointLyingOnItemSide)
            {
                temporaryBin.Points.Add(firstPoint);

                firstPoint = new Position()
                {
                    Pposition = point.PfinalPosition,
                    Qposition = point.QfinalPosition + pricedItem.Height,
                    Rposition = 1,
                    IsUsed = false

                };

                temporaryBin.Points.Add(firstPoint);
            }
            else
            {
                pointFound = null;
            }

            isPointLyingOnItemSide = false;

            //controllo se il secondo nuovo punto (BR) da aggiungere è già presente nella lista temporaryBin.Points e non è statao usato
            pointFound = temporaryBin.Points.Where(x => x.Pposition == point.PfinalPosition + pricedItem.Width &&
                                                        x.Qposition == point.QfinalPosition &&
                                                        x.IsUsed == false).FirstOrDefault();

            //definisco il secondo nuovo punto
            Position secondPoint = new Position()
            {
                Pposition = point.PfinalPosition + pricedItem.Width,
                Qposition = point.QfinalPosition,
                Rposition = 0,
                IsUsed = false
            };

            //controllo se il secondo nuovo punto è idoneo ad essere aggiunto perché
            //potrebbe essere erroneaemente creato sul lato di un item già esistente
            foreach (Item ni in temporaryBin.NestedItems)
            {
                if (secondPoint.Pposition == ni.BLpPosition &&
                    secondPoint.Qposition > ni.BLqPosition &&
                    secondPoint.Qposition < ni.TLqPosition)
                {
                    isPointLyingOnItemSide = true;
                    break;
                }
            }

            //aggiungo il secondo nuovo punto
            if (pointFound == null && !isPointLyingOnItemSide)
            {
                temporaryBin.Points.Add(secondPoint);

                secondPoint = new Position()
                {
                    Pposition = point.PfinalPosition + pricedItem.Width,
                    Qposition = point.QfinalPosition,
                    Rposition = 1,
                    IsUsed = false
                };

                temporaryBin.Points.Add(secondPoint);
            }

            //setto item a nestato
            sortedTemporaryItem.IsRemoved = true;
        }

        /// <summary>
        /// metodo che setta come usate le tuple generate che 
        /// sono uguali alle dimensioni del bin o le eccedono
        /// </summary>
        /// <param name="temporaryBin"></param>
        /// <param name="temporaryItem"></param>
        /// <returns></returns>
        private void SetFeasiblePoints(Bin temporaryBin)
        {
            foreach (Position point in temporaryBin.Points)
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
        private double GetHatchedArea(Bin temporaryBin, Item newItem, Position feasiblePoint)
        {
            PushItemDown(temporaryBin, newItem, feasiblePoint);
            IList<Item> leftIntersectedItems = PushItemLeft(temporaryBin, newItem, feasiblePoint);
            //conto il numero di intersezioni che ho da sotto, perché potrebbero cambiare (da quelle inzialemnte trovate con
            //la prima invocazione di PushItemDown) dopo aver spinto l'item tutto a sinistra 
            IList<Item> downIntersectedItems = CheckDownInsersectionNumber(temporaryBin, newItem, feasiblePoint);

            //controllo se l'oggetto, anche essendo stato spostato in basso a sintra, sborderebbe e
            //se la posizione in cui deve essere nestato il nuovo item comporterebbe delle sovrapposizioni con item già in soluzione
            if (IsBorderObserved(newItem, temporaryBin.Height, temporaryBin.Width) && IsOverlappingOk(newItem, temporaryBin))
            {
                ComputeHatchedArea(feasiblePoint, newItem, downIntersectedItems, leftIntersectedItems);
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
        private void ComputeHatchedArea(Position feasiblePoint, Item newItem, IList<Item> downIntersectedItems, IList<Item> leftIntersectedItems)
        {
            double totalHatchedArea = 0;
            //variabile per l'hatched area che eventualmente rimane sotto e a sinitra 
            double partialHatchedArea;

            if (downIntersectedItems.Count > 0)
            {
                //definiso la green area sotto il nuovo item
                AdjacentItem adjacentItem = new AdjacentItem()
                {
                    BRpPosition = feasiblePoint.PfinalPosition + newItem.Width,
                    BRqPosition = 0,
                    TRpPosition = feasiblePoint.PfinalPosition + newItem.Width,
                    TRqPosition = feasiblePoint.QfinalPosition,
                    BLpPosition = feasiblePoint.PfinalPosition,
                    BLqPosition = 0,
                    TLpPosition = feasiblePoint.PfinalPosition,
                    TLqPosition = feasiblePoint.QfinalPosition,
                    Height = newItem.BRqPosition,
                    Width = newItem.Width,
                    Area = newItem.BRqPosition * newItem.Width
                };

                //per ogni item già in posizione, che è stato intersecato, 
                //calcolo quanta parte di area di esso rientra nella green area e 
                //infine sommo tale area all'area totale degli item intersecati
                double itemsInSolutionArea = 0;
                double intersectedWidth;

                foreach (Item intersectedItem in downIntersectedItems)
                {
                    //if else per scegliere la width dell'item in soluzione 
                    //di cui devo calcolare l'area 
                    //per fare valore assoluto Math.Abs( ... );
                    if (newItem.BLpPosition < intersectedItem.BLpPosition && //new item + a sx di interesect item
                       newItem.BRpPosition < intersectedItem.BRpPosition)
                    {
                        intersectedWidth = newItem.BRpPosition - intersectedItem.BLpPosition;
                    }
                    else if (intersectedItem.BLpPosition < newItem.BLpPosition && //new item è + a  dx di intersected item
                       intersectedItem.BRpPosition < newItem.BRpPosition)
                    {
                        intersectedWidth = intersectedItem.BRpPosition - newItem.BLpPosition;
                    }
                    else if ((newItem.BLpPosition == intersectedItem.BLpPosition && //new item inizia come insertected item ma termina prima
                    newItem.BRpPosition < intersectedItem.BRpPosition) ||
                    (intersectedItem.BLpPosition < newItem.BLpPosition && // new item inizia dopo di insertected item ma terminano uguali
                    intersectedItem.BRpPosition == newItem.BRpPosition) ||
                    (intersectedItem.BLpPosition < newItem.BLpPosition &&  //le coordinate p del new item cadono dentro quelle p dell'intersected item
                    intersectedItem.BRpPosition > newItem.BRpPosition))
                    {
                        intersectedWidth = adjacentItem.Width;
                    }
                    else
                    {
                        intersectedWidth = intersectedItem.Width;
                    }

                    itemsInSolutionArea += (intersectedItem.Height * intersectedWidth);
                }

                partialHatchedArea = adjacentItem.Area - itemsInSolutionArea;
                totalHatchedArea += partialHatchedArea;
            }

            if (leftIntersectedItems.Count > 0)
            {
                //definiso la green area a sintra del nuovo item
                AdjacentItem adjacentItem = new AdjacentItem()
                {
                    BRpPosition = feasiblePoint.PfinalPosition,
                    BRqPosition = feasiblePoint.QfinalPosition,
                    TRpPosition = feasiblePoint.PfinalPosition,
                    TRqPosition = feasiblePoint.QfinalPosition + newItem.Height,
                    BLpPosition = 0,
                    BLqPosition = feasiblePoint.QfinalPosition,
                    TLpPosition = 0,
                    TLqPosition = feasiblePoint.QfinalPosition + newItem.Height,
                    Height = newItem.Height,
                    Width = newItem.BLpPosition,
                    Area = newItem.Height * newItem.BLpPosition
                };

                //per ogni item già in posizione, che è stato intersecato, 
                //calcolo quanta parte di area di esso rientra nella green area e 
                //infine sommo tale area all'area totale degli item intersecati
                double itemsInSolutionArea = 0;
                double intersectedHeight;
                foreach (Item intersectedItem in leftIntersectedItems)
                {
                    //if else per scegliere la height dell'item in soluzione 
                    //di cui devo calcolare l'area 
                    //per fare valore assoluto Math.Abs( ... );
                    if (intersectedItem.BLqPosition < newItem.BLqPosition && //new item + in alto di intersected item
                       intersectedItem.TLqPosition < newItem.TLqPosition)
                    {
                        intersectedHeight = intersectedItem.TLqPosition - newItem.BLqPosition;
                    }
                    else if (newItem.BLqPosition < intersectedItem.BLqPosition && //new item + in basso di interesect item
                      newItem.TLqPosition < intersectedItem.TLqPosition)
                    {
                        intersectedHeight = newItem.TLqPosition - intersectedItem.BLqPosition;
                    }
                    else if ((newItem.BLqPosition == intersectedItem.BLqPosition && //new item inizia come insertected item ma termina prima
                       newItem.TLqPosition < intersectedItem.TLqPosition) ||
                       (intersectedItem.BLqPosition < newItem.BLqPosition && // new item inizia dopo di insertected item ma terminano uguali
                       intersectedItem.TLqPosition == newItem.TLqPosition) ||
                       (intersectedItem.BLqPosition < newItem.BLqPosition && //le coordinate q del new item cadono dentro quelle dell'intersected item 
                       intersectedItem.TLqPosition > newItem.TLqPosition))
                    {
                        intersectedHeight = adjacentItem.Height;
                    }
                    else
                    {
                        intersectedHeight = intersectedItem.Height;
                    }

                    itemsInSolutionArea += (intersectedHeight * intersectedItem.Width);
                }

                partialHatchedArea = adjacentItem.Area - itemsInSolutionArea;
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
        private IList<Item> PushItemDown(Bin temporaryBin, Item newItem, Position feasiblePoint)
        {
            //lista delle intersezioni tra item nuovo e item già in soluzione
            IList<Item> intersectedItems = new List<Item>();

            foreach (Item nestedItem in temporaryBin.NestedItems)
            {
                //cerco intersezioni verticali tra nuovo item e item già in soluzione (HO TOLTO UGUALE DESTRA -> CHECK)
                if (((newItem.BLpPosition >= nestedItem.BLpPosition && newItem.BLpPosition < nestedItem.BRpPosition) ||
                   (newItem.BRpPosition > nestedItem.BLpPosition && newItem.BRpPosition <= nestedItem.BRpPosition) ||
                   (newItem.BLpPosition <= nestedItem.TLpPosition && newItem.BRpPosition >= nestedItem.TRpPosition) || //le p di OI cadono dentro le p di NI o le p di OI sono uguali alle p di NI
                   (newItem.BLpPosition > nestedItem.TLpPosition && newItem.BRpPosition < nestedItem.TRpPosition) //le p di NI cadono dentro le p di OI
                   ) &&
                    newItem.BLqPosition >= nestedItem.TLqPosition)
                {
                    intersectedItems.Add(nestedItem);
                }
            }

            //3 possibili risultati
            if (intersectedItems.Count == 0) //non ho intersezioni
            {
                newItem.BLqPosition = 0;
                newItem.TLqPosition = newItem.Height;
                newItem.BRqPosition = 0;
                newItem.TRqPosition = newItem.Height;
            }
            else
            {
                if (intersectedItems.Count == 1) //1 sola intersezione
                {
                    double delta = feasiblePoint.Qposition - intersectedItems.ElementAt(0).Height;
                    newItem.BLqPosition -= delta;
                    newItem.TLqPosition -= delta;
                    newItem.BRqPosition -= delta;
                    newItem.TRqPosition -= delta;
                }
                else if (intersectedItems.Count > 1) //N intersezioni
                {
                    double heightSum = intersectedItems.OrderBy(x => x.TLqPosition).Last().TLqPosition;
                    double delta = feasiblePoint.Qposition - heightSum;
                    newItem.BLqPosition -= delta;
                    newItem.TLqPosition -= delta;
                    newItem.BRqPosition -= delta;
                    newItem.TRqPosition -= delta;
                }
            }

            feasiblePoint.QfinalPosition = newItem.BLqPosition;
            return intersectedItems;

        }

        private IList<Item> CheckDownInsersectionNumber(Bin temporaryBin, Item newItem, Position feasiblePoint)
        {
            //lista delle intersezioni tra item nuovo e item già in soluzione
            IList<Item> intersectedItems = new List<Item>();

            foreach (Item nestedItem in temporaryBin.NestedItems)
            {
                //cerco intersezioni verticali tra nuovo item e item già in soluzione (HO TOLTO UGUALE DESTRA -> CHECK)
                if (((newItem.BLpPosition >= nestedItem.BLpPosition && newItem.BLpPosition < nestedItem.BRpPosition) ||
                   (newItem.BRpPosition > nestedItem.BLpPosition && newItem.BRpPosition <= nestedItem.BRpPosition) ||
                   (newItem.BLpPosition <= nestedItem.TLpPosition && newItem.BRpPosition >= nestedItem.TRpPosition) || //le p di OI cadono dentro le p di NI o le p di OI sono uguali alle p di NI
                   (newItem.BLpPosition > nestedItem.TLpPosition && newItem.BRpPosition < nestedItem.TRpPosition) //le p di NI cadono dentro le p di OI
                   ) &&
                    newItem.BLqPosition >= nestedItem.TLqPosition)
                {
                    intersectedItems.Add(nestedItem);
                }
            }

            return intersectedItems;
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
        private IList<Item> PushItemLeft(Bin temporaryBin, Item newItem, Position feasiblePoint)
        {
            //lista delle intersezioni tra item nuovo e item già in soluzione
            IList<Item> intersectedItems = new List<Item>();

            foreach (Item nestedItem in temporaryBin.NestedItems)
            {
                //cerco interesezioni orizzontali tra nuovo item e item già in soluzione (HO TOLTO UGUALE DESTRA -> CHECK)
                if (((newItem.BLqPosition >= nestedItem.BLqPosition && newItem.BLqPosition < nestedItem.TLqPosition) ||
                    (newItem.TLqPosition > nestedItem.BLqPosition && newItem.TLqPosition <= nestedItem.TLqPosition) ||
                    (newItem.BLqPosition <= nestedItem.BRqPosition && newItem.TLqPosition >= nestedItem.TRqPosition) || //le coord q di OI cadono entrambe dentro a NI o sono uguali
                    (newItem.BLqPosition > nestedItem.BRqPosition && newItem.TLqPosition < nestedItem.TRqPosition) //le coord q di NI cadono entrambe dentro OI
                    ) &&
                    newItem.BLpPosition >= nestedItem.BRpPosition)
                {
                    intersectedItems.Add(nestedItem);
                }
            }

            //3 possibili risultati
            if (intersectedItems.Count == 0) //non ho intersezioni
            {
                newItem.BLpPosition = 0;
                newItem.TLpPosition = 0;
                newItem.BRpPosition = newItem.Width;
                newItem.TRpPosition = newItem.Width;
            }
            else
            {
                if (intersectedItems.Count == 1) //1 sola intersezione
                {
                    double delta = feasiblePoint.Pposition - intersectedItems.ElementAt(0).Width;
                    newItem.BLpPosition -= delta;
                    newItem.TLpPosition -= delta;
                    newItem.BRpPosition -= delta;
                    newItem.TRpPosition -= delta;
                }
                else if (intersectedItems.Count > 1) //N intersezioni
                {
                    double widthSum = intersectedItems.OrderBy(x => x.BRpPosition).Last().BRpPosition;
                    double delta = feasiblePoint.Pposition - widthSum;
                    newItem.BLpPosition -= delta;
                    newItem.TLpPosition -= delta;
                    newItem.BRpPosition -= delta;
                    newItem.TRpPosition -= delta;
                }
            }

            feasiblePoint.PfinalPosition = newItem.BLpPosition;
            return intersectedItems;

        }

        /// <summary>
        /// metodo che controlla se, a fronte del posizione del nuovo item da nestare, 
        /// le dimensioni di tale item sforano rispetto alle dimensioni del bin
        /// </summary>
        /// <param name="newNestedItem"></param>
        /// <param name="temporaryBinHeight"></param>
        /// <param name="temporaryBinWidth"></param>
        /// <returns></returns>
        bool IsBorderObserved(Item newItem, double temporaryBinHeight, double temporaryBinWidth)
        {
            return newItem.TLqPosition <= temporaryBinHeight && newItem.BRpPosition <= temporaryBinWidth;
        }

        /// <summary>
        /// metodo per controllare se nestare un nuovo item in un certo punto
        /// comporta delle sovrapposizioni tra nuovo item e item già in soluzione
        /// </summary>
        /// <param name="newItem"></param>
        /// <param name="temporaryBin"></param>
        /// <returns></returns>
        bool IsOverlappingOk(Item newItem, Bin temporaryBin)
        {
            foreach (Item nestedItem in temporaryBin.NestedItems)
            {
                //se c'è overlap
                if (nestedItem.BLpPosition < newItem.BRpPosition && nestedItem.BRpPosition > newItem.BLpPosition
                    && nestedItem.BLqPosition < newItem.TLqPosition && nestedItem.TLqPosition > newItem.BLqPosition)
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
        private Position ApplyRule(IList<Position> minHatchedAreaTuples, string itemAllocationMethod)
        {
            Position result = null;

            if (itemAllocationMethod == "AC1")
            {
                IList<Position> pMinTuples = new List<Position>();
                double minFinalP = minHatchedAreaTuples.OrderBy(x => x.PfinalPosition).First().PfinalPosition;

                foreach (Position minHatchedAreaTuple in minHatchedAreaTuples)
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
                    IList<Position> qMinTuples = new List<Position>();
                    double minFinalQ = pMinTuples.OrderBy(x => x.QfinalPosition).First().QfinalPosition;
                    foreach (Position qMinTuple in pMinTuples)
                    {
                        if (qMinTuple.QfinalPosition == minFinalQ)
                        {
                            qMinTuples.Add(qMinTuple);
                        }
                    }
                    if (qMinTuples.Count == 1)
                    {
                        result = qMinTuples.ElementAt(0);
                    }
                    else
                    {
                        result = qMinTuples.OrderBy(x => x.Rposition).First();
                    }
                }
            }
            else if (itemAllocationMethod == "AC2")
            {

                IList<Position> qMinTuples = new List<Position>();
                double minFinalQ = minHatchedAreaTuples.OrderBy(x => x.QfinalPosition).First().QfinalPosition;

                foreach (Position minHatchedAreaTuple in minHatchedAreaTuples)
                {
                    if (minHatchedAreaTuple.QfinalPosition == minFinalQ)
                    {
                        qMinTuples.Add(minHatchedAreaTuple);
                    }
                }

                if (qMinTuples.Count == 1)
                {
                    result = qMinTuples.ElementAt(0);
                }
                else
                {
                    IList<Position> pMinTuples = new List<Position>();
                    double minFinalP = qMinTuples.OrderBy(x => x.PfinalPosition).First().PfinalPosition;
                    foreach (Position pMinTuple in qMinTuples)//_____________
                    {
                        if (pMinTuple.PfinalPosition == minFinalP)
                        {
                            pMinTuples.Add(pMinTuple);
                        }
                    }
                    if (pMinTuples.Count == 1)
                    {
                        result = pMinTuples.ElementAt(0);
                    }
                    else
                    {
                        result = pMinTuples.OrderBy(x => x.Rposition).First();
                    }
                }
            }
            else
            {
                Console.WriteLine("L'item allocation method non è stato settato");
            }
            return result;

        }

        public bool IsSolutionCorrect(IList<Item> items, IList<Bin> bins, int iter)
        {
            int nestedItemSum = 0;
            foreach (Bin bin in bins)
            {
                if (bin.NestedItems != null)
                {
                    nestedItemSum += bin.NestedItems.Count;
                }
            }

            if (nestedItemSum != items.Count)
            {
                Console.WriteLine("==================================== ERROR =======================================");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

