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
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace GameDevWare.Serialization.Serializers
{
	public sealed class DateTimeOffsetSerializer : TypeSerializer
	{
		public override Type SerializedType { get { return typeof(DateTimeOffset); } }

		public override object Deserialize(IJsonReader reader)
		{
			if (reader.Token == JsonToken.DateTime)
				return new DateTimeOffset(reader.Value.AsDateTime, TimeSpan.Zero);

			var dateTimeOffsetStr = reader.ReadString(false);
			try
			{
				var value = default(DateTimeOffset);
				if (!DateTimeOffset.TryParse(dateTimeOffsetStr, reader.Context.Format, DateTimeStyles.RoundtripKind, out value))
					value = DateTimeOffset.ParseExact(dateTimeOffsetStr, reader.Context.DateTimeFormats, reader.Context.Format,
						DateTimeStyles.AdjustToUniversal);

				return value;
			}
			catch (FormatException fe)
			{
				throw new SerializationException(
					string.Format("Failed to parse date '{0}' in with pattern '{1}'.", dateTimeOffsetStr, reader.Context.DateTimeFormats[0]), fe);
			}
		}

		public override void Serialize(IJsonWriter writer, object valueObj)
		{
			var value = (DateTimeOffset)valueObj;

			var dateTimeFormat = Enumerable.FirstOrDefault(writer.Context.DateTimeFormats);
			var valueStr = value.ToString(dateTimeFormat, writer.Context.Format);
			writer.Write(valueStr);
		}
	}
}
