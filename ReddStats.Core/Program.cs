namespace ReddStats.Core
{
    using System;
    using System.Diagnostics;

    using ReddStats.Core.Providers;
    using ReddStats.Core.RPC;

    class Program
    {
        static void Main(string[] args)
        {
           // Structure.CreateStructure();
            Console.WriteLine("Processing Blocks");

            var p = RpcClient.DoRequest<long>("getblockcount");


            var o = new RpcProvider().GetBlock(300);
            var bp = new BlockParser();
            //bp.ParseChain();

            Console.WriteLine("Saving Blocks");
            //var sw = new Stopwatch();
            //sw.Start();
            //bp.SaveDb();
            //sw.Stop();

            //Console.WriteLine(sw.ElapsedMilliseconds);
            //bp.ProcessBalances(true);
            //bp.SaveFile("E:\\Reddcoin\\balances-4.xlsx");
        }
    }
}
