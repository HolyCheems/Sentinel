using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EagleEye.Models;
using System.Threading;
namespace EagleEye.Controllers
{
    public class ParkingLotController : Controller
    {
		/// <summary>
		///	Provides a common interface for requests that
		///	directly involve ParkingLot and Annotation models
		/// </summary>
		/// <remarks>Author: Gage Coates</remarks>
		public ParkingLotController()
		{
		}
		//--------------------------------------------------
		// Views

		/// <summary>
		/// Creates a view that lists all parking lot instances
		/// </summary>
		/// <returns>An html view</returns>
		[HttpGet]
		public ActionResult Index()
        {
            return View(Repository<ParkingLot>.Models.Values.Select(lot => new Views.ParkingLot.ParkingLot(lot)).ToList());
        }
		/// <summary>
		/// Creates a view of the parking lot that allows
		///	editing of annotations and the name
		/// </summary>
		/// <param name="id">The assocated lot to edit</param>
		/// <returns>An html view</returns>
		[HttpGet]
		public ActionResult Edit(int id)
		{
			ParkingLot lot;
			if (TryGetLot(id, out lot))
			{
				return View("Edit", new Views.ParkingLot.ParkingLot(lot));
			}
			return new HttpNotFoundResult();
		}
		/// <summary>
		/// Creates a view for users to monitor a parking lot
		/// </summary>
		/// <param name="id">The assocated lot to monitor</param>
		/// <returns>An html view</returns>
		[HttpGet]
		public ActionResult Monitor(int id)
		{
			ParkingLot lot;
			if (TryGetLot(id, out lot))
			{
				return View("Monitor", new Views.ParkingLot.ParkingLot(lot));
			}
			return new HttpNotFoundResult();
		}
		/// <summary>
		/// Creates a form view for instantiation of a new
		///	parking lot
		/// </summary>
		/// <returns>An html view</returns>
		[HttpGet]
		public ActionResult New()
		{
			return View("New");
		}



		/// <summary>
		/// Creates a base64 encoded image in the response body
		///	for the parking lot baseline
		/// </summary>
		/// <param name="id">The assocated lot from which to get the baseline</param>
		/// <returns>A base64 encoded image in the response body</returns>
		[HttpGet]
		public ActionResult Baseline(int id)
		{
			ParkingLot lot;
			if (TryGetLot(id, out lot) && lot.Baseline != null)
			{
				// Create a memory stream for the bitmap to write to
				using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
				{
					
					lot.Baseline.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
					Response.ContentType = "image/png";
					Response.Write(System.Convert.ToBase64String(stream.GetBuffer()));
				}
				return new EmptyResult();
			}
			return new HttpNotFoundResult();
		}

		/// <summary>
		/// Updates a parking lot baseline to the current
		///	image held in the associated Camera instance
		/// </summary>
		/// <param name="id">The assocated lot to which the baseline will be updated</param>
		/// <returns>An empty result</returns>
		[HttpGet]
		public ActionResult UpdateBaseline(int id)
		{
			ParkingLot lot;
			if (TryGetLot(id, out lot))
			{
				lot.Baseline = lot.Camera.CurrentImage;
				EagleEyeConfig.ExportDatabase();
				return new EmptyResult();
			}
			return new HttpNotFoundResult();
		}
		//--------------------------------------------------
		// REST

		/// <summary>
		/// Creates a new parking lot from a name and
		///	camera ID
		/// </summary>
		/// <param name="name">The new parking lot's name</param>
		/// <param name="cameraID">The new parking lot's assocated camera</param>
		/// <returns>An empty result</returns>
		[HttpGet]
		public ActionResult Create(string name, int cameraID)
		{
			if (Repository<Camera>.Contains(cameraID))
			{
				Camera camera = Repository<Camera>.Get(cameraID);
				ParkingLot newLot = new ParkingLot(Repository<ParkingLot>.NextID, name, camera);
				Repository<ParkingLot>.Add(newLot);

				EagleEyeConfig.ExportDatabase();
			}
			return new EmptyResult();
		}
		/// <summary>
		/// Retrieves a parking lot instance by ID
		/// </summary>
		/// <param name="id">The assocated parking lot</param>
		/// <returns>A json response body of the ParkingLot ViewModel</returns>
		[HttpGet]
		public ActionResult Get(int id)
		{
			ParkingLot lot;
			if (TryGetLot(id, out lot))
			{
				lock (lot)
				{
					return Json(new Views.ParkingLot.ParkingLot(lot), JsonRequestBehavior.AllowGet);
				}
			}
			return new HttpNotFoundResult();
		}
		/// <summary>
		/// Updates a parking lot model to mirror the provided
		///	ParkingLot viewmodel
		/// </summary>
		/// <param name="lot">The ParkingLot ViewModel used to update the assocated ParkingLot Model</param>
		/// <returns>An empty result</returns>
		[HttpPost]
		public ActionResult Update(Views.ParkingLot.ParkingLot lot)
		{
			ParkingLot model;
			if (TryGetLot(lot.ID, out model))
			{
				lock (model)
				{
					model.Annotations.Clear();
					// Add annotations
					if (lot.Annotations != null)
					{
						foreach (var annotation in lot.Annotations)
						{
							var modelAnnotation = new Annotation(model.Annotations.Count > 0 ? model.Annotations.Max(a => a.ID) + 1 : 0, (Annotation.AnnotationType)Enum.Parse(typeof(Annotation.AnnotationType), annotation.Type));
							model.Annotations.Add(modelAnnotation);
							foreach (var point in annotation.Points)
							{
								modelAnnotation.Add(new Models.Geometry.Vector2(point.X, point.Y));
							}
						}
					}
					// Set the camera
					model.Camera = Repository<Camera>.Get(lot.CameraID);
				}
				EagleEyeConfig.ExportDatabase();
				return new EmptyResult();
			}
			return new HttpNotFoundResult();
		}
		/// <summary>
		/// Deletes a parking lot by ID
		/// </summary>
		/// <param name="id">The assocated parking lot to delete</param>
		/// <returns>An empty result</returns>
		[HttpGet]
		public ActionResult Delete(int id)
		{
			Repository<ParkingLot>.Delete(id);

			EagleEyeConfig.ExportDatabase();
			return new EmptyResult();
		}
		/// <summary>
		/// Tries to get a parking lot by id, returning a boolean
		///	value for success
		/// </summary>
		/// <param name="id">The assocated parking lot to get</param>
		/// <param name="lot">The Model to be returned if successful</param>
		/// <returns>
		/// If successful, the model is passed out through
		///	the lot parameter and true is returned, else
		///	false is returned
		/// </returns>
		private bool TryGetLot(int id, out ParkingLot lot)
		{
			if (Repository<ParkingLot>.Contains(id))
			{
				lot = Repository<ParkingLot>.Get(id);
				return true;
			}
			lot = null;
			return false;
		}
    }
}