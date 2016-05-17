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
	internal sealed class FieldDescription : DataMemberDescription
	{
		private readonly FieldInfo fieldInfo;
		private readonly Func<object, object> getFn;
		private readonly Action<object, object> setFn;

		public override bool CanGet { get { return true; } }
		public override bool CanSet { get { return this.fieldInfo.IsInitOnly == false; } }
		public override Type ValueType { get { return this.fieldInfo.FieldType; } }

		public FieldDescription(FieldInfo fieldInfo)
			: base(fieldInfo)
		{
			if (fieldInfo == null) throw new ArgumentNullException("fieldInfo");

			this.fieldInfo = fieldInfo;

			GettersAndSetters.TryGetAssessors(fieldInfo, out this.getFn, out this.setFn);

		}

		public override object GetValue(object target)
		{
			if (!this.CanGet) throw new InvalidOperationException("Field is write-only.");

			if (this.getFn != null)
				return this.getFn(target);
			else
				return fieldInfo.GetValue(target);
		}

		public override void SetValue(object target, object value)
		{
			if (!this.CanSet) throw new InvalidOperationException("Field is read-only.");

			if (this.setFn != null)
				this.setFn(target, value);
			else
				this.fieldInfo.SetValue(target, value);
		}
	}
}
