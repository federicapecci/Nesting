
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
        public IList<Container> Containers { get; set; } = null;

        public DxfDrawer(IList<Container> Containers)
        {
            this.Containers = Containers;
        }

        public void WriteAllData(string fileName)
        {
            string file = fileName + ".dxf";
            DxfDocument dxf = new DxfDocument();
            TextStyle style = new TextStyle("MyStyle", "Helvetica", FontStyle.Italic | FontStyle.Bold);
            int offsetX = 0;
            int offsetY = 0;

            foreach (var container in Containers)
            {
                foreach (var bin in container.Bins)
                {
                    if (bin.NestedItems != null)
                    {
                        //un wipeout rettangolare che contiene tutte le altre forme 
                        Wipeout wipeout = new Wipeout(0 + offsetX, 0 + offsetY, container.Bins.ElementAt(0).Width, container.Bins.ElementAt(0).Height);
                        dxf.AddEntity(wipeout);

                        foreach (var nestedItem in bin.NestedItems)
                        {
                            //un wipeout rettangolare che rapprsenta una forma
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
                    offsetX += 20;
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
