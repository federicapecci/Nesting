using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_3
{
    /// <summary>
    /// classe che rappresenta un item nel 
    /// contesto di un piano cartesiano
    /// </summary>
    public class BasicItem : Dimension
    {
        /// <summary>
        /// coordinata p in basso a sinitra dell'item
        /// </summary>
        public float BLpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata q in basso a sinistra dell'item
        /// </summary>
        public float BLqPosition { get; set; } = 0;

        /// <summary>
        /// coordinata p in alto a sinistra dell'item
        /// </summary>
        public float TLpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata q in alto a sinistra dell'item
        /// </summary>
        public float TLqPosition { get; set; } = 0;

        /// <summary>
        /// coordinata p in basso a destra dell'item
        /// </summary>
        public float BRpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata q in basso a destra dell'item
        /// </summary>
        public float BRqPosition { get; set; } = 0;

        /// <summary>
        /// coordinata p in alto a destra dell'item
        /// </summary>
        public float TRpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata q in altto a destra dell'item
        /// </summary>
        public float TRqPosition { get; set; } = 0;

    }
}
