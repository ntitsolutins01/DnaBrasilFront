namespace WebApp.Dto
{
    public class ModuloDto
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public List<FuncionalidadeDto> Funcionalidades { get; init; }
    }
}
