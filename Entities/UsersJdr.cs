using System;
using System.Collections.Generic;

namespace EndpointsPage.Entities;

public partial class UsersJdr
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public int DomainId { get; set; }

    public bool? Status { get; set; }

    public string? Name { get; set; }

    public string? Adp { get; set; }

    public virtual DomainJdr Domain { get; set; } = null!;

    public virtual ICollection<MatchesJdr> MatchesJdrs { get; set; } = new List<MatchesJdr>();
}
