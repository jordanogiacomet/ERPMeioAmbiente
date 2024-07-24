using System.ComponentModel.DataAnnotations;

namespace ERPMeioAmbienteAPI.Models
{
    public class Motorista
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do motorista é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O número da carteira de habilitação é obrigatório")]
        public string NumeroCarteiraHabilitacao { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório")]
        [Phone(ErrorMessage = "O telefone não é válido")]
        public string Telefone { get; set; }

        public int? VeiculoId { get; set; }
        public virtual Veiculo Veiculo { get; set; }

        public virtual ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
    }
}