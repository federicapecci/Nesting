﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    class Item
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
        public bool IsRemoved { get; set; } = false;

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
