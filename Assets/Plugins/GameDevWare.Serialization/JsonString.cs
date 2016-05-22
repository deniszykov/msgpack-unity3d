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
using System.Diagnostics;
using System.Text;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization
{
	[DebuggerDisplay("{Length}")]
	public sealed class JsonString : IEquatable<JsonString>
	{
		private string jsonString;
		private StringBuilder jsonStringBuilder;

		public int Length
		{
			get { return jsonString != null ? jsonString.Length : jsonStringBuilder.Length; }
		}

		public JsonString()
		{
			jsonString = string.Empty;
		}
		public JsonString(string jsonString)
		{
			if (jsonString == null)
				throw new ArgumentNullException("jsonString");


			this.jsonString = jsonString;
		}
		public JsonString(StringBuilder jsonString)
		{
			if (jsonString == null)
				throw new ArgumentNullException("jsonString");


			jsonStringBuilder = jsonString;
		}
		public JsonString(char[] jsonString, int index, int length)
		{
			if (jsonString == null)
				throw new ArgumentNullException("jsonString");
			if (length < 0)
				throw new ArgumentOutOfRangeException("length");
			if (index < 0 || index + length > jsonString.Length)
				throw new ArgumentOutOfRangeException("index");


			jsonStringBuilder = new StringBuilder(length + 1);
			jsonStringBuilder.Append(jsonString, index, length);
		}
		public JsonString(string jsonString, int index, int length)
		{
			if (jsonString == null)
				throw new ArgumentNullException("jsonString");
			if (length < 0)
				throw new ArgumentOutOfRangeException("length");
			if (index < 0 || index + length > jsonString.Length)
				throw new ArgumentOutOfRangeException("index");

			jsonStringBuilder = new StringBuilder(length + 1);
			jsonStringBuilder.Append(jsonString, index, length);
		}
		public JsonString(string[] jsonStrings)
		{
			if (jsonStrings == null)
				throw new ArgumentNullException("jsonStrings");

			var length = 0;
			for (var i = 0; i < jsonStrings.Length; i++)
			{
				if (jsonStrings[i] == null)
					jsonStrings[i] = string.Empty;

				length += jsonStrings[i].Length;
			}

			jsonStringBuilder = new StringBuilder(length + 1);
			for (var i = 0; i < jsonStrings.Length; i++)
				jsonStringBuilder.Append(jsonStrings[i]);
		}

		private void EnsureStringValue()
		{
			if (jsonStringBuilder != null)
				jsonString = jsonStringBuilder.ToString();
			jsonStringBuilder = null;
		}

		public override int GetHashCode()
		{
			if (jsonString == null && jsonStringBuilder == null)
				throw new InvalidOperationException();


			return jsonStringBuilder != null ? jsonStringBuilder.GetHashCode() : jsonString.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			return this.Equals(obj as JsonString);
		}
		public bool Equals(JsonString jsString)
		{
			if (jsonString == null && jsonStringBuilder == null)
				throw new InvalidOperationException();


			if (ReferenceEquals(this, jsString))
				return true;

			if (jsString.jsonString == null && this.jsonString == null)
				return jsString.jsonStringBuilder.Equals(this.jsonStringBuilder);
			else
				return this.ToString().Equals(jsString.ToString());
		}
		public IJsonReader ToJsonReader(ISerializationContext context = null)
		{
			if (jsonString == null && jsonStringBuilder == null)
				throw new InvalidOperationException();


			if (jsonString != null)
				return new JsonStringReader(jsonString, context);
			else
				return new JsonStringBuilderReader(jsonStringBuilder, context);
		}

		public override string ToString()
		{
			if (jsonString == null && jsonStringBuilder == null)
				throw new InvalidOperationException();


			this.EnsureStringValue();

			return jsonString.ToString();
		}

		internal static string UnquoteAndUnescape(string stringToUnescape)
		{
			if (stringToUnescape == null)
				throw new ArgumentNullException("stringToUnescape");
			if (stringToUnescape.Length < 2)
				throw new ArgumentException("stringToUnescape");


			return JsonUtils.UnescapeBuffer(stringToUnescape.ToCharArray(), 1, stringToUnescape.Length - 2);
		}
		public static string Unescape(string stringToUnescape)
		{
			if (stringToUnescape == null)
				throw new ArgumentNullException("stringToUnescape");


			return JsonUtils.UnescapeBuffer(stringToUnescape.ToCharArray(), 0, stringToUnescape.Length);
		}
		public static string Escape(string stringToEscape)
		{
			if (stringToEscape == null)
				throw new ArgumentNullException("stringToEscape");


			return JsonUtils.EscapeAndQuote(stringToEscape);
		}
	}
}
