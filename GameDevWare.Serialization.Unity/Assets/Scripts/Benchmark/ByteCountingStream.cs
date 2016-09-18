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

 // ReSharper disable once CheckNamespace
namespace System.IO
{
	class ByteCountingStream : Stream
	{
		private long length;
		private long position;

		public override bool CanRead { get { return false; } }
		public override bool CanSeek { get { return true; } }
		public override bool CanWrite { get { return true; } }
		public override long Length { get { return length; } }
		public override long Position { get { return position; } set { this.Seek(value, SeekOrigin.Begin); } }

		public override void Flush()
		{

		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
		public override long Seek(long offset, SeekOrigin origin)
		{
			var value = offset;

			if (origin == SeekOrigin.End)
				this.position = this.length - offset;
			else if (origin == SeekOrigin.Current)
				this.position = this.position + offset;

			value = Math.Max(0, value);

			return (this.position = value);
		}
		public override void SetLength(long value)
		{
			this.length = value;
			if (this.position > value)
				this.position = value;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (count < 0)
				throw new ArgumentOutOfRangeException("count");

			this.position += count;

			if (this.position > this.length)
				this.length = this.position;
		}
	}
}
