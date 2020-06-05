using System;

namespace ShortHide5
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("1. plaintext\n2. -d ciphertext\n3. -m plaintext [number]");
            }
            else if(args.Length == 1)
            {
                Console.WriteLine(ShortHide5.Encrypt(args[0]));
            }
            else if(args.Length >= 2)
            {
                if(args[0] == "-d" || args[0] == "/d")
                {
                    Console.WriteLine(ShortHide5.Decrypt(args[1]));
                }
                else if(args[0] == "-s" || args[0] == "/s")
                {
                    Console.WriteLine(ShortHide5.FromString(args[1]).ToJson());
                }
                else if(args[0] == "-m" || args[0] == "/m")
                {
                    uint n = 15;
                    if(args.Length > 2)
                    {
                        uint.TryParse(args[2], out n);
                    }
                    foreach(var item in new ShortHide5(args[1]).MuitiFindCast(n))
                    {
                        Console.WriteLine(item);
                    }
                }
            }
        }
    }
}
