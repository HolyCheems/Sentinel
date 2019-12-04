using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Views.ParkingLot
{
	/// <summary>
	/// A ViewModel for the ParkingLot Model.
	/// This is used to provide information about a parking lot
	/// to views
	/// </summary>
	public class ParkingLot
	{
		/// <summary>
		/// Necessary for the framework to do ViewModel binding from POST requests
		/// </summary>
		public ParkingLot() { }
		/// <summary>
		/// Constructs a ParkingLot ViewModel from a ParkingLot Model
		/// </summary>
		/// <param name="lot">The ParkingLot Model</param>
		public ParkingLot(Models.ParkingLot lot)
		{
			ID = lot.ID;
			Name = lot.Name;
			Annotations = lot.Annotations.Select(a => new Annotation(a)).ToList();
			if (lot.Camera != null)
				CameraID = lot.Camera.ID;
		}
		/// <summary>
		/// The ParkingLot id
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// The ParkingLot name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// The ParkingLot camera id
		/// </summary>
		public int CameraID { get; set; }
		/// <summary>
		/// The ParkingLot annotations
		/// </summary>
		public List<Annotation> Annotations { get; set; }
	}
}