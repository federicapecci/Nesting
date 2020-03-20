
using netDxf;
using netDxf.Blocks;
using netDxf.Entities;
using netDxf.Header;
using netDxf.Objects;
using netDxf.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting_4
{
    /// <summary>
    /// classe per gestire i file dxf
    /// </summary>
    class DxfDrawer : IDrawer
    {
        /// <summary>
        /// metodo per scrivere un file dxf a partire
        /// dalle sequenze di bin generate da hsolve
        /// </summary>
        /// <param name="sequences"></param>
        /// <param name="fileName"></param>
        public void WriteAllData(IList<Sequence> sequences, string fileName)
        {
            string file = fileName + ".dxf";
            DxfDocument dxf = new DxfDocument();
            TextStyle style = new TextStyle("MyStyle", "Helvetica", FontStyle.Italic | FontStyle.Bold);
            int offsetX = 0;
            int offsetY = 0;
            int offsetCriteria = 0;
            foreach (var sequence in sequences)
            {

                if (sequence.IteratioNumber == 0) //scrivo il criterio
                {
                    MText criteria = null;

                    foreach (var c in sequence.Criterias)
                    {
                        criteria = new MText(c)
                        {
                            Position = new Vector3(-900 + offsetX + offsetCriteria, 700 + offsetY, 0.0),
                            Height = 70,
                            Style = style
                        };
                        dxf.AddEntity(criteria);
                        offsetCriteria += 250;
                    }
                    offsetCriteria = 0;
                }

                MText title = new MText("ITERAZIONE N° " + sequence.IteratioNumber)
                {
                    Position = new Vector3(0 + offsetX, 1800 + offsetY, 0.0),
                    Height = 70,
                    Style = style
                };
                dxf.AddEntity(title);
                foreach (var bin in sequence.Bins)
                {
                    if (bin.NestedItems != null)
                    {
                        //un wipeout rettangolare che contiene tutte le altre forme                         
                        Wipeout wipeout = new Wipeout(0 + offsetX, 0 + offsetY, sequence.Bins.ElementAt(0).Width, sequence.Bins.ElementAt(0).Height);
                        dxf.AddEntity(wipeout);
                        foreach (var pricedItem in bin.NestedItems)
                        {
                            //un wipeout rettangolare che rappresenta una forma
                            wipeout = new Wipeout(pricedItem.BLpPosition + offsetX, pricedItem.BLqPosition + offsetY, pricedItem.Width, pricedItem.Height);

                            //un id progressivo per il wipeout rettangolare
                            MText text = new MText(pricedItem.Id.ToString())
                             {
                                 Position = new Vector3(pricedItem.BLpPosition + 10 + offsetX, pricedItem.BLqPosition + 45 + offsetY, 0.0),
                                 Height = 30,
                                 Style = style
                             };
                            dxf.AddEntity(wipeout);
                            dxf.AddEntity(text);
                        }
                        offsetX += 4000;
                    }
                    
                }
                offsetX += 3000;

                if(sequence.IteratioNumber != 0) //scrivo il criterio
                {//se ho finito due sequenze, modifico l'indice della y, perché dovrò disgegnare le prossime sequenze più in basso
                    offsetX = 0;
                    offsetY += -4000;
                }
            }
            dxf.Save(file);
        }
    }
}
