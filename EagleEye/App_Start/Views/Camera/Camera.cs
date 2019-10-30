using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EagleEye.Models;
namespace EagleEye.Views.Camera
{
	/// <summary>
	/// A ViewModel representation of a Camera Model.
	/// This is used to provide information about a camera
	/// to views
	/// </summary>
	public class Camera
	{
		/// <summary>
		/// Necessary for the framework to do ViewModel binding from POST requests
		/// </summary>
		public Camera() { }
		/// <summary>
		/// Constructs a Camera ViewModel from the Camera Model
		/// </summary>
		/// <param name="camera">The model to be used</param>
		public Camera(Models.Camera camera)
		{
			ID = camera.ID;
			Name = camera.Name;
			CurrentImage = "";
			if (camera.CurrentImage != null)
			{
				using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
				{
					// Scaling
					//camera.CurrentImage.Scale(EagleEyeConfig.WebImageWidth).Save(stream, System.Drawing.Imaging.ImageFormat.Png);
					
					// No scaling
					camera.CurrentImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

					CurrentImage = System.Convert.ToBase64String(stream.GetBuffer());
				}
			}
		}
		/// <summary>
		/// The Camera id
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// The Camera name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// The base64 encoded image for the Camera Model's CurrentImage property
		/// </summary>
		public string CurrentImage { get; set; }
	}
}