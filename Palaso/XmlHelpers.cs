using System;
using System.Collections.Generic;
using System.Xml;

namespace Palaso
{
	public class XmlHelpers
	{
		public static void AddOrUpdateAttribute(XmlNode node, string attributeName, string value)
		{
			AddOrUpdateAttribute(node, attributeName, value, null);
		}

		public static void AddOrUpdateAttribute(XmlNode node, string attributeName, string value, IComparer<XmlNode> nodeOrderComparer)
		{
			XmlAttribute attr = GetDocument(node).CreateAttribute(attributeName);
			attr.Value = value;
			if (nodeOrderComparer == null)
			{
				node.Attributes.SetNamedItem(attr);
			}
			else
			{
				InsertNodeUsingDefinedOrder(node, attr, nodeOrderComparer);
				/*XmlAttribute insertAfterAttribute = null;
				foreach (XmlAttribute childAttribute in node.Attributes)
				{
					if (nodeOrderComparer.Compare(attr, childAttribute) < 0)
					{
						break;
					}
					insertAfterAttribute = childAttribute;
				}
				node.Attributes.InsertAfter(attr, insertAfterAttribute);*/
			}
		}

		public static XmlNode GetOrCreateElement(XmlNode node, string xpathNotIncludingElement,
			string elementName, string nameSpace, XmlNamespaceManager nameSpaceManager)
		{
			return GetOrCreateElement(node, xpathNotIncludingElement, elementName, nameSpace, nameSpaceManager, null);
		}

		public static XmlNode GetOrCreateElement(XmlNode node, string xpathNotIncludingElement,
			string elementName, string nameSpace, XmlNamespaceManager nameSpaceManager, IComparer<XmlNode> nodeOrderComparer)
		{
			//enhance: if the parent path isn't found, strip of the last piece and recurse,
			//so that the path will always be created if needed.

			XmlNode parentNode = node.SelectSingleNode(xpathNotIncludingElement,nameSpaceManager);
			if (parentNode == null)
			{
				throw new ApplicationException(string.Format("The path {0} could not be found", xpathNotIncludingElement));
			}
			string prefix = "";
			if (!String.IsNullOrEmpty(nameSpace))
			{
				prefix = nameSpace + ":";
			}
			XmlNode n = parentNode.SelectSingleNode(prefix+elementName,nameSpaceManager);
			if (n == null)
			{
				if (!String.IsNullOrEmpty(nameSpace))
				{
					n = GetDocument(node).CreateElement(string.Empty, elementName, nameSpaceManager.LookupNamespace(nameSpace));
				}
				else
				{
					n = GetDocument(node).CreateElement(elementName);
				}
				if (nodeOrderComparer == null)
				{
					parentNode.AppendChild(n);
				}
				else
				{
					XmlNode insertAfterNode = null;
					foreach (XmlNode childNode in parentNode.ChildNodes)
					{
						if (childNode.NodeType != XmlNodeType.Element)
						{
							continue;
						}
						if (nodeOrderComparer.Compare(n, childNode) < 0)
						{
							break;
						}
						insertAfterNode = childNode;
					}
					parentNode.InsertAfter(n, insertAfterNode);
				}
			}
			return n;
		}

		public static void RemoveElement(XmlNode node, string xpath, XmlNamespaceManager nameSpaceManager)
		{
			XmlNode found = node.SelectSingleNode(xpath, nameSpaceManager);
			if (found != null)
			{
				found.ParentNode.RemoveChild(found);
			}

		}
		private static XmlDocument GetDocument(XmlNode nodeOrDoc)
		{
				if (nodeOrDoc is XmlDocument)
				{
					return (XmlDocument)nodeOrDoc;
				}
				else
				{
					return nodeOrDoc.OwnerDocument;
				}
		}


		/// <summary>
		/// Get an optional attribute value from an XmlNode.
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The attribute to find.</param>
		/// <param name="defaultValue"></param>
		/// <returns>The value of the attribute, or the default value, if the attribute dismissing</returns>
		public static bool GetOptionalBooleanAttributeValue(XmlNode node, string attrName, bool defaultValue)
		{
			return GetBooleanAttributeValue(GetOptionalAttributeValue(node, attrName, defaultValue ? "true" : "false"));
		}

		/// <summary>
		/// Returns true if value of attrName is 'true' or 'yes' (case ignored)
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The optional attribute to find.</param>
		/// <returns></returns>
		public static bool GetBooleanAttributeValue(XmlNode node, string attrName)
		{
			return GetBooleanAttributeValue(GetOptionalAttributeValue(node, attrName));
		}

		/// <summary>
		/// Returns true if sValue is 'true' or 'yes' (case ignored)
		/// </summary>
		public static bool GetBooleanAttributeValue(string sValue)
		{
			return (sValue != null
				&& (sValue.ToLower().Equals("true")
				|| sValue.ToLower().Equals("yes")));
		}

		/// <summary>
		/// Deprecated: use GetOptionalAttributeValue instead.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="attrName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static string GetAttributeValue(XmlNode node, string attrName, string defaultValue)
		{
			return GetOptionalAttributeValue(node, attrName, defaultValue);
		}

		/// <summary>
		/// Get an optional attribute value from an XmlNode.
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The attribute to find.</param>
		/// <returns>The value of the attribute, or null, if not found.</returns>
		public static string GetAttributeValue(XmlNode node, string attrName)
		{
			return GetOptionalAttributeValue(node, attrName);
		}

		/// <summary>
		/// Get an optional attribute value from an XmlNode.
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The attribute to find.</param>
		/// <returns>The value of the attribute, or null, if not found.</returns>
		public static string GetOptionalAttributeValue(XmlNode node, string attrName)
		{
			return GetOptionalAttributeValue(node, attrName, null);
		}

		/// <summary>
		/// Get an optional attribute value from an XmlNode.
		/// </summary>
		/// <param name="node">The XmlNode to look in.</param>
		/// <param name="attrName">The attribute to find.</param>
		/// <returns>The value of the attribute, or null, if not found.</returns>
		public static string GetOptionalAttributeValue(XmlNode node, string attrName, string defaultString)
		{
			if (node != null && node.Attributes != null)
			{
				XmlAttribute xa = node.Attributes[attrName];
				if (xa != null)
					return xa.Value;
			}
			return defaultString;
		}

		public static void InsertNodeUsingDefinedOrder(XmlNode parent, XmlNode newChild, IComparer<XmlNode> nodeOrderComparer)
		{
			if (newChild.NodeType == XmlNodeType.Attribute)
			{
				XmlAttribute insertAfterNode = null;
				foreach (XmlAttribute childNode in parent.Attributes)
				{
					if (nodeOrderComparer.Compare(newChild, childNode) < 0)
					{
						break;
					}
					insertAfterNode = childNode;
				}
				parent.Attributes.InsertAfter((XmlAttribute)newChild, insertAfterNode);
			}
			else
			{
				XmlNode insertAfterNode = null;
				foreach (XmlNode childNode in parent.ChildNodes)
				{
					if (nodeOrderComparer.Compare(newChild, childNode) < 0)
					{
						break;
					}
					insertAfterNode = childNode;
				}
				parent.InsertAfter(newChild, insertAfterNode);
			}
		}
	}
}
