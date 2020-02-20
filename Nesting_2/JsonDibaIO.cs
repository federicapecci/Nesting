using System;
using System.IO;
using Newtonsoft.Json;

namespace Nesting_2
{
	/// <summary>
	/// Lettore della distinta base.
	/// </summary>
	public class JsonDibaIO : IDibaIO
	{
		/// <inheritdoc />
		public Configuration ReadAllData(string filename)
		{
            Configuration result = null;

			if (string.IsNullOrWhiteSpace(filename))
			{
				throw new ArgumentException(nameof(filename));
			}

			var jsonSerializer = new JsonSerializer
			{
				Formatting = Formatting.Indented
			};

			using (StreamReader streamReader = File.OpenText(filename))
			{
				result = (Configuration)jsonSerializer.Deserialize(streamReader, typeof(Configuration));
			}

			return result;
		}

		/// <inheritdoc />
		public void WriteAllData(ref Configuration dibaDb, string filename)
		{
			if (dibaDb == null)
			{
				throw new ArgumentNullException(nameof(dibaDb));
			}
			if (string.IsNullOrWhiteSpace(filename))
			{
				throw new ArgumentException(nameof(filename));
			}

			var jsonSerializer = new JsonSerializer
			{
				Formatting = Formatting.Indented
			};

			using (var streamWriter = new StreamWriter(filename))
			{
				using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
				{
					jsonSerializer.Serialize(jsonWriter, dibaDb);
				}
			}
		}
    }
}