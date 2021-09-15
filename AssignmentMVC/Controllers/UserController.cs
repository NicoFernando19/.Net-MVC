using AssignmentMVC.Data.Entities;
using AssignmentMVC.Helper;
using AssignmentMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AssignmentMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;

        public UserController(SignInManager<Users> signInManager,
            UserManager<Users> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LoginPost([Bind("Email, Password")] LoginForm loginForm)
        {
            var user = await _userManager.FindByNameAsync(loginForm.Email);
            if(user == null || !await _userManager.CheckPasswordAsync(user, loginForm.Password))
            {
                ViewBag.FailedLogin = "Incorrect Username and/or Password entered.";
            }
            else
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginForm.Password, false);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, true);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View("~/Views/User/Login.cshtml");
        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> RegisterPost([Bind("Name,Email,Password,ConfirmPassword")] RegistrationForm registrationForm)
        {
            var user = await _userManager.FindByEmailAsync(registrationForm.Email);
            if (user == null)
            {
                var newUser = new Users { Email = registrationForm.Email, UserName = registrationForm.Email, Name = registrationForm.Name };
                var result = await _userManager.CreateAsync(newUser, registrationForm.Password);

                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(newUser, "User");
                    return RedirectToAction("Login");
                }
            }
            ViewBag.EmailExist = "Email already exist!";
            return View("~/Views/User/Register.cshtml");
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult> LogOutPost()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPut]
        public async Task<ActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            string Decodetoken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordModel.Token));
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            var result = await _userManager.ResetPasswordAsync(user, Decodetoken, resetPasswordModel.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }
            return RedirectToAction("Login");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SendResetPaswordLink(ValidateEmail validateEmail)
        {
            var user = await _userManager.FindByEmailAsync(validateEmail.Email);
            if(user != null)
            {
                string[] email = { validateEmail.Email };
                string url = "https://localhost:44396/reset-password";
                string emailFrom = "email-from-here";
                var generateToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                generateToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(generateToken));
                string passwordResetUrl = $"{url}?email={validateEmail.Email}&token={generateToken}";
                var mail_template = "<table width='100%' cellspacing='40' cellpadding='0' bgcolor='#F5F5F5'><tbody><tr><td>";
                mail_template += "<table width='100%' cellspacing='0' cellpadding='0' border='0' bgcolor='#F5F5F5' style='border-spacing:0;font-family:sans-serif;color:#475159;margin:0 auto;width:100%;max-width:70%'><tbody>";
                mail_template += "<tr><td style='padding-top:20px;padding-left:0px;padding-right:0px;width:100%;text-align:right; font-size:12px;line-height:22px'>This email is sent from AssignmentMVC</td></tr>";
                mail_template += "</tbody></table>";
                mail_template += "<table width='100%' cellspacing='0' cellpadding='0' border='0' bgcolor='#F5F5F5' style='padding: 50px; border-spacing:0;font-family:sans-serif;color:#475159;margin:0 auto;width:100%;max-width:70%; background-color:#ffffff;'><tbody>";
                mail_template += "<tr><td style='font-weight:bold;font-family:Arial,sans-serif;font-size:36px;line-height:42px'>Reset Password Link</td></tr>";
                mail_template += "<tr><td style='padding-top:25px;padding-bottom:40px; font-size:16px;'>";
                mail_template += "<p style='display:block;margin-bottom:10px;'>Here is your reset password link:<br> <a href='" + HtmlEncoder.Default.Encode(passwordResetUrl) + "'><button style='border: none; color: white; padding: 10px 20px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer; background-color: #008CBA;'> Click here </button></a></p>";
                mail_template += "</td></tr>";
                mail_template += "<tr><td style='padding-top:16px;font-size:12px;line-height:24px;color:#767676; border-top:1px solid #f5f7f8;'>Now</td></tr>";
                mail_template += "<tr><td style='font-size:12px;line-height:24px;color:#767676'>From: " + emailFrom + "</td></tr>";
                mail_template += "</tbody></table>";
                mail_template += "</td></tr></tbody></table>";
                EmailHelper.SendEmail(email, emailFrom, "Reset Password Link", mail_template);
                ViewBag.SentMail = "Reset password link has been sent to your email address!";
            }
            else
            {
                ViewBag.SentMail = "Email not match with our records!";
            }
            return View("~/Views/User/ForgotPassword.cshtml");
        }
    }
}
