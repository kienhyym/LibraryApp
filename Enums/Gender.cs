using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Enums;

public enum Gender
{
    [Display(Name = "Nam")]
    Male = 1,

    [Display(Name = "Nữ")]
    Female = 2,

    [Display(Name = "Khác")]
    Other = 3
}