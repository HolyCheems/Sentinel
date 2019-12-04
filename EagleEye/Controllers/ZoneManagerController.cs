using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EagleEye.Models;

namespace EagleEye.Controllers
{
    public class ZoneManagerController : Controller
    {
        /// <summary>
        /// Provides a common interface for requests that directly involve the
        /// ZoneManager model
        /// </summary>
        /// <remarks>Author: William Fry</remarks>
        public ZoneManagerController()
        {
        }
        //---------------------------------------------------------------
        //Views

        [HttpGet]
        public ActionResult ZMIndex()
        {
            return View(Repository<ZoneManager>.Models.Values.Select(zm => new Views.ZoneManager.ZoneManager(zm)).ToList());
        }

        /// <summary>
        /// Creates a view for users to monitor a ZoneManager
        /// </summary>
        /// <param name="id">The associated ZM to monitor</param>
        /// <returns>An HTML View</returns>
        [HttpGet]
        public ActionResult ZMMonitor(int id)
        {
            ZoneManager zm;
            if (TryGetZm(id, out zm))
            {
                return View("ZMMonitor", new Views.ZoneManager.ZoneManager(zm));
            }
            return new HttpNotFoundResult();
        }

        /// <summary>
        /// Creates a form view for the instantiation of a new 
        /// ZoneManager
        /// </summary>
        /// <returns>An HMTL View</returns>
        [HttpGet]
        public ActionResult ZMNew()
        {
            return View("ZMNew");
        }

        //---------------------------------------------------------------
        //Functions

        /// <summary>
        /// Creates a new Zone Manager from a name.
        /// </summary>
        /// <param name="name">The chosen name of the new Zone Manager</param>
        /// <returns>An empty result</returns>
        [HttpGet]
        public ActionResult Create(string name)
        {
            ZoneManager NewZM = new ZoneManager(Repository<ZoneManager>.NextID, name);
            Repository<ZoneManager>.Add(NewZM);
            EagleEyeConfig.ExportDatabase();

            return new EmptyResult();
        }

        /// <summary>
		/// Retrieves a ZoneManager instance by ID
		/// </summary>
		/// <param name="id">The associated ZoneManager</param>
		/// <returns>A json response body of the ZoneManager ViewModel</returns>
		[HttpGet]
        public ActionResult Get(int id)
        {
            ZoneManager zm;
            if (TryGetZm(id, out zm))
            {
                lock (zm)
                {
                    return Json(new Views.ZoneManager.ZoneManager(zm), JsonRequestBehavior.AllowGet);
                }
            }
            return new HttpNotFoundResult();
        }
        ///<summary>
        /// Updates a ZoneManager model to mirror the provided
        /// ZoneManager view model.
        /// </summary>
        /// <param name="zm">The ZoneManager viewmodel used to update the associated ZoneManager model</param>
        /// <returns>An empty result</returns>
        [HttpPost]
        public ActionResult Update(Views.ZoneManager.ZoneManager zm)
        {
            ZoneManager model;
            if (TryGetZm(zm.ID, out model))
            {
                lock (model)
                {
                    model.Lots.Clear();
                    //Add associated ParkingLot objects
                    if (zm.Lots != null)
                    {
                        foreach (var lot in zm.Lots)
                        {
                            var modelLot = new ParkingLot(model.Lots.Count > 0 ? model.Lots.Max(p => p.ID) + 1 : 0, lot.Name, Repository<Camera>.Get(lot.CameraID));
                            model.Lots.Add(modelLot);
                            
                        }
                    }
                }
                EagleEyeConfig.ExportDatabase();
                return new EmptyResult();
            }
            return new HttpNotFoundResult();
        }

        /// <summary>
		/// Deletes a ZoneManager by ID
		/// </summary>
		/// <param name="id">The associated ZoneManager to delete</param>
		/// <returns>An empty result</returns>
		[HttpGet]
        public ActionResult Delete(int id)
        {
            Repository<ZoneManager>.Delete(id);

            EagleEyeConfig.ExportDatabase();
            return new EmptyResult();
        }
        
        /// <summary>
		/// Tries to get a ZoneManager by id, returning a boolean
		///	value for success
		/// </summary>
		/// <param name="id">The associated ZoneManager to get</param>
		/// <param name="zm">The Model to be returned if successful</param>
		/// <returns>
		/// If successful, the model is passed out through
		///	the lot parameter and true is returned, else
		///	false is returned
		/// </returns>
		private bool TryGetZm(int id, out ZoneManager zm)
        {
            if (Repository<ZoneManager>.Contains(id))
            {
                zm = Repository<ZoneManager>.Get(id);
                return true;
            }
            zm = null;
            return false;
        }
    }   
}