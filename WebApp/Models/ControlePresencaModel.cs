﻿using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Dto;

namespace WebApp.Models
{
    public class ControlePresencaModel
    {
        public ControlePresencaDto ControlePresenca { get; set; }
        public PaginatedListDto<ControlePresencaDto>? ControlesPresencas { get; set; }
        public string ControlePresencaId { get; set; }
        public SelectList ListControlePresencas { get; set; }
        public string EstadoId { get; set; }
        public SelectList ListEstados { get; set; }
        public string MunicipioId { get; set; }
        public SelectList ListMunicipios { get; set; }
        public SelectList ListModalidades { get; set; }
        public int ModalidadeId { get; set; }
        public string FomentoId { get; set; }
        public SelectList ListFomentos { get; set; }
        public int LocalidadeId { get; set; }
        public SelectList ListLocalidades { get; set; }
        public string AlunoId { get; set; }
        public SelectList ListAlunos { get; set; }
        public SelectList ListEtnias { get; set; }
        public string EtniaId { get; set; }
        public SelectList ListDeficiencias { get; set; }
        public int DeficienciaId { get; set; }

        public class CreateUpdateControlePresencaCommand
        {
            public int Id { get; set; }
			public  string Controle { get; init; }
			public string Justificativa { get; init; }
			public bool Status { get; init; } = true;
			public int? LocalidadeId { get; set; }
			public string? MunicipioId { get; set; }
			public string? AlunoId { get; set; }
			public int? EventoId { get; set; }
		}
    }

}
