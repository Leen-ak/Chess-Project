using System;
using System.Collections.Generic;

namespace DAL;

public partial class Account : ChessEntity
{
   // public int Id { get; set; }

    public int? UserID { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

   // public byte[] Timer { get; set; } = null!;

    public virtual UserInfo? User { get; set; }
}
