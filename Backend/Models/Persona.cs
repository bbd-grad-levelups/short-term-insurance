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

    public Persona(int id = default, long personaId = default, int electronics = default, bool blacklisted = default)
    {
      Id = id;
      PersonaId = personaId;
      Electronics = electronics;
      Blacklisted = blacklisted;
    }

    public Persona()
    {
      Electronics = default;
      Blacklisted = default;
    }
  }
}
