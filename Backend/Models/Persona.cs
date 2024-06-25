using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
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
    [Column("blacklisted")]
    public bool Blacklisted { get; set; }

    [Required]
    [Column("bank_account")]
    public string BankAccount { get; set; }

    public Persona(int id = default, long personaId = default, int electronics = default, bool blacklisted = default, string bankAccount = "")
    {
      Id = id;
      PersonaId = personaId;
      Electronics = electronics;
      Blacklisted = blacklisted;
      BankAccount = bankAccount;
    }

    public Persona()
    {
      Electronics = default;
      Blacklisted = default;
      BankAccount = string.Empty;
    }
  }
}
