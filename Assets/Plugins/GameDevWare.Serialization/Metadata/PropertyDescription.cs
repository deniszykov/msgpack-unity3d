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
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Metadata
{
	internal sealed class PropertyDescription : DataMemberDescription
	{
		private readonly PropertyInfo propertyInfo;
		private readonly Func<object, object> getFn;
		private readonly Action<object, object> setFn;
		private readonly MethodInfo getMethod;
		private readonly MethodInfo setMethod;

		public override bool CanGet { get { return this.getMethod != null; } }
		public override bool CanSet { get { return this.setMethod != null; } }
		public override Type ValueType { get { return this.propertyInfo.PropertyType; } }

		public PropertyDescription(PropertyInfo propertyInfo)
			: base(propertyInfo)
		{
			if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");

			this.propertyInfo = propertyInfo;

			this.getMethod = propertyInfo.GetGetMethod(nonPublic: true);
			this.setMethod = propertyInfo.GetSetMethod(nonPublic: true);

			GettersAndSetters.TryGetAssessors(this.getMethod, this.setMethod, out this.getFn, out this.setFn);
		}

		public override object GetValue(object target)
		{
			if (!this.CanGet) throw new InvalidOperationException("Property is write-only.");

			if (this.getFn != null)
				return this.getFn(target);
			else
				return this.getMethod.Invoke(target, null);
		}
		public override void SetValue(object target, object value)
		{
			if (!this.CanSet) throw new InvalidOperationException("Property is read-only.");

			if (this.setFn != null)
				this.setFn(target, value);
			else
				this.setMethod.Invoke(target, new object[] { value });
		}
	}
}
