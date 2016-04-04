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

namespace GameDevWare.Serialization.Metadata
{
	internal static class GettersAndSetters
	{
		private static readonly bool AotRuntime;
		private static readonly Dictionary<MemberInfo, Func<object, object>> ReadFunctions;
		private static readonly Dictionary<MemberInfo, Action<object, object>> WriteFunctions;

		static GettersAndSetters()
		{
			try { Expression.Lambda<Func<bool>>(Expression.Constant(true)).Compile(); }
			catch (Exception) { AotRuntime = true; }

			ReadFunctions = new Dictionary<MemberInfo, Func<object, object>>();
			WriteFunctions = new Dictionary<MemberInfo, Action<object, object>>();
		}

		public static bool TryGetAssessors(Type valueType, MethodInfo getMethod, MethodInfo setMethod, out Func<object, object> getFn, out Action<object, object> setFn)
		{
			if (valueType == null) throw new ArgumentNullException("valueType");

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
						var getMethodDeclaringType = getMethod.DeclaringType;
						Debug.Assert(getMethodDeclaringType != null, "getMethodDeclaringType != null");
						getFn = Expression.Lambda<Func<object, object>>(
							Expression.Convert(
								Expression.Call(
									Expression.Convert(instanceParam, getMethodDeclaringType), getMethod),
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
						var setMethodDeclaringType = setMethod.DeclaringType;
						var valueParameter = setMethod.GetParameters().Single();
						Debug.Assert(setMethodDeclaringType != null, "getMethodDeclaringType != null");
						var setDynamicMethod = new DynamicMethod(setMethodDeclaringType.FullName + "::" + setMethod.Name, typeof(void), new Type[] { typeof(object), typeof(object) }, restrictedSkipVisibility: true);
						var il = setDynamicMethod.GetILGenerator();
						il.Emit(OpCodes.Ldarg_0); // instance
						il.Emit(OpCodes.Castclass, setMethodDeclaringType);
						il.Emit(OpCodes.Ldarg_1); // value
						if (valueParameter.ParameterType.IsValueType)
							il.Emit(OpCodes.Unbox_Any, valueParameter.ParameterType);
						else
							il.Emit(OpCodes.Castclass, valueParameter.ParameterType);
						il.Emit(OpCodes.Callvirt, setMethod); // call instance.Set(value)
						il.Emit(OpCodes.Ret);
						setFn = (Action<object, object>)setDynamicMethod.CreateDelegate(typeof(Action<object, object>));
						WriteFunctions.Add(setMethod, setFn);
					}
				}
			}

			return true;
		}

	}
}