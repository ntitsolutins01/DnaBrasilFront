namespace WebApp.Models
{
    public class FotoEventoModel
    {

        public class CreateUpdateFotoEventoCommand
        {
            public int Id { get; set; }
            public bool Status { get; set; } = true;

        }
    }

}
