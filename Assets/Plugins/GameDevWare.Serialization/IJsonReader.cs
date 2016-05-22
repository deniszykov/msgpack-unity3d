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
using System.IO;
using System.Text;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization
{
	public interface IJsonReader
	{
		ISerializationContext Context { get; }

		JsonToken Token { get; }
		object RawValue { get; }
		IValueInfo Value { get; }
		long CharactersReaded { get; }

		bool NextToken();

		bool IsEndOfStream();

		/// <summary>
		///     Resets Line/Column numbers, CharactersReaded and Token information of reader
		/// </summary>
		void Reset();
	}

	public interface IValueInfo
	{
		bool HasValue { get; }
		object Raw { get; }
		Type ProbableType { get; }
		bool AsBoolean { get; }
		byte AsByte { get; }
		short AsInt16 { get; }
		int AsInt32 { get; }
		long AsInt64 { get; }
		sbyte AsSByte { get; }
		ushort AsUInt16 { get; }
		uint AsUInt32 { get; }
		ulong AsUInt64 { get; }
		float AsSingle { get; }
		double AsDouble { get; }
		decimal AsDecimal { get; }
		string AsString { get; }
		DateTime AsDateTime { get; }

		int LineNumber { get; }
		int ColumnNumber { get; }

		void CopyJsonTo(StringBuilder stringBuilder);
		void CopyJsonTo(TextWriter writer);
		void CopyJsonTo(Stream stream);
		void CopyJsonTo(IJsonWriter writer);
	}
}
