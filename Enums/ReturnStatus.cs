using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Enums;

public enum ReturnStatus
{
    [Display(Name = "Nhận sách")]
    Received = 1,

    [Display(Name = "Không nhận sách")]
    NotReceived = 2
}