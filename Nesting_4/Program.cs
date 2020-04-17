using System;
using System.Collections.Generic;

namespace Nesting_4
{
    class Program
    {
        /// <summary>
        /// entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //aggiungo un timer
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

            //dichiaro oggetto per manipolare la configurazione dell'algortimo hsolve   
            IConfigurationIO jsonConfigurationIO = new JsonConfigurationIO();

            //leggo da file json come impostare i parametri dell'algortimo hsolve e li salvo nell'oggetto configuration
            Configuration configuration = jsonConfigurationIO.ReadAllData("1_configuration");

            //lancio l'algoritmo passando i parametri della configurazione
            HBP hbp = new HBP(configuration);
            hbp.ComputeAlgorithm();

            //scrivo il file dxf
            IDrawer dxfDrawer = new DxfDrawer();
            dxfDrawer.WriteAllData(hbp.Sequences, "1_output_all_criterias_bis");

            //leggo risultato timer
            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("elapsedMs " + elapsedMs);

        }
    }
}
