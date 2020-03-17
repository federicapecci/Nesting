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
        public double BLpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata Q in basso a sinistra della green area
        /// </summary>
        public double BLqPosition { get; set; } = 0;

        /// <summary>
        /// coordinata P in alto a sinistra della green area
        /// </summary>
        public double TLpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata Q in alto a sinistra della green area
        /// </summary>
        public double TLqPosition { get; set; } = 0;

        /// <summary>
        /// coordinata in basso a destra della green area
        /// </summary>
        public double BRpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata in basso a destra della green area
        /// </summary>
        public double BRqPosition { get; set; } = 0;

        /// <summary>
        /// coordinata in basso a destra della green area
        /// </summary>
        public double TRpPosition { get; set; } = 0;

        /// <summary>
        /// coordinata in basso a destra della green area
        /// </summary>
        public double TRqPosition { get; set; } = 0;

    }
}
