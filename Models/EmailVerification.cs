using System;
using System.Collections.Generic;

namespace LibraryApp.Models;

public partial class EmailVerification
{
    public int EmailVerificationId { get; set; }

    public string Email { get; set; } = null!;

    public string OtpCode { get; set; } = null!;

    public DateTime ExpiredAt { get; set; }

    public bool IsVerified { get; set; }

    public DateTime CreatedAt { get; set; }
}
