using System;
using System.Collections.Generic;

namespace EndpointsPage.Entities;

public partial class MatchesJdr
{
    public int MatchId { get; set; }

    public int UserId { get; set; }

    public int Score { get; set; }

    public decimal TimeSeconds { get; set; }

    public DateTime? MatchDate { get; set; }

    public int LevelId { get; set; }

    public int GameId { get; set; }

    public virtual GamesJdr Game { get; set; } = null!;

    public virtual LevelsJdr Level { get; set; } = null!;

    public virtual UsersJdr User { get; set; } = null!;
}
