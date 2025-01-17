namespace WebApiPoc.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public Cliente() { }
    }
}
