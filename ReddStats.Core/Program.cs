namespace ReddStats.Core
{
    class Program
    {
        static void Main(string[] args)
        {
           var bp = new BlockParser();
            bp.ParseChain();
            bp.SaveDb();
            //bp.ProcessBalances(true);
            //bp.SaveFile("E:\\Reddcoin\\balances-4.xlsx");
        }
    }
}
