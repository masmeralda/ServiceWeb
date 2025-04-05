using Service.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Service.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email
                })
                .ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(
                    await _userManager.FindByIdAsync(user.Id));
                user.Roles = _roleManager.Roles
                    .Select(r => new RoleViewModel
                    {
                        Name = r.Name,
                        IsSelected = roles.Contains(r.Name)
                    })
                    .ToList();
            }

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return View(new UserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Roles = _roleManager.Roles
                    .Select(r => new RoleViewModel
                    {
                        Name = r.Name,
                        IsSelected = roles.Contains(r.Name)
                    })
                    .ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null) return NotFound();

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.UserName = model.Email;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                    var selectedRoles = model.Roles
                        .Where(r => r.IsSelected)
                        .Select(r => r.Name);

                    await _userManager.AddToRolesAsync(user, selectedRoles);

                    return RedirectToAction("Users");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                // Обработка ошибок
            }

            return RedirectToAction("Users");
        }


        [HttpGet]
        public async Task<IActionResult> CreateUser()
        {
            var model = new CreateUserViewModel
            {
                Roles = (await _roleManager.Roles.ToListAsync())
                    .Select(r => new RoleSelection
                    {
                        RoleId = r.Id,
                        RoleName = r.Name,
                        IsSelected = false
                    }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    RegistrationDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Назначаем выбранные роли
                    var selectedRoles = model.Roles
                        .Where(r => r.IsSelected)
                        .Select(r => r.RoleName);

                    if (selectedRoles.Any())
                    {
                        await _userManager.AddToRolesAsync(user, selectedRoles);
                    }

                    return RedirectToAction("Users");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Если что-то пошло не так, перезагружаем список ролей
            model.Roles = (await _roleManager.Roles.ToListAsync())
                .Select(r => new RoleSelection
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    IsSelected = model.Roles?.Any(mr => mr.RoleId == r.Id && mr.IsSelected) ?? false
                }).ToList();

            return View(model);
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Панель администратора";
            return View();
        }
    }


}
