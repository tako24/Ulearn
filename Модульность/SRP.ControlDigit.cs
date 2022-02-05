using System;
using System.Collections.Generic;
using System.Linq;

namespace SRP.ControlDigit
{
    public static class Extensions
    {
        public static int SumOfMultiplyingDigit(this long number, int factor, int sum =0)
        {
            do
            {
                sum += factor * (int)(number % 10);
                factor += 1; ;
                number /= 10;
            }
            while (number > 0);
            return sum;
        }

        public static int SumOfDigitsMultiplyedByFunc(this long number
            , Func<int, bool, int> change, int sum=0, bool isEven = true)
        {
            do
            {
                sum += change((int)(number % 10), isEven);
                number /= 10;
                isEven = !isEven;
            }
            while (number > 0);
            return sum;
        }

        public static int CheckSum(this int sum, int module)
        {
            if (sum % module == 0)
                return 0;
            return module - sum % module;
        }
    }

    public static class ControlDigitAlgo
    {
        public static int Isbn10(long number)
        {
            var temp = number.SumOfMultiplyingDigit(2).CheckSum(11);
            return temp == 10 ? 'X' : '0' + temp;
        }

        public static int Upc(long number)
        {
            return number.SumOfDigitsMultiplyedByFunc((i, isEven) =>
            {
                return isEven ? i * 3 : i;
            }).CheckSum(10);
        }

        public static int Luhn(long number)
        {
            var controlDigit = number.SumOfDigitsMultiplyedByFunc((i, isEven) =>
            {
                if (isEven)
                    i = i * 2 > 9 ? i * 2 - 9 : i * 2;
                return i;
            }).CheckSum(10);
            return controlDigit;
        }
    }
}
