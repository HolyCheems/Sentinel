using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Views.User
{
	/// <summary>
	/// Defines the possible levels of access that a user can have
	/// </summary>
	public enum AccessLevel
	{
		User,
		Administrator
	}
	/// <summary>
	/// A ViewModel for users
	/// This is used to provide information about a user
	/// to views
	/// </summary>
	public class User
	{
		/// <summary>
		/// Necessary for the framework to do ViewModel binding from POST requests
		/// </summary>
		public User() { }
		/// <summary>
		/// The user's access level
		/// </summary>
		public AccessLevel AccessLevel { get; set; }
	}
}