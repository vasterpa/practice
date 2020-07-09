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
            var fr1 = new Fraction(2, 5);
            var fr2 = new Fraction(2, 5);
            var complex = new Complex(fr1, fr2);
            
            try
            {

                Console.WriteLine((complex).ToString());

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }
    }
}
