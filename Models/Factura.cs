namespace WebApiPoc.Models
{
    public class Factura
    {
        public int Id { get; set; }
        public string Fecha { get; set; }
        public string IdCliente { get; set; }
        public double Total { get; set; }
        public Factura() { }
    }
}
