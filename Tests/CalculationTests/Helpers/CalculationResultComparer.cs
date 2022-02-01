using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Tests.CalculationTests.Helpers
{
    internal class CalculationResultComparer : IEqualityComparer<KeyValuePair<string, List<Point>>>
    {
        public bool Equals(KeyValuePair<string, List<Point>> x, KeyValuePair<string, List<Point>> y)
        {
            bool areNamesEqual = x.Key == y.Key;
            bool areValuesEqual = x.Value.SequenceEqual(y.Value);

            return areNamesEqual && areValuesEqual;
        }

        /// <summary>
        /// Needless for now
        /// </summary>
        public int GetHashCode(KeyValuePair<string, List<Point>> obj)
        {
            throw new NotImplementedException(
                $"{nameof(CalculationResultComparer.GetHashCode)} is not implemented yet.");
        }
    }
}
