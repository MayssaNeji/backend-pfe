using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class HistoriqueImport
{
    public int Id { get; set; }

    public string NomFichier { get; set; } = null!;

    public DateTime DateImport { get; set; }

    public string? Creater { get; set; }
}
