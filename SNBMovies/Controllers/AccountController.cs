using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SNBMovies.Data;
using SNBMovies.Data.Roles;
using SNBMovies.Models;
using SNBMovies.Models.ViewModels;
using System.Net;
using System.Net.Mail;

namespace SNBMovies.Controllers
{
    /// @Author Атанас Цветков
    /// <summary>
    /// Това е контролер, който извършва операции, свързани с потребителските акаунти.
    /// Той се състои от нялколко метода за профил на потребител, вход, регистрация, изход, изтриване на потребител и забранен достъп.
    /// </summary>

    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        //
        // Обобщение:
        //     Дисплей на списък с всички регистрирани потребители в приложението
        //     
        // Връща:
        //     Изглед с модел - списък с обекти "ApplicationUser"
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            // Променлива users се инициализира със списък от всички потребители, извлечени от базата.
            return View(users);
            // Списъкът с потребителите се подава като модел на изгледа и се връща на клиента.
        }

        #region UserProfile
        //
        // Обобщение:
        //     Дисплей на профилна страница на потребител
        //     
        // Връща:
        //     Изглед с модел - "Applicationuser"
        public async Task<IActionResult> UserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            // Променлива user се инициализира с текущия потребител, използвайки UserManager и асинхронна операция.
            if (user == null)
            {// Ако потребителят не съществува, се връща грешка "Not Found" (404).
                return NotFound();
            }
            return View(user);
            // Иначе се връща изгледа, свързан с потребителя
        }

        //
        // Обобщение:
        //     Дисплей на страница за промяна на информацията в профил
        //     
        // Връща:
        //     Изглед с модел - "ApplicationUser"
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            // Променлива user се инициализира с текущия потребител, използвайки UserManager и асинхронна операция.
            if (user == null)
            {// Ако потребителят не съществува, се връща изгледа "NotFound".
                return View("NotFound");
            }

            var model = new EditProfileVM
            {// Създава се модел EditProfileVM с информацията на потребителя.
                FullName = user.FullName,
                EmailAddress = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
            // Изгледът се връща със създадения модел като модел.
        }

        //
        // Обобщение:
        //     Извършване на действието за промяна на информация в профил
        //     
        // Параметри:
        //   profile:
        //      Моделът, изобразен от изгледа.
        //
        // Връща:
        //     Изглед с модел - "EditProfileVM"
        //     или
        //     Изглед "userProfile"
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileVM profile)
        {
            if (!ModelState.IsValid) return View(profile);
            // Ако моделът не е валиден, се връща отново изгледа на модела.

            var user = await _userManager.GetUserAsync(User);
            // Променлива user се инициализира с текущия потребител, използвайки UserManager и асинхронна операция.
            if (user != null)
            {// Ако потребителят не е null, тогава се обновяват неговите данни.
                user.FullName= profile.FullName;
                user.PhoneNumber= profile.PhoneNumber;
                user.UserName = profile.EmailAddress;

                if (!string.IsNullOrEmpty(profile.EmailAddress))
                {// Ако имейлът не е празен, тогава се обновява и полето Email на потребителя.
                    user.Email = profile.EmailAddress;
                }

                var response = await _userManager.UpdateAsync(user);
                // Изпълнява се обновяването на потребителя чрез UserManager и асинхронна операция.
                if (response.Succeeded)
                {
                    return RedirectToAction("UserProfile");
                    // Ако обновяването е успешно, връща се изгледа UserProfile.
                }
                else
                {
                    return View(profile);
                    // Иначе се връща изгледа с модела profile, като модел
                }
            }
            return View(profile);
        }
        #endregion

        #region Login, Register, Logout
        //
        // Обобщение:
        //     Дисплей на вход за приложението
        //
        // Връща:
        //     Изглед с модел - "LoginVM"
        public IActionResult Login() => View(new LoginVM());

        //
        // Обобщение:
        //     Извършване на действието за вход в приложението
        //     
        // Параметри:
        //   loginVM:
        //      Моделът, изобразен от изгледа.
        //
        // Връща:
        //     Изглед с модел - "LoginVM" 
        //     или
        //     Пренасочване към метод "Index" в контролер "Home"
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);
            // Ако моделът не е валиден, се връща изгледа с loginVM модела, заедно с грешките на модела.

            var user = await _userManager.FindByEmailAsync(loginVM.EmailAddress);
            // Търси потребител по имейла, използвайки UserManager и асинхронна операция.
            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                // Ако потребителят не е null, тогава се проверява дали въведената парола е вярна.
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    // Ако паролата е вярна, потребителят се вписва чрез SignInManager и асинхронна операция.
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                        // Ако вписването е успешно, потребителят се препраща към изгледа на Home контролера.
                    }
                }
                TempData["ErrorPass"] = "Грешена парола!";
                TempData["ErrorLocation"] = "pass";
                return View(loginVM);
                // Ако паролата не е вярна, се връща изгледа с loginVM модела, заедно с грешката за грешна парола.
            }

            TempData["ErrorEmail"] = "Грешен имейл!";
            TempData["ErrorLocation"] = "email";
            return View(loginVM);
            // Ако потребителят не е намерен, се връща изгледа с loginVM модела, заедно с грешката за грешен имейл.
        }

        //
        // Обобщение:
        //     Създава роли при първа регистрация в приложението
        //
        // Връща:
        //     Изглед за регистрация с модел - "RegisterVM"
        [HttpGet]
        public async Task<IActionResult> Register() 
        {

            if (!_roleManager.RoleExistsAsync(UserRoles.Admin).GetAwaiter().GetResult())
            {// Проверява дали ролята на "Администратор" вече съществува в базата данни, използвайки RoleManager и синхронен метод.
                 await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                 await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                // Ако ролята "Администратор" не съществува, се създава заедно с ролята "Потребител".
            }
            return View(new RegisterVM());
            // Връща изгледа за регистрация на нов потребител с празен RegisterVM модел.
        }

        //
        // Обобщение:
        //     Извършва действието за регистрация на потребител
        //     
        // Параметри:
        //   registerVM:
        //      Моделът, с данните напотребителя.
        //
        // Връща:
        //     Изглед с модел - "EditProfileVM"
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);
            // Проверява дали моделът за регистрация е валиден.

            var user = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
            // Търси потребител със същия имейл адрес в базата данни, използвайки UserManager и асинхронен метод.
            if (user != null)
            {
                TempData["Error"] = "Този имейл вече се използва!";
                TempData["ErrorLocation"] = "usedEmail";
                return View(registerVM);
                // Ако потребителят със същия имейл адрес вече съществува в базата данни, се връща същия изглед с грешка.
            }

            var newUser = new ApplicationUser()
            {
                FullName = registerVM.FullName,
                Email = registerVM.EmailAddress,
                UserName = registerVM.FullName

            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);
            // Създава нов потребител с данните от RegisterVM модела и създадената парола, използвайки UserManager и асинхронен метод.

            if (newUserResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                return View("RegisterSuccessful");
                // Ако създаването на потребителя е успешно, потребителят се добавя в ролята "Потребител" и се връща изглед за успешна регистрация.
            }
            else
            {
                return View(registerVM);
                // Ако създаването на потребителя е неуспешно, се връща същия изглед с грешките.
            }
        }

        //
        // Обобщение:
        //     Създава роли при първа регистрация в приложението
        //
        // Връща:
        //     Изглед за регистрация с модел - "RegisterVM"
        [HttpGet]
        public async Task<IActionResult> Reg1sterAdm1n()
        {

            if (!_roleManager.RoleExistsAsync(UserRoles.Admin).GetAwaiter().GetResult())
            {// Проверява дали ролята на "Администратор" вече съществува в базата данни, използвайки RoleManager и синхронен метод.
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                // Ако ролята "Администратор" не съществува, се създава заедно с ролята "Потребител".
            }
            return View(new RegisterVM());
            // Връща изгледа за регистрация на нов потребител с празен RegisterVM модел.
        }

        //
        // Обобщение:
        //     Извършва действието за регистрация на потребител
        //     
        // Параметри:
        //   registerVM:
        //      Моделът, с данните напотребителя.
        //
        // Връща:
        //     Изглед с модел - "EditProfileVM"
        [HttpPost]
        public async Task<IActionResult> Reg1sterAdm1n(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) return View(registerVM);
            // Проверява дали моделът за регистрация е валиден.

            var user = await _userManager.FindByEmailAsync(registerVM.EmailAddress);
            // Търси потребител със същия имейл адрес в базата данни, използвайки UserManager и асинхронен метод.
            if (user != null)
            {
                TempData["Error"] = "Този имейл вече се използва!";
                TempData["ErrorLocation"] = "usedEmail";
                return View(registerVM);
                // Ако потребителят със същия имейл адрес вече съществува в базата данни, се връща същия изглед с грешка.
            }

            var newUser = new ApplicationUser()
            {
                FullName = registerVM.FullName,
                Email = registerVM.EmailAddress,
                UserName = registerVM.FullName

            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);
            // Създава нов потребител с данните от RegisterVM модела и създадената парола, използвайки UserManager и асинхронен метод.

            if (newUserResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.Admin);
                return View("RegisterSuccessful");
                // Ако създаването на потребителя е успешно, потребителят се добавя в ролята "Администратор" и се връща изглед за успешна регистрация.
            }
            else
            {
                return View(registerVM);
                // Ако създаването на потребителя е неуспешно, се връща същия изглед с грешките.
            }
        }

        //
        // Обобщение:
        //     Преупълномощаване на потребител.
        //
        // Връща:
        //     Пренасочване към действие "Index" в контролер "Home"
        [HttpPost]
        public async Task<IActionResult> Logout() 
        {
            await _signInManager.SignOutAsync();
            //Прекратява текущата сесия на потребителя.
            return RedirectToAction("Index", "Home");
            //Пренасочва потребителя към начална страница.
        }
        #endregion

        //
        // Обобщение:
        //     Извършва изтриването на потребител от приложението
        //     
        // Параметри:
        //   id:
        //      Идентификационен номер на потребител
        //
        // Връща:
        //     Изглед с модел - "Applicationuser"
        //     или
        //     Пренасочване към действието "Index"
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            //Намира потребителя със съответния идентификационен номер.
            if (user == null)
            {//Проверка дали потребителят не е намерен.
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            //Изтрива се потребителят, чрвез UserManager.
            if (result.Succeeded)
            {//Проверка дали изтриването е успешно.
                return RedirectToAction("Index");
                //Пренасочване към началната страница.
            }

            return View(user);
        }

        //
        // Обобщение:
        //     Дисплей на страница за забранен досътп
        //
        // Връща:
        //     Изгледът "AccessDenied"
        public IActionResult AccessDenied()
        {
            return View();
        }

        //
        // Обобщение:
        //     Търси потребител, който съдържа съответния низ
        //     
        // Параметри:
        //   searchTerm:
        //      Низ, представляващ написаната стойност в полето за търсене.
        //
        // Връща:
        //     Съответният изглед с резултата от търсенето
        [AllowAnonymous]
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {//Ако търсеният термин е празен или null.
                return View("NotFound");
                //Показва съответната страница.
            }
            var results = _context.Users.Where(x => x.FullName.Contains(searchTerm));
            //Търси в базата данни за потребители, чиито имена съдържат търсения термин и записва резултата в results.
            return View("Index", results);
            //Показва списъка от резултати в гледката "Index".
        }
    }
}
