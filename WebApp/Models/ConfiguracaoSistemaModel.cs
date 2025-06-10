using WebApp.Dto;

namespace WebApp.Models
{
    public class ConfiguracaoSistemaModel
    {
        public ModuloDto Modulo { get; set; }
        public FuncionalidadeDto Funcionalidade { get; set; }

        public class CreateUpdateModuloCommand
        {
            public int Id { get; set; }
            public string Nome { get; set; }
        }
        public class CreateUpdateFuncionalidadeCommand
        {
            public int Id { get; set; }
            public string Nome { get; set; }
        }
    }

}
