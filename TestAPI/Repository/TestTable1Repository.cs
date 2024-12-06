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

        public List<liveStock> reportPos()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var p = new DynamicParameters();

                var query = @"select a.nama_produk, b.updateStock 
                                from master_produk a 
                                join (
	                                SELECT prod_id, (sum(move_item) * -1) updateStock FROM transaksi_produk where description = 'penjualan' group by prod_id 
                                ) 
                                b on b.prod_id = a.id";

                var result = connection.Query<liveStock>(query).ToList();

                return result;
            }
        }

        public IEnumerable<produkModel> produkRepository()
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var p = new DynamicParameters();

                var query = @"SELECT * FROM public.master_produk";

                var result = connection.Query<produkModel>(query).ToList();

                return result;
            }
        }

        public void insertPos(List<penjualanParam> param, string kode_penjualan)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var p = new DynamicParameters();

                string query = @"insert into penjualan (produk_id, qty, price, kode_penjualan) values ";
                string values = "";

                foreach (var v in param) {
                    values += ",(" + v.id.ToString() + ","+v.qty.ToString() + ",'"+v.price+"','"+kode_penjualan+"')";
                }
                query = query + values.Substring(1)+";";

                var query2 = @"insert into transaksi_produk (prod_id, move_item, description, ref_id) values ";
                string values2 = "";

                foreach (var v in param)
                {
                    values2 += ",(" + v.id.ToString() + ",'-" + v.qty.ToString() + "','penjualan','" + kode_penjualan + "')";
                }

                query += query2 + values2.Substring(1) + ";";

                connection.Execute(query);
            }
        }

        public produkModel updateItem(produkModel param)
        {
            using (var connection = _dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var p = new DynamicParameters();
                p.Add("@id", param.id, DbType.Int32);
                p.Add("@nama_produk", param.nama_produk, DbType.String);
                p.Add("@price", param.price, DbType.Decimal);

                var query = @"update master_produk 
                            SET 
	                            nama_produk = @nama_produk,  
	                            price = @price 
                            where id = @id; 
                            select * from master_produk where id = @id;
                            ";

                var result = connection.QueryFirstOrDefault<produkModel>(query,p);
                return result;
            }
        }

    }
}
