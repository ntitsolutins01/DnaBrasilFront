﻿using System.Collections.Specialized;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Data;
using WebApp.Dto;

namespace WebApp.Models
{
    public class PerfilModel
    {
        public PerfilDto Perfil { get; set; }
        public List<PerfilDto> Perfis { get; set; }
        public string PerfilId { get; set; }
        public SelectList ListPerfis { get; set; }
        public List<ModuloDto> Modulos { get; set; }
        public List<Claim> Claims { get; set; }

        public class CreateUpdateCommand    
        {
        public string Id { get; set; }
        public string Nome { get; set; }
        public ListDictionary Claims { get; set; }
        public string Descricao { get; set; }
        }
    }

}
