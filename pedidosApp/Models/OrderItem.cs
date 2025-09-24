using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pedidosApp.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El subtotal debe ser 0 o mayor")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Required(ErrorMessage = "El pedido es obligatorio")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio")]
        public int ProductId { get; set; }
    }
}
