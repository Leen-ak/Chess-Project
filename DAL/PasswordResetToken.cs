using System;
using System.Collections.Generic;

namespace DAL;

public partial class PasswordResetToken : ChessEntity
{
    public int UserId { get; set; }

    public string? ResetToken { get; set; }

    public DateTime RestTokenExpiry { get; set; }

    public virtual UserInfo User { get; set; } = null!;
}
