namespace ClienteService.Domain.Entities
{
    public class Persona
    {
        public int Id { get; set; }  // Clave primaria (PK)
        public string Nombre { get; set; }
        public string Genero { get; set; }
        public int Edad { get; set; }
        public string Identificacion { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
    }
}
