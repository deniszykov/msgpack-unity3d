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
using GameDevWare.Serialization.Exceptions;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Metadata
{
	internal class TypeDescription : MemberDescription
	{
		private readonly Type objectType;
		private readonly Func<object> constructorFn;
		private readonly ReadOnlyCollection<DataMemberDescription> members;
		private readonly Dictionary<string, DataMemberDescription> membersByName;

		public Type ObjectType { get { return this.objectType; } }
		public ReadOnlyCollection<DataMemberDescription> Members { get { return this.members; } }

		public TypeDescription(Type objectType)
			: base(objectType)
		{
			if (objectType == null) throw new ArgumentNullException("objectType");

			this.objectType = objectType;

			var members = FindMembers(objectType);
			this.members = members.AsReadOnly();
			this.membersByName = members.ToDictionary(m => m.Name);

			GettersAndSetters.TryGetConstructor(objectType, out this.constructorFn);
		}

		private static List<DataMemberDescription> FindMembers(Type objectType)
		{
			if (objectType == null) throw new ArgumentNullException("objectType");

			var members = new List<DataMemberDescription>();
			var memberNames = new HashSet<string>();

			var isOptIn = objectType.GetCustomAttributes(false).Any(a => a.GetType().Name == DATA_CONTRACT_ATTRIBUTE_NAME);
			var publicProperties =
				objectType.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
			var publicFields = objectType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);

			foreach (var member in publicProperties.Cast<MemberInfo>().Concat(publicFields.Cast<MemberInfo>()))
			{
				if (member is PropertyInfo && (member as PropertyInfo).GetIndexParameters().Length != 0)
					continue;

				var dataMemberAttribute =
					member.GetCustomAttributes(false).FirstOrDefault(a => a.GetType().Name == DATA_MEMBER_ATTRIBUTE_NAME);
				var ignoreMemberAttribute =
					member.GetCustomAttributes(false).FirstOrDefault(a => a.GetType().Name == IGNORE_DATA_MEMBER_ATTRIBUTE_NAME);

				if (isOptIn && dataMemberAttribute == null)
					continue;
				else if (!isOptIn && ignoreMemberAttribute != null)
					continue;

				var dataMember = default(DataMemberDescription);
				if (member is PropertyInfo) dataMember = new PropertyDescription(member as PropertyInfo);
				else if (member is FieldInfo) dataMember = new FieldDescription(member as FieldInfo);
				else throw new InvalidOperationException("Unknown member type. Should be PropertyInfo or FieldInfo.");

				if (string.IsNullOrEmpty(dataMember.Name))
					throw new TypeContractViolation(objectType, "has no members with empty name");

				if (memberNames.Contains(dataMember.Name))
					throw new TypeContractViolation(objectType, string.Format("has no duplicate member's name ('{0}.{1}' and '{2}.{1}')", members.First(m => m.Name == dataMember.Name).Member.DeclaringType.Name, dataMember.Name, objectType.Name));

				members.Add(dataMember);
			}

			return members;
		}

		public bool TryGetMember(string name, out DataMemberDescription member)
		{
			return this.membersByName.TryGetValue(name, out member);
		}

		public object CreateInstance()
		{
			if (this.constructorFn != null)
				return this.constructorFn();
			else
				return Activator.CreateInstance(this.objectType);
		}

		public override string ToString()
		{
			return this.objectType.ToString();
		}
	}
}
