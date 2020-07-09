using System;

namespace Practice
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = Dichotomy.Solve(new Fraction(), new Fraction(1, 2), (x) => 8 * x * x * x * x * x - 9 * x + 1, new Fraction(1, 1000));

            try
            {

                Console.WriteLine(result.GetDecimalRepresentation(4));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
