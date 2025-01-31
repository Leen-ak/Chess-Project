using System;
using System.Collections.Generic;

namespace DAL;

public partial class UserInfo : ChessEntity
{
   // public int Id { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte[]? Picture { get; set; }

 //   public byte[] Timer { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Follower> FollowerFollowerNavigations { get; set; } = new List<Follower>();

    public virtual ICollection<Follower> FollowerFollowings { get; set; } = new List<Follower>();
}
