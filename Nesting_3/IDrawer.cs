using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting_3
{
    interface IDrawer
    {
        void WriteAllData(IList<Sequence> sequences, string fileName);
    }
}
