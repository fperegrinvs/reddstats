namespace ReddStats.Practices
{
    using Apache.Cassandra;

    using Lesula.Cassandra;
    using Lesula.Cassandra.Cluster;
    using Lesula.Cassandra.FrontEnd;

    /// <summary>
    /// Classe base para todas as DALC que utilizam o cassandra.
    /// </summary>
    public abstract class CassandraDalcBase
    {
        /// <summary>
        /// Nome do cluster utilizado pela dalc.
        /// </summary>
        private string clusterName = "ReddStats";

        /// <summary>
        /// Variável privada usada para armazenar o valor do singleton TTL
        /// </summary>
        private int ttl = Mutator.NoTtl;

        /// <summary>
        /// Nível de consistência usado nas operações do casssandra.
        /// </summary>
        private ConsistencyLevel consistencyLevel = ConsistencyLevel.ONE;

        /// <summary>
        /// Keyspace usado para armazenar os dados
        /// </summary>
        public string Keyspace { get; set; }

        /// <summary>
        /// Nome da ColumnFamily sem o prefixo do cliente
        /// </summary>
        public string FamilyNameBase { get; set; }

        /// <summary>
        /// Nível de consistência usado nas operações do casssandra.
        /// </summary>
        public ConsistencyLevel ConsistencyLevel
        {
            get
            {
                return this.consistencyLevel;
            }

            set
            {
                this.consistencyLevel = value;
            }
        }

        /// <summary>
        /// Nome do cluster
        /// </summary>
        public string ClusterName
        {
            get
            {
                return this.clusterName;
            }

            set
            {
                this.clusterName = value;
            }
        }

        /// <summary>
        /// Time to Live da Column Family
        /// </summary>
        public int TTL
        {
            get
            {
                return this.ttl;
            }

            set
            {
                this.ttl = value;
            }
        }

        /// <summary>
        /// Construtor para facilitar a criação de um mutator.
        /// </summary>
        /// <returns>
        /// Mutator pronto para ser utilizado.
        /// </returns>
        public Mutator CreateMutator()
        {
            return new Mutator(this.GetConnection(), this.Keyspace, this.TTL);
        }

        /// <summary>
        /// Construtor para facilitar a criação de um RowDeletor.
        /// </summary>
        /// <returns>
        /// RowDeletor pronto para ser utilizado.
        /// </returns>
        public RowDeletor CreateRowDeletor()
        {
            return new RowDeletor(this.GetConnection(), this.Keyspace);
        }

        /// <summary>
        /// Construtor para facilitar a criação de um Selector.
        /// </summary>
        /// <returns>
        /// Selector pronto para ser utilizado.
        /// </returns>
        public Selector CreateSelector()
        {
            return new Selector(this.GetConnection(), this.Keyspace);
        }

        /// <summary>
        /// Disponibiliza uma conexão.
        /// </summary>
        /// <returns>
        /// Conexão disponibilizada.
        /// </returns>
        public ICluster GetConnection()
        {
            return AquilesHelper.RetrieveCluster(this.clusterName);
        }
    }
}
