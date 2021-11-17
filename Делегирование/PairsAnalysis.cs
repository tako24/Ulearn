using System;
using System.Collections.Generic;
using System.Linq;

namespace Delegates.PairsAnalysis
{
    public static class Analysis
    {
        public static int FindMaxPeriodIndex(params DateTime[] data)
        {
            if (data.Count() == 0)
                throw new InvalidOperationException();
            return data
                .Pairs()
                .Select((pair) => (pair.Item2 - pair.Item1).TotalSeconds)
                .MaxIndex();
        }

        public static double FindAverageRelativeDifference(params double[] data)
        {
            if (data.Count() <= 1)
                throw new InvalidOperationException();
            var sequenceRelativeDifferences = data
                .Pairs()
                .Select((pair) => (pair.Item2 - pair.Item1) / pair.Item1);
            return sequenceRelativeDifferences.Sum() / sequenceRelativeDifferences.Count();
        }
    }
	
    public static class Extentions
    {
        public static IEnumerable<Tuple<T, T>> Pairs<T>(this IEnumerable<T> sequence)
        {
            var isFirstElement = true;
            var previous = default(T);
            foreach (var val in sequence)
            {
                if (isFirstElement)
                {
                    previous = val;
                    isFirstElement = false;
                    continue;
                }
                yield return Tuple.Create(previous, val);
                previous = val;
            }
        }

        public static int MaxIndex<T>(this IEnumerable<T> data)
    where T : IComparable<T>
        {
            return data.Select((x, y) => Tuple.Create(x, y))
                .OrderByDescending(x => x.Item1)
                .First()
                .Item2;
        }
    }
}