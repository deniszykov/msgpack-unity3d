/* 
Copyright (c) 2016 Denis Zykov, GameDevWare.com

https://www.assetstore.unity3d.com/#!/content/56706

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Serialization.Json.Metadata
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
				name = ReflectionExtentions.GetDataMemberName(dataMemberAttribute) ?? name;

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