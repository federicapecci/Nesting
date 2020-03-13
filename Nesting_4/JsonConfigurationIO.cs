using System;
using System.IO;
using Newtonsoft.Json;

namespace Nesting_4
{
	/// <summary>
	/// classe per leggere/scrivere la configurazione del bin e degli item
	/// </summary>
	public class JsonConfigurationIO : IConfigurationIO
	{
        /// <summary>
        ///  metodo per leggere la configurazione del bin e degli item
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Configuration ReadAllData(string fileName)
		{
            Configuration result = null;

			if (string.IsNullOrWhiteSpace(fileName))
			{
				throw new ArgumentException(nameof(fileName));
			}

			var jsonSerializer = new JsonSerializer
			{
				Formatting = Formatting.Indented
			};

			using (StreamReader streamReader = File.OpenText(fileName))
			{
				result = (Configuration)jsonSerializer.Deserialize(streamReader, typeof(Configuration));
			}

			return result;
		}

        /// <summary>
        /// metodo per scrivere la configurazione del bin e degli item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="fileName"></param>
        public void WriteAllData(ref Configuration configuration, string fileName)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException(nameof(configuration));
			}
			if (string.IsNullOrWhiteSpace(fileName))
			{
				throw new ArgumentException(nameof(fileName));
			}

			var jsonSerializer = new JsonSerializer
			{
				Formatting = Formatting.Indented
			};

			using (var streamWriter = new StreamWriter(fileName))
			{
				using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
				{
					jsonSerializer.Serialize(jsonWriter, configuration);
				}
			}
		}
    }
}