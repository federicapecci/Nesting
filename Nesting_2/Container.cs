using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_2
{
    /// <summary>
    /// il container contiene i bin ad una certa iterazione iter
    /// dell'algortimo hsolve (es. iterazione i = 1 con 8 bin )
    /// </summary>
    class Container
    {
        /// <summary>
        /// lista di bin relativi ad un certo container (aka iterazione)
        /// </summary>
        public IList<Bin<Tuple>> Bins { get; set; } = null;

    }
}
