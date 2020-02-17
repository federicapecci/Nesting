
using netDxf;
using netDxf.Entities;
using netDxf.Header;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nesting
{
    class DxfDrawer : IDrawer
    {
        public Bin<Tuple> Bin { get; set; } = null;

        public DxfDrawer(Bin<Tuple> Bin)
        {
            this.Bin = Bin;
        }

        public void WriteDxfDocument()
        {
            string file = "bin3.dxf";

            // by default it will create an AutoCad2000 DXF version
            DxfDocument dxf = new DxfDocument();

            // a rectangular wipeout defined by its bottom-left corner and its width and height
            Wipeout wipeout = new Wipeout(0, 0, Bin.Width, Bin.Height);
            dxf.AddEntity(wipeout);

            if (Bin.NestedItems.Count >= 1)
            {
                foreach (var nestedItem in Bin.NestedItems)
                {
                    wipeout = new Wipeout(nestedItem.BLpPosition, nestedItem.BLqPosition, nestedItem.Width, nestedItem.Height);
                    dxf.AddEntity(wipeout);
                }

                // save to file
                dxf.Save(file);
            }

            // this check is optional but recommended before loading a DXF file
            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file);
            // netDxf is only compatible with AutoCad2000 and higher DXF version
            if (dxfVersion < DxfVersion.AutoCad2000) return;
            // load file
            DxfDocument loaded = DxfDocument.Load(file);
        }
    }
}
