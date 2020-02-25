
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

namespace Nesting_2
{
    class DxfDrawer : IDrawer
    {
        public IList<Bin<Tuple>> Bins { get; set; } = null;

        public DxfDrawer(IList<Bin<Tuple>> Bins)
        {
            this.Bins = Bins;
        }

        public void WriteAllData(string fileName)
        {
            string file = fileName + ".dxf";

            //// by default it will create an AutoCad2000 DXF version
            DxfDocument dxf = new DxfDocument();
            int offset = 0;
            foreach (var bin in Bins)
            {
                //un wipeout rettangolare che contiene tutte le altre forme 
                Wipeout wipeout = new Wipeout(0+offset, 0, bin.Width, bin.Height);
                dxf.AddEntity(wipeout);

                TextStyle style = new TextStyle("MyStyle", "Helvetica", FontStyle.Italic | FontStyle.Bold);

                if (bin.NestedItems != null)
                {
                    foreach (var nestedItem in bin.NestedItems)
                    {
                        //un wipeout rettangolare che rapprsenta una forma
                        wipeout = new Wipeout(nestedItem.BLpPosition + offset, nestedItem.BLqPosition, nestedItem.Width, nestedItem.Height);
                        //un id progressivo per il wipeout rettangolare
                        MText text = new MText(nestedItem.Id.ToString())
                        {
                            Position = new Vector3(nestedItem.BLpPosition + 0.3 + offset, nestedItem.BLqPosition + 0.5, 0.0),
                            Height = 0.2,
                            Style = style
                        };
                        dxf.AddEntity(wipeout);
                        dxf.AddEntity(text);
                    }
                }
                offset += 20;
            }

            // save to file
            dxf.Save(file);
            
            //// this check is optional but recommended before loading a DXF file
            //DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file);
            //// netDxf is only compatible with AutoCad2000 and higher DXF version
            //if (dxfVersion < DxfVersion.AutoCad2000) return;
            //// load file
            //DxfDocument loaded = DxfDocument.Load(file);

        }
    }
}
