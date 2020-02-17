using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting_1
{
    /// <summary>
    /// classe che rappresenta l'item nel contesto 
    /// di un bin 
    /// </summary>
    class NestedItem : Item
    {
        /// <summary>
        /// coordinata P in basso a sinitra dell'item
        /// </summary>
        public float BLpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata Q in basso a sinistra dell'item
        /// </summary>
        public float BLqPosition { get; set; } = 0;

        /// <summary>
        /// coordinata P in alto a sinistra dell'item
        /// </summary>
        public float TLpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata Q in alto a sinistra dell'item
        /// </summary>
        public float TLqPosition { get; set; } = 0;

        /// <summary>
        /// coordinata in basso a destra dell'item
        /// </summary>
        public float BRpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata in basso a destra dell'item
        /// </summary>
        public float BRqPosition { get; set; } = 0;

        public NestedItem(Item item) : base(item)
        {
            BLpPosition = 0;
            BLqPosition = 0;
            
        }
    }
}
