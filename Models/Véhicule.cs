using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class Véhicule
{
    public string NomDeReference { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Agence { get; set; } = null!;

    public DateTime? DateDeMiseEnRoute { get; set; }

    public int Capacité { get; set; }

    public int Id { get; set; }

    public virtual Agence AgenceNavigation { get; set; } = null!;
}
