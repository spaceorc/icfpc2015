using System;
using System.Collections.Generic;

namespace SomeSecretProject.Algorithm
{
	public static class EnumerableExtensions
	{
		public static T ArgMax<T>(this IEnumerable<T> items, Func<T, double> selector)
		{
			var maxValue = Double.MinValue;
			T maxArg = default(T);
			foreach (var arg in items)
			{
				var value = selector(arg);
				if (value > maxValue)
				{
					maxValue = value;
					maxArg = arg;
				}
			}
			return maxArg;
		}
	}
}