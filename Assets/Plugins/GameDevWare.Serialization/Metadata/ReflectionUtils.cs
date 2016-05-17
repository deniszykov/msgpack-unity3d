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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Metadata
{
	internal static class GettersAndSetters
	{
		private static readonly bool AotRuntime;
		private static readonly Dictionary<MemberInfo, Func<object, object>> ReadFunctions;
		private static readonly Dictionary<MemberInfo, Action<object, object>> WriteFunctions;
		private static readonly Dictionary<MemberInfo, Func<object>> ConstructorFunctions;

		static GettersAndSetters()
		{
			try { Expression.Lambda<Func<bool>>(Expression.Constant(true)).Compile(); }
			catch (Exception) { AotRuntime = true; }

			ReadFunctions = new Dictionary<MemberInfo, Func<object, object>>();
			WriteFunctions = new Dictionary<MemberInfo, Action<object, object>>();
			ConstructorFunctions = new Dictionary<MemberInfo, Func<object>>();
		}

		public static bool TryGetAssessors(MethodInfo getMethod, MethodInfo setMethod, out Func<object, object> getFn, out Action<object, object> setFn)
		{
			getFn = null;
			setFn = null;

			if (AotRuntime)
				return false;

			if (getMethod != null && !getMethod.IsStatic && getMethod.GetParameters().Length == 0)
			{
				lock (ReadFunctions)
				{
					if (ReadFunctions.TryGetValue(getMethod, out getFn) == false)
					{
						var instanceParam = Expression.Parameter(typeof(object), "instance");
						var declaringType = getMethod.DeclaringType;
						Debug.Assert(declaringType != null, "getMethodDeclaringType != null");
						getFn = Expression.Lambda<Func<object, object>>(
							Expression.Convert(
								Expression.Call(
									Expression.Convert(instanceParam, declaringType), getMethod),
									typeof(object)),
								instanceParam
						).Compile();
						ReadFunctions.Add(getMethod, getFn);
					}
				}
			}

			if (setMethod != null && !setMethod.IsStatic && setMethod.GetParameters().Length == 1)
			{
				lock (WriteFunctions)
				{
					if (WriteFunctions.TryGetValue(setMethod, out setFn) == false)
					{
						var declaringType = setMethod.DeclaringType;
						var valueParameter = setMethod.GetParameters().Single();
						Debug.Assert(declaringType != null, "getMethodDeclaringType != null");
						var setDynamicMethod = new DynamicMethod(declaringType.FullName + "::" + setMethod.Name, typeof(void), new Type[] { typeof(object), typeof(object) }, restrictedSkipVisibility: true);
						var il = setDynamicMethod.GetILGenerator();

						if (declaringType.IsValueType)
						{
							il.Emit(OpCodes.Ldarg_0); // instance
							il.Emit(OpCodes.Mkrefany, declaringType); // typed ref with instance
							il.Emit(OpCodes.Refanyval, declaringType); // &instance
							il.Emit(OpCodes.Ldarg_1); // value
							if (valueParameter.ParameterType.IsValueType)
								il.Emit(OpCodes.Unbox_Any, valueParameter.ParameterType);
							else
								il.Emit(OpCodes.Castclass, valueParameter.ParameterType);
							il.Emit(OpCodes.Callvirt, setMethod); // call instance.Set(value)
							il.Emit(OpCodes.Ret);
						}
						else
						{
							il.Emit(OpCodes.Ldarg_0); // instance
							il.Emit(OpCodes.Castclass, declaringType);
							il.Emit(OpCodes.Ldarg_1); // value
							if (valueParameter.ParameterType.IsValueType)
								il.Emit(OpCodes.Unbox_Any, valueParameter.ParameterType);
							else
								il.Emit(OpCodes.Castclass, valueParameter.ParameterType);
							il.Emit(OpCodes.Callvirt, setMethod); // call instance.Set(value)
							il.Emit(OpCodes.Ret);
						}
						setFn = (Action<object, object>)setDynamicMethod.CreateDelegate(typeof(Action<object, object>));
						WriteFunctions.Add(setMethod, setFn);
					}
				}
			}

			return true;
		}
		public static bool TryGetAssessors(FieldInfo fieldInfo, out Func<object, object> getFn, out Action<object, object> setFn)
		{
			getFn = null;
			setFn = null;

			if (AotRuntime || fieldInfo.IsStatic)
				return false;


			lock (ReadFunctions)
			{
				if (ReadFunctions.TryGetValue(fieldInfo, out getFn) == false)
				{
					var instanceParam = Expression.Parameter(typeof(object), "instance");
					var declaringType = fieldInfo.DeclaringType;
					Debug.Assert(declaringType != null, "getMethodDeclaringType != null");
					getFn = Expression.Lambda<Func<object, object>>(
						Expression.Convert(
							Expression.Field(
								Expression.Convert(instanceParam, declaringType),
								fieldInfo),
								typeof(object)),
							instanceParam
					).Compile();
					ReadFunctions.Add(fieldInfo, getFn);
				}
			}

			if (fieldInfo.IsInitOnly == false)
			{
				lock (WriteFunctions)
				{
					if (WriteFunctions.TryGetValue(fieldInfo, out setFn) == false)
					{
						var declaringType = fieldInfo.DeclaringType;
						var fieldType = fieldInfo.FieldType;
						Debug.Assert(declaringType != null, "getMethodDeclaringType != null");
						var setDynamicMethod = new DynamicMethod(declaringType.FullName + "::" + fieldInfo.Name, typeof(void), new Type[] { typeof(object), typeof(object) }, restrictedSkipVisibility: true);
						var il = setDynamicMethod.GetILGenerator();

						if (declaringType.IsValueType)
						{
							il.Emit(OpCodes.Ldarg_0); // instance
							il.Emit(OpCodes.Mkrefany, declaringType); // typed ref with instance
							il.Emit(OpCodes.Refanyval, declaringType); // &instance
							il.Emit(OpCodes.Ldarg_1); // value
							if (fieldType.IsValueType)
								il.Emit(OpCodes.Unbox_Any, fieldType);
							else
								il.Emit(OpCodes.Castclass, fieldType);
							il.Emit(OpCodes.Stfld, fieldInfo); // call instance.Set(value)
							il.Emit(OpCodes.Ret);
						}
						else
						{
							il.Emit(OpCodes.Ldarg_0); // instance
							il.Emit(OpCodes.Castclass, declaringType);
							il.Emit(OpCodes.Ldarg_1); // value
							if (fieldType.IsValueType)
								il.Emit(OpCodes.Unbox_Any, fieldType);
							else
								il.Emit(OpCodes.Castclass, fieldType);
							il.Emit(OpCodes.Stfld, fieldInfo); // call instance.Set(value)
							il.Emit(OpCodes.Ret);
						}
						setFn = (Action<object, object>)setDynamicMethod.CreateDelegate(typeof(Action<object, object>));
						WriteFunctions.Add(fieldInfo, setFn);
					}
				}
			}

			return true;
		}
		public static bool TryGetConstructor(Type type, out Func<object> ctrFn)
		{
			if (type == null) throw new ArgumentNullException("type");

			ctrFn = null;

			if (AotRuntime)
				return false;

			var defaultCtr = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(ctr => ctr.GetParameters().Length == 0);

			if (defaultCtr == null)
				return false;

			lock (ConstructorFunctions)
			{
				if (ConstructorFunctions.TryGetValue(type, out ctrFn))
					return true;

				ctrFn = Expression.Lambda<Func<object>>(
					Expression.Convert(
						Expression.New(defaultCtr),
						typeof(object))
					).Compile();

				ConstructorFunctions.Add(type, ctrFn);
			}

			return true;
		}
	}
}
