namespace WebApp.Dto
{
    public class DisciplinaDto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public List<FuncionalidadeDto> Funcionalidades { get; init; }
    }
}
