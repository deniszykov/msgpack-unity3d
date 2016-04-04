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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GameDevWare.Serialization.Serializers;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization
{
	public sealed class DefaultSerializationContext : ISerializationContext
	{
		private readonly Stack hierarchy;
		private readonly Dictionary<Type, TypeSerializer> serializers;

		public Stack Hierarchy
		{
			get { return this.hierarchy; }
		}

		public IFormatProvider Format { get; set; }
		public string[] DateTimeFormats { get; set; }
		public Encoding Encoding { get; set; }

		public Dictionary<Type, TypeSerializer> Serializers
		{
			get { return this.serializers; }
			set
			{
				if (value == null) throw new ArgumentNullException("value");
				foreach (var kv in value)
					this.serializers[kv.Key] = kv.Value;
			}
		}

		public SerializationOptions Options { get; set; }

		public Func<Type, TypeSerializer> ObjectSerializerFactory { get; set; }
		public Func<Type, TypeSerializer> EnumSerializerFactory { get; set; }
		public Func<Type, TypeSerializer> DictionarySerializerFactory { get; set; }
		public Func<Type, TypeSerializer> ArraySerializerFactory { get; set; }

		public DefaultSerializationContext()
		{
			this.hierarchy = new Stack();

			this.Format = Json.DefaultFormat;
			this.DateTimeFormats = Json.DefaultDateTimeFormats;
			this.Encoding = Json.DefaultEncoding;
			this.serializers = Json.DefaultSerializers.ToDictionary(s => s.SerializedType);
		}

		public TypeSerializer GetSerializerForType(Type valueType)
		{
			if (valueType == null) throw new ArgumentNullException("valueType");

			if (valueType.BaseType == typeof (MulticastDelegate) || valueType.BaseType == typeof (Delegate))
				throw new InvalidOperationException(string.Format("Unable to serialize delegate type '{0}'.", valueType));

			var serializer = default(TypeSerializer);
			if (this.serializers.TryGetValue(valueType, out serializer))
				return serializer;

			var typeSerializerAttribute =
				valueType.GetCustomAttributes(typeof (TypeSerializerAttribute), inherit: false).FirstOrDefault() as
					TypeSerializerAttribute;
			if (typeSerializerAttribute != null)
				this.serializers.Add(valueType, serializer = this.CreateCustomSerializer(valueType, typeSerializerAttribute));
			else if (valueType.IsEnum)
				this.serializers.Add(valueType, serializer = this.CreateEnumSerializer(valueType));
			else if (typeof (IDictionary).IsAssignableFrom(valueType) &&
			         (!valueType.IsInstanceOfType(typeof (IDictionary<,>)) ||
			          DictionarySerializer.IsStringKeyType(valueType.GetInstantiationArguments(typeof (IDictionary<,>))[0])))
				this.serializers.Add(valueType, serializer = this.CreateDictionarySerializer(valueType));
			else if (valueType.IsArray || typeof (IEnumerable).IsAssignableFrom(valueType))
				this.serializers.Add(valueType, serializer = this.CreateArraySerializer(valueType));
			else
				this.serializers.Add(valueType, serializer = this.CreateObjectSerializer(valueType));

			return serializer;
		}

		private TypeSerializer CreateDictionarySerializer(Type valueType)
		{
			if (this.DictionarySerializerFactory != null)
				return this.DictionarySerializerFactory(valueType);
			else
				return new DictionarySerializer(valueType);
		}

		private TypeSerializer CreateEnumSerializer(Type valueType)
		{
			if (this.EnumSerializerFactory != null)
				return this.EnumSerializerFactory(valueType);
			else
				return new EnumSerializer(valueType);
		}

		private TypeSerializer CreateArraySerializer(Type valueType)
		{
			if (this.ArraySerializerFactory != null)
				return this.ArraySerializerFactory(valueType);
			else
				return new ArraySerializer(valueType);
		}

		private TypeSerializer CreateObjectSerializer(Type valueType)
		{
			if (this.ObjectSerializerFactory != null)
				return this.ObjectSerializerFactory(valueType);
			else
				return new ObjectSerializer(this, valueType);
		}

		private TypeSerializer CreateCustomSerializer(Type valueType, TypeSerializerAttribute typeSerializerAttribute)
		{
			var serializerType = typeSerializerAttribute.SerializerType;

			var typeCtr = serializerType.GetConstructor(new[] {typeof (Type)});
			if (typeCtr != null)
				return (TypeSerializer) typeCtr.Invoke(new object[] {valueType});

			var ctxTypeCtr = serializerType.GetConstructor(new[] {typeof (ISerializationContext), typeof (Type)});
			if (ctxTypeCtr != null)
				return (TypeSerializer) ctxTypeCtr.Invoke(new object[] {this, valueType});

			var ctxCtr = serializerType.GetConstructor(new[] {typeof (ISerializationContext)});
			if (ctxCtr != null)
				return (TypeSerializer) ctxCtr.Invoke(new object[] {this});

			return (TypeSerializer) Activator.CreateInstance(serializerType);
		}

		public Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			return Type.GetType(name, throwOnError, ignoreCase);
		}

		public Type GetType(string name, bool throwOnError)
		{
			return Type.GetType(name, throwOnError);
		}

		public Type GetType(string name)
		{
			return Type.GetType(name);
		}

		#region NotSupported

		public Assembly GetAssembly(AssemblyName name, bool throwOnError)
		{
			throw new NotSupportedException();
		}

		public Assembly GetAssembly(AssemblyName name)
		{
			throw new NotSupportedException();
		}

		public string GetPathOfAssembly(AssemblyName name)
		{
			throw new NotSupportedException();
		}

		public void ReferenceAssembly(AssemblyName name)
		{
			throw new NotSupportedException();
		}

		#endregion
	}
}
