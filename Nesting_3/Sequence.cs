using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_3
{
    /// <summary>
    /// la sequenza contiene i bin ad una certa iterazione iter
    /// dell'algortimo hsolve (es. iterazione i = 1 con 8 bin )
    /// </summary>
    class Sequence
    {
        /// <summary>
        /// lista di bin relativi ad una certa sequenza.
        /// ogni sequenza è generata ad una certa iterazione
        /// </summary>
        public IList<Bin<Tuple>> Bins { get; set; } = null;


    }
}
