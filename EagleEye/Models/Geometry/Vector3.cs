using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Models.Geometry
{
	public struct Vector3
	{
		public Vector3(double x = 0.0, double y = 0.0, double z = 0.0)
		{
			X = x;
			Y = y;
			Z = z;
		}
		public double X;
		public double Y;
		public double Z;
		static public Vector3 Up => new Vector3(0, 1, 0);
		static public Vector3 UnitX => new Vector3(1,0, 0);
		static public Vector3 UnitY => new Vector3(0, 1, 0);
		static public Vector3 UnitZ => new Vector3(0, 0, 1);
		static public Vector3 operator *(double scalar, Vector3 vector)
		{
			return new Vector3(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
		}
		static public Vector3 operator *(Vector3 vector,double scalar)
		{
			return new Vector3(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);
		}
		static public Vector3 operator /(Vector3 vector, double scalar)
		{
			return new Vector3(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);
		}
		static public Vector3 operator +(Vector3 a, Vector3 b)
		{
			return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}
		static public Vector3 operator -(Vector3 a, Vector3 b)
		{
			return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}
		static public Vector3 operator -(Vector3 b)
		{
			return new Vector3(- b.X, - b.Y,- b.Z);
		}
		public double Length { get => Math.Sqrt(LengthSquared()); }
		public double LengthSquared()
		{
			return X * X + Y * Y + Z * Z;
		}
		public double Dot(Vector3 b)
		{
			return X * b.X + Y * b.Y + Z * b.Z;
		}
		public Vector3 Cross(Vector3 b)
		{
			return new Vector3(
				Y * b.Z - Z * b.Y,
				Z * b.X - X * b.Z,
				X * b.Y - Y * b.X
			);
		}
		public Vector3 Normalized()
		{
			return this / Length;
		}
	}
}
