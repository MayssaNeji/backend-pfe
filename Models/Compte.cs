using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class Compte
{
    public string Login { get; set; } = null!;

    public int Id { get; set; }

    public int? Matricule { get; set; }

    public string? Nom { get; set; }

    public string? Prenom { get; set; }

    public int? Tel { get; set; }

    public DateTime? DateDeNaissance { get; set; }

    public string? Role { get; set; }

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }
}
