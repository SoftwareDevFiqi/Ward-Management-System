using System;
using System.ComponentModel.DataAnnotations;
using TimelessTechnicians.UI.Services;

public class AuditLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200, ErrorMessage = "Action cannot exceed 200 characters.")]
    public string Action { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    public string FirstName { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string LastName { get; set; }

    [StringLength(500, ErrorMessage = "Details cannot exceed 500 characters.")]
    public string Details { get; set; }

    [Required]
    [NoPastOrFutureDate(ErrorMessage = "Action Date must be today's date.")]
    public DateTime ActionDate { get; set; } = DateTime.Now;
}
