using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_4
{
    class HBP
    {
        // <summary>
        /// la configurazione contenwnte i parametri con 
        /// cui lanciare l'algortimo hsolve
        /// </summary>
        public Configuration Configuration { get; set; } = null;

        /// <summary>
        /// ogni container appartente a containers contiene
        /// gli n bin di una certa iterazione
        /// </summary>
        public IList<Sequence> Sequences { get; set; } = null;

        public IHSolve HSolve { get; set; } = null;

        /// <summary>
        /// costruttore in cui setto la configuration di hsolve
        /// </summary>
        /// <param name="configuration"></param>
        public HBP(Configuration configuration)
        {
            Configuration = configuration;
            Sequences = new List<Sequence>();
            HSolve = new HSolve();
        }

        //AC2, PU3, PU05, PU002R, PU02R.

        public void ComputeAlgorithm()
        {
            double widthCovered = double.MaxValue;
            double areaCovered = double.MaxValue;

            IList<string> itemAllocationMethods = new List<string>
            {
                "AC1",
                //"AC2"
            };
            IList<string> pricingRules = new List<string>
            {
                "IP1",
                "IP2",
                "IP3",
                "IP4"
            };
            IList<string> priceUpdatingRules = new List<string>
            {
                "PU1",
                "PU2",
                //"PU3",

                "PU001", //
                "PU002", //
                "PU005", //
                "PU02", //
                //"PU05",

                "PU001R", //
                //"PU002R",
                "PU005R",//
                //"PU02R",
                "PU05R"//
            };

            Sequences.Add(new Sequence());
            Sequences.Add(new Sequence());

            int counter = 0;
            foreach (string itemAllocationMethod in itemAllocationMethods)
            {
                foreach (string pricingRule in pricingRules)
                {
                    foreach (string priceUpdatingRule in priceUpdatingRules)
                    {
                        //ogni volta mi arrivano da hsolve due sequenze, la prima e l'ultima
                        IList<Sequence> sequences = HSolve.ComputeHeuristic(Configuration, itemAllocationMethod,
                            pricingRule, priceUpdatingRule);

                        //controllo se la lunghezza coperta dall'ultimo bin della soluzione è 
                        //minore rispetto alla lunghezza trovata nelle sequenze precedentementi 
                        if (sequences[0].WidthCovered < widthCovered)
                        {
                            widthCovered = sequences[0].WidthCovered;
                            Console.WriteLine("LUNGHEZZA UPDATE -> widthCovered " + widthCovered + ", " + itemAllocationMethod + " " +
                                pricingRule + " " + priceUpdatingRule);
                            Sequences.RemoveAt(0);
                            Sequences.Insert(0,sequences[0]);

                        }

                        //controllo se l'area coperta dall'ultimo bin della soluzione è 
                        //minore rispetto all'area trovata nelle sequenze precedentementi 
                        if (sequences[1].AreaCovered < areaCovered)
                        {
                            areaCovered = sequences[1].AreaCovered;
                            Console.WriteLine("AREA UPDATE -> areaCovered " + areaCovered + ", " + itemAllocationMethod + " " +
                                pricingRule + " " + priceUpdatingRule);
                            Sequences.RemoveAt(1);
                            Sequences.Insert(1, sequences[1]);

                        }
                        Console.WriteLine("check " + counter);
                        counter += 1;
                    }
                }
            }

            Console.WriteLine("\n SEQUENZA W// MIN LUNGHEZZA");
            Console.WriteLine("numero bin " + Sequences[0].Bins.Count);
            Console.WriteLine("lunghezza coperta ultimo bin "+ Sequences[0].WidthCovered);
            Console.WriteLine("area usata ultimo bin - valore assoluto " + Sequences[0].UsedAreaAbsoluteValue);
            Console.WriteLine("area usata ultimo bin - percentuale " + Sequences[0].UsedAreaPercentageValue + "%");

            Console.WriteLine("\n SEQUENZA W// MIN AREA");
            Console.WriteLine("numero bin " + Sequences[1].Bins.Count);
            Console.WriteLine("Area coperta ultimo bin " + Sequences[1].AreaCovered);
            Console.WriteLine("lunghezza coperta ultimo bin " + Sequences[1].WidthCovered);
            Console.WriteLine("area usata ultimo bin - valore assoluto " + Sequences[1].UsedAreaAbsoluteValue);
            Console.WriteLine("area usata ultimo bin - percentuale " + Sequences[1].UsedAreaPercentageValue + "%");

        }
    }              
}
