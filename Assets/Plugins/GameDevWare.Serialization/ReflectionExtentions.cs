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
using System.Reflection;

namespace Serialization.Json
{
	internal static class ReflectionExtentions
	{
		private static readonly Dictionary<Type, MethodInfo> GetNameMethods = new Dictionary<Type, MethodInfo>();
		private static readonly object[] EmptyArgs = new object[0];

		public static bool IsInstantiationOf(this Type type, Type openGenericType)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (openGenericType == null)
				throw new ArgumentNullException("openGenericType");

			if (openGenericType.IsGenericType && !openGenericType.IsGenericTypeDefinition)
				throw new ArgumentException($"Type should be open generic type '{openGenericType}'.");

			var genericType = type;
			if (type.IsGenericType)
			{
				if (type.IsGenericType && !type.IsGenericTypeDefinition)
					genericType = type.GetGenericTypeDefinition();

				if (genericType == openGenericType || genericType.IsSubclassOf(openGenericType))
					return true;
			}
			// clean
			genericType = null;

			// check interfaces
			foreach (var interfc in type.GetInterfaces())
			{
				genericType = interfc;

				if (!interfc.IsGenericType)
					continue;

				if (!interfc.IsGenericTypeDefinition)
					genericType = interfc.GetGenericTypeDefinition();

				if (genericType == openGenericType || genericType.IsSubclassOf(openGenericType))
					return true;
			}

			if (type.BaseType != null && type.BaseType != typeof (object))
				return IsInstantiationOf(type.BaseType, openGenericType);

			return false;
		}

		public static bool HasMultipleInstantiations(this Type type, Type openGenericType)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (openGenericType == null)
				throw new ArgumentNullException("openGenericType");

			if (openGenericType.IsGenericType && !openGenericType.IsGenericTypeDefinition)
				throw new ArgumentException($"Type should be open generic type '{openGenericType}'.");

			// can't has multiple implementations of class
			if (!openGenericType.IsInterface)
				return false;

			var found = 0;

			var genericType = type;
			if (type.IsGenericType)
			{
				if (type.IsGenericType && !type.IsGenericTypeDefinition)
					genericType = type.GetGenericTypeDefinition();

				if (genericType == openGenericType || genericType.IsSubclassOf(openGenericType))
					found++;
			}
			// clean
			genericType = null;

			// check interfaces
			foreach (var interfc in type.GetInterfaces())
			{
				genericType = interfc;

				if (!interfc.IsGenericType)
					continue;

				if (!interfc.IsGenericTypeDefinition)
					genericType = interfc.GetGenericTypeDefinition();

				if (genericType == openGenericType || genericType.IsSubclassOf(openGenericType))
					found++;
			}


			return found > 1;
		}

		public static Type[] GetInstantiationArguments(this Type type, Type openGenericType)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (openGenericType == null)
				throw new ArgumentNullException("openGenericType");

			if (openGenericType.IsGenericType && !openGenericType.IsGenericTypeDefinition)
				throw new ArgumentException($"Type should be open generic type '{openGenericType}'.");

			var genericType = type;
			if (type.IsGenericType)
			{
				if (type.IsGenericType && !type.IsGenericTypeDefinition)
					genericType = type.GetGenericTypeDefinition();

				if (genericType == openGenericType || genericType.IsSubclassOf(openGenericType))
					return type.GetGenericArguments();
			}

			// clean
			genericType = null;

			// check interfaces
			foreach (var _interface in type.GetInterfaces())
			{
				genericType = _interface;

				if (!_interface.IsGenericType)
					continue;

				if (!_interface.IsGenericTypeDefinition)
					genericType = _interface.GetGenericTypeDefinition();


				if (genericType == openGenericType || genericType.IsSubclassOf(openGenericType))
					return _interface.GetGenericArguments();
			}


			if (type.BaseType != null && type.BaseType != typeof (object))
				return GetInstantiationArguments(type.BaseType, openGenericType);

			return null;
		}

		public static string GetDataMemberName(object dataMemberAttribute)
		{
			if (dataMemberAttribute == null) throw new ArgumentNullException("dataMemberAttribute");

			var type = dataMemberAttribute.GetType();
			var getName = default(MethodInfo);

			lock (GetNameMethods)
			{
				if (!GetNameMethods.TryGetValue(type, out getName))
				{
					getName = type.GetMethod("get_Name", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);

					if (getName == null || getName.ReturnType != typeof (string) || getName.GetParameters().Length != 0)
						getName = null;

					if (getName == null)
					{
						var getNameProperty = type.GetProperty("Name", BindingFlags.Instance | BindingFlags.Public, null, typeof (string),
							Type.EmptyTypes, null);
						if (getNameProperty != null)
							getName = getNameProperty.GetGetMethod(nonPublic: false);
					}

					GetNameMethods.Add(type, getName);
				}
			}

			if (getName != null)
				return (string) getName.Invoke(dataMemberAttribute, EmptyArgs);
			else
				return null;
		}
	}
}