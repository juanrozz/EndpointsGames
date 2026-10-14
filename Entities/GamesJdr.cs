using System;
using System.Collections.Generic;

namespace EndpointsPage.Entities;

public partial class GamesJdr
{
    public int GameId { get; set; }

    public string GameName { get; set; } = null!;

    public bool Status { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? IconUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? GameNameEs { get; set; }

    public string? DescriptionEs { get; set; }

    public virtual ICollection<MatchesJdr> MatchesJdrs { get; set; } = new List<MatchesJdr>();
}
