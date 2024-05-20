using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Zine.App.Model.DB;

[Index(nameof(Key), IsUnique = true)]
public class Setting
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public required string Key { get; set; }

    [MaxLength(255)]
    public string? Value { get; set; }

    [MaxLength(255)]
    public string? InitialValue { get; set; }
}
