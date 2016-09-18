/*
	Copyright (c) 2016 Denis Zykov, GameDevWare.com

	This a part of "Json & MessagePack Serialization" Unity Asset - https://www.assetstore.unity3d.com/#!/content/59918

	THIS SOFTWARE IS DISTRIBUTED "AS-IS" WITHOUT ANY WARRANTIES, CONDITIONS AND
	REPRESENTATIONS WHETHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION THE
	IMPLIED WARRANTIES AND CONDITIONS OF MERCHANTABILITY, MERCHANTABLE QUALITY,
	FITNESS FOR A PARTICULAR PURPOSE, DURABILITY, NON-INFRINGEMENT, PERFORMANCE
	AND THOSE ARISING BY STATUTE OR FROM CUSTOM OR USAGE OF TRADE OR COURSE OF DEALING.

	This source code is distributed via Unity Asset Store,
	to use it in your project you should accept Terms of Service and EULA
	https://unity3d.com/ru/legal/as_terms
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Metadata
{
	internal abstract class MemberDescription
	{
		protected const string DATA_CONTRACT_ATTRIBUTE_NAME = "DataContractAttribute";
		protected const string DATA_MEMBER_ATTRIBUTE_NAME = "DataMemberAttribute";
		protected const string IGNORE_DATA_MEMBER_ATTRIBUTE_NAME = "IgnoreDataMemberAttribute";

		private readonly string name;
		private readonly MemberInfo member;
		private readonly ReadOnlyCollection<Attribute> attributes;
		private readonly ILookup<Type, Attribute> attributesByType;
		private readonly ILookup<string, Attribute> attributesByTypeName;

		public MemberInfo Member { get { return this.member; } }
		public IList<Attribute> Attributes { get { return this.attributes; } }
		public string Name { get { return this.name; } }

		protected MemberDescription(MemberInfo member)
		{
			if (member == null) throw new ArgumentNullException("member");

			this.member = member;
			this.name = member.Name;

			var dataMemberAttribute = member.GetCustomAttributes(false).FirstOrDefault(a => a.GetType().Name == DATA_MEMBER_ATTRIBUTE_NAME);
			if (dataMemberAttribute != null)
				this.name = ReflectionExtentions.GetDataMemberName(dataMemberAttribute) ?? this.name;

			var attrs = new List<Attribute>();
			foreach (Attribute attr in member.GetCustomAttributes(true))
				attrs.Add(attr);

			this.attributes = new ReadOnlyCollection<Attribute>(attrs);
			this.attributesByType = attrs.ToLookup(a => a.GetType());
			this.attributesByTypeName = attrs.ToLookup(a => a.GetType().Name);
		}

		public bool HasAttributes(string typeName)
		{
			if (typeName == null) throw new ArgumentNullException("typeName");

			return this.attributesByTypeName.Contains(typeName);
		}

		public bool HasAttributes(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");

			return this.attributesByType.Contains(type);
		}

		public IEnumerable<Attribute> GetAttributesOrEmptyList(string typeName)
		{
			if (typeName == null) throw new ArgumentNullException("typeName");

			var attrs = this.attributesByTypeName[typeName];
			if (attrs == null)
				return Enumerable.Empty<Attribute>();

			return attrs;
		}

		public IEnumerable<Attribute> GetAttributesOrEmptyList(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");

			var attrs = this.attributesByType[type];
			if (attrs == null)
				return Enumerable.Empty<Attribute>();

			return attrs;
		}
	}
}
