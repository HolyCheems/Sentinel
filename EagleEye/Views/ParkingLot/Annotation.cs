using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Views.ParkingLot
{
	/// <summary>
	/// A ViewModel for the Annotation Model.
	/// This is used to provide information about an annotation
	/// to views
	/// </summary>
	public class Annotation
	{
		/// <summary>
		/// Necessary for the framework to do ViewModel binding from POST requests
		/// </summary>
		public Annotation() { }
		/// <summary>
		/// Constructs an Annotation ViewModel from an Annotation Model
		/// </summary>
		/// <param name="annotation">The Annotation model</param>
		public Annotation(Models.Annotation annotation)
		{
			ID = annotation.ID;
			Type = annotation.Type.ToString();
			Points = annotation.Points.Select(p => new Vector2(p)).ToList();
			PercentDifference = annotation.PercentDifference;
		}
		/// <summary>
		/// The Annotation id
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// The Annotation Type name
		/// </summary>
		/// <see cref="Models.Annotation.AnnotationType"/>
		public string Type { get; set; }
		/// <summary>
		/// The PercentDifference
		/// </summary>
		public double PercentDifference { get; set; }
		/// <summary>
		/// The Annotation Points
		/// </summary>
		public List<Vector2> Points { get; set; }
	}
}