namespace ReddStats.Core
{
    using Lesula.Cassandra;
    using Lesula.Cassandra.FrontEnd;
    using Lesula.Cassandra.FrontEnd.Enumerators;

    using ReddStats.Practices;

    public class Structure
    {
        public static void CreateStructure()
        {
            var connection = AquilesHelper.RetrieveCluster("ReddStats");
            var manager = new KeyspaceManager(connection);
            manager.AddKeyspace("blockchain", 1);

            var famManager = new ColumnFamilyManager(connection, "blockchain");
            famManager.TryAddColumnFamily("block", ColumnTypeEnum.Standard, ComparatorTypeEnum.UTF8Type);
            famManager.TryAddColumnFamily("transaction", ColumnTypeEnum.Standard, ComparatorTypeEnum.UTF8Type);
        }
    }

    public class BlockChainDb : CassandraDalcBase
    {
        public BlockChainDb()
        {
            this.Keyspace = "blockchain";
            mutator = this.CreateMutator();
        }

        private readonly Mutator mutator;

        public void Insert<T>(string key, string family, T value) where T : new()
        {
            var data = value.SerializeProtobuf().CompressLZ4();
            mutator.InsertColumn(family, key, mutator.NewColumn("D", data), this.ConsistencyLevel);
        }
    }

}
