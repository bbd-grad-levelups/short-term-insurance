using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

[Table("personas")]
public partial class Persona
{
  [Key]
  [Column("id")]
  public int Id { get; set; }

  [Required]
  [Column("persona_id")]
  public long PersonaId { get; set; }

  [Required]
  [Column("electronics")]
  public int Electronics { get; set; }

  [Required]
  [Column("last_payment_date")]
  public string LastPaymentDate { get; set; }

  [Required]
  [Column("debit_order_id")]
  public int DebitOrderId { get; set; }

  public Persona(int id = default, long personaId = default, int electronics = default, string lastPaymentDate = "01|01|01", int debitOrderId = 0)
  {
    Id = id;
    PersonaId = personaId;
    Electronics = electronics;
    LastPaymentDate = lastPaymentDate;
    DebitOrderId = debitOrderId;
  }

  public Persona()
  {
    Electronics = default;
    LastPaymentDate = "01|01|01";
  }
}

