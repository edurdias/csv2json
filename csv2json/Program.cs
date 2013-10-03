using System;
using System.IO;
using System.Linq;
using CsvHelper;

namespace csv2json
{
	class Program
	{
		/// <summary>
		/// Parses a CSV file into a JSON file
		/// </summary>
		/// <param name="args">First argument is the CSV input file, the second argument should be the output JSON file. If the JSON file is not specified, it uses the same name of CSV file.</param>
		static void Main(string[] args)
		{
			if (args.Any())
			{
				var input = args[0];
				if (!File.Exists(input))
				{
					Console.Error.WriteLine("Invalid CSV file name.");
					return;
				}

				var inputInfo = new FileInfo(input);
				var output = args.Count() == 2 ? args[1] : inputInfo.Name.Replace(inputInfo.Extension, string.Empty) + ".json";


				using (var writer = new StreamWriter(new FileStream(output, FileMode.OpenOrCreate)))
				using (var reader = new CsvReader(new CsvParser(new StreamReader(new FileStream(input, FileMode.Open)))))
				{
					writer.Write("[");
					reader.Read();
					var headers = reader.FieldHeaders.ToList();
					do
					{
						writer.Write("{");
						foreach (var header in headers)
						{
							writer.Write(string.Format("'{0}':'{1}'", header, reader.GetField(header)));
							if (headers.IndexOf(header) < headers.Count - 1)
								writer.Write(",");
						}
						writer.Write("}");

						reader.Read();
						writer.WriteLine(reader.CurrentRecord != null ? "," : string.Empty);

					} while (reader.CurrentRecord != null);
					writer.Write("]");
				}
			}
		}
	}
}
