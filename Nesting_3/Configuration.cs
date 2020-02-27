using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_3
{
    /// <summary>
    /// questa classe rappresenta la configurazione dell'algoritmo hsolve
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// altezza del bin in cui nestare gli item
        /// </summary>
        public int BinHeight { get; set; } = 0;

        /// <summary>
        /// lunghezza del bin in cui nestare gli item
        /// </summary>
        public int BinWidth { get; set; } = 0;

        /// <summary>
        /// lista di item da nestare nel bin
        /// </summary>
        public List<Dimension> Dimensions { get; set; } = null;

        /// <summary>
        /// numero massimo di iterazioni per hsolve
        /// </summary>
        public int MaxIter { get; set; } = 0;

        /// <summary>
        /// variazione concessa per limite inferiore e superiore del lower bound
        /// </summary>
        public float LowerBoundDelta { get; set; } = 0;

    }
}
