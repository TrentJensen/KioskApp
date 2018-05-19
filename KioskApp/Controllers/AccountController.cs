using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KioskApp.Models;
using KioskApp.Models.AccountViewModels;
using KioskApp.Services;
using KioskApp.Data;
using Microsoft.EntityFrameworkCore;
using KioskApp.ViewModels;

namespace KioskApp.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICustomerRepository _customerRepository;
        private readonly IVendorRepository _vendorRepository;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            ApplicationDbContext applicationDbContext,
            ICustomerRepository customerRepository,
            IVendorRepository vendorRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _customerRepository = customerRepository;
            _vendorRepository = vendorRepository;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} logged in with a recovery code.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.LogWarning("Invalid recovery code entered for user with ID {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User created a new account with password.");

                    //Create a vendor with the identity's UserId
                    if (model.Role[0].Equals("Vendor"))
                    {
                        var vendor = new Vendor
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            LoginId = user.Id
                        };
                        _applicationDbContext.Vendors.Add(vendor);
                        _applicationDbContext.SaveChanges();
						PopulateNewVendorInventory(vendor.LoginId, vendor.Id);
                        return RedirectToLocal(returnUrl);
                    }
                    else if (model.Role[0].Equals("Customer"))
                    {
                        var cust = new Customer
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Email = model.Email,
                            LoginId = user.Id,
                        };
                        _applicationDbContext.Customers.Add(cust);
                        _applicationDbContext.SaveChanges();

                        return RedirectToAction("CreateCustomer");
                    }

                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                   $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult CreateCustomer()
        {
            //Get the Guid of the current vendor from the UserManager and use that to get only the vendor's products
            var userGuid = _userManager.GetUserId(HttpContext.User);

            //Get customer and Vendors to pass into Create Customer view
            Customer cust = _customerRepository.GetCustomerByGuid(userGuid);
            List<Vendor> vendors = _vendorRepository.Vendors.ToList();
            List<SelectListItem> vendorList = new List<SelectListItem>();
            foreach (var vendor in vendors)
            {
                vendorList.Add(new SelectListItem { Text = vendor.FirstName, Value = vendor.Id.ToString() });
            }
            CreateCustomerViewModel createCustomerViewModel = new CreateCustomerViewModel
            {
                Vendors = vendorList,
                Customer = cust
            };

            return View(createCustomerViewModel);
        }

        [HttpPost]
        public IActionResult CreateCustomer(CreateCustomerViewModel cust)
        {
            //Get the customer
            var userGuid = _userManager.GetUserId(HttpContext.User);
            Customer customer = _customerRepository.GetCustomerByGuid(userGuid);

            customer.Address = cust.Customer.Address;
            customer.City = cust.Customer.City;
            customer.State = cust.Customer.State;
            customer.Zip = cust.Customer.Zip;
            customer.Loyalty = cust.Customer.Loyalty;
            customer.VendorId = cust.Customer.VendorId;

            _applicationDbContext.Customers.Add(customer);
            _applicationDbContext.Entry(customer).State = EntityState.Modified;
            _applicationDbContext.SaveChanges();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

		private void PopulateNewVendorInventory(string loginId, int vendorId)
		{

			{
				_applicationDbContext.Products.AddRangeAsync
					(
					new Product
					{
						Name = "EnviroCloth",
						Description = "Dense microfiber cloth",
						Price = 17.99M,
						VendorCost = 11.70M,
						Color = ColorEnum.Graphite,
						Image = @"envirocloth-graphite.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					}, 
					new Product
					{
						Name = "EnviroCloth",
						Description = "Dense microfiber cloth",
						Price = 17.99M,
						VendorCost = 11.70M,
						Color = ColorEnum.Pink,
						Image = @"envirocloth-pink.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					}, 
					new Product
					{
						Name = "EnviroCloth",
						Description = "Dense microfiber cloth",
						Price = 17.99M,
						VendorCost = 11.70M,
						Color = ColorEnum.Green,
						Image = @"envirocloth-green.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					}, 
					new Product
					{
						Name = "EnviroCloth",
						Description = "Dense microfiber cloth",
						Price = 17.99M,
						VendorCost = 11.70M,
						Color = ColorEnum.Blue,
						Image = @"envirocloth-blue.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					}, 
					new Product
					{
						Name = "Window Cloth",
						Description = "Polishing microfiber",
						Price = 19.99M,
						VendorCost = 13.00M,
						Color = ColorEnum.Purple,
						Image = @"window-cloth.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					}, 
					new Product
					{
						Name = "Window Cloth",
						Description = "Polishing microfiber",
						Price = 19.99M,
						VendorCost = 13.00M,
						Color = ColorEnum.Pink,
						Image = @"window-cloth.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					},

					new Product
					{
						Name = "Body Cloth Pack",
						Description = "Dense microfiber cloth",
						Price = 19.99M,
						VendorCost = 13.00M,
						Color = ColorEnum.Tranquil,
						Image = @"three-pack-tranquil.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					}, 
					new Product
					{
						Name = "Body Cloth Pack",
						Description = "Dense microfiber cloth",
						Price = 19.99M,
						VendorCost = 13.00M,
						Color = ColorEnum.Vibrant,
						Image = @"three-pack-vibrant.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					}, 
					new Product
					{
						Name = "Body Cloth Pack",
						Description = "Dense microfiber cloth",
						Price = 19.99M,
						VendorCost = 13.00M,
						Color = ColorEnum.Graphite,
						Image = @"three-pack-graphite.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					}, 
					new Product
					{
						Name = "Body Cloth Pack",
						Description = "Dense microfiber cloth",
						Price = 19.99M,
						VendorCost = 13.00M,
						Color = ColorEnum.Coastal,
						Image = @"three-pack-coastal.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					}, 
					new Product
					{
						Name = "Body Cloth",
						Description = "Dense microfiber cloth",
						Price = 7.00M,
						VendorCost = 4.55M,
						Color = ColorEnum.Denim,
						Image = @"three-pack-tranquil.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					}, new Product
					{
						Name = "Body Cloth",
						Description = "Dense microfiber cloth",
						Price = 7.00M,
						VendorCost = 4.55M,
						Color = ColorEnum.Lavender,
						Image = @"three-pack-tranquil.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					}, 
					new Product
					{
						Name = "Body Cloth",
						Description = "Dense microfiber cloth",
						Price = 7.00M,
						VendorCost = 4.55M,
						Color = ColorEnum.Vanilla,
						Image = @"three-pack-tranquil.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					}, 
					new Product
					{
						Name = "Body Cloth",
						Description = "Dense microfiber cloth",
						Price = 7.00M,
						VendorCost = 4.55M,
						Color = ColorEnum.Yellow,
						Image = @"three-pack-vibrant.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					},
					new Product
					{
						Name = "Body Cloth",
						Description = "Dense microfiber cloth",
						Price = 7.00M,
						VendorCost = 4.55M,
						Color = ColorEnum.Pink,
						Image = @"three-pack-vibrant.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					},
					new Product
					{
						Name = "All-Purpose Cloth",
						Description = "Microfiber cloth for use anywhere",
						Price = 13.99M,
						VendorCost = 9.10M,
						Color = ColorEnum.None,
						Image = @"all-purpose-kitchen-cloth.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 5
					},
					new Product
					{
						Name = "Baby Body Pack",
						Description = "Smaller body cloth",
						Price = 14.99M,
						VendorCost = 9.75M,
						Color = ColorEnum.None,
						Image = @"baby-body-pack.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 7
					},
					new Product
					{
						Name = "Body Scrub Mitt",
						Description = "Rejuvenate your entire body with our Body Scrub Mitt.",
						Price = 14.99M,
						VendorCost = 9.75M,
						Color = ColorEnum.None,
						Image = @"body-scrub-mitt.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 3
					},
					new Product
					{
						Name = "Bathroom Scrub Mitt",
						Description = "Microfiber on one side, scrub mesh on the other",
						Price = 24.99M,
						VendorCost = 16.24M,
						Color = ColorEnum.Graphite,
						Image = @"bathroom-scrubb-mitt-graphite.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 4
					},
					new Product
					{
						Name = "Bath Towel",
						Description = "Large microfiber bath towel with silver",
						Price = 39.99M,
						VendorCost = 25.99M,
						Color = ColorEnum.Denim,
						Image = @"bath-towel-denim.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 3
					},
					new Product
					{
						Name = "Bath Towel",
						Description = "Large microfiber bath towel with silver",
						Price = 39.99M,
						VendorCost = 25.99M,
						Color = ColorEnum.Lavender,
						Image = @"bath-towel-lavender.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 3
					},
					new Product
					{
						Name = "Bath Towel",
						Description = "Large microfiber bath towel with silver",
						Price = 39.99M,
						VendorCost = 25.99M,
						Color = ColorEnum.Teal,
						Image = @"bath-towel-teal.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 3
					},
					new Product
					{
						Name = "Bath Towel",
						Description = "Large microfiber bath towel with silver",
						Price = 39.99M,
						VendorCost = 25.99M,
						Color = ColorEnum.Graphite,
						Image = @"bath-towel-graphite.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 3
					},
					new Product
					{
						Name = "Bath Towel",
						Description = "Large microfiber bath towel with silver",
						Price = 39.99M,
						VendorCost = 25.99M,
						Color = ColorEnum.Vanilla,
						Image = @"bath-towel-vanilla.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 3
					},
					new Product
					{
						Name = "Blue Diamond Bathroom Cleaner",
						Description = "Enzyme for cleaning mold, mildew, and urine",
						Price = 29.99M,
						VendorCost = 19.49M,
						Color = ColorEnum.Denim,
						Image = @"blue-diamond-bathroom-cleaner.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 4
					},
					new Product
					{
						Name = "Body Balm",
						Description = "Glide away chapped skin on elbows, hands, and feet",
						Price = 19.99M,
						VendorCost = 12.99M,
						Color = ColorEnum.None,
						Image = @"body-balm.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 6
					},
					new Product
					{
						Name = "Bath Towel",
						Description = "Large microfiber bath towel with silver",
						Price = 39.99M,
						VendorCost = 25.99M,
						Color = ColorEnum.Denim,
						Image = @"bath-towel-denim.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 3
					}, 
					new Product
					{
						Name = "Car Cloth",
						Description = "Large microfiber cloth for drying your car",
						Price = 25.99M,
						VendorCost = 16.89M,
						Color = ColorEnum.None,
						Image = @"car-mitt-car-cloth.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 7
					},
					new Product
					{
						Name = "Car Wash Mitt",
						Description = "Layered microfiber mitt to polish your car",
						Price = 26.99M,
						VendorCost = 17.54M,
						Color = ColorEnum.None,
						Image = @"car-mitt-car-cloth.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 7
					},
					new Product
					{
						Name = "Chenille Hand Towel",
						Description = "Microfiber fingered hand towel with silver",
						Price = 24.99M,
						VendorCost = 16.29M,
						Color = ColorEnum.None,
						Image = @"chenille-hand-towels-all-colors.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 3
					},
					new Product
					{
						Name = "Cleaning Paste",
						Description = "Paste for scrubbing with minimum abrasion",
						Price = 29.99M,
						VendorCost = 19.49M,
						Color = ColorEnum.None,
						Image = @"cleaning-paste.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 5
					},
					new Product
					{
						Name = "Counter Cloth",
						Description = "Microfiber cloth to replace paper towels",
						Price = 19.99M,
						VendorCost = 12.99M,
						Color = ColorEnum.Blue,
						Image = @"counter-cloths-marine-teal-seamist.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 5
					},
					new Product
					{
						Name = "Counter Cloth",
						Description = "Microfiber cloth to replace paper towels",
						Price = 19.99M,
						VendorCost = 12.99M,
						Color = ColorEnum.Graphite,
						Image = @"counter-cloths-slate-vanilla-mushroom.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 5
					},
					new Product
					{
						Name = "Crystal Deoderant",
						Description = "Deoderizing crystal for use on your body",
						Price = 13.99M,
						VendorCost = 9.09M,
						Color = ColorEnum.None,
						Image = @"crystal-deoderant.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 6
					},
					new Product
					{
						Name = "Cutting Board (Large)",
						Description = "Large rice husk cutting board",
						Price = 39.99M,
						VendorCost = 25.99M,
						Color = ColorEnum.None,
						Image = @"cutting-board.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 4
					},
					new Product
					{
						Name = "Deodorant Stick",
						Description = "Natural chemical free deoderant",
						Price = 15.99M,
						VendorCost = 10.39M,
						Color = ColorEnum.None,
						Image = @"deodorant-stick.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 6
					},
					new Product
					{
						Name = "Descaler",
						Description = "Enzyme for lime and rust",
						Price = 29.99M,
						VendorCost = 19.49M,
						Color = ColorEnum.None,
						Image = @"descaler.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 4
					},
					new Product
					{
						Name = "Dish Cloth",
						Description = "Microfiber cloth for drying dishes",
						Price = 9.99M,
						VendorCost = 6.49M,
						Color = ColorEnum.Blue,
						Image = @"dish-cloth-blue.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 5
					},

					new Product
					{
						Name = "All Purpose Kitchen Cloth",
						Description = "All purpose microfiber",
						Price = 13.99M,
						VendorCost = 9.10M,
						Color = ColorEnum.Graphite,
						Image = @"all-purpose-kitchen-cloth.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 5
					}, 
					new Product
					{
						Name = "Kitchen Scrub Cloth",
						Description = "Scrubbing microfiber",
						Price = 13.99M,
						VendorCost = 9.10M,
						Color = ColorEnum.Blue,
						Image = @"kitchen-scrub-cloth.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 5
					}, 
					new Product
					{
						Name = "Veggie and Fruit Scrub Cloth",
						Description = "Scrubs and peels vegetables and fruit",
						Price = 13.99M,
						VendorCost = 9.10M,
						Color = ColorEnum.Green,
						Image = @"veggie-and-fruit-scrub-cloth.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					},                   
					new Product
					{
						Name = "Veggie and Fruit Scrub Cloth",
						Description = "Scrubs and peels vegetables and fruit",
						Price = 13.99M,
						VendorCost = 9.10M,
						Color = ColorEnum.Green,
						Image = @"veggie-and-fruit-scrub-cloth.jpg",
						UnitsInStock = 0,
						IsLimitedEdition = false,
						VendorId = vendorId,
						VendorGuid = loginId,
						CategoryId = 1
					}




					);
			}
		}

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
