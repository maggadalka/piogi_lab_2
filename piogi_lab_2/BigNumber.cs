using System;
using System.Text;

namespace piogi_lab_2
{
    public class BigNumber
    {
        private const int Base = 1000;

        // Разряды по основанию 1000, младший разряд — digits[0]
        private int[] digits;

        // Сколько разрядов реально используется
        private int length;

        // Знак числа (у нуля всегда false)
        private bool isNegative;

        private BigNumber(int size)
        {
            digits = new int[Math.Max(size, 1)];
            length = 0;
            isNegative = false;
        }

        // Конструктор из обычного числа
        public BigNumber(long value)
        {
            isNegative = value < 0;

            // Модуль считаем через ulong, иначе -long.MinValue переполнится
            ulong abs = isNegative ? (ulong)(-(value + 1)) + 1UL : (ulong)value;

            if (abs == 0)
            {
                digits = new int[1];
                length = 1;
                isNegative = false;
                return;
            }

            int size = 0;
            for (ulong t = abs; t > 0; t /= Base)
            {
                size++;
            }

            digits = new int[size];
            length = 0;

            while (abs > 0)
            {
                digits[length] = (int)(abs % Base);
                length++;
                abs /= Base;
            }
        }

        private bool IsZero => length == 1 && digits[0] == 0;

        // Убирает ведущие нули и не даёт появиться "-0"
        private void Normalize()
        {
            while (length > 1 && digits[length - 1] == 0)
            {
                length--;
            }

            if (length == 0)
            {
                digits[0] = 0;
                length = 1;
            }

            if (IsZero)
            {
                isNegative = false;
            }
        }

        private BigNumber Clone()
        {
            BigNumber copy = new BigNumber(length);
            Array.Copy(digits, copy.digits, length);
            copy.length = length;
            copy.isNegative = isNegative;
            return copy;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            if (isNegative) result.Append('-');

            result.Append(digits[length - 1]);

            for (int i = length - 2; i >= 0; i--)
            {
                result.Append(digits[i].ToString("D3"));
            }

            return result.ToString();
        }


        // Сравнение модулей: 1, если |a| > |b|; -1, если |a| < |b|; 0, если равны
        private static int CompareAbs(BigNumber a, BigNumber b)
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

        // |a| + |b|
        private static BigNumber AddAbs(BigNumber a, BigNumber b)
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
            result.Normalize();
            return result;
        }

        // |a| - |b|, только при |a| >= |b|
        private static BigNumber SubtractAbs(BigNumber a, BigNumber b)
        {
            BigNumber result = new BigNumber(a.length);
            int borrow = 0;

            for (int i = 0; i < a.length; i++)
            {
                int diff = a.digits[i] - borrow;
                if (i < b.length) diff -= b.digits[i];

                if (diff < 0)
                {
                    diff += Base;
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }

                result.digits[i] = diff;
            }

            result.length = a.length;
            result.Normalize();
            return result;
        }


        private static BigNumber Add(BigNumber a, BigNumber b)
        {
            BigNumber result;

            if (a.isNegative == b.isNegative)
            {
                // Знаки одинаковые: складываем модули, знак общий
                result = AddAbs(a, b);
                result.isNegative = a.isNegative;
            }
            else
            {
                // Знаки разные: из большего модуля вычитаем меньший,
                // знак берём у числа с большим модулем
                int cmp = CompareAbs(a, b);

                if (cmp == 0) return new BigNumber(0);

                if (cmp > 0)
                {
                    result = SubtractAbs(a, b);
                    result.isNegative = a.isNegative;
                }
                else
                {
                    result = SubtractAbs(b, a);
                    result.isNegative = b.isNegative;
                }
            }

            result.Normalize();
            return result;
        }

        private static BigNumber Subtract(BigNumber a, BigNumber b)
        {
            // a - b = a + (-b)
            return Add(a, -b);
        }

        public static BigNumber operator +(BigNumber a, BigNumber b)
        {
            return Add(a, b);
        }

        public static BigNumber operator -(BigNumber a, BigNumber b)
        {
            return Subtract(a, b);
        }

        // Унарный минус
        public static BigNumber operator -(BigNumber a)
        {
            BigNumber result = a.Clone();
            if (!result.IsZero) result.isNegative = !result.isNegative;
            return result;
        }


        private static BigNumber Multiply(BigNumber a, int multiplier)
        {
            if (multiplier == 0 || a.IsZero) return new BigNumber(0);

            // через long, чтобы Math.Abs(int.MinValue) не переполнился
            long m = Math.Abs((long)multiplier);

            // |multiplier| < 1000^4, поэтому хватит 4 дополнительных разрядов
            BigNumber result = new BigNumber(a.length + 4);
            long carry = 0;
            int i = 0;

            for (; i < a.length; i++)
            {
                long product = a.digits[i] * m + carry;
                result.digits[i] = (int)(product % Base);
                carry = product / Base;
            }

            while (carry > 0)
            {
                result.digits[i] = (int)(carry % Base);
                carry /= Base;
                i++;
            }

            result.length = i;
            result.isNegative = a.isNegative ^ (multiplier < 0);
            result.Normalize();
            return result;
        }

        public static BigNumber operator *(BigNumber a, int multiplier)
        {
            return Multiply(a, multiplier);
        }

        public static BigNumber operator *(int multiplier, BigNumber a)
        {
            return Multiply(a, multiplier);
        }


        // Возвращает 1, если a > b. -1, если a < b. 0, если они равны.
        public static int Compare(BigNumber a, BigNumber b)
        {
            if (a.isNegative != b.isNegative)
            {
                return a.isNegative ? -1 : 1;
            }

            int cmp = CompareAbs(a, b);

            // Для отрицательных: чем больше модуль, тем меньше число
            return a.isNegative ? -cmp : cmp;
        }

        public static bool operator >(BigNumber a, BigNumber b) => Compare(a, b) > 0;
        public static bool operator <(BigNumber a, BigNumber b) => Compare(a, b) < 0;
        public static bool operator >=(BigNumber a, BigNumber b) => Compare(a, b) >= 0;
        public static bool operator <=(BigNumber a, BigNumber b) => Compare(a, b) <= 0;
    }
}