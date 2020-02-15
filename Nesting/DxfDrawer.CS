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
            string file = "samplemiao.dxf";

            // by default it will create an AutoCad2000 DXF version
            DxfDocument dxf = new DxfDocument();

            // a rectangular wipeout defined by its bottom-left corner and its width and height
            Wipeout bin = new Wipeout(0, 0, 7.0, 5.0);
            Wipeout rectangle1 = new Wipeout(0, 0, 2.0, 2.0);
            Wipeout rectangle2 = new Wipeout(0, 2, 2.0, 2.0);
            Wipeout rectangle3 = new Wipeout(2, 0, 2.0, 2.0);
            Wipeout rectangle4 = new Wipeout(2, 2, 2.0, 2.0);
            dxf.AddEntity(bin);
            dxf.AddEntity(rectangle1);
            dxf.AddEntity(rectangle2);
            dxf.AddEntity(rectangle3);
            dxf.AddEntity(rectangle4);

            // save to file
            dxf.Save(file);

            // this check is optional but recommended before loading a DXF file
            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file);
            // netDxf is only compatible with AutoCad2000 and higher DXF version
            if (dxfVersion < DxfVersion.AutoCad2000) return;
            // load file
            DxfDocument loaded = DxfDocument.Load(file);
        }
    }
}
