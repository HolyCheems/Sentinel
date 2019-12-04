using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
namespace EagleEye
{
	/// <summary>
	/// Defines helpful functions for use in views
	/// </summary>
	public static class HtmlHelpers
	{
		/// <summary>
		/// Tests whether the current user is an Administrator
		/// </summary>
		/// <param name="request">The request base that this extension is applied to</param>
		/// <returns>true if the user is an Administrator, else false</returns>
		public static bool IsAdmin(this HttpRequestBase request)
		{
			return request.GetOwinContext().Authentication.User.IsInRole(EagleEye.Views.User.AccessLevel.Administrator.ToString());
		}
	}
}