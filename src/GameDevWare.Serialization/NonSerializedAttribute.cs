#if NETSTANDARD
using System;

namespace GameDevWare.Serialization
{
	[AttributeUsage(AttributeTargets.All)]
	internal class NonSerializedAttribute : Attribute
	{
	}
}
#endif
