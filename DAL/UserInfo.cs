using System;
using System.Collections.Generic;

namespace DAL;

public partial class UserInfo
{
    public int UserID { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Phonenumber { get; set; }

    public string? Email { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
