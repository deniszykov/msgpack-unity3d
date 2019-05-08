using System;
namespace GameDevWare.Serialization
{
	internal static class ArrayExtensions
	{
		public static OutputT[] ConvertAll<T, OutputT>(this T[] array, Func<T, OutputT> converter)
		{
			if (array == null) throw new ArgumentNullException(nameof(array));
			if (converter == null) throw new ArgumentNullException(nameof(converter));

			var newList = new OutputT[array.Length];
			var i = 0;
			foreach (var item in array)
			{
				newList[i++] = converter(item);
			}
			return newList;
		}
	}
}
