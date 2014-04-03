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

    public sealed class TransactionMapper : ClassMapper<Transaction>
    {
        public TransactionMapper()
        {
            Map(f => f.TransactionId).Key(KeyType.Assigned);
            Map(f => f.Inputs).Ignore();
            Map(f => f.Outputs).Ignore();
            AutoMap();
        }
    }

    public sealed class InputTransactionMapper : ClassMapper<TransactionInput>
    {
        public InputTransactionMapper()
        {
            Map(f => f.TransactionId).Key(KeyType.Assigned);
            Map(f => f.Index).Key(KeyType.Assigned);
            Map(f => f.PreviousTxReference).Ignore();
            Map(f => f.PreviousTxOutputKeyBinary).Ignore();
            AutoMap();
        }
    }

    public sealed class OutputTransactionMapper : ClassMapper<TransactionOutput>
    {
        public OutputTransactionMapper()
        {
            Map(f => f.TransactionId).Key(KeyType.Assigned);
            Map(f => f.Index).Key(KeyType.Assigned);
            Map(f => f.ScriptPublicKeyBinary).Ignore();
            AutoMap();
        }
    }

}
