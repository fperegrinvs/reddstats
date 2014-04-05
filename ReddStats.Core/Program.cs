namespace ReddStats.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var blocks = FileReader.ParseChain();

            //bp.ParseChain();

            //Console.WriteLine("Saving Blocks");
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
