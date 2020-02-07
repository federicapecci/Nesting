using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    class NestedItem : Item
    {
        /// <summary>
        /// coordinata P di aggancio dell'item
        /// </summary>
        public float Pposition { get; set; } = 0;

        /// <summary>
        /// coordinata Q di aggancio dell'item
        /// </summary>
        public float Qposition { get; set; } = 0;

        public NestedItem(Item item) : base(item)
        {
            Pposition = 0;
            Qposition = 0;
        }

        public NestedItem()
        {
        }
    }
}
