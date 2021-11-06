using System;
using System.IO;
using AMLBS;

namespace AMLBS.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            AMLBS amlbs = new();

            if (args.Length == 1)
            {
                string file = File.ReadAllText(args[0]);

                Console.WriteLine(amlbs.Execute(file));
            }
            else
            {
                while (true)
                {
                    Console.Write(">");
                    string command = Console.ReadLine();
                    Console.WriteLine(amlbs.Execute(command));
                }
            }
        }
    }
}
