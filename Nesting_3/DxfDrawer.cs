
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
            int offsetX = 0;
            foreach (var sequence in sequences)
            {
                foreach (var bin in sequence.Bins)
                {
                    
                    if (bin.PricedItems != null)
                    {
                        //un wipeout rettangolare che contiene tutte le altre forme 
                        Wipeout wipeout = new Wipeout(0 + offsetX, 0, sequence.Bins.ElementAt(0).Width, sequence.Bins.ElementAt(0).Height);
                        dxf.AddEntity(wipeout);
                        foreach (var pricedItem in bin.PricedItems)
                        {
                            //un wipeout rettangolare che rappresenta una forma
                            wipeout = new Wipeout(pricedItem.BLpPosition+offsetX, pricedItem.BLqPosition, pricedItem.Width, pricedItem.Height);

                            //un id progressivo per il wipeout rettangolare
                            MText text = new MText(pricedItem.Id.ToString())
                             {
                                 Position = new Vector3(pricedItem.BLpPosition + 30 + offsetX, pricedItem.BLqPosition + 45 , 0.0),
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
            }
            dxf.Save(file);
        }
    }
}
