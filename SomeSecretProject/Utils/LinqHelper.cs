using System;
using System.Collections.Generic;

namespace SomeSecretProject.Utils
{
	public static class LinqHelper
	{
		public static T MaxOrDefault<T>(this IEnumerable<T> enumerable, Func<T, int> selector)
		{
			T maxItem = default(T);
			int max = int.MinValue;
			foreach(var item in enumerable)
			{
				var value = selector(item);
				if(value > max)
				{
					maxItem = item;
					max = value;
				}
			}
			return maxItem;
		}
	}
}