using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Zine.App.Domain.Settings;

[Table("Settings")]
[Index(nameof(Key), IsUnique = true)]
public class Setting
{
    public int Id { get; init; }

    [Required]
    [MaxLength(255)]
    public required string Key { get; init; }

    [MaxLength(255)]
    public string? Value { get; set; }

    [MaxLength(255)]
    public string? InitialValue { get; init; }
}
