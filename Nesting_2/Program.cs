using System;
using System.Collections.Generic;

namespace Nesting_2
{
    class Program
    {
        /// <summary>
        /// entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //scrivo su file json la configurazione
            IConfigurationIO jsonConfigurationIO = new JsonConfigurationIO();
            //jsonDibaIO.WriteAllData(ref configuration, "hsolveConfiguration");

            //leggo da file json come impostare i parametri dell'algortimo hsolve
            Configuration configuration = jsonConfigurationIO.ReadAllData("6_hsolve_configuration");

            //lancio l'euristica passando i parametri
            IHSolve hsolve = new HSolve(configuration);
            IList<Container> container = hsolve.ComputeHeuristic();

            //scrivo il file dxf
            IDrawer dxfDrawer = new DxfDrawer(container);
            dxfDrawer.WriteAllData("output_6_hsolve_output");

        }
    }
}
