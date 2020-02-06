using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    class EnrichedTuple : Tuple
    {
        /// <summary>
        /// questo campo stabilisce se una determinata 
        /// tripla è accettabile per agganciare un item j
        /// </summary>
        public bool IsFeasible { get; set; } = false;

        public float HatchedRegion { get; set; } = 0;

        public EnrichedTuple(Tuple tuple) : base(tuple) {
            IsFeasible = false;
            HatchedRegion = 0;
        }
    }
}
