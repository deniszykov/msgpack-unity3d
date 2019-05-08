using System;
using System.Linq;
using System.Reflection;

namespace GameDevWare.Serialization
{
	internal static class TypeExtensions
	{
#if NET35
		public static Type GetTypeInfo(this Type type)
		{
			return type;
		}
#endif
#if NETSTANDARD
		public static MethodInfo GetGetMethod(this PropertyInfo property, bool nonPublic)
		{
			if (property == null) throw new ArgumentNullException("property");

			if (!nonPublic && property.GetMethod != null && !property.GetMethod.IsPublic)
			{
				return null;
			}
			return property.GetMethod;
		}
		public static MethodInfo GetSetMethod(this PropertyInfo property, bool nonPublic)
		{
			if (property == null) throw new ArgumentNullException("property");

			if (!nonPublic && property.SetMethod != null && !property.SetMethod.IsPublic)
			{
				return null;
			}
			return property.SetMethod;
		}
		public static Type[] GetGenericArguments(this TypeInfo type)
		{
			if (type == null) throw new ArgumentNullException("type");

			return type.GenericTypeArguments;
		}
		public static ConstructorInfo GetConstructor(this TypeInfo type, params Type[] paramTypes)
		{
			if (type == null) throw new ArgumentNullException("type");

			foreach (var ctr in type.DeclaredConstructors)
			{
				var parameters = ctr.GetParameters();
				if (parameters.Length == 0 && paramTypes.Length == 0)
				{
					return ctr;
				}

				if (parameters.Select(p => p.ParameterType).SequenceEqual(paramTypes))
				{
					return ctr;
				}
			}

			return null;
		}
#endif
	}
}
