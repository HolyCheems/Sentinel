using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EagleEye.Models.Geometry;

namespace EagleEye.Models
{
    /// <summary>
    /// Developer:
    /// 	Gage Coates
    ///
    /// Purpose:
    /// 	Defines a set of points that make up a 2D region
    /// 	marked as a specific Type, which is defined by
    /// 	the AnnotationType enum
    ///
    /// Dependencies:
    /// 	Vector2:
    /// 		Used to define points
    /// </summary>
    public class Annotation : IID
    {
        /// <summary>
        /// Differentiates annotation's semantic meaning
        /// </summary>
        public enum AnnotationType
        {
            /// <summary>
            /// Represents a parking space
            /// </summary>
            Parking,
            /// <summary>
            /// Represents a driving Aisle
            /// </summary>
            Aisle,
            /// <summary>
            /// Represents a region that does not change, e.g. no vehicles can obstruct this region
            /// </summary>
            Constant,
            /// <summary>
            /// Represents Handicap-Accessible Parking 
            /// </summary>
            Handicap,
			/// <summary>
            ///	 Represents Motorcycle Parking 
            /// </summary>
			Motorcycle,
			
        }
        /// <summary>
        /// Constructs an annotation from a unique ID and type
        /// </summary>
        /// <param name="id">A unique integer ID</param>
        /// <param name="type">The type of annotation</param>
        public Annotation(int id, AnnotationType type)
        {
            ID = id;
            Type = type;
        }
        /// <summary>
        /// Adds a point to the Points collection, ensuring
        /// there is a maximum of 4 points sorted in a uniform
        /// order about their common center
        /// </summary>
        public void Add(Vector2 point)
        {
            if (Points.Count < 4)
            {
                Points.Add(point);
                Points.Sort(new PointSorter(Points.Centroid()));
            }
        }
        /// <summary>
        /// Calculates the area of a triangle defined by
        /// three Vector2 vertices
        /// </summary>
        /// <returns>The area of the triangle as a double</returns>

        private double TriangleArea(Vector2 a, Vector2 b, Vector2 c)
        {
            var ab = b - a;
            // Area is defined by 1/2 * base * height
            // Taking the dot product of one edge (ac) with
            // a normal vector perpendicular to another edge (ab)
            // will yield the height of the triangle
            return 0.5 * ab.Length * Math.Abs(ab.Normal().Normalized().Dot(c - a));
        }
        /// <summary>
        /// The convex area enclosed by all points
        /// </summary>
        public double Area
        {
            get
            {
                // Sums the area of two adjacent triangles
                return TriangleArea(Points[0], Points[1], Points[2]) + TriangleArea(Points[0], Points[2], Points[3]);
            }
        }
        /// <summary>
        /// Determines whether the given point is contained
        /// within the annotation
        /// </summary>
        /// <returns>
        /// true if contained, else false
        /// </returns>
        public bool Contains(Vector2 point)
        {
            // Sum the area of all 4 triangles (one triangle for each point in the annotation)
            double area = 0.0;
            for (int i = 0; i < Points.Count; i++)
            {
                area += TriangleArea(point, Points[i], Points[(i + 1) % Points.Count]);
            }
            // If the difference between the actual area and the 4 triangles is not equal, 
            // the point must lie outside the annotation
            return Math.Abs(area - Area) <= 0.0001; // A small fudge factor to compensate for floating point inacuraccy
        }
        /// <summary>
        /// the Separating Axis Theorem (SAT)
        /// Determines whether two convex sets of points intersect
        /// </summary>
        /// <returns>true if an intersection is found, else false</returns>
        private bool SAT(IList<Vector2> A, IList<Vector2> B)
        {
            List<Vector2> axes = new List<Vector2>();
            if (A.Count > 1)
                for (int i = 0; i < A.Count; i++)
                    axes.Add((A[i] - A[(i + 1) % 4]).Normal().Normalized());
            if (B.Count > 1)
                for (int i = 0; i < B.Count; i++)
                    axes.Add((B[i] - B[(i + 1) % 4]).Normal().Normal());
            foreach (var axis in axes)
            {
                var projAmin = double.PositiveInfinity;
                var projBmin = double.PositiveInfinity;
                var projAmax = double.NegativeInfinity;
                var projBmax = double.NegativeInfinity;

                foreach (var p in A)
                {
                    var proj = p.Dot(axis);
                    if (proj < projAmin)
                        projAmin = proj;
                    if (proj > projAmax)
                        projAmax = proj;
                }
                foreach (var p in B)
                {
                    var proj = p.Dot(axis);
                    if (proj < projBmin)
                        projBmin = proj;
                    if (proj > projBmax)
                        projBmax = proj;
                }
                // A separating axis has been found, early out with false
                if (!(projAmin < projBmax && projBmin < projAmax))
                    return false;
            }
            // Search was exhaustive, there must be an intersection
            return true;
        }
        /// <summary>
        /// A unique identifier
        /// </summary>
        public int ID { get; private set; } = -1;
        /// <summary>
        /// A list of points that define the region
        /// </summary>
        public List<Vector2> Points { get; private set; } = new List<Vector2>();
        /// <summary>
        /// The type of annotation
        /// </summary>
        public AnnotationType Type { get; set; } = AnnotationType.Parking;
        /// <summary>
        /// The percentage of occupied pixels
        /// </summary>
        public double PercentDifference { get; set; }
    }
}