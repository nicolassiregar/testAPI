using Dapper;
using System.Data;
using TestAPI.Model;

namespace TestAPI.Repository
{
    public class TestTable1Repository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public TestTable1Repository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        

        public List<liveStock> checkStock()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var p = new DynamicParameters();

                var query = @"  select a.nama_produk, b.updateStock 
                                from master_produk a 
                                join (
	                                SELECT prod_id, sum(move_item) updateStock FROM transaksi_produk group by prod_id 
                                ) 
                                b on b.prod_id = a.id";

                var result = connection.Query<liveStock>(query).ToList();

                return result;
            }
        }
    }
}
