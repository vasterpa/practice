using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice
{
    class Program
    {
        static void Main(string[] args)
        {
            var fr1 = new Fraction(2, 9);
            var fr2 = new Fraction(3, 7);
            var fr3 = new Fraction(4, 18);

            try
            {

                Console.WriteLine((fr1 + fr2).ToString());
                Console.WriteLine((fr1 * fr2).ToString());
                Console.WriteLine((fr1 < fr2).ToString());
                Console.WriteLine((fr1 == fr3).ToString());

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
