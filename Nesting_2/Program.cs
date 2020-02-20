using System;
using System.Collections.Generic;

namespace Nesting_2
{
    class Program
    {
        /// <summary>
        /// metodo entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //scrivo la configurazione per hsolve
            /*var configuration = new Configuration()
            {
                BinHeight = 5,
                BinWidth = 8,
                Items = new List<Item>(){
                    new Item()
                    {
                        Id = 0,
                        Height = 3,
                        Width = 5,
                        Price = 15,
                        IsRemoved = false
                    },

                    new Item()
                    {
                        Id = 1,
                        Height = 1,
                        Width = 2,
                        Price = 2,
                        IsRemoved = false
                    },

                    new Item()
                    {
                        Id = 2,
                        Height = 2,
                        Width = 3,
                        Price = 6,
                        IsRemoved = false
                    },

                    new Item()
                    {
                        Id = 3,
                        Height = 2,
                        Width = 5,
                        Price = 10,
                        IsRemoved = false
                    },

                    new Item()
                    {
                        Id = 4,
                        Height = 1,
                        Width = 1,
                        Price = 1,
                        IsRemoved = false
                    }
                },
                MaxIter = 100
            };*/

            //scrivo su file json la configurazione
            IDibaIO jsonDibaIO = new JsonDibaIO();
            //jsonDibaIO.WriteAllData(ref configuration, "hsolveConfiguration");

            //leggo da file json come impostare i parametri dell'algortimo hsolve
            Configuration configuration = jsonDibaIO.ReadAllData("hsolveConfiguration");

            //lancio l'euristica
            IHSolve hsolve = new HSolve(configuration);
            Bin<Tuple> bin = hsolve.ComputeHeuristic();

            //scrivo il dxf
            IDrawer drawer = new DxfDrawer(bin);
            drawer.WriteDxfDocument();

        }
    }
}
