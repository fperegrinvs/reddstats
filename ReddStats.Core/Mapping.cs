namespace ReddStats.Core
{
    using DapperExtensions.Mapper;

    public sealed class BlockMapper : ClassMapper<Block>
    {
        public BlockMapper()
        {
            Map(f => f.Id).Key(KeyType.Assigned);
            Map(f => f.Transactions).Ignore();
            AutoMap();
        }
    }
}
