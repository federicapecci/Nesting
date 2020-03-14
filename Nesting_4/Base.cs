using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_4
{
    public class Base: Dimension
    {
        /// <summary>
        /// coordinata P in basso a sinitra della green area
        /// </summary>
        public float BLpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata Q in basso a sinistra della green area
        /// </summary>
        public float BLqPosition { get; set; } = 0;

        /// <summary>
        /// coordinata P in alto a sinistra della green area
        /// </summary>
        public float TLpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata Q in alto a sinistra della green area
        /// </summary>
        public float TLqPosition { get; set; } = 0;

        /// <summary>
        /// coordinata in basso a destra della green area
        /// </summary>
        public float BRpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata in basso a destra della green area
        /// </summary>
        public float BRqPosition { get; set; } = 0;

        /// <summary>
        /// coordinata in basso a destra della green area
        /// </summary>
        public float TRpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata in basso a destra della green area
        /// </summary>
        public float TRqPosition { get; set; } = 0;

    }
}
