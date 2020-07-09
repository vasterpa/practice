using System;
using System.Collections.Generic;
using System.Numerics;

namespace Practice
{
    class Complex : IEquatable<Complex>, IComparable, IComparable<Complex>, ICloneable
    {
        #region Properties
        public Fraction Real { get; private set; }
        public Fraction Imaginary { get; private set; }
        #endregion

        #region Constructors
        public Complex()
        {
            Real = new Fraction();
            Imaginary = new Fraction();
        }

        public Complex(Fraction real)
        {
            Real = new Fraction(real);
            Imaginary = new Fraction();
        }

        public Complex(Fraction real, Fraction imaginary)
        {
            Real = new Fraction(real);
            Imaginary = new Fraction(imaginary);
        }

        public Complex(BigInteger real)
        {
            Real = new Fraction(real);
            Imaginary = new Fraction();
        }

        public Complex(BigInteger real, BigInteger imaginary)
        {
            Real = new Fraction(real);
            Imaginary = new Fraction(imaginary);
        }

        public Complex(Complex complex)
        {
            Real = new Fraction(complex.Real);
            Imaginary = new Fraction(complex.Imaginary);
        }
        #endregion

        #region Private
        private static BigInteger Fact(int n)
        {
            if (n < 0)
                throw new ArgumentException("It is impossible to calculate the factorial of a negative number!ы");

            BigInteger result = BigInteger.One;
            for (int i = 2; i <= n; i++)
                result *= i;
            return result;
        }

        private static Fraction GetTaylorSin(Fraction x)
        {
            const int iterations = 10;

            Fraction result = new Fraction();

            for (int n = 0; n <= iterations; n++)
                result += BigInteger.Pow(BigInteger.MinusOne, n) * (Fraction.Pow(x, 2 * n + 1) / Fact(2 * n + 1));


            return result;
        }

        private static Fraction GetTaylorCos(Fraction x)
        {
            const int iterations = 10;

            Fraction result = new Fraction();

            for (int n = 0; n <= iterations; n++)
                result += BigInteger.Pow(BigInteger.MinusOne, n) * (Fraction.Pow(x, 2 * n) / Fact(2 * n));

            return result;
        }

        private static Fraction GetTaylorArctan(Fraction x)
        {
            if (Fraction.Abs(x) > 1)
                throw new ArgumentException($"Argument absolute value must be less than one!");

            const int iterations = 10;

            Fraction result = new Fraction();

            for (int n = 1; n <= iterations; n++)
                result += BigInteger.Pow(BigInteger.MinusOne, n - 1) * (Fraction.Pow(x, 2 * n - 1) / (2 * n - 1));

            return result;
        }

        private static Fraction GetLeibnizPi()
        {
            const int iterations = 1000;

            Fraction result = new Fraction();

            Fraction one = new Fraction(1);

            for (int n = 0; n <= iterations; n++)
                result += (2 * one) / ((4 * n + 1) * (4 * n + 3));

            return result * 4;
        }
        #endregion

        #region Public
        public static Fraction Abs(Complex complex)
        {
            return Fraction.Sqrt(complex.Real * complex.Real + complex.Imaginary * complex.Imaginary);
        }

        public static Fraction Arg(Complex complex)
        {
            Fraction temp = complex.Imaginary / (complex.Real + Abs(complex));

            return 2 * GetTaylorArctan(temp) % (2 * GetLeibnizPi());
        }

        public static Array SqrtN(Complex complex, int n)
        {
            if (n < 2)
                throw new ArgumentException("The root of the nth degree must be greater than or equal to 2!");

            List<Complex> roots = new List<Complex>();

            Fraction absCoeff = Fraction.SqrtN(Abs(complex), n);

            for (int k = 0; k < n; k++)
            {
                Fraction real = absCoeff * GetTaylorCos((Arg(complex) + 2 * k * GetLeibnizPi()) / n);
                Fraction imaginary = absCoeff * GetTaylorSin((Arg(complex) + 2 * k * GetLeibnizPi()) / n);

                roots.Add(new Complex(real, imaginary));
            }

            return roots.ToArray();
        }

        public static Complex Pow(Complex complex, int exponent)
        {
            Fraction absCoeff = Fraction.Pow(Abs(complex), exponent);

            Fraction real = absCoeff * GetTaylorCos(exponent * Arg(complex));
            Fraction imaginary = absCoeff * GetTaylorSin(exponent * Arg(complex));

            return new Complex(real, imaginary);
        }
        #endregion

        #region Operators
        public static Complex UnaryMinus(Complex complex)
        {
            return new Complex(-complex.Real, complex.Imaginary);
        }
        public static Complex operator -(Complex complex)
        {
            return UnaryMinus(complex);
        }

        public static Complex UnaryPlus(Complex complex)
        {
            return complex;
        }
        public static Complex operator +(Complex complex)
        {
            return UnaryPlus(complex);
        }

        public static Complex Addition(Complex left, Complex right)
        {
            return new Complex(left.Real + right.Real, left.Imaginary + right.Imaginary);
        }
        public static Complex operator +(Complex left, Complex right)
        {
            return Addition(left, right);
        }

        public static Complex Substraction(Complex left, Complex right)
        {
            return Addition(left, -right);
        }
        public static Complex operator -(Complex left, Complex right)
        {
            return Substraction(left, right);
        }

        public static Complex Multiplication(Complex left, Complex right)
        {
            return new Complex(left.Real * right.Real - left.Imaginary * right.Imaginary,
                left.Real * right.Imaginary + right.Real * left.Imaginary);
        }
        public static Complex operator *(Complex left, Complex right)
        {
            return Multiplication(left, right);
        }

        public static Complex Division(Complex left, Complex right)
        {
            if ((right.Real == 0) && (right.Imaginary == 0))
                throw new DivideByZeroException("Can not divide by zero!");

            Fraction newReal = (left.Real * right.Real + left.Imaginary * right.Imaginary) /
                (right.Real * right.Real + right.Imaginary * right.Imaginary);
            Fraction newImaginary = (right.Real * left.Imaginary - left.Real * right.Imaginary) /
                (right.Real * right.Real + right.Imaginary * right.Imaginary);

            return new Complex(newReal, newImaginary);
        }
        public static Complex operator /(Complex left, Complex right)
        {
            return Division(left, right);
        }

        public static bool operator ==(Complex left, Complex right)
        {
            if (ReferenceEquals(left, null) && right is null)
                return true;
            else if (left is object && right is null)
                return left.Equals(right);
            else
                return right.Equals(left);
        }

        public static bool operator !=(Complex left, Complex right)
        {
            return !(left == right);
        }

        public static bool operator >(Complex left, Complex right)
        {
            if (left.CompareTo(right) == 1)
                return true;
            return false;
        }

        public static bool operator <(Complex left, Complex right)
        {
            if (left.CompareTo(right) == -1)
                return true;
            return false;
        }

        public static bool operator >=(Complex left, Complex right)
        {
            if (left.CompareTo(right) >= 0)
                return true;
            return false;
        }

        public static bool operator <=(Complex left, Complex right)
        {
            if (left.CompareTo(right) <= 0)
                return true;
            return false;
        }
        #endregion

        #region Overriden
        public override string ToString()
        {
            return $"[{Real.GetDecimalRepresentation(2)} + {Imaginary.GetDecimalRepresentation(2)}i]";
        }

        public override int GetHashCode()
        {
            return Real.GetHashCode() ^ Imaginary.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj is Complex)
                return Equals(obj as Complex);

            return false;
        }
        #endregion

        #region IEquatable<T>
        public bool Equals(Complex other)
        {
            if (other is null)
                return false;

            if ((Real == other.Real) && (Imaginary == other.Imaginary))
                return true;

            return false;
        }
        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            if (obj is null)
                return 1;

            if (obj is Complex)
                return CompareTo(obj as Complex);

            throw new ArgumentException("Object is not a Complex!");
        }
        #endregion

        #region IComparable<T>
        public int CompareTo(Complex other)
        {
            if (other is null)
                return 1;

            int realComparisonResult = Real.CompareTo(other.Real);
            if (realComparisonResult == 0)
                return Imaginary.CompareTo(other.Imaginary);
            return realComparisonResult;
        }
        #endregion

        #region ICloneable
        public object Clone()
        {
            return new Complex(this);
        }
        #endregion
    }
}
