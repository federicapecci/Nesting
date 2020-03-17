using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_4
{

    /// <summary>
    /// classe che rappresenta l'area a sinistra dell'item nuovo da nestare o
    /// quella sotto l'item nuovo da nestare
    /// </summary>
    class AdjacentItem : Base
    {
        /// <summary>
        /// superficie di estensione della green area
        /// </summary>
        public double Area { get; set; } = 0;
    }
}
