using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    class FeasibleTriple : Triple
    {
        /// <summary>
        /// questo campo stabilisce se una determinata 
        /// tripla è accettabile per agganciare un item j
        /// </summary>
        public bool IsFeasible { get; set; } = false;

        public FeasibleTriple(Triple triple) : base(triple) {
            IsFeasible = false;
        }
    }
}
