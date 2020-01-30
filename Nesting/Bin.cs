using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    class Bin
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
        /// lista di item all'interno del bin
        /// </summary>
        public IList<OrientedItem> OrientedItems { get; set; } = null;

        /// <summary>
        /// lista di triple in cui è possibile nestare il nuovo item 
        /// </summary>
        public IList<Triple> Triples { get; set; } = null;

    }
}
