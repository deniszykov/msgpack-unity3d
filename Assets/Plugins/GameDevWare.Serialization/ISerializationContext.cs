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
using System.ComponentModel.Design;
using System.Text;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization
{
	public interface ISerializationContext : ITypeResolutionService
	{
		/// <summary>
		///     Format for serialization/deserialization of primitive values. Default to
		///     System.Globalization.CultureInfo.InvariantCulture
		/// </summary>
		IFormatProvider Format { get; }

		/// <summary>
		///     Date and time formats for serialization/deserialization of DateTime values Default to
		///     GameDevWare.Serialization.Json.DateTimeFormat
		/// </summary>
		/// <see cref="GameDevWare.Serialization.Json.DateTimeFormat" />
		string[] DateTimeFormats { get; }

		/// <summary>
		///     Encdoing used for binary/text convetion. Default is UTF-8
		/// </summary>
		Encoding Encoding { get; }

		/// <summary>
		///     Write object's type in every serialized object
		/// </summary>
		SerializationOptions Options { get; }

		/// <summary>
		///     Get objects hierarchy during serialization process.
		/// </summary>
		Stack Hierarchy { get; }

		TypeSerializer GetSerializerForType(Type valueType);
	}
}
