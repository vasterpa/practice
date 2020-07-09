using System;
using System.Numerics;
using System.Text;

namespace Practice
{
    public sealed class Fraction : IEquatable<Fraction>, IComparable, IComparable<Fraction>, ICloneable
    {
        #region Properties
        public BigInteger Numerator { get; private set; }
        public BigInteger Denominator { get; private set; }
        #endregion

        #region Constructors
        public Fraction()
        {
            Numerator = BigInteger.Zero;
            Denominator = BigInteger.One;
        }

        public Fraction(BigInteger numerator)
        {
            Numerator = numerator;
            Denominator = BigInteger.One;
        }

        public Fraction(BigInteger numerator, BigInteger denominator)
        {
            if (denominator == BigInteger.Zero)
                throw new ArgumentException("The denominator must be nonzero!");

            Numerator = numerator;
            Denominator = denominator;

            Reduce();
        }

        public Fraction(Fraction fraction)
        {
            Numerator = fraction.Numerator;
            Denominator = fraction.Denominator;
        }
        #endregion

        #region Private
        private Fraction Reduce()
        {
            BigInteger gcd = BigInteger.GreatestCommonDivisor(Numerator, Denominator);

            Numerator /= gcd;
            Denominator /= gcd;

            if (Denominator.Sign == -1)
            {
                Numerator = BigInteger.Negate(Numerator);
                Denominator = BigInteger.Negate(Denominator);
            }

            return this;
        }

        private Fraction Reciprocate()
        {
            if (Numerator == BigInteger.Zero)
                return this;

            return new Fraction(Denominator, Numerator);
        }

        private void ToDenominator(BigInteger targetDenominator)
        {
            if ((targetDenominator < Denominator) || (targetDenominator % Denominator != 0))
                return;

            if (Denominator != targetDenominator)
            {
                BigInteger factor = targetDenominator / Denominator;
                Numerator *= factor;
                Denominator = targetDenominator;
            }
        }

        private static BigInteger GetLeastCommonMultiple(BigInteger first, BigInteger second)
        {
            return (first * second) / BigInteger.GreatestCommonDivisor(first, second);
        }
        #endregion

        #region Public
        public static Fraction Abs(Fraction fraction)
        {
            return new Fraction(BigInteger.Abs(fraction.Numerator), BigInteger.Abs(fraction.Denominator));
        }

        public static Fraction Pow(Fraction fraction, int exponent)
        {
            Fraction result = fraction.Clone() as Fraction;

            if (exponent < 0)
            {
                result.Reciprocate();
                exponent = -exponent;
            }

            result.Numerator = BigInteger.Pow(result.Numerator, exponent);
            result.Denominator = BigInteger.Pow(result.Denominator, exponent);

            return result.Reduce();
        }

        public static Fraction Sqrt(Fraction fraction)
        {
            if (fraction < 0)
                throw new ArgumentException("Unable to extract square root from negative fraction!");

            if (fraction == 0 || fraction == 1)
                return fraction;

            Fraction eps = new Fraction(1, 100000);

            Fraction A0 = fraction / 2;
            Fraction A1 = (A0 + fraction / A0) / 2;

            while (Abs(A1 - A0) > eps)
            {
                A0 = A1;
                A1 = (A0 + fraction / A0) / 2;
            }

            return A1.Reduce();
        }

        public static Fraction SqrtN(Fraction fraction, int n)
        {
            if ((fraction < 0) && (n % 2 == 0))
                throw new ArgumentException("It is not possible to isolate the even root of a negative fraction!");

            if (n < 2)
                throw new ArgumentException("The root of the nth degree must be greater than or equal to 2!");

            if (fraction == 0 || fraction == 1)
                return fraction;

            Fraction eps = new Fraction(1, 1000);

            Fraction A0 = fraction / n;
            Fraction A1 = ((n - 1) * A0 + fraction / Pow(A0, n - 1)) / n;

            while (Abs(A1 - A0) > eps)
            {
                A0 = A1;
                A1 = ((n - 1) * A0 + fraction / Pow(A0, n - 1)) / n;
            }

            return A1.Reduce();
        }

        public string GetDecimalRepresentation(int decimals = 28)
        {
            if (decimals < 0)
                throw new ArgumentException("Decimals can not be less than Zero!");

            BigInteger numerator = BigInteger.Abs(Numerator);
            BigInteger denumerator = BigInteger.Abs(Denominator);

            BigInteger quotient = BigInteger.DivRem(numerator, denumerator, out BigInteger remainder);
            StringBuilder sb = new StringBuilder();

            if (this < 0)
                sb.Append('-');

            sb.Append(quotient.ToString());

            if (decimals != 0)
                sb.Append('.');

            for (int i = 0; i < decimals; i++)
            {
                remainder *= 10;
                sb.Append(BigInteger.DivRem(remainder, denumerator, out remainder));
            }

            return sb.ToString();
        }
        #endregion

        #region Operators
        public static Fraction UnaryPlus(Fraction fraction)
        {
            return fraction;
        }
        public static Fraction operator +(Fraction fraction)
        {
            return UnaryPlus(fraction);
        }

        public static Fraction UnaryNegation(Fraction fraction)
        {
            return new Fraction(BigInteger.Negate(fraction.Numerator), fraction.Denominator);
        }
        public static Fraction operator -(Fraction fraction)
        {
            return UnaryNegation(fraction);
        }

        public static Fraction Addition(Fraction left, Fraction right)
        {
            BigInteger lcm = GetLeastCommonMultiple(left.Denominator, right.Denominator);

            left.ToDenominator(lcm);
            right.ToDenominator(lcm);

            return new Fraction(left.Numerator + right.Numerator, lcm).Reduce();
        }
        public static Fraction operator +(Fraction left, Fraction right)
        {
            return Addition(left, right);
        }
        public static Fraction operator +(Fraction left, BigInteger right)
        {
            return Addition(left, new Fraction(right));
        }
        public static Fraction operator +(BigInteger left, Fraction right)
        {
            return Addition(new Fraction(left), right);
        }

        public static Fraction Substraction(Fraction left, Fraction right)
        {
            return Addition(left, -right);
        }
        public static Fraction operator -(Fraction left, Fraction right)
        {
            return Substraction(left, right);
        }
        public static Fraction operator -(Fraction left, BigInteger right)
        {
            return Substraction(left, new Fraction(right));
        }
        public static Fraction operator -(BigInteger left, Fraction right)
        {
            return Substraction(new Fraction(left), right);
        }

        public static Fraction Multiplication(Fraction left, Fraction right)
        {
            BigInteger newNumerator = left.Numerator * right.Numerator;
            BigInteger newDenominator = left.Denominator * right.Denominator;

            return new Fraction(newNumerator, newDenominator).Reduce();
        }
        public static Fraction operator *(Fraction left, Fraction right)
        {
            return Multiplication(left, right);
        }
        public static Fraction operator *(Fraction left, BigInteger right)
        {
            return Multiplication(left, new Fraction(right));
        }
        public static Fraction operator *(BigInteger left, Fraction right)
        {
            return Multiplication(new Fraction(left), right);
        }

        public static Fraction Division(Fraction left, Fraction right)
        {
            if (right == 0)
                throw new DivideByZeroException("Can not divide by Zero!");

            return Multiplication(left, right.Reciprocate());
        }
        public static Fraction operator /(Fraction left, Fraction right)
        {
            return Division(left, right);
        }
        public static Fraction operator /(Fraction left, BigInteger right)
        {
            return Division(left, new Fraction(right));
        }
        public static Fraction operator /(BigInteger left, Fraction right)
        {
            return Division(new Fraction(left), right);
        }

        public static Fraction Modulus(Fraction left, Fraction right)
        {
            if (right == 0)
                throw new DivideByZeroException("Can not divide by Zero!");

            while (left >= right)
                left -= right;

            return left;
        }
        public static Fraction operator %(Fraction left, Fraction right)
        {
            return Modulus(left, right);
        }
        public static Fraction operator %(Fraction left, BigInteger right)
        {
            return Modulus(left, new Fraction(right));
        }
        public static Fraction operator %(BigInteger left, Fraction right)
        {
            return Modulus(new Fraction(left), right);
        }

        public static Fraction Increment(Fraction fraction)
        {
            return Addition(fraction, new Fraction(BigInteger.One));
        }
        public static Fraction operator ++(Fraction fraction)
        {
            return Increment(fraction);
        }

        public static Fraction Decrement(Fraction fraction)
        {
            return Substraction(fraction, new Fraction(BigInteger.One));
        }
        public static Fraction operator --(Fraction fraction)
        {
            return Decrement(fraction);
        }

        public static bool operator ==(Fraction left, Fraction right)
        {
            if (ReferenceEquals(left, null) && right is null)
                return true;
            else if (left is object && right is null)
                return left.Equals(right);
            else
                return right.Equals(left);
        }
        public static bool operator ==(Fraction left, BigInteger right)
        {
            return (left == new Fraction(right));
        }
        public static bool operator ==(BigInteger left, Fraction right)
        {
            return (new Fraction(left) == right);
        }

        public static bool operator !=(Fraction left, Fraction right)
        {
            return !(left == right);
        }
        public static bool operator !=(Fraction left, BigInteger right)
        {
            return (left != new Fraction(right));
        }
        public static bool operator !=(BigInteger left, Fraction right)
        {
            return (new Fraction(left) != right);
        }

        public static bool operator >(Fraction left, Fraction right)
        {
            if (left.CompareTo(right) == 1)
                return true;
            return false;
        }
        public static bool operator >(Fraction left, BigInteger right)
        {
            return (left > new Fraction(right));
        }
        public static bool operator >(BigInteger left, Fraction right)
        {
            return (new Fraction(left) > right);
        }

        public static bool operator <(Fraction left, Fraction right)
        {
            if (left.CompareTo(right) == -1)
                return true;
            return false;
        }
        public static bool operator <(Fraction left, BigInteger right)
        {
            return (left < new Fraction(right));
        }
        public static bool operator <(BigInteger left, Fraction right)
        {
            return (new Fraction(left) < right);
        }

        public static bool operator >=(Fraction left, Fraction right)
        {
            if (left.CompareTo(right) >= 0)
                return true;
            return false;
        }
        public static bool operator >=(Fraction left, BigInteger right)
        {
            return (left >= new Fraction(right));
        }
        public static bool operator >=(BigInteger left, Fraction right)
        {
            return (new Fraction(left) >= right);
        }

        public static bool operator <=(Fraction left, Fraction right)
        {
            if (left.CompareTo(right) <= 0)
                return true;
            return false;
        }
        public static bool operator <=(Fraction left, BigInteger right)
        {
            return (left <= new Fraction(right));
        }
        public static bool operator <=(BigInteger left, Fraction right)
        {
            return (new Fraction(left) <= right);
        }
        #endregion

        #region Overriden
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Numerator).Append('/').Append(Denominator);
            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return Numerator.GetHashCode() ^ Denominator.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj is Fraction)
                return Equals(obj as Fraction);

            return false;
        }
        #endregion

        #region IEquatable<T>
        public bool Equals(Fraction other)
        {
            if (other is null)
                return false;

            if ((Denominator == other.Denominator) && (Numerator == other.Numerator))
                return true;

            return false;
        }
        #endregion

        #region IComparable
        public int CompareTo(object obj)
        {
            if (obj is null)
                return 1;

            if (obj is Fraction)
                return CompareTo(obj as Fraction);

            throw new ArgumentException("Object is not a Fraction!");
        }
        #endregion

        #region IComparable<T>
        public int CompareTo(Fraction other)
        {
            if (other is null)
                return 1;

            BigInteger lcm = GetLeastCommonMultiple(Denominator, other.Denominator);

            ToDenominator(lcm);
            other.ToDenominator(lcm);

            return Numerator.CompareTo(other.Numerator);
        }
        #endregion

        #region ICloneable
        public object Clone()
        {
            return new Fraction(this);
        }
        #endregion
    }
}


