using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace piogi_lab_2
{
    internal class Class1
    {
        public class BigNumber
        {

            private const int Base = 1000;

            private int[] digits;

            private int length;

            private BigNumber(int size)
            {
                digits = new int[size];
                length = 0;
            }

            // Конструктор из обычного числа
            public BigNumber(long value)
            {
                if (value == 0)
                {
                    digits = new int[1];
                    length = 1;
                    return;
                }

                int temp = (int)value;
                int size = 0;
                while (temp > 0)
                {
                    size++;
                    temp /= Base;
                }

                digits = new int[size];
                length = 0;

                while (value > 0)
                {
                    digits[length] = (int)(value % Base);
                    length++;
                    value /= Base;
                }
            }

            public override string ToString()
            {
                if (length == 0) return "0";

                string result = digits[length - 1].ToString();

                for (int i = length - 2; i >= 0; i--)
                {
                    result += digits[i].ToString("D3");
                }
                return result;
            }


        }

    }
}
