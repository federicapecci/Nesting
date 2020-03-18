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
                                    break;
                                }
                            }
                        }
                    }
                }
            }else if (priceUpdatingRule == "PU2")
            {
                double alpha;
                double beta;
                double rnd;

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
                                    rnd = GetRandomDouble(0.001, 0.999 + double.Epsilon);
                                    //aggiorno il prezzo dell'item dato in base a se il nested item con id corrispondente si trova nella prima o nella seconda metà dei bin
                                    if (bin.Id <= (0.5 * z))
                                    {
                                        alpha = 1 - rnd;
                                        item.Price = alpha * nestedItem.Price;
                                        
                                    }
                                    else if (bin.Id > (0.5 * z))
                                    {
                                        beta = 1 + rnd;
                                        item.Price = beta * nestedItem.Price;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("La princing updating rule non è stata settata");
            }
        }

        private double GetRandomDouble(double minimum, double maximum)
        {
            Random rand = new Random();
            return rand.NextDouble() * (maximum - minimum) + minimum;
        }

    }
}
