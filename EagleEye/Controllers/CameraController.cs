using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EagleEye.Models;
namespace EagleEye.Controllers
{

    public class CameraController : Controller
    {
		/// <summary>
		/// Provides a common interface for requests that
		///	directly involve Camera models
		/// </summary>
		/// <remarks>Author: Gage Coates</remarks>
		public CameraController()
		{
		}
		//--------------------------------------------------
		// Views

		/// <summary>
		/// Creates a view that lists all cameras
		/// </summary>
		/// <returns>An html view</returns>
		[HttpGet]
		[Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View(Repository<Camera>.Models.Values.Select(m => new Views.Camera.Camera(m)).ToList());
        }
		/// <summary>
		/// Creates a partial view that defines a select list
		///	of all cameras
		/// </summary>
		/// <returns>An html partial view</returns>
		[HttpGet]
		[Authorize(Roles = "Administrator")]
		public ActionResult ListView(int cameraID = -1)
		{
			ViewBag.SelectedCamera = cameraID;
			return PartialView("List", Repository<Camera>.Models.Values.Select(c => new Views.Camera.Camera(c)).ToList());
		}
		/// <summary>
		/// Creates a view that allows for the setup of a
		///	camera instace by name
		/// </summary>
		/// <returns>An html view</returns>
		[HttpGet]
		[Authorize(Roles = "Administrator")]
		public ActionResult Client()
        {
            return View();
        }
		//--------------------------------------------------
		// REST

		/// <summary>
		/// Creates a Json representation of a camera
		///	model
		/// </summary>
		/// <param name="id">The associated camera to get</param>
		/// <returns>A json response body</returns>
		[HttpGet]
		public ActionResult Get(int id)
		{
			Camera camera;
			if (TryGetCamera(id, out camera))
			{
				return Json(new Views.Camera.Camera(camera), JsonRequestBehavior.AllowGet);
			}
			return new HttpNotFoundResult();
		}
		/// <summary>
		/// Creates a new Camera instance by name
		/// </summary>
		/// <param name="name">The name of the new camera</param>
		/// <returns>An empty response</returns>
		[HttpGet]
		[Authorize(Roles = "Administrator")]
		public ActionResult Create(string name)
		{
			Repository<Camera>.Add(new Camera(Repository<Camera>.NextID, name));
			EagleEyeConfig.ExportDatabase();
			return new EmptyResult();
		}
		/// <summary>
		/// Updates a Camera model
		/// </summary>
		/// <param name="camera">The camera ViewModel used to update the corresponding model</param>
		/// <returns>An empty response</returns>
		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public ActionResult Update(Views.Camera.Camera camera)
		{
			Camera model = Repository<Camera>.Models.Values.FirstOrDefault(c => c.Name == camera.Name);
			if (model == null)
			{
				model = new Camera(Repository<Camera>.NextID, camera.Name);
				Repository<Camera>.Add(model);

				EagleEyeConfig.ExportDatabase();
			}
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
			{
				
				byte[] buffer = Convert.FromBase64String(camera.CurrentImage);
				stream.Write(buffer, 0, buffer.Length);
				model.CurrentImage = System.Drawing.Bitmap.FromStream(stream) as System.Drawing.Bitmap;
			}
			return new EmptyResult();
		}
		/// <summary>
		/// Deletes a Camera model by id
		/// </summary>
		/// <param name="id">The assocated camera to delete</param>
		/// <returns>An empty response</returns>
		[HttpGet]
		[Authorize(Roles = "Administrator")]
		public ActionResult Delete(int id)
		{
			Repository<Camera>.Delete(id);

			EagleEyeConfig.ExportDatabase();
			return new EmptyResult();
		}
		/// <summary>
		///	Tries to get a camera by id, returning a boolean
		///	value for success
		/// </summary>
		/// <param name="id">The assocated camera to get</param>
		/// <param name="camera">The camera model to be passed out if found</param>
		/// <returns>
		/// If successful, the model is passed out through
		///	the camera parameter and true is returned, else
		///	false is returned
		///	</returns>
		private bool TryGetCamera(int id, out Camera camera)
		{
			if (Repository<Camera>.Contains(id))
			{
				camera = Repository<Camera>.Get(id);
				return true;
			}
			camera = null;
			return false;
		}
    }
}