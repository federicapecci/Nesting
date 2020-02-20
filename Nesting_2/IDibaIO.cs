namespace Nesting_2
{
	/// <summary>
	/// Interfaccia di Input Output della distinta base.
	/// </summary>
	public interface IDibaIO
	{
        /// <summary>
        /// Lettura di tutta la distinta base.
        /// </summary>
        /// <param name="filename">Nome del file della distinta.</param>
        /// <returns>La distinta base.</returns>
        Configuration ReadAllData(string filename);

		/// <summary>
		/// Scrittura di tutta la distinta base.
		/// </summary>
		/// <param name="dibaDb">Riferimento al contenitore della distinta base.</param>
		/// <param name="filename">Nome del file della distinta.</param>
		/// <returns>La distinta base.</returns>
		void WriteAllData(ref Configuration dibaDb, string filename);
	}
}