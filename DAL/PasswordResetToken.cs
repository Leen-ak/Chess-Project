using System;
using System.Collections.Generic;

namespace DAL;

public partial class PasswordResetToken : ChessEntity
{
  //  public int Id { get; set; }

    public int UserId { get; set; }

    public string ResetToken { get; set; } = null!;

    public DateTime RestTokenExpiry { get; set; }

  //  public byte[] Timer { get; set; } = null!;

    public virtual UserInfo User { get; set; } = null!;
}
