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
