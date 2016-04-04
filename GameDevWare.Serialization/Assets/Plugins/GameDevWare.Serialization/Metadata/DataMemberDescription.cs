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
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Serialization.Json.Metadata
{
	internal abstract class DataMemberDescription : MemberDescription
	{
		public abstract bool CanGet { get; }
		public abstract bool CanSet { get; }
		public object DefaultValue { get; private set; }
		public abstract Type ValueType { get; }

		protected DataMemberDescription(MemberInfo member)
			: base(member)
		{
			var defaultValue =
				(DefaultValueAttribute) this.GetAttributesOrEmptyList(typeof (DefaultValueAttribute)).FirstOrDefault();
			if (defaultValue != null)
				this.DefaultValue = defaultValue.Value;
		}

		public abstract object GetValue(object target);
		public abstract void SetValue(object target, object value);
	}
}