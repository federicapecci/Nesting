
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

namespace Nesting_3
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
            double offsetX = 0;
            int offsetY = 0;

            foreach (var sequence in sequences)
            {
                foreach (var bin in sequence.Bins)
                {
                    if (bin.PricedItems != null)
                    {
                        //un wipeout rettangolare che contiene tutte le altre forme 
                        Wipeout wipeout = new Wipeout(0 + offsetX, 0 + offsetY, sequence.Bins.ElementAt(0).Width, sequence.Bins.ElementAt(0).Height);
                        dxf.AddEntity(wipeout);

                        foreach (var nestedItem in bin.PricedItems)
                        {
                            //un wipeout rettangolare che rappresenta una forma
                            wipeout = new Wipeout(nestedItem.BLpPosition + offsetX, nestedItem.BLqPosition + offsetY, nestedItem.Width, nestedItem.Height);
                            //un id progressivo per il wipeout rettangolare
                            MText text = new MText(nestedItem.Id.ToString())
                            {
                                Position = new Vector3(nestedItem.BLpPosition + 0.3 + offsetX, nestedItem.BLqPosition + 0.5 + offsetY, 0.0),
                                Height = 0.2,
                                Style = style
                            };
                            dxf.AddEntity(wipeout);
                            dxf.AddEntity(text);
                        }
       
                    }
                    offsetX += sequence.Bins.ElementAt(0).Width/0.8; 
                }
                //dato che ricomincio a disegnare i bin di una nuova iterazione, riporto offsetX = 0
                offsetX = 0;
                //inoltre mi sposto in basso di 25, così ho i bin del container i+1 sotto i bin del container i
                offsetY = -25;
            }

            dxf.Save(file);
            
        }
    }
}
