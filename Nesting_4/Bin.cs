using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting_4
{
    /// <summary>
    /// classe che rappresenta un contenitore
    /// di item. E' generico in X perché la lista 
    /// di punti potrebbe essere sia di tuple che di triple
    /// </summary>
    /// <typeparam name="X"></typeparam>
    public class Bin<X>
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
        public IList<PricedItem> PricedItems { get; set; } = null;

        /// <summary>
        /// lista di punti contenuti nel bin 
        /// (Una coppia di punti viene generata ogni volta che un 
        /// nuovo elemento viene nestato nel bin)
        /// </summary>
        public IList<X> Points { get; set; } = null;

        public Bin() {}
    }
}
