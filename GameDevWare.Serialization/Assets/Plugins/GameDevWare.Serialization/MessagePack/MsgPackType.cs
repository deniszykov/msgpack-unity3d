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
namespace Serialization.Json.MessagePack
{
	public enum MsgPackType : byte
	{
		PositiveFixIntStart = 0x00,
		PositiveFixIntEnd = 0x7f,
		FixMapStart = 0x80,
		FixMapEnd = 0x8f,
		FixArrayStart = 0x90,
		FixArrayEnd = 0x9f,
		FixStrStart = 0xa0,
		FixStrEnd = 0xbf,
		Nil = 0xc0,
		Unused = 0xc1,
		False = 0xc2,
		True = 0xc3,
		Bin8 = 0xc4,
		Bin16 = 0xc5,
		Bin32 = 0xc6,
		Ext8 = 0xc7,
		Ext16 = 0xc8,
		Ext32 = 0xc9,
		Float32 = 0xca,
		Float64 = 0xcb,
		UInt8 = 0xcc,
		UInt16 = 0xcd,
		UInt32 = 0xce,
		UInt64 = 0xcf,
		Int8 = 0xd0,
		Int16 = 0xd1,
		Int32 = 0xd2,
		Int64 = 0xd3,
		FixExt1 = 0xd4,
		FixExt2 = 0xd5,
		FixExt4 = 0xd6,
		FixExt8 = 0xd7,
		FixExt16 = 0xd8,
		Str8 = 0xd9,
		Str16 = 0xda,
		Str32 = 0xdb,
		Array16 = 0xdc,
		Array32 = 0xdd,
		Map16 = 0xde,
		Map32 = 0xdf,
		NegativeFixIntStart = 0xe0,
		NegativeFixIntEnd = 0xff
	}
}