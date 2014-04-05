namespace ReddStats.Core.Interface
{
    public interface IBlockProvider
    {
        int GetBlockCount();

        Block GetBlock(int id);

        void SaveBlock(Block block);
    }
}
