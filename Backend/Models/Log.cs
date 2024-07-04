using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models;

[Table("logs")]
public class Log(string timeStamp, string message) 
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  [Column("id")]
  public int Id { get; set; }

  [Required]
  [Column("log_date")]
  [StringLength(16)]
  public string TimeStamp { get; set; } = timeStamp;

  [Required]
  [Column("log_message")]
  [StringLength(255)]
  public string Message { get; set; } = message;
}