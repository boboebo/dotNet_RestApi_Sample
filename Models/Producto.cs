namespace WebApiPoc.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public double Precio { get; set; }
        public string Categoria { get; set; }

        public int CantEnStock { get; set; }
        public Producto() { }
    }
}
