using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AddinExpress.Installer.WiXDesigner.DesignTime
{
	public static class AttributeCollectionExtension
	{
		public static void Add(this AttributeCollection ac, Attribute attribute)
		{
			FieldInfo field = ac.GetType().GetField("_attributes", BindingFlags.Instance | BindingFlags.NonPublic);
			Attribute[] value = (Attribute[])field.GetValue(ac);
			List<Attribute> attributes = new List<Attribute>();
			if (value != null)
			{
				attributes.AddRange(value);
			}
			attributes.Add(attribute);
			field.SetValue(ac, attributes.ToArray());
		}

		public static void Add(this AttributeCollection ac, Attribute attribute, bool removeBeforeAdd)
		{
			FieldInfo field = ac.GetType().GetField("_attributes", BindingFlags.Instance | BindingFlags.NonPublic);
			Attribute[] value = (Attribute[])field.GetValue(ac);
			List<Attribute> attributes = new List<Attribute>();
			if (value != null)
			{
				attributes.AddRange(value);
			}
			if (removeBeforeAdd)
			{
				attributes.RemoveAll((Attribute a) => a.Match(attribute));
			}
			attributes.Add(attribute);
			field.SetValue(ac, attributes.ToArray());
		}

		public static void Add(this AttributeCollection ac, Attribute attribute, Type typeToRemoveBeforeAdd)
		{
			FieldInfo field = ac.GetType().GetField("_attributes", BindingFlags.Instance | BindingFlags.NonPublic);
			Attribute[] value = (Attribute[])field.GetValue(ac);
			List<Attribute> attributes = new List<Attribute>();
			if (value != null)
			{
				attributes.AddRange(value);
			}
			if (typeToRemoveBeforeAdd != null)
			{
				attributes.RemoveAll((Attribute a) => {
					if (a.GetType() == typeToRemoveBeforeAdd)
					{
						return true;
					}
					return a.GetType().IsSubclassOf(typeToRemoveBeforeAdd);
				});
			}
			attributes.Add(attribute);
			field.SetValue(ac, attributes.ToArray());
		}

		public static void AddRange(this AttributeCollection ac, Attribute[] attributes)
		{
			FieldInfo field = ac.GetType().GetField("_attributes", BindingFlags.Instance | BindingFlags.NonPublic);
			Attribute[] value = (Attribute[])field.GetValue(ac);
			List<Attribute> attributes1 = new List<Attribute>();
			if (value != null)
			{
				attributes1.AddRange(value);
			}
			attributes1.AddRange(attributes);
			field.SetValue(ac, attributes1.ToArray());
		}

		public static void Clear(this AttributeCollection ac)
		{
			ac.GetType().GetField("_attributes", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ac, null);
		}

		public static Attribute Get(this AttributeCollection ac, Attribute attribute)
		{
			Attribute[] value = (Attribute[])ac.GetType().GetField("_attributes", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ac);
			if (value == null)
			{
				return null;
			}
			return value.FirstOrDefault<Attribute>((Attribute a) => a.Match(attribute));
		}

		public static List<Attribute> Get(this AttributeCollection ac, params Attribute[] attributes)
		{
			Attribute[] value = (Attribute[])ac.GetType().GetField("_attributes", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ac);
			if (value == null)
			{
				return null;
			}
			List<Attribute> attributes1 = new List<Attribute>();
			attributes1.AddRange(value);
			AttributeCollection attributeCollections = new AttributeCollection(attributes);
			return attributes1.FindAll((Attribute a) => attributeCollections.Matches(a));
		}

		public static Attribute Get(this AttributeCollection ac, Type attributeType)
		{
			return ((Attribute[])ac.GetType().GetField("_attributes", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ac)).FirstOrDefault<Attribute>((Attribute a) => a.GetType() == attributeType);
		}

		public static Attribute Get(this AttributeCollection ac, Type attributeType, bool derivedType)
		{
			Attribute[] value = (Attribute[])ac.GetType().GetField("_attributes", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ac);
			Attribute attribute = null;
			attribute = (derivedType ? value.FirstOrDefault<Attribute>((Attribute a) => {
				if (a.GetType() == attributeType)
				{
					return true;
				}
				return a.GetType().IsSubclassOf(attributeType);
			}) : value.FirstOrDefault<Attribute>((Attribute a) => a.GetType() == attributeType));
			return attribute;
		}

		public static List<Attribute> Get(this AttributeCollection ac, params Type[] attributeTypes)
		{
			Attribute[] value = (Attribute[])ac.GetType().GetField("_attributes", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ac);
			if (value == null)
			{
				return null;
			}
			List<Attribute> attributes = new List<Attribute>();
			attributes.AddRange(value);
			return attributes.FindAll((Attribute a) => a.GetType() == attributeTypes.FirstOrDefault<Type>((Type b) => b.GetType() == a.GetType()));
		}

		public static void Remove(this AttributeCollection ac, Attribute attribute)
		{
			FieldInfo field = ac.GetType().GetField("_attributes", BindingFlags.Instance | BindingFlags.NonPublic);
			Attribute[] value = (Attribute[])field.GetValue(ac);
			List<Attribute> attributes = new List<Attribute>();
			if (value != null)
			{
				attributes.AddRange(value);
			}
			attributes.RemoveAll((Attribute a) => a.Match(attribute));
			field.SetValue(ac, attributes.ToArray());
		}

		public static void Remove(this AttributeCollection ac, Type type)
		{
			FieldInfo field = ac.GetType().GetField("_attributes", BindingFlags.Instance | BindingFlags.NonPublic);
			Attribute[] value = (Attribute[])field.GetValue(ac);
			List<Attribute> attributes = new List<Attribute>();
			if (value != null)
			{
				attributes.AddRange(value);
			}
			attributes.RemoveAll((Attribute a) => a.GetType() == type);
			field.SetValue(ac, attributes.ToArray());
		}

		public static Attribute[] ToArray(this AttributeCollection ac)
		{
			return (Attribute[])ac.GetType().GetField("_attributes", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ac);
		}
	}
}