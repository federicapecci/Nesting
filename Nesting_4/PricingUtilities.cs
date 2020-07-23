﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_4
{
    class PricingUtilities: IPricingUtilities
    {
        public double ComputePricingRule(string pricingRule, double height, double width)
        {
            switch (pricingRule)
            {
                case "IP1":
                    return height * width;
                case "IP2":
                    return width;
                case "IP3":
                    return height;
                case "IP4":
                    return 2 * height + 2 * width;
                default:
                    Console.WriteLine("La princing rule non è stata settata");
                    break;
            }
            Console.WriteLine("La princing rule non è stata settata");
            return -1;

        }

        /// <summary>
        /// questo metodo aggiorna il prezzo di ogni item 
        /// </summary>
        /// <param name="z"></param>
        /// <param name="items"></param>
        /// <param name="temporaryBin"></param>
        public void ComputePricingUpdateRule(double z, IList<Item> items, IList<Bin> bins, string priceUpdatingRule, string partitionType)
        {
            if (priceUpdatingRule == "PU1")
            {
                double alpha = 0.9;
                double beta = 1.1;

                UpdateItemPrice(z, items, bins, alpha, beta, partitionType);
            }
            else if (priceUpdatingRule == "PU2")
            {
                double rnd = GetRandomDouble(0, 1);
                double alpha = 1 - rnd;
                double beta = 1 + rnd;

                UpdateItemPrice(z, items, bins, alpha, beta, partitionType);
            }
            else if (priceUpdatingRule == "PU3")
            {
                double rnd = GetRandomDouble(0, 1);
                double alpha = 1 - rnd;
                rnd = GetRandomDouble(0, 1); 
                double beta = 1 + rnd;

                UpdateItemPrice(z, items, bins, alpha, beta, partitionType);
            }
            else if(priceUpdatingRule == "PU001" || priceUpdatingRule == "PU002" || priceUpdatingRule == "PU005" ||
                priceUpdatingRule == "PU02" || priceUpdatingRule == "PU05")
            {
               
                 double x;
                 switch (priceUpdatingRule)
                 {
                    case "PU001":
                        x = 0.01;
                        break;
                    case "PU002":
                        x = 0.02;
                        break;
                    case "PU005":
                        x = 0.05;
                        break;
                    case "PU02":
                        x = 0.2;
                        break;
                    case "PU05":
                        x = 0.5;
                        break;
                    default:
                        Console.WriteLine("Valore x non settato");
                        x = -1;
                        break;
                 }

                 double alpha = 1 - x;
                 double beta = 1 + x;

                 UpdateItemPrice(z, items, bins, alpha, beta, partitionType); 
            }
            else if (priceUpdatingRule == "PU001R" || priceUpdatingRule == "PU002R" || priceUpdatingRule == "PU005R" ||
                 priceUpdatingRule == "PU02R" || priceUpdatingRule == "PU05R")
            {
                 double rnd;
                 switch (priceUpdatingRule)
                 {
                    case "PU001R":
                        rnd = GetRandomDouble(0, 0.01); 
                        break;
                    case "PU002R":
                        rnd = GetRandomDouble(0, 0.02); 
                        break;
                    case "PU005R":
                        rnd = GetRandomDouble(0, 0.05);
                        break;
                    case "PU02R":
                        rnd = GetRandomDouble(0, 0.2); 
                        break;
                    case "PU05R":
                        rnd = GetRandomDouble(0, 0.5);
                        break;
                    default:
                        Console.WriteLine("Valore x non settato");
                        rnd = -1;
                        break;
                 }

                 double alpha = 1 - rnd;
                 double beta = 1 + rnd;

                 UpdateItemPrice(z, items, bins, alpha, beta, partitionType);
            }
            else
            {
                 Console.WriteLine("La princing updating rule non è stata settata");
            }
        }

        private void UpdateItemPrice(double z, IList<Item> items, IList<Bin> bins, double alpha, double beta, string partitionType)
        {
            if (partitionType == "REGPART")
            {
                bool itemFound = false;
                //dato un certo item
                foreach (Item item in items)
                {

                    foreach (Bin bin in bins) //scorro tutti i bins
                    {
                        if (bin.NestedItems != null)
                        {
                            foreach (Item nestedItem in bin.NestedItems) //e scorro tutti i nested items di ogni bin
                            {
                                if (nestedItem.Id == item.Id) //se trovo l'id di un nested item che corrisponde all'id dell'item dato
                                {
                                    //aggiorno il prezzo dell'item dato in base a se il nested item con id corrispondente si trova nella prima o nella seconda metà dei bin
                                    if (bin.Id <= (0.5 * z))
                                    {
                                        item.Price = alpha * nestedItem.Price;
                                    }
                                    else if (bin.Id > (0.5 * z))
                                    {
                                        item.Price = beta * nestedItem.Price;
                                    }
                                    itemFound = true;
                                    break;
                                }
                            }
                        }
                        if (itemFound)
                        {
                            itemFound = false;
                            break;
                        }
                    }
                }
            }else if (partitionType == "EXTRAPART")
            {
                //dato un certo item
                foreach (Item item in items)
                {
                    if (item.IsRemoved == false)
                    {
                        item.Price = beta * item.Price;
                    }
                    else
                    {
                        item.Price = alpha * item.Price;
                    }
                }
            }
            else
            {
                Console.WriteLine("Il partition type non è stata settato");
            }
        }

        private double GetRandomDouble(double minimum, double maximum)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

    }
}
