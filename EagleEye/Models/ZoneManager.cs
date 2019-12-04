using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Models
{
    /// <summary>
    /// Developer: 
    ///     William Fry
    /// 
    /// Purpose: 
    ///     Stores associated vacancy data and names. Updates
    ///     vacancy information based on sub-lot status.
    ///     
    /// Dependencies:
    ///     ParkingLot:
    ///         Defines the vacancy status of specific lot in Zone.
    /// </summary>
    public class ZoneManager : IID
    {
        /// <summary>
        /// Constructs a Zone Manager from a unique id, name, and selection of parking lots.
        /// </summary>
        /// <param name="id">A unique id</param>
        /// <param name="name">A name</param>
        /// <param name="lot">A parking lot model</param>
        public ZoneManager(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public int ID { get; private set; } = -1;

        public string Name { get; set; }

        /// <summary>
        /// The list of associative parking lots contained in a single
        /// zone manager instance.
        /// </summary>
        public List<ParkingLot> Lots { get; private set; } = new List<ParkingLot>();

    }
}