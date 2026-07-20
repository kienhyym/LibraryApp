namespace LibraryApp.Results;

public class LoginResult
{
    public bool Success { get; set; }

    public string? ErrorMessage { get; set; }

    public int? MaTaiKhoan { get; set; }

    public string? TenDangNhap { get; set; }

    public string? VaiTro { get; set; }
}