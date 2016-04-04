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

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization
{
	public interface IJsonWriter
	{
		ISerializationContext Context { get; }

		long CharactersWritten { get; }

		void Flush();

		void Write(string value);
		void Write(JsonMember value);
		void Write(int number);
		void Write(uint number);
		void Write(long number);
		void Write(ulong number);
		void Write(float number);
		void Write(double number);
		void Write(decimal number);
		void Write(bool value);
		void Write(DateTime datetime);
		void WriteObjectBegin(int size);
		void WriteObjectEnd();
		void WriteArrayBegin(int size);
		void WriteArrayEnd();
		void WriteNull();

		void WriteJson(string jsonString);
		void WriteJson(char[] jsonString, int index, int charCount);

		void Reset();
	}
}
