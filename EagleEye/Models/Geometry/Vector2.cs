using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Models.Geometry
{
	/// <summary>
	/// Store an associative pair of doubles that represent
	/// 2d cartesian coordinates
	/// </summary>
	/// <remarks>Author: Gage Coates</remarks>
	public struct Vector2
	{
		/// <summary>
		/// Constructs a new vector from two components
		/// </summary>
		/// <param name="x">The cartesian x component</param>
		/// <param name="y">The cartesian y component</param>
		public Vector2(double x, double y)
		{
			X = x;
			Y = y;
		}
		/// <summary>
		/// The X component
		/// </summary>
		public double X;
		/// <summary>
		/// The Y component
		/// </summary>
		public double Y;
		/// <summary>
		/// Creates a Zero vector
		/// </summary>
		static public Vector2 Zero = new Vector2(0, 0);
		/// <summary>
		/// Scales a vector by a scalar value
		/// </summary>
		/// <param name="vector">The vector to be scaled</param>
		/// <param name="scalar">The scalar value</param>
		/// <returns>A new scaled vector</returns>
		static public Vector2 operator *(Vector2 vector, double scalar)
		{
			return new Vector2(vector.X * scalar, vector.Y * scalar);
		}
		/// <summary>
		/// Scales a vector by a scalar value
		/// </summary>
		/// <param name="vector">The vector to be scaled</param>
		/// <param name="scalar">The scalar value</param>
		/// <returns>A new scaled vector</returns>
		static public Vector2 operator /(Vector2 vector, double scalar)
		{
			return new Vector2(vector.X / scalar, vector.Y / scalar);
		}
		/// <summary>
		/// Adds two vectors component-wise
		/// </summary>
		/// <param name="a">The left hand side</param>
		/// <param name="b">The right hand side</param>
		/// <returns>A new vector that is the sum of a and b</returns>
		static public Vector2 operator +(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X + b.X, a.Y + b.Y);
		}
		/// <summary>
		/// Subtracts two vectors component-wise
		/// </summary>
		/// <param name="a">The left hand side</param>
		/// <param name="b">The right hand side</param>
		/// <returns>A new vector that is the difference of a and b</returns>
		static public Vector2 operator -(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X - b.X, a.Y - b.Y);
		}
		/// <summary>
		/// Tests equality between two vectors component-wise
		/// </summary>
		/// <param name="a">The left hand side</param>
		/// <param name="b">The right hand side</param>
		/// <returns>true if equal, else false</returns>
		static public bool operator==(Vector2 a, Vector2 b)
		{
			return a.X == b.X && a.Y == b.Y;
		}
		/// <summary>
		/// Tests inequality between two vectors component-wise
		/// </summary>
		/// <param name="a">The left hand side</param>
		/// <param name="b">The right hand side</param>
		/// <returns>true if unequal, else false</returns>
		static public bool operator !=(Vector2 a, Vector2 b)
		{
			return !(a == b);
		}
		/// <summary>
		/// Returns x squared + y squared
		/// </summary>
		/// <returns>
		/// A double representing the length squared
		/// </returns>
		public double LengthSquared()
		{
			return X * X + Y * Y;
		}
		/// <summary>
		/// Returns the length of this vector
		/// </summary>
		public double Length { get => Math.Sqrt(LengthSquared()); }
		/// <summary>
		/// Returns the sum of the products for each component
		/// </summary>
		/// <param name="b"> A second vector </param>
		/// <returns> The dot product </returns>
		public double Dot(Vector2 b)
		{
			return X * b.X + Y * b.Y;
		}
		/// <summary>
		/// Creates a perpindicualr vector with the same magnitude
		/// </summary>
		/// <returns>A new Vector2</returns>
		public Vector2 Normal()
		{
			return new Vector2(-Y, X);
		}
		/// <summary>
		/// Creates a normalized vector in the same direction as this
		/// vector, e.g. a vector of length = 1
		/// </summary>
		/// <returns>A new Vector2</returns>
		public Vector2 Normalized()
		{
			return this / Length;
		}
		/// <summary>
		/// Compares this to another object
		/// </summary>
		/// <param name="obj">Another object</param>
		/// <returns>True if equal, else false</returns>
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}
		/// <summary>
		/// Generates a string representation of the vector
		/// </summary>
		/// <returns>A string</returns>
		public override string ToString()
		{
			return X.ToString() + ',' + Y.ToString();
		}
	}
}
