using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    class Triple 
    {
        public float Pposition { get; set; } = 0;

        public float Qposition { get; set; } = 0;

        public bool Rotation { get; set; } = false;

        public bool IsUsed { get; set; } = false;

        public Triple() 
        {
            Pposition = 0;
            Qposition = 0;
            Rotation = false;
            IsUsed = false;
        }
    }
}
