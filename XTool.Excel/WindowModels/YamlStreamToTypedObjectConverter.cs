using System.IO;
using System.Text;
using FantaziaDesign.Core;
using YamlDotNet.Serialization;

namespace XTool.Excel.WindowModels
{
	public class YamlStreamToTypedObjectConverter<T> : IValueConverter<Stream, T>
	{
		public virtual Stream ConvertBack(T target)
		{
			object o = target;
			if (o == null) return null;
			var serializer = new Serializer();
			var memory = new MemoryStream();
			using (var writer = new StreamWriter(memory, Encoding.UTF8, 1024, true))
			{
				serializer.Serialize(writer, target);
				writer.Flush();
			}
			memory.Seek(0, SeekOrigin.Begin);
			return memory;
		}

		public virtual T ConvertTo(Stream source)
		{
			string yamlText = null;
			using (var sr = new StreamReader(source, Encoding.UTF8, true, 1024, true))
			{
				yamlText = sr.ReadToEnd();
			}
			if (!string.IsNullOrWhiteSpace(yamlText))
			{
				return new Deserializer().Deserialize<T>(yamlText);
			}
			return default(T);
		}
	}

}
