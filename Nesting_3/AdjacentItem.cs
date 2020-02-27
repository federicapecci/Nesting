using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_3
{
    /// <summary>
    ///  classe che rappresenta l'item  adiacente (in basso o sinistra) al nuovo item
    ///  che si vuole nestare
    /// </summary>

    class AdjacentItem : BasicItem
    {
        /// <summary>
        /// superficie adiacente (in basso o sinistra) al nuovo item
        /// che si vuole nestare
        /// </summary>
        public float Area { get; set; } = 0;

        /// <summary>
        /// coordinata p in alto a destra della superficie adiacente (in basso o sinistra) 
        /// al nuovo item che si vuole nestare
        /// </summary>
        public float TRpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata q in alto a destra della superficie adiacente (in basso o sinistra) 
        /// al nuovo item che si vuole nestare
        /// </summary>
        public float TRqPosition { get; set; } = 0;
    }
}
