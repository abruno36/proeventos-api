using System.ComponentModel.DataAnnotations;

namespace ProEventos.Application.Dtos
{
    public class LoteDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Nome { get; set; }
         [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public decimal Preco { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public int Quantidade { get; set; }
        public int EventoId { get; set; }
        public EventoDto EventoDto { get; set; }
    }
}