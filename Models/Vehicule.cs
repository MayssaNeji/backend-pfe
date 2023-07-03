using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class Vehicule
{
    public string NomDeReference { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Agence { get; set; } = null!;

    public DateTime? DateDeMiseEnRoute { get; set; }

    public int Capacite { get; set; }

    public int Id { get; set; }

    public string? NumSerie { get; set; }
    [JsonIgnore]
    public virtual Agence? AgenceNavigation { get; set; } = null!;
}
