using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    class Tuple 
    {
        public float Pposition { get; set; } = 0;

        public float Qposition { get; set; } = 0;

        public bool IsUsed { get; set; } = false;

        public Tuple() 
        {
            Pposition = 0;
            Qposition = 0;
            IsUsed = false;
        }

        public Tuple(Tuple triple)
        {
            Pposition = triple.Pposition;
            Qposition = triple.Qposition;
            IsUsed = triple.IsUsed;
        }
    }
}
