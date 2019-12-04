using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EagleEye.Controllers
{
	/// <summary>
	/// Provides a place for non-model related actions
	/// </summary>
	public class HomeController : Controller
	{
		/// <summary>
		/// Creates a homepage view
		/// </summary>
		/// <returns>An html view</returns>
		public ActionResult Index()
		{
			return View();
		}
		public ActionResult Error()
		{
			return View("~/Views/Shared/Error.cshtml");
		}
	}
}