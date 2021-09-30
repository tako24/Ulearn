using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.Weights
{
    class Indexer
    {
        readonly double[] array;
        public int Length { get; }
        public int Start { get; }
        public Indexer(double[] array, int start, int length)
        {
            if (start < 0 || length < 0 || length > array.Length || length > array.Length - start)
                throw new ArgumentException();
            Start = start;
            Length = length;
            this.array = array;
        }

        public double this[int index]
        {
            get
            {
                if (index < 0 || index > Length - 1)
                    throw new IndexOutOfRangeException();
                return array[Start + index];
            }
            set
            {
                if (index < 0 || index > Length - 1)
                    throw new IndexOutOfRangeException();
                array[Start + index] = value;
            }
        }
    }
}
