using System;
using System.Collections.Generic;

namespace EndpointsPage.Entities;

public partial class LevelsJdr
{
    public int LevelId { get; set; }

    public string LevelName { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<MatchesJdr> MatchesJdrs { get; set; } = new List<MatchesJdr>();
}
