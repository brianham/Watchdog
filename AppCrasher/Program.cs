using System;
using System.Collections.Generic;

namespace AppCrasher
{
    class Program
    {
        static void Main(string[] args)
        {
            string input;

            Console.WriteLine("Type 'crash', 'hang', or 'exit'...");

            while ((input = Console.ReadLine()) != "exit")
            {
                switch (input)
                {
                    case "crash":
                        {
                            Console.WriteLine("Crashing app...");
                            throw new Exception();
                        }

                    case "hang":
                        {
                            Console.WriteLine("Hanging app...");
                            List<byte[]> wastedMemory = new List<byte[]>();

                            while (true)
                            {
                                byte[] buffer = new byte[4096]; // Allocate 4kb
                                wastedMemory.Add(buffer);
                            }
                        }

                    default:
                        {
                            Console.WriteLine("I don't understand, try again. Type 'crash', 'hang', or 'exit'...");
                            break;
                        }
                }
            }
        }
    }
}
