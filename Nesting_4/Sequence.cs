using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_4
{
    /// <summary>
    /// la sequenza contiene i bin ad una certa iterazione iter
    /// dell'algortimo hsolve (es. iterazione i = 1 con 8 bin )
    /// </summary>
    class Sequence
    {
        public int Zstar { get; set; } = -1;
        /// <summary>
        /// progressivo per indicare la sequenza a che
        /// iterazione appartiene
        /// </summary>
        public int IteratioNumber { get; set; } = -1;

        /// <summary>
        /// criteri adottati per generare una
        /// certa sequenza
        /// </summary>
        public IList<string> Criterias { get; set; } = null;

        /// <summary>
        /// lista di bin relativi ad una certa sequenza.
        /// ogni sequenza è generata ad una certa iterazione
        /// </summary>
        public IList<Bin<Tuple>> Bins { get; set; } = null;

        /// <summary>
        /// campo in cui salvo la lunghezza massima coperta dagli item
        /// nell'ultimo bin
        /// </summary>
        public double WidthCovered { get; set; } = double.MaxValue;

        /// <summary>
        /// area in valore assoluto utilizzata nell'ultimo bin
        /// </summary>
        public double UsedAreaAbsoluteValue { get; set; } = -1;

        /// <summary>
        /// area in percentuale utilizzata nell'ultimo bin
        /// </summary>
        public double UsedAreaPercentageValue { get; set; } = -1;
    }
}
