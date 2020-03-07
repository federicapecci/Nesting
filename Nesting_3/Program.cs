using System;
using System.Collections.Generic;

namespace Nesting_3
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

            //leggo da file json come impostare i parametri dell'algortimo hsolve e li salvo nell'oggetto configuration
            Configuration configuration = jsonConfigurationIO.ReadAllData("3_hsolve_configuration_temp4");

            //lancio l'euristica passando i parametri della configurazione
            IHSolve hsolve = new HSolve(configuration);
            IList<Sequence> sequences = hsolve.ComputeHeuristic();

            //scrivo il file dxf
            IDrawer dxfDrawer = new DxfDrawer();
            dxfDrawer.WriteAllData(sequences, "3_hsolve_output_temp4");

        }
    }
}
