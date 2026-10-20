using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace piogi_lab_2
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

        // Вспомогательный метод для сложения
        private static BigNumber Add(BigNumber a, BigNumber b)
        {
            int maxLength = Math.Max(a.length, b.length);
            BigNumber result = new BigNumber(maxLength + 1);

            int carry = 0;
            int i = 0;

            while (i < a.length || i < b.length || carry > 0)
            {
                int sum = carry;
                if (i < a.length) sum += a.digits[i];
                if (i < b.length) sum += b.digits[i];

                result.digits[i] = sum % Base;
                carry = sum / Base;
                i++;
            }

            result.length = i;
            return result;
        }

        public static BigNumber operator +(BigNumber a, BigNumber b)
        {
            return Add(a, b);
        }

        // Вспомогательный метод для вычитания
        private static BigNumber Subtract(BigNumber a, BigNumber b)
        {
            BigNumber result = new BigNumber(0);
            result.digits = new int[a.length];

            int borrow = 0; // Заем
            int i = 0;

            while (i < a.length)
            {
                int diff = a.digits[i] - borrow;
                if (i < b.length) diff -= b.digits[i];

                if (diff < 0)
                {
                    diff += Base; // Занимаем 1000 у старшего разряда
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }

                result.digits[i] = diff;
                i++;
            }

            // Обрезаем лишние нули
            result.length = i;
            while (result.length > 1 && result.digits[result.length - 1] == 0)
            {
                result.length--;
            }

            return result;
        }


        public static BigNumber operator -(BigNumber a, BigNumber b)
        {
            return Subtract(a, b);
        }

        //умножение
        private static BigNumber Multiply(BigNumber a, int multiplier)
        {
            BigNumber result = new BigNumber(a.length + 1);
            int carry = 0;

            for (int i = 0; i < a.length; i++)
            {
                long product = (long)a.digits[i] * multiplier + carry;
                result.digits[i] = (int)(product % Base);
                carry = (int)(product / Base);
            }

            result.length = a.length;
            if (carry > 0)
            {
                result.digits[result.length] = carry;
                result.length++;
            }

            return result;
        }

        public static BigNumber operator *(BigNumber a, int multiplier)
        {
            return Multiply(a, multiplier);
        }


        // Метод для сравнения двух чисел. 
        // Возвращает 1, если a > b. -1, если a < b. 0, если они равны.
        public static int Compare(BigNumber a, BigNumber b)
        {
            if (a.length > b.length) return 1;
            if (a.length < b.length) return -1;

            for (int i = a.length - 1; i >= 0; i--)
            {
                if (a.digits[i] > b.digits[i]) return 1;
                if (a.digits[i] < b.digits[i]) return -1;
            }

            return 0;
        }

        public static bool operator >(BigNumber a, BigNumber b) => Compare(a, b) > 0;
        public static bool operator <(BigNumber a, BigNumber b) => Compare(a, b) < 0;
        public static bool operator >=(BigNumber a, BigNumber b) => Compare(a, b) >= 0;
        public static bool operator <=(BigNumber a, BigNumber b) => Compare(a, b) <= 0;
     
    

     // Оператор равенства
    public static bool operator ==(BigNumber a, BigNumber b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return Compare(a, b) == 0;
        }

        // Оператор неравенства (нужен, если есть ==)
        public static bool operator !=(BigNumber a, BigNumber b)
        {
            return !(a == b);
        }

    }
}
