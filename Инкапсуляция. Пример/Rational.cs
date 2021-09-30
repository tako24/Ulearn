using System;
using System.Numerics;

namespace Incapsulation.RationalNumbers
{
    public class Rational
    {
        int denominator;
        int numerator;
        public int Numerator { get => numerator; }
        public int Denominator { get => denominator; }
        public Rational(int numerator, int denominator = 1)
        {
            if (denominator < 0)
            {
                numerator *= -1;
            }
            this.numerator = numerator;
            this.denominator = Math.Abs(denominator);

            Reduce();
        }

        public bool IsNan
        {
            get => denominator == 0;
        }
        #region
        private void Reduce()
        {
            int gcf = (int)BigInteger.GreatestCommonDivisor(numerator, denominator);
            if (gcf == 0)
                return;
            numerator /= gcf;
            denominator /= gcf;
        }

        private static Rational GetNaN() => new Rational(0, 0);

        private static void SetToCommonDenominator(Rational a, Rational b)
        {
            if (a.denominator != b.denominator)
            {
                int common = a.denominator * b.denominator;
                int mn_a = b.denominator;
                int mn_b = a.denominator;
                a.denominator = common;
                a.numerator = a.numerator * mn_a;
                b.denominator = common;
                b.numerator = b.numerator * mn_b;
            }
        }
        #endregion
        #region
        public static implicit operator double(Rational a)
        {
            if (a.denominator == 0)
                return double.NaN;
            return a.numerator / (double)a.denominator;
        }

        public static implicit operator Rational(int r)
        {
            return new Rational(r);
        }

        public static explicit operator int(Rational a)
        {
            if (a.numerator % a.denominator != 0)
                throw new ArgumentException();
            if (a.denominator == 0)
                return 1;
            return a.numerator / a.denominator;
        }
        #endregion
        #region
        public static Rational operator *(Rational a, Rational b)
        {
            if (a.IsNan || b.IsNan)
                return GetNaN();
            return new Rational(a.numerator * b.numerator, a.denominator * b.denominator);
        }

        public static Rational operator /(Rational a, Rational b)
        {
            if (a.IsNan || b.IsNan)
                return GetNaN();
            if (a.denominator == 0 || b.denominator == 0)
                return new Rational(a.numerator * b.denominator, 0);
            else
                return new Rational(a.numerator * b.denominator, a.denominator * b.numerator);
        }

        public static Rational operator +(Rational a, Rational b)
        {
            if (a.IsNan || b.IsNan)
                return GetNaN();
            if (a.denominator == 0 || b.denominator == 0)
                return GetNaN();
            SetToCommonDenominator(a, b);
            return new Rational(a.numerator + b.numerator, a.denominator);
        }

        public static Rational operator -(Rational a, Rational b)
        {
            if (a.IsNan || b.IsNan)
                return GetNaN();
            if (a.denominator == 0 || b.denominator == 0)
                return GetNaN();
            SetToCommonDenominator(a, b);
            return new Rational(a.numerator - b.numerator, a.denominator);
        }
        #endregion
    }
}