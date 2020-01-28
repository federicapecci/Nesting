using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    class OrientedItem : Item
    {
        public int Pposition { get; set; } = 0;

        public int Qposition { get; set; } = 0;

        public bool Rotation { get; set; } = false;

        public OrientedItem(Item item) : base(item)  {
            Pposition = 0;
            Qposition = 0;
            Rotation = false;
        }

        public OrientedItem()
        {
        }
    }
}
