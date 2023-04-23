using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProEventos.Application.Dtos
{
    public class PalestranteDto
    {
        public int Id { get; set; }
         [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Nome { get; set; }
        public string MiniCurriculo { get; set; }
        public string ImagemURL { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public IEnumerable<RedeSocialDto> RedeSociais { get; set; }
        public IEnumerable<PalestranteDto> Palestrantes { get; set; }
    }
}