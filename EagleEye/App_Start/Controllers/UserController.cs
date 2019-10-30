using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
namespace EagleEye.Controllers
{
	/// <summary>
	///	Provides authentication actions for unauthenticated
	///	users to login to the system as either an administrator
	///	or standard user
	/// </summary>
	/// <remarks>Author: Gage Coates</remarks>
	[AllowAnonymous]
	public class UserController : Controller
    { 
		/// <summary>
		/// Creates a login view
		/// </summary>
		/// <returns>
		/// An html view
		/// </returns>
		[HttpGet]
        public ActionResult Index()
        {
            return View("Login",new Views.User.User());
        }
		/// <summary>
		/// Logs a user in and provisions them with the
		/// appropriate role. Subsiquent requests are
		///	authenticated with this role
		/// </summary>
		/// <returns>A redirect to /Home/Index</returns>

		[HttpPost]
		public ActionResult Login(Views.User.User user)
		{
			var identity = new ClaimsIdentity(new[] {
				new Claim(ClaimTypes.Role, user.AccessLevel.ToString())
			}, "ApplicationCookie");

			var ctx = Request.GetOwinContext();
			var authManager = ctx.Authentication;
			authManager.SignIn(identity);

			return RedirectToAction("Index", "Home");
		}
		/// <summary>
		/// Logs a user out and unauthenticates requests
		/// </summary>
		/// <returns>A redirect to /User/Index</returns>
		[HttpGet]
		public ActionResult Logout()
		{
			var ctx = Request.GetOwinContext();
			var authManager = ctx.Authentication;

			authManager.SignOut("ApplicationCookie");
			return RedirectToAction("Index", "User");
		}
	}
}