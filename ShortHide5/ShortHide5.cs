using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShortHide5
{
    internal class ShortHide5
    {
        private byte[] chars;
        private long mp;

        private ShortHide5()
        {
            FactorRange = 26;
            FactorsCount = 4;
        }

        public ShortHide5(string word) : this()
        {
            chars = new byte[word.Length];
            word = word.ToLower();
            for(int i = 0; i < word.Length; i++)
            {
                chars[i] = (byte)(word[i] - 'a' + 1);
                if(chars[i] == 26)
                {
                    chars[i] = 0;
                }
                if(chars[i] > 26)
                {
                    throw new FormatException("word只支持由纯26个字母组成的单词");
                }
            }
        }

        public string[] MuitiFindCast(uint n)
        {
            List<string> list = new List<string>();
            try
            {
                FindFirstNumbers();
                list.Add(ToString());
                for(uint i = 1; i < n; i++)
                {
                    FindNextNumbers();
                    list.Add(ToString());
                }
            }
            catch(NotSupportedException)
            {
                
            }
            return list.ToArray();
        }

        public byte[] Factors { get; private set; }
        public long[] Numbers { get; private set; }

        public byte FactorRange { get; set; }
        public byte FactorsCount { get; set; }
        public sbyte BlankByte { get; set; }

        private static long From36Hex(string str)
        {
            string alphaBet = "0123456789abcdefghijklmnopqrstuvwxyz";
            long result = 0;
            for(int i = 0; i < str.Length; i++)
            {
                int pos = alphaBet.IndexOf(str[i]);
                result += pos * Pow(36, str.Length - i - 1);
            }
            return result;
        }

        private static long Pow(int x, int y)
        {
            if(y == 0)
            {
                return 1;
            }
            if(y < 0)
            {
                throw new Exception("不支持负指数");
            }
            long result = x;
            for(int i = 1; i < y; i++)
            {
                result *= x;
            }
            return result;
        }

        private static string To36Hex(long n)
        {
            Stack<char> stack = new Stack<char>();
            string alphaBet = "0123456789abcdefghijklmnopqrstuvwxyz";
            while(n / 36 != 0)
            {
                stack.Push(alphaBet[(int)(n % 36)]);
                n /= 36;
            }
            stack.Push(alphaBet[(int)n]);
            return new string(stack.ToArray());
        }

        private static sbyte[] ToBalancedPentagram(long n)
        {
            Stack<sbyte> stack = new Stack<sbyte>();
            while(n / 5 != 0)
            {
                stack.Push((sbyte)(n % 5 - 2));
                n /= 5;
            }
            stack.Push((sbyte)(n - 2));
            return stack.ToArray();
        }

        private static byte[] ToXHex(long n, byte x)
        {
            Stack<byte> stack = new Stack<byte>();
            while(n / x != 0)
            {
                stack.Push((byte)(n % x));
                n /= x;
            }
            stack.Push((byte)n);
            return stack.ToArray();
        }

        public static string Decrypt(string str)
        {
            string[] strs = str.Split(',');
            string result = "";
            foreach(var item in strs)
            {
                ShortHide5 sh5 = FromString(item);
                result += sh5.GetWord();
                result += ' ';
            }
            return result;
        }

        public static string Encrypt(string str)
        {
            string[] strs = str.Trim().Split(new[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
            string result = "";
            foreach(var item in strs)
            {
                ShortHide5 sh5 = new ShortHide5(item);
                sh5.FindFirstNumbers();
                result += sh5.ToString();
                result += ',';
            }
            return result.Substring(0, result.Length - 1);
        }

        public static ShortHide5 FromString(string str)
        {
            List<string> list = new List<string>();
            string temp = "";
            for(int i = 0; i < str.Length; i++)
            {
                if(char.IsUpper(str[i]) && i > 0)
                {
                    list.Add(temp);
                    temp = str[i].ToString();
                }
                else
                {
                    temp += str[i];
                }
            }
            list.Add(temp);
            ShortHide5 sh5 = new ShortHide5
            {
                Factors = new byte[list.Count],
                Numbers = new long[list.Count]
            };
            for(int i = 0; i < list.Count; i++)
            {
                sh5.Factors[i] = (byte)(list[i][0] - 'A' + 1);
                sh5.Numbers[i] = From36Hex(list[i].Substring(1));
            }
            return sh5;
        }

        private long[] CalcNumbers(params byte[] factors)
        {
            if(factors.Length > chars.Length && factors.Length > 5)
            {
                throw new ArgumentException("因数个数过多");
            }
            foreach(var item in factors)
            {
                if(item >= 26)
                {
                    throw new ArgumentException("因数不能大于25");
                }
            }

            Numbers = new long[factors.Length];
            sbyte[][] buffer = new sbyte[chars.Length][];
            long max = Pow(5, factors.Length);
            long sum;

            for(int i = 0; i < chars.Length; i++)
            {
                for(long j = 0; j < max; j++)
                {
                    sbyte[] temp = ToBalancedPentagram(j);
                    sum = 0;
                    for(int k = 0; k < factors.Length; k++)
                    {
                        if(k < temp.Length)
                        {
                            sum += temp[k] * factors[k];
                        }
                        else
                        {
                            sum += BlankByte * factors[k];
                        }
                    }
                    while(sum < 0)
                    {
                        sum += 26;
                    }
                    if(sum % 26 == chars[i])
                    {
                        buffer[i] = temp;
                        break;
                    }
                }
                if(buffer[i] == null)
                {
                    throw new NotSupportedException("给定的参数不能计算出值");
                }
            }

            Array.Reverse(buffer);

            for(int i = 0; i < factors.Length; i++)
            {
                sum = 0;
                for(int j = 0; j < chars.Length; j++)
                {
                    if(i < buffer[j].Length)
                    {
                        sum += (buffer[j][i] + 2) * Pow(5, j);
                    }
                    else
                    {
                        sum += 2 * Pow(5, j);
                    }
                }
                Numbers[i] = sum;
            }
            Factors = factors;
            return Numbers;
        }

        private long[] FindFirstNumbers()
        {
            byte[] factors = new byte[FactorsCount];
            long max = Pow(FactorRange, FactorsCount);
            for(mp = 0; mp < max; mp++)
            {
                byte[] temp = ToXHex(mp, FactorRange);
                for(int j = 0; j < FactorsCount; j++)
                {
                    if(j < temp.Length)
                    {
                        factors[j] = temp[j];
                    }
                }
                try
                {
                    return CalcNumbers(factors);
                }
                catch(NotSupportedException)
                {
                    continue;
                }
            }

            throw new NotSupportedException("找不到可用的组合");
        }

        private long[] FindNextNumbers()
        {
            byte[] factors = new byte[FactorsCount];
            long max = Pow(FactorRange, FactorsCount);
            for(mp++; mp < max; mp++)
            {
                byte[] temp = ToXHex(mp, FactorRange);
                for(int j = 0; j < FactorsCount; j++)
                {
                    if(j < temp.Length)
                    {
                        factors[j] = temp[j];
                    }
                }
                try
                {
                    return CalcNumbers(factors);
                }
                catch(NotSupportedException)
                {
                    continue;
                }
            }
            throw new NotSupportedException("找不到可用的组合");
        }

        public string GetWord()
        {
            sbyte[][] buffer = new sbyte[Numbers.Length][];
            int len = 0;
            for(int i = 0; i < Numbers.Length; i++)
            {
                buffer[i] = ToBalancedPentagram(Numbers[i]);
                len = Math.Max(len, buffer[i].Length);
            }
            chars = new byte[len];
            for(int i = 0; i < len; i++)
            {
                long sum = 0;
                for(int j = 0; j < Factors.Length; j++)
                {
                    if(i + buffer[j].Length >= len)
                    {
                        sum += buffer[j][i - len + buffer[j].Length] * Factors[j];
                    }
                    else
                    {
                        sum += -2 * Factors[j];
                    }
                }
                while(sum < 0)
                {
                    sum += 26;
                }
                chars[i] = (byte)(sum % 26);
                if(chars[i] == 0)
                {
                    chars[i] = 26;
                }
            }
            return new string(chars.Select(b => (char)(b + 'a' - 1)).ToArray());
        }

        public string ToJson()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('[');
            for(int i = 0; i < Factors.Length; i++)
            {
                if(Factors[i] == 0)
                {
                    continue;
                }
                stringBuilder.Append('(');
                stringBuilder.Append(Factors[i]);
                stringBuilder.Append(',');
                stringBuilder.Append(Numbers[i]);
                stringBuilder.Append(')');
                stringBuilder.Append(',');
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append(']');
            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for(int i = 0; i < Factors.Length; i++)
            {
                if(Factors[i] == 0)
                {
                    if(i == 0)
                    {
                        stringBuilder.Append('Z');
                        stringBuilder.Append(To36Hex(Numbers[i]));
                    }
                    continue;
                }
                stringBuilder.Append((char)(Factors[i] + 'A' - 1));
                stringBuilder.Append(To36Hex(Numbers[i]));
            }
            return stringBuilder.ToString();
        }
    }
}
