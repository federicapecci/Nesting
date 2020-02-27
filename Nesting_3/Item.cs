using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting_3
{
    /// <summary>
    /// classe che rappresenta un oggetto (rettangolo)
    /// che deve essere nestato nel bin
    /// </summary>
    public class Item
    {
        /// <summary>
        /// identificativo dell'item
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// altezza dell'item
        /// </summary>
        public float Height { get; set; } = 0;

        /// <summary>
        /// lunghezza dell'item
        /// </summary>
        public float Width { get; set; } = 0;

        /// <summary>
        /// costo v dell'item
        /// </summary>
        public float Price { get; set; } = 0;

        /// <summary>
        /// campo per marcare come eliminato 
        /// un item una volta che è stato inserito nel bin
        /// </summary>
        public bool IsRemoved { get; set; } = false;

        //non cancellare questo costruttore
        public Item(){}

        public Item(Item item)
        {
            Id = item.Id;
            Height = item.Height;
            Width = item.Width;
            Price = item.Price;
            IsRemoved = item.IsRemoved;
        }
    }
}
