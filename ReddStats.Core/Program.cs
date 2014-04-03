namespace ReddStats.Core
{
    using System;
    using System.Diagnostics;

    class Program
    {
        static void Main(string[] args)
        {
            //Structure.CreateStructure();
            Console.WriteLine("Processing Blocks");

            var bp = new BlockParser();
            bp.ParseChain();

            Console.WriteLine("Saving Blocks");
            var sw = new Stopwatch();
            sw.Start();
            bp.SaveDb();
            sw.Stop();

            Console.WriteLine(sw.ElapsedMilliseconds);
            //bp.ProcessBalances(true);
            //bp.SaveFile("E:\\Reddcoin\\balances-4.xlsx");
        }
    }
}
