using System;
using System.Collections.Generic;

namespace DAL;

public partial class Account : ChessEntity
{
   // public int Id { get; set; }

    public int? UserID { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

   // public byte[] Timer { get; set; } = null!;

    public virtual UserInfo? User { get; set; }
}
