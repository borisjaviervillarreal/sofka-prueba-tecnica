namespace ClienteService.Domain.Entities
{
    public class Cliente : Persona
    {
        public string ClienteId { get; set; } 
        public string Contrasena { get; set; }
        public string Estado { get; set; }  
    }
}
