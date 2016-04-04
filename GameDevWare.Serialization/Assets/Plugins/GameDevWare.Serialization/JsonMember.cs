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
using System.Linq;

namespace Serialization.Json
{
	public struct JsonMember : IEquatable<JsonMember>, IEquatable<string>
	{
		internal string NameString;
		internal char[] NameChars;
		internal bool IsEscapedAndQuoted;

		public int Length
		{
			get { return this.NameString != null ? this.NameString.Length : this.NameChars.Length; }
		}

		public JsonMember(string name)
			: this(name, false)
		{
		}

		public JsonMember(string name, bool escapedAndQuoted)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			this.NameString = name;
			this.IsEscapedAndQuoted = escapedAndQuoted;
			this.NameChars = null;
		}

		public JsonMember(char[] name)
			: this(name, false)
		{
		}

		public JsonMember(char[] name, bool escapedAndQuoted)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			this.NameChars = name;
			this.IsEscapedAndQuoted = escapedAndQuoted;
			this.NameString = null;
		}

		public override int GetHashCode()
		{
			return this.NameString != null ? this.NameString.GetHashCode() : this.NameChars.Aggregate(0, (a, c) => a ^ (int) c);
		}

		public override bool Equals(object obj)
		{
			if (obj is JsonMember)
				return this.Equals((JsonMember) obj);
			else if (obj is string)
				return this.Equals((string) obj);
			else
				return false;
		}

		public bool Equals(JsonMember other)
		{
			return this.ToString().Equals(other.ToString(), StringComparison.Ordinal);
		}

		public bool Equals(string other)
		{
			return this.ToString().Equals(other, StringComparison.Ordinal);
		}

		public static explicit operator string(JsonMember member)
		{
			return member.ToString();
		}

		public static explicit operator JsonMember(string memberName)
		{
			return new JsonMember(memberName);
		}

		public override string ToString()
		{
			var name = NameString;
			if (this.NameChars != null)
				name = new string(NameChars, 0, NameChars.Length);

			// this is used in tests, so perf is not primary
			if (this.IsEscapedAndQuoted)
			{
				if (name.EndsWith(":"))
					name = name.Substring(0, name.Length - 1);

				name = JsonUtils.UnescapeAndUnquote(name);
			}

			return name;
		}
	}
}