using System;
using System.Collections.Generic;

namespace DAL;

public partial class UserInfo : ChessEntity
{
    //public int Id { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    //public byte[] Timer { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
