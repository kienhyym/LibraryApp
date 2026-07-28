using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Personnel")]
public abstract class AdminBaseController : Controller
{
}