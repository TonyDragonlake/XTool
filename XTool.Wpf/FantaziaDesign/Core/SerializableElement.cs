using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;


namespace FantaziaDesign.Core
{
	public sealed class BasicElement
	{
		private static int GetBasicTypeCode<T>()
		{
			var type = typeof(T);
			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}
			var typeCode = (int)Type.GetTypeCode(type);
			return typeCode;
		}

		public static BasicElement AsBasicElement<T>(T value)
		{
			int typeCode = GetBasicTypeCode<T>();
			if (typeCode > 2)
			{
				return new BasicElement(value, typeCode);
			}
			else
			{
				return null;
			}
		}

		private object m_value;
		private int m_typeCode;

		private BasicElement(object value, int typeCode)
		{
			m_value = value;
			m_typeCode = typeCode;
		}

		public object Value => m_value;

		public Type Type => m_value?.GetType();

		public int TypeCode => m_typeCode;

		public bool OverrideValue<T>(T value)
		{
			int typeCode = GetBasicTypeCode<T>();
			if (typeCode > 2)
			{
				m_value = value;
				m_typeCode = typeCode;
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return m_value.ToString();
		}
	}

	public interface ISerializableElement
	{
		StringRef ElementId { get; }
		BasicElement ElementValue { get; }
		IEnumerable<StringRef> PropertyIds { get; }
		IEnumerable<BasicElement> PropertyValues { get; }
		IEnumerable<IReadOnlyPair<StringRef, BasicElement>> Properties { get; }
		IEnumerable<IReadOnlyPair<StringRef, IReadOnlyList<ISerializableElement>>> ChildrenClasses { get; }
		int PropertyCount { get; }
		int ChildrenClassesCount { get; }
		void SetElementValue<T>(T value);
		bool AddOrSetProperty<T>(StringRef propertyId, T propertyValue);
		bool ContainsProperty(StringRef propertyId);
		bool TryGetPropertyValue(StringRef propertyId, out BasicElement propertyValue);
		bool RemoveProperty(StringRef propertyId);
		void ClearProperties();
		void AddChild(StringRef classId, ISerializableElement childElement);
		bool ContainsChild(ISerializableElement childElement);
		bool ContainsChild(StringRef classId, ISerializableElement childElement);
		bool TryGetChild(StringRef classId, int index, out ISerializableElement childElement);
		bool TryGetChildrenClassById(StringRef classId, out IReadOnlyList<ISerializableElement> childrenClass);
		bool RemoveChildByIndex(StringRef classId, int index);
		bool RemoveChildrenClass(StringRef classId);
		void ClearChildrenClasses();
	}

	public interface ISerializableElementFactory
	{
		ISerializableElement CreateElement(StringRef eleId);
	}

	public sealed class SerializableElement : ISerializableElement
	{
		private StringRef m_eleId;
		private BasicElement m_eleValue;

		private Dictionary<StringRef, BasicElement> m_propDict;
		private Dictionary<StringRef, List<ISerializableElement>> m_childrenClasses;

		public SerializableElement(StringRef eleId)
		{
			if (StringRef.IsNullOrWhiteSpace(eleId))
			{
				throw new ArgumentException($" {nameof(eleId)} cannot be null or whitespace", nameof(eleId));
			}
			m_eleId = eleId;
		}

		public StringRef ElementId => m_eleId;
		public BasicElement ElementValue => m_eleValue;

		public IEnumerable<StringRef> PropertyIds => m_propDict?.Keys;

		public IEnumerable<BasicElement> PropertyValues => m_propDict?.Values;

		public IEnumerable<IReadOnlyPair<StringRef, BasicElement>> Properties => EnumerateProperties();

		public IEnumerable<IReadOnlyPair<StringRef, IReadOnlyList<ISerializableElement>>> ChildrenClasses => EnumerateChildrenClasses();

		public int PropertyCount => m_propDict is null ? 0 : m_propDict.Count;

		public int ChildrenClassesCount => m_childrenClasses is null ? 0 : m_childrenClasses.Count;

		public void SetElementValue<T>(T value)
		{
			if (m_eleValue is null)
			{
				m_eleValue = BasicElement.AsBasicElement(value);
			}
			else
			{
				m_eleValue.OverrideValue(value);
			}
		}

		public bool AddOrSetProperty<T>(StringRef propertyId, T propertyValue)
		{
			if (StringRef.IsNullOrWhiteSpace(propertyId))
			{
				return false;
			}
			var bEle = BasicElement.AsBasicElement(propertyValue);
			if (bEle is null)
			{
				return false;
			}
			if (m_propDict is null)
			{
				m_propDict = new Dictionary<StringRef, BasicElement>();
				m_propDict.Add(propertyId, bEle);
				return true;
			}
			if (m_propDict.TryGetValue(propertyId, out BasicElement oldEle))
			{
				oldEle.OverrideValue(propertyValue);
			}
			else
			{
				m_propDict.Add(propertyId, bEle);
			}
			return true;
		}

		public bool ContainsProperty(StringRef propertyId)
		{
			if (StringRef.IsNullOrWhiteSpace(propertyId) || m_propDict is null)
			{
				return false;
			}
			return m_propDict.ContainsKey(propertyId);
		}

		public bool TryGetPropertyValue(StringRef propertyId, out BasicElement propertyValue)
		{
			if (StringRef.IsNullOrWhiteSpace(propertyId) || m_propDict is null)
			{
				propertyValue = null;
				return false;
			}
			return m_propDict.TryGetValue(propertyId, out propertyValue);
		}

		public bool RemoveProperty(StringRef propertyId)
		{
			if (StringRef.IsNullOrWhiteSpace(propertyId) || m_propDict is null)
			{
				return false;
			}
			return m_propDict.Remove(propertyId);
		}

		public void ClearProperties()
		{
			m_propDict?.Clear();
		}

		public void AddChild(StringRef classId, ISerializableElement childElement)
		{
			if (StringRef.IsNullOrWhiteSpace(classId))
			{
				return;
			}
			if (m_childrenClasses is null)
			{
				m_childrenClasses = new Dictionary<StringRef, List<ISerializableElement>>
				{
					{
						classId,
						new List<ISerializableElement>()
						{
							childElement
						}
					}
				};
			}
			else
			{
				List<ISerializableElement> elements;
				if (m_childrenClasses.TryGetValue(classId, out elements))
				{
					if (elements is null)
					{
						elements = new List<ISerializableElement>();
					}
					elements.Add(childElement);
				}
				else
				{
					m_childrenClasses.Add(classId, new List<ISerializableElement>() { childElement });
				}
			}
		}

		public bool ContainsChild(ISerializableElement childElement)
		{
			if (childElement is null || m_childrenClasses is null)
			{
				return false;
			}

			foreach (var elements in m_childrenClasses.Values)
			{
				if (elements != null && elements.Contains(childElement))
				{
					return true;
				}
			}
			return false;
		}

		public bool ContainsChild(StringRef classId, ISerializableElement childElement)
		{
			if (childElement is null || m_childrenClasses is null)
			{
				return false;
			}
			if (StringRef.IsNullOrWhiteSpace(classId))
			{
				return false;
			}
			List<ISerializableElement> elements;
			if (m_childrenClasses.TryGetValue(classId, out elements))
			{
				if (elements != null && elements.Contains(childElement))
				{
					return true;
				}
			}
			return false;
		}

		public bool TryGetChild(StringRef classId, int index, out ISerializableElement childElement)
		{
			if (m_childrenClasses is null)
			{
				childElement = null;
				return false;
			}
			if (StringRef.IsNullOrWhiteSpace(classId))
			{
				childElement = null;
				return false;
			}
			List<ISerializableElement> elements;
			if (m_childrenClasses.TryGetValue(classId, out elements))
			{
				if (elements != null && index >= 0 && index < elements.Count)
				{
					childElement = elements[index];
					return true;
				}
			}
			childElement = null;
			return false;
		}

		public bool TryGetChildrenClassById(StringRef classId, out IReadOnlyList<ISerializableElement> childrenClass)
		{
			if (m_childrenClasses is null)
			{
				childrenClass = null;
				return false;
			}
			if (StringRef.IsNullOrWhiteSpace(classId))
			{
				childrenClass = null;
				return false;
			}
			List<ISerializableElement> elements;
			if (m_childrenClasses.TryGetValue(classId, out elements))
			{
				if (elements != null)
				{
					childrenClass = elements;
					return true;
				}
			}
			childrenClass = null;
			return false;
		}

		public bool RemoveChildByIndex(StringRef classId, int index)
		{
			if (m_childrenClasses is null)
			{
				return false;
			}
			if (StringRef.IsNullOrWhiteSpace(classId))
			{
				return false;
			}
			List<ISerializableElement> elements;
			if (m_childrenClasses.TryGetValue(classId, out elements))
			{
				if (elements != null && index >= 0 && index < elements.Count)
				{
					elements.RemoveAt(index);
					return true;
				}
			}
			return false;
		}

		public bool RemoveChildrenClass(StringRef classId)
		{
			if (m_childrenClasses is null)
			{
				return false;
			}
			if (StringRef.IsNullOrWhiteSpace(classId))
			{
				return false;
			}
			return m_childrenClasses.Remove(classId);
		}

		public void ClearChildrenClasses()
		{
			if (m_childrenClasses is null)
			{
				return;
			}
			m_childrenClasses.Clear();
		}

		private IEnumerable<IReadOnlyPair<StringRef, BasicElement>> EnumerateProperties()
		{
			if (m_propDict is null)
			{
				yield break;
			}
			var pairCache = new Pair<StringRef, BasicElement>();
			foreach (var prop in m_propDict)
			{
				pairCache.SetPair(prop.Key, prop.Value);
				yield return pairCache;
			}
			yield break;
		}

		private IEnumerable<IReadOnlyPair<StringRef, IReadOnlyList<ISerializableElement>>> EnumerateChildrenClasses()
		{
			if (m_childrenClasses is null)
			{
				yield break;
			}
			var pairCache = new Pair<StringRef, IReadOnlyList<ISerializableElement>>();
			foreach (var item in m_childrenClasses)
			{
				pairCache.SetPair(item.Key, item.Value);
				yield return pairCache;
			}
			yield break;
		}
	}

	public static class ElementSerialization
	{
		public static XmlDocument ToXmlDocument(this ISerializableElement element)
		{
			if (element is null)
			{
				throw new ArgumentNullException(nameof(element));
			}
			XmlDocument xmlDoc = new XmlDocument();
			var rootEle = xmlDoc.CreateElement(element.ElementId);
			ToXml(element, xmlDoc, rootEle);
			//foreach (var item in element.Properties)
			//{
			//	rootEle.SetAttribute(item.First, item.Second.ToString());
			//}
			//if (element.ChildrenClassesCount > 0)
			//{
			//	foreach (var childClassPair in element.ChildrenClasses)
			//	{
			//		var eleList = childClassPair.Second;
			//		if (eleList is null)
			//		{
			//			continue;
			//		}
			//		var propEleName = $"{element.ElementId}.{childClassPair.First}";
			//		var xmlContentEle = xmlDoc.CreateElement(propEleName);
			//		rootEle.AppendChild(xmlContentEle);
			//		foreach (var childEle in eleList)
			//		{
			//			var xmlEle = xmlDoc.CreateElement(childEle.ElementId);
			//			xmlContentEle.AppendChild(xmlEle);
			//			ToXml(childEle, xmlDoc, xmlEle);
			//		}
			//	}
			//}
			//else
			//{
			//	rootEle.InnerText = element.ElementValue.ToString();
			//}
			return xmlDoc;
		}

		private static void ToXml(ISerializableElement element, XmlDocument xmlDoc, XmlElement thisEle)
		{
			foreach (var item in element.Properties)
			{
				thisEle.SetAttribute(item.First, item.Second.ToString());
			}
			if (element.ChildrenClassesCount > 0)
			{
				foreach (var childClassPair in element.ChildrenClasses)
				{
					var eleList = childClassPair.Second;
					if (eleList is null)
					{
						continue;
					}
					var propEleName = $"{element.ElementId}.{childClassPair.First}";
					var xmlContentEle = xmlDoc.CreateElement(propEleName);
					thisEle.AppendChild(xmlContentEle);
					foreach (var childEle in eleList)
					{
						var xmlEle = xmlDoc.CreateElement(childEle.ElementId);
						xmlContentEle.AppendChild(xmlEle);
						ToXml(childEle, xmlDoc, xmlEle);
					}
				}
			}
			else
			{
				thisEle.InnerText = element.ElementValue.ToString();
			}

		}

		public static ISerializableElement FromXmlDocument(XmlDocument xmlDoc, ISerializableElementFactory elementFactory)
		{
			if (xmlDoc is null)
			{
				throw new ArgumentNullException(nameof(xmlDoc));
			}
			var rootEle = xmlDoc.DocumentElement;
			var element = elementFactory.CreateElement(rootEle.Name);
			FromXml(rootEle, element, elementFactory);

			//foreach (XmlAttribute attri in rootEle.Attributes)
			//{
			//	element.AddOrSetProperty(attri.Name, attri.Value);
			//}
			//if (rootEle.HasChildNodes)
			//{
			//	var list = rootEle.ChildNodes.OfType<XmlElement>();
			//	foreach (var content in list)
			//	{
			//		if (content.HasChildNodes)
			//		{
			//			var propKey = content.Name.Split('.').Last();
			//			var childEles = content.ChildNodes.OfType<XmlElement>();
			//			foreach (var childEle in childEles)
			//			{
			//				var childElement = elementFactory.CreateElement(childEle.Name);
			//				element.AddChild(propKey, childElement);
			//				FromXml(childEle, childElement, elementFactory);
			//			}
			//		}
			//	}
			//}
			//else
			//{
			//	element.SetElementValue(rootEle.InnerText);
			//}
			return element;
		}

		private static void FromXml(XmlElement thisEle, ISerializableElement thisElement, ISerializableElementFactory elementFactory)
		{
			foreach (XmlAttribute attri in thisEle.Attributes)
			{
				thisElement.AddOrSetProperty(attri.Name, attri.Value);
			}
			if (thisEle.HasChildNodes)
			{
				var list = thisEle.ChildNodes.OfType<XmlElement>();
				foreach (var content in list)
				{
					if (content.HasChildNodes)
					{
						var propKey = content.Name.Split('.').Last();
						var childEles = content.ChildNodes.OfType<XmlElement>();
						foreach (var childEle in childEles)
						{
							var childElement = elementFactory.CreateElement(childEle.Name);
							thisElement.AddChild(propKey, childElement);
							FromXml(childEle, childElement, elementFactory);
						}
					}
				}
			}
			else
			{
				thisElement.SetElementValue(thisEle.InnerText);
			}
		}
	}

}
