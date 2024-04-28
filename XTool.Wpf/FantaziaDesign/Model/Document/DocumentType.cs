using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FantaziaDesign.Core;

namespace FantaziaDesign.Model.Document
{
	public sealed class DocumentType : IValueConverter<Stream, object>, IEnumerable<string>
	{
		private static readonly Dictionary<StringRef, DocumentType> s_docTypes = new Dictionary<StringRef, DocumentType>();

		public static DocumentType Register(string name)
		{
			if (s_docTypes.TryGetValue(name, out var docType))
			{
				if (docType is null)
				{
					docType = new DocumentType(name);
					s_docTypes[docType.NameRef] = docType;
				}
			}
			else
			{
				docType = new DocumentType(name);
				s_docTypes.Add(docType.NameRef, docType);
			}
			return docType;
		}

		public static bool TryGetDocumentType(string name, out DocumentType docType)
		{
			return s_docTypes.TryGetValue(name, out docType);
		}

		public static readonly char Spliter = '|';

		private readonly StringRef m_name;
		private HashSet<StringRef> m_extensions;
		private IValueConverter<Stream, object> m_docConverter;

		internal DocumentType(StringRef name)
		{
			m_name = name;
			m_extensions = new HashSet<StringRef>();
		}

		public string Name => m_name;

		internal StringRef NameRef => m_name;

		public bool IsFormatable => !m_name.IsStringNullOrWhitespace && m_extensions != null && m_extensions.Count > 0;

		public IValueConverter<Stream, object> DocumentConverter => m_docConverter;

		public DocumentType AddExtension(string extension)
		{
			if (!string.IsNullOrWhiteSpace(extension))
			{
				m_extensions.Add(extension.ToLower());
			}
			return this;
		}

		public DocumentType AddExtensions(params string[] extensions)
		{
			foreach (var extension in extensions)
			{
				if (!string.IsNullOrWhiteSpace(extension))
				{
					m_extensions.Add(extension.ToLower());
				}
			}
			return this;
		}

		public DocumentType RemoveExtension(string extension)
		{
			if (!string.IsNullOrWhiteSpace(extension))
			{
				m_extensions.Remove(extension.ToLower());
			}
			return this;
		}

		public DocumentType RemoveExtensions(params string[] extensions)
		{
			foreach (var extension in extensions)
			{
				if (!string.IsNullOrWhiteSpace(extension))
				{
					m_extensions.Remove(extension.ToLower());
				}
			}
			return this;
		}

		public DocumentType ClearExtensions()
		{
			m_extensions.Clear();
			return this;
		}

		public bool ContainsExtension(string extension)
		{
			if (string.IsNullOrWhiteSpace(extension))
			{
				return false;
			}
			return m_extensions.Contains(extension.ToLower());
		}

		public DocumentType SetDocumentConverter(IValueConverter<Stream, object> converter)
		{
			m_docConverter = converter;
			return this;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			FormatDocumentType(builder, this);
			return builder.ToString();
		}

		public object ConvertTo(Stream source)
		{
			return m_docConverter?.ConvertTo(source);
		}

		public Stream ConvertBack(object target)
		{
			return m_docConverter?.ConvertBack(target);
		}

		public static void FormatAny(StringBuilder builder, string name = null)
		{
			if (builder is null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			if (string.IsNullOrWhiteSpace(name))
			{
				name = "All File";
			}
			builder.AppendFormat("{0}|*.*", name);
		}

		public static bool FormatDocumentType(StringBuilder builder, DocumentType documentType, string name = null)
		{
			if (builder is null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			if (documentType is null)
			{
				throw new ArgumentNullException(nameof(documentType));
			}

			if (!documentType.IsFormatable)
			{
				return false;
			}
			if (string.IsNullOrWhiteSpace(name))
			{
				name = documentType.Name;
			}
			builder.AppendFormat("{0}|", name);
			var enumerator = documentType.m_extensions.GetEnumerator();
			return FormatExtensionsInternal(builder, GetValueFromStringRef, enumerator);
		}

		private static string GetValueFromStringRef(StringRef strRef) => strRef;

		public static bool FormatExtensions<T>(StringBuilder builder, IReadOnlyList<T> extensions, Func<T, string> extensionConverter)
		{
			if (builder is null)
			{
				throw new ArgumentNullException(nameof(builder));
			}
			if (extensions is null)
			{
				throw new ArgumentNullException(nameof(extensions));
			}
			var enumerator = extensions.GetEnumerator();
			if (enumerator is null)
			{
				return false;
			}
			return FormatExtensionsInternal(builder, extensionConverter, enumerator);
		}

		private static bool FormatExtensionsInternal<T>(StringBuilder builder, Func<T, string> extensionConverter, IEnumerator<T> enumerator)
		{
			using (enumerator)
			{
				if (!enumerator.MoveNext())
				{
					return false;
				}
				if (extensionConverter is null)
				{
					builder.AppendFormat("*.{0}", enumerator.Current);
					while (enumerator.MoveNext())
					{
						builder.AppendFormat(";*.{0}", enumerator.Current);
					}
				}
				else
				{
					var current = extensionConverter.Invoke(enumerator.Current);
					builder.AppendFormat("*.{0}", current);
					while (enumerator.MoveNext())
					{
						current = extensionConverter.Invoke(enumerator.Current);
						builder.AppendFormat(";*.{0}", current);
					}
				}
			}
			return true;
		}

		private IEnumerable<string> Enumeration()
		{
			foreach (var extension in m_extensions)
			{
				yield return extension.Item;
			}
		}

		public IEnumerator<string> GetEnumerator()
		{
			return Enumeration().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Enumeration().GetEnumerator();
		}
	}
}
