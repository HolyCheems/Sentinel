using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace EagleEye.Models.Geometry
{
	/// <summary>
	/// Developer:
	///		Gage Coates
	///
	/// Purpose:
	/// 	Sort Vector2 instances counterclockwise according to a given
	///		center point
	///
	/// Dependencies:
	/// 	Vector2:
	/// 		The subject of the comparison;
	/// 		used to calculate relative angles in 2d space
	///
	/// Notes:
	/// 	Compare deferres angle comparisons to the default
	/// 	CompareTo function defined for double
	/// </summary>
	public class PointSorter : IComparer<Vector2>
	{
		/// <summary>
		/// 	Consructs a sorter with a specific center point
		/// </summary>
		public PointSorter(Vector2 center)
		{
			Center = center;
		}
		/// <summary>
		/// 	Stores the center point across calls to Compare
		/// </summary>
		private Vector2 Center { get; set; }
		/// <summary>
		/// 	Returns the ordinal position of a relative to b
		///	</summary>
		/// <returns>
		/// 	-1 if a is located clockwise of b
		/// 	0 if a is located along the same rotational axis as b
		/// 	1 if a is located counterclockwise of b
		/// </returns>
		public int Compare(Vector2 a, Vector2 b)
		{
			// angles are wrapped within the [0,2 PI) range
			double angleA = Math.Atan2(a.X - Center.X, a.Y - Center.Y) % (Math.PI * 2);

			double angleB = Math.Atan2(b.X - Center.X, b.Y - Center.Y) % (Math.PI * 2);

			return angleA.CompareTo(angleB);

		}
	}
}