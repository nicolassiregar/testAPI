namespace TestAPI.Model
{
    public class ToDo
    {
    }

    public class LoginRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class liveStock
    {
        public string nama_produk { get; set; }
        public string updateStock { get; set; }
    }

    public class produkModel
    {
        public int id { get; set; }
        public string nama_produk { get; set; }
        public int price { get; set; }
    }

    public class penjualanModel
    {
        public int id { get; set; }
        public int produk_id { get; set; }
        public int qty { get; set; }
        public int price { get; set; }
        public string kode_penjualan { get; set; }
    }

    public class penjualanParam
    {
        public int id { get; set; }
        public int qty { get; set; }
        public Decimal price { get; set; }
    }

    

}
