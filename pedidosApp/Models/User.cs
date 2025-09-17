using System.ComponentModel.DataAnnotations;

namespace pedidosApp.Models
{

    public class User
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; } // admin, cliente, empleado
    }
}

