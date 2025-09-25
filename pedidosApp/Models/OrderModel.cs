using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pedidosApp.Models
{
    public class OrderModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha del pedido es obligatoria")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(50)]
        public string Status { get; set; } = "Pendiente";

        [Range(0, double.MaxValue, ErrorMessage = "El total debe ser 0 o mayor")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int UserId { get; set; }

  
        public static List<string> GetValidStatuses()
        {
            return new List<string>
            {
                "Pendiente",
                "Procesado",
                "Enviado",
                "Entregado"
            };
        }
    }
}
