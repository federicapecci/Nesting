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
            //scrivo su file json la configurazione
            IConfigurationIO jsonDibaIO = new JsonConfigurationIO();
            //jsonDibaIO.WriteAllData(ref configuration, "hsolveConfiguration");

            //leggo da file json come impostare i parametri dell'algortimo hsolve
            Configuration configuration = jsonDibaIO.ReadAllData("5_hsolve_configuration");

            //lancio l'euristica passando i parametri
            IHSolve hsolve = new HSolve(configuration);
            Bin<Tuple> bin = hsolve.ComputeHeuristic();

            //scrivo il dxf
            IDrawer drawer = new DxfDrawer(bin);
            drawer.WriteDxfDocument("output_5_hsolve_output");

        }
    }
}
