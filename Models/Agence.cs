using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class Agence
{
    public string Nom { get; set; } = null!;

    public string Adresse { get; set; } = null!;

    public int Telephone { get; set; }

    public int MatriculeFiscale { get; set; }

    public string? SiteInternet { get; set; }

    public int Id { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Chauffeur> Chauffeurs { get; } = new List<Chauffeur>();

    public virtual ICollection<Circuit> Circuits { get; } = new List<Circuit>();

    public virtual ICollection<Vehicule> Vehicules { get; } = new List<Vehicule>();
}
