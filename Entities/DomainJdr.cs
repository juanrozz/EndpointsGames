using System;
using System.Collections.Generic;

namespace EndpointsPage.Entities;

public partial class DomainJdr
{
    public int DomainId { get; set; }

    public string Domain { get; set; } = null!;

    public virtual ICollection<UsersJdr> UsersJdrs { get; set; } = new List<UsersJdr>();
}
