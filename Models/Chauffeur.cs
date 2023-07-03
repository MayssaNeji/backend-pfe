using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class Chauffeur
{
    public string Nom { get; set; } = null!;

    public string Prenom { get; set; } = null!;

    public DateTime DateDeNaissance { get; set; }

    public int Telephone { get; set; }

    public string Agence { get; set; } = null!;

    public int Id { get; set; }
    [JsonIgnore]
    public virtual Agence ?AgenceNavigation { get; set; } = null!;

    public virtual ICollection<Audit> Audits { get; } = new List<Audit>();
}
