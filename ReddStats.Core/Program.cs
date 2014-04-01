namespace bsparser
{
    using ReddStats.Core;

    class Program
    {
        static void Main(string[] args)
        {
           var bp = new BlockParser();
            bp.ParseChain();
            bp.ProcessBalances(false);
            bp.SaveFile("E:\\Reddcoin\\balances-4.xlsx");
        }
    }
}
