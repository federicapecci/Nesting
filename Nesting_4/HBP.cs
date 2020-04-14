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

        public void ComputeAlgorithm()
        {
            //int upperBound = int.MaxValue;
            double widthCovered = double.MaxValue;

            IList<string> itemAllocationMethods = new List<string>
            {
                "AC1",
                "AC2"
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
                "PU3",

                "PU001",
                "PU002",
                "PU005",
                "PU02",
                "PU05",

                "PU001R",
                "PU002R",
                "PU005R",
                "PU02R",
                "PU05R"
            };



            foreach (var itemAllocationMethod in itemAllocationMethods)
            {
                foreach (var pricingRule in pricingRules)
                {
                    foreach (var priceUpdatingRule in priceUpdatingRules)
                    {
                        //ogni volta mi arrivano da hsolve due sequenze, la prima e l'ultima
                        Sequence sequence = HSolve.ComputeHeuristic(Configuration, itemAllocationMethod,
                            pricingRule, priceUpdatingRule);
                        
                        //controllo se la lunghezza coperta dall'ultimo bin della soluzione è 
                        //minore rispetto alle sequenze precedentemente trovate 
                        if (sequence.WidthCovered < widthCovered)
                        {
                            widthCovered = sequence.WidthCovered;
                            Console.WriteLine("widthCovered " + widthCovered + ", " + itemAllocationMethod + " " +
                                pricingRule + " " + priceUpdatingRule);
                            Sequences.Clear();
                            Sequences.Add(sequence);
                        }

                        /*if (Sequences[Sequences.Count - 1].Zstar < upperBound)
                        {
                            upperBound = Sequences[Sequences.Count - 1].Zstar;
                        }*/
                    }
                }
            }
            Console.WriteLine("\n");
            Console.WriteLine("numero bin " + Sequences[0].Bins.Count);
            Console.WriteLine("lunghezza coperta ultimo bin "+ Sequences[0].WidthCovered);
            Console.WriteLine("area usata ultimo bin - valore assoluto " + Sequences[0].UsedAreaAbsoluteValue);
            Console.WriteLine("area usata ultimo bin - percentuale " + Sequences[0].UsedAreaPercentageValue + "%");
            //Console.WriteLine(upperBound + 1);


        }
    }              
}
