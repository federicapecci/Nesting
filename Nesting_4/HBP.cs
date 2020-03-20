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
            int upperBound = int.MaxValue;
            IList<string> itemAllocationMethods = new List<string>
            {
                "AC1",
                //"AC2"
            };
            IList<string> pricingRules = new List<string>
            {
                "IP1",
                //"IP2",
                //"IP3",
                //"IP4"
            };
            IList<string> priceUpdatingRules = new List<string>
            {
                "PU1",
                "PU2"
            };

            foreach (var itemAllocationMethod in itemAllocationMethods)
            {
                foreach (var pricingRule in pricingRules)
                {
                    foreach (var priceUpdatingRule in priceUpdatingRules)
                    {
                        //ogni volta mi arrivano da hsolve due sequenze, la prima e l'ultima
                        IList<Sequence> sequences = HSolve.ComputeHeuristic(Configuration, itemAllocationMethod,
                            pricingRule, priceUpdatingRule);

                        foreach(var sequence in sequences)
                        {
                            Sequences.Add(sequence);
                        }

                        if (Sequences[Sequences.Count - 1].Zstar < upperBound)
                        {
                            upperBound = Sequences[Sequences.Count - 1].Zstar;
                        }
                    }
                }
            }
            Console.WriteLine(upperBound + 1);
        }
    }              
}
