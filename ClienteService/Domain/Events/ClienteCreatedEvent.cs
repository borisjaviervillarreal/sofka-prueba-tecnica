namespace ClienteService.Domain.Events
{
    public class ClienteCreatedEvent
    {
        public string ClienteId { get; set; }
        public string Nombre { get; set; }
        public string Identificacion { get; set; }
    }
}
