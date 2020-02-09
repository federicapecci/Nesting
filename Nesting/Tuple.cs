using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    /// <summary>
    /// classe che rappresenta i punti in cui è 
    /// possibile nestare un nuovo item 
    /// </summary>
    class Tuple 
    {
        /// <summary>
        /// campo che rappresenta la coordinata p iniziale
        /// dove si nesta inizialmente un item
        /// </summary>
        public float Pposition { get; set; } = 0;

        /// <summary>
        /// campo che rappresenta la coordinata q iniziale
        /// dove si nesta inizialmente un item
        /// </summary>
        public float Qposition { get; set; } = 0;

        /// <summary>
        /// campo che stabilisce se un punto è stato già usato 
        /// per nestare un item oppure no
        /// </summary>
        public bool IsUsed { get; set; } = false;

        /// <summary>
        /// campo che rappresenta la coordinata p dell'item
        /// dopo aver cercato di compattare gli item
        /// </summary>
        public float PfinalPosition { get; set; } = 0;

        /// <summary>
        /// campo che rappresenta la coordinata q dell'item
        /// dopo aver cercato di compattare gli item
        /// </summary>
        public float QfinalPosition { get; set; } = 0;

        /// <summary>
        /// campo che rappresenta lo scarto potenziale in basso e a sinistra dell'item, 
        /// dopo che si è cercato di compattare la soluzione
        /// </summary>
        public float HatchedRegion { get; set; } = 0;

        public Tuple() 
        {
            Pposition = 0;
            Qposition = 0;
            IsUsed = false;
            PfinalPosition = 0;
            QfinalPosition = 0;
            HatchedRegion = 0;
        }

        public Tuple(Tuple triple)
        {
            Pposition = triple.Pposition;
            Qposition = triple.Qposition;
            IsUsed = triple.IsUsed;
            PfinalPosition = 0;
            QfinalPosition = 0;
            HatchedRegion = 0;
        }
    }
}
