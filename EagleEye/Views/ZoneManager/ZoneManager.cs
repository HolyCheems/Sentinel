using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Views.ZoneManager
{
    /// <summary>
    /// A ViewModel for the ZoneManager Model.
    /// This is used to provide information about a 
    /// ZoneManager object to views.
    /// </summary>
    public class ZoneManager
    {
        /// <summary>
        /// Necessary for the framework to do ViewModel binding from POST requests.
        /// </summary>
        public ZoneManager() { }
        /// <summary>
        /// Constructs a ZoneManager ViewModel from a ZoneManager model.
        /// </summary>
        /// <param name="zm"> The ZoneManager Model</param>
        public ZoneManager(Models.ZoneManager zm)
        {
            ID = zm.ID;
            Name = zm.Name;
            Lots = zm.Lots.Select(a => new ParkingLot.ParkingLot(a)).ToList();
        }
        /// <summary>
		/// The ZoneManager id
		/// </summary>
		public int ID { get; set; }
        /// <summary>
        /// The ZoneManager name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The ZoneManager lot id's
        /// </summary>
        public List<ParkingLot.ParkingLot> Lots { get; set; }
    }
}