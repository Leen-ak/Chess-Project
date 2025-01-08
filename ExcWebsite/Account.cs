using System;
using System.Collections.Generic;

namespace ExcWebsite;

public partial class Account
{
    public int UserAccountID { get; set; }

    public int? UserID { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public virtual UserInfo? User { get; set; }
}
