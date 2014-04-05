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

        public void InsertValueNameValue<T>(string key, string family, string name, string value)
        {
            mutator.InsertColumn(family, key, mutator.NewColumn(name, value), this.ConsistencyLevel);
        }

        public T Get<T>(string key, string family) where T : new()
        {
            var data = this.CreateSelector().GetColumnFromRow(family, key, "D", this.ConsistencyLevel);
            return data.Value.DecompressLZ4().DeSerializeProtobuf<T>();
        }

        public string GetValue(string key, string name, string family)
        {
            return this.CreateSelector().GetColumnFromRow(family, key, name, this.ConsistencyLevel).Value.ToString();
        }
    }
}
