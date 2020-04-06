using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_4
{
    public class PricingUtilities: IPricingUtilities
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
        public void ComputePricingUpdateRule(double z, IList<Item> items, IList<Bin<Tuple>> bins, string priceUpdatingRule)
        {
            if (priceUpdatingRule == "PU1")
            {
                double alpha = 0.9;
                double beta = 1.1;

                UpdateItemPrice(z, items, bins, alpha, beta);
            }
            else if (priceUpdatingRule == "PU2")
            {
                
                double rnd = GetRandomDouble(0.001, 0.999 + double.Epsilon);
                double alpha = 1 - rnd;
                double beta = 1 + rnd;

                UpdateItemPrice(z, items, bins, alpha, beta);
            }
            else if (priceUpdatingRule == "PU3")
            {

                double rnd = GetRandomDouble(0.001, 0.999 + double.Epsilon);
                double alpha = 1 - rnd;
                rnd = GetRandomDouble(0.001, 0.999 + double.Epsilon);
                double beta = 1 + rnd;

                UpdateItemPrice(z, items, bins, alpha, beta);
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

                 UpdateItemPrice(z, items, bins, alpha, beta); 
            }
            else if (priceUpdatingRule == "PU001R" || priceUpdatingRule == "PU002R" || priceUpdatingRule == "PU005R" ||
                 priceUpdatingRule == "PU02R" || priceUpdatingRule == "PU05R")
            {
                 double rnd;
                 switch (priceUpdatingRule)
                 {
                    case "PU001R":
                        rnd = GetRandomDouble(0, 0.01 + double.Epsilon);
                        break;
                    case "PU002R":
                        rnd = GetRandomDouble(0, 0.02 + double.Epsilon);
                        break;
                    case "PU005R":
                        rnd = GetRandomDouble(0, 0.05 + double.Epsilon);
                        break;
                    case "PU02R":
                        rnd = GetRandomDouble(0, 0.2 + double.Epsilon);
                        break;
                    case "PU05R":
                        rnd = GetRandomDouble(0, 0.5 + double.Epsilon);
                        break;
                    default:
                        Console.WriteLine("Valore x non settato");
                        rnd = -1;
                        break;
                 }

                 double alpha = 1 - rnd;
                 double beta = 1 + rnd;

                 UpdateItemPrice(z, items, bins, alpha, beta);
            }
            else
            {
                 Console.WriteLine("La princing updating rule non è stata settata");
            }
        }

        private void UpdateItemPrice(double z, IList<Item> items, IList<Bin<Tuple>> bins, double alpha, double beta)
        {
            bool itemFound = false;
            //dato un certo item
            foreach (var item in items)
            {
                
                foreach (var bin in bins) //scorro tutti i bins
                {
                    if (bin.NestedItems != null)
                    {
                        foreach (var nestedItem in bin.NestedItems) //e scorro tutti i nested items di ogni bin
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
                    if(itemFound)
                    {
                        itemFound = false;
                        break;
                    }
                }
            }
        }

        private double GetRandomDouble(double minimum, double maximum)
        {
            Random rand = new Random();
            return rand.NextDouble() * (maximum - minimum) + minimum;
        }

    }
}
