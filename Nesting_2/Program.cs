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
            //dichiaro oggetto per manipolare la configurazione dell'algortimo hsolve   
            IConfigurationIO jsonConfigurationIO = new JsonConfigurationIO();

            //scrivo su file json la configurazione
            //jsonDibaIO.WriteAllData(ref configuration, "hsolveConfiguration");

            //leggo da file json come impostare i parametri dell'algortimo hsolve e li salvo nell'oggetto configuration
            Configuration configuration = jsonConfigurationIO.ReadAllData("6_hsolve_configuration");

            //lancio l'euristica passando i parametri della configurazione
            IHSolve hsolve = new HSolve(configuration);
            IList<Sequence> sequences = hsolve.ComputeHeuristic();

            //scrivo il file dxf
            IDrawer dxfDrawer = new DxfDrawer();
            dxfDrawer.WriteAllData(sequences, "output_6_hsolve_output");

        }
    }
}
