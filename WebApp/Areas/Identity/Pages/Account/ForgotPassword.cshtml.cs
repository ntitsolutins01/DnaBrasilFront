﻿using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using WebApp.Data;
using WebApp.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace WebApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IHostingEnvironment _host;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IOptions<ParametersModel> _parameters;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender,
            IHostingEnvironment host,
            ApplicationDbContext db,
            IOptions<ParametersModel> parameters
            )
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _host = host;
            _db = db;
            _parameters = parameters;
        }

        [BindProperty] public InputModel Input { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Usuário não cadastrado.");
                    return Page();
                }
                //|| !await _userManager.IsEmailConfirmedAsync(user))
                    // Don't reveal that the user does not exist or is not confirmed
                    //return RedirectToPage("./ForgotPasswordConfirmation");

                await SendForgotPasswordEmail(user, Input.Email);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }

        private async Task SendForgotPasswordEmail(IdentityUser user, string email)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                null,
                new { code, email },
                Request.Scheme);


            var message =
                System.IO.File.ReadAllText(Path.Combine(_host.WebRootPath, "EmailTemplates/ForgotPassword.html"));
            message = message.Replace("%NAME%", user.UserName);
            message = message.Replace("%SITE%", "https://dnabrasil.net");
            message = message.Replace("%TEMPOTOKEN%", _parameters.Value.TokenTime.ToString());
            message = message.Replace("%HORA%", DateTime.Now.ToString("hh:mm"));
            message = message.Replace("%DATA%", DateTime.Now.ToString("dd/MM/yyyy"));
            message = message.Replace("%CALLBACK%", HtmlEncoder.Default.Encode(callbackUrl));





            await _emailSender.SendEmailAsync(user.Email, "Recupere sua senha do sistema Dna Brasil",
                message);
        }

        public class InputModel
        {
            [Required]
            [RegularExpression(@"^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$", ErrorMessage = "O e-mail informado deve atender um formato padrão válido.")]
            public string Email { get; set; }
        }
    }
}