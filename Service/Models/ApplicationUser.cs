// Models/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    [Required]
    [Display(Name = "ФИО")]
    public string FullName { get; set; }

    [Display(Name = "Дата регистрации")]
    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    // Дополнительные поля при необходимости
}