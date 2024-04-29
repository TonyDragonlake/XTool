using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using FantaziaDesign.Core;
using YamlDotNet.Serialization;
using System;

namespace XTool.Excel.WindowModels
{
	public sealed class YamlStreamToFilterValuesSettingsConverter : IValueConverter<Stream, object>
	{
		private sealed class __Settings
		{
			public FilterValuesSetting[] FilterValuesSettings { get; set; }
		}

		public Stream ConvertBack(object target)
		{
			var enumerable = target as IEnumerable<FilterValuesSetting>;
			if (enumerable != null)
			{
				FilterValuesSetting[] array;
				var collection = enumerable as ICollection<FilterValuesSetting>;
				if (collection != null)
				{
					array = new FilterValuesSetting[collection.Count];
					collection.CopyTo(array, 0);
				}
				else
				{
					array = enumerable.ToArray();
				}
				if (array != null && array.Length > 0)
				{
					var settings = new __Settings() { FilterValuesSettings = array };
					var serializer = new Serializer();
					var memory = new MemoryStream();
					using (var writer = new StreamWriter(memory, Encoding.UTF8, 1024, true))
					{
						serializer.Serialize(writer, settings);
						writer.Flush();
					}
					memory.Seek(0, SeekOrigin.Begin);
					return memory;
				}
			}
			return null;
		}

		public object ConvertTo(Stream source)
		{
			string yamlText = null;
			using (var sr = new StreamReader(source, Encoding.UTF8, true, 1024, true))
			{
				yamlText = sr.ReadToEnd();
			}
			if (!string.IsNullOrWhiteSpace(yamlText))
			{
				var settings = new Deserializer().Deserialize<__Settings>(yamlText);
				if (settings != null)
				{
					var fvSettings = settings.FilterValuesSettings;
					if (fvSettings != null && fvSettings.Length > 0)
					{
						return new List<FilterValuesSetting>(fvSettings);
					}
				}
			}
			return null;
		}
	}
}
