using System;
using System.Collections.Generic;

namespace DAL;

public partial class Follower : ChessEntity
{
    public int? FollowerId { get; set; }

    public int? FollowingId { get; set; }

    public string? Status { get; set; }

    public virtual UserInfo? FollowerNavigation { get; set; }

    public virtual UserInfo? Following { get; set; }
}
