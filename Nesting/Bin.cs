using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    class Bin<X>
    {
        /// <summary>
        /// identificativo del bin
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// altezza del bin
        /// </summary>
        public int Height { get; set; } = 0;

        /// <summary>
        /// lunghezza del bin
        /// </summary>
        public int Width { get; set; } = 0;

        /// <summary>
        /// lista di item nestati all'interno del bin
        /// </summary>
        public IList<NestedItem> NestedItems { get; set; } = null;

        /// <summary>
        /// lista di punti contenuti nel bin 
        /// </summary>
        public IList<X> Points { get; set; } = null;

    }
}
