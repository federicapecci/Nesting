﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Nesting_4
{
    /// <summary>
    /// classe che rappresenta un item durante le 
    /// iterazioni  dell'algoritmo hsolve
    /// </summary>

    public class Item : Base
    {
        /// <summary>
        /// identificativo dell'item nelle iterazioni
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// costo v dell'item nelle iterazioni
        /// </summary>
        public double Price { get; set; } = 0;

        /// <summary>
        /// campo per marcare come eliminato 
        /// un item una volta che è stato inserito nel bin
        /// durante un'iterazione
        /// </summary>
        public bool IsRemoved { get; set; } = false;
    }
}
