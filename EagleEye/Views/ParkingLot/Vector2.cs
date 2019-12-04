using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Views.ParkingLot
{
	/// <summary>
	/// A ViewModel for the Vector2 Model.
	/// This is used to provide information about a 2d vector
	/// to views
	/// </summary>
	public class Vector2
	{
		/// <summary>
		/// Necessary for the framework to do ViewModel binding from POST requests
		/// </summary>
		public Vector2() { }
		/// <summary>
		/// Constructs a Vector2 ViewModel from a Vector2 Model
		/// </summary>
		/// <param name="vector">The Vector2 model</param>
		public Vector2(Models.Geometry.Vector2 vector)
		{
			X = vector.X;
			Y = vector.Y;
		}
		/// <summary>
		/// The X coordinate
		/// </summary>
		public double X { get; set; }
		/// <summary>
		/// The Y coordinate
		/// </summary>
		public double Y { get; set; }
	}
}