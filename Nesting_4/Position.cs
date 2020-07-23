using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting_4
{
    /// <summary>
    /// classe che rappresenta i punti in cui è 
    /// possibile nestare un nuovo item 
    /// </summary>
    public class Position 
    {
        /// <summary>
        /// campo che rappresenta la coordinata p iniziale
        /// dove si nesta inizialmente un item
        /// </summary>
        public double Pposition { get; set; } = 0;

        /// <summary>
        /// campo che rappresenta la coordinata q iniziale
        /// dove si nesta inizialmente un item
        /// </summary>
        public double Qposition { get; set; } = 0;

        /// <summary>
        /// campo che rappresenta la rotazione r 
        /// di un item, 0 => no  rotazione, 1 => rotazione di 90°
        /// </summary>
        public double Rposition { get; set; } = 0;

        /// <summary>
        /// campo che stabilisce se un punto è stato già usato 
        /// per nestare un item oppure no
        /// </summary>
        public bool IsUsed { get; set; } = false;

        /// <summary>
        /// campo che rappresenta la coordinata p dell'item
        /// dopo aver cercato di compattare gli item
        /// </summary>
        public double PfinalPosition { get; set; } = 0;

        /// <summary>
        /// campo che rappresenta la coordinata q dell'item
        /// dopo aver cercato di compattare gli item
        /// </summary>
        public double QfinalPosition { get; set; } = 0;

        /// <summary>
        /// campo che rappresenta lo scarto potenziale in basso e a sinistra dell'item, 
        /// dopo che si è cercato di compattare la soluzione
        /// </summary>
        public double HatchedArea { get; set; } = 0;

        public Position() 
        {
            Pposition = 0;
            Qposition = 0;
            Rposition = 0;
            IsUsed = false;
            PfinalPosition = 0;
            QfinalPosition = 0;
            HatchedArea = 0;
        }

    }
}
