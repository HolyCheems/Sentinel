using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Models.Geometry
{
	class Matrix
	{
		public Matrix()
		{
		}
		public Matrix(Matrix other)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					data[i, j] = other.data[i, j];
				}
			}
		}
		public void ScaleColumn(int j, double scalar)
		{
			for (int i = 0; i < 3; i++)
			{
				Set(i, j, Get(i, j) * scalar);
			}
		}
		public Matrix(double[,] d)
		{
			data = d;
		}
		private double[,] data = new double[3, 3];

		public double Get(int i, int j)
		{
			return data[i, j];
		}
		public void Set(int i, int j, double value)
		{
			data[i, j] = value;
		}
		public double Determinant()
		{
			double determinant = 0;
			for (int j = 0; j < 3; j++)
			{
				determinant += Get(0, j) * (Get(1, (j + 1) % 3) * Get(2, (j + 2) % 3) - Get(1, (j + 2) % 3) * Get(2, (j + 1) % 3));
			}
			return determinant;
		}
		public Matrix Transpose()
		{
			Matrix transpose = new Matrix();
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					transpose.Set(j, i, Get(i, j));
				}
			}
			return transpose;
		}
		public Matrix Adjugate()
		{
			Matrix adjugate = new Matrix();
			int index = 0;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (index % 2 == 1)
					{
						adjugate.Set(i, j, -Get(i, j));
					}
					else
					{
						adjugate.Set(i, j, Get(i, j));
					}
					index++;
				}
			}
			return adjugate;
		}
		public bool Inverse(out Matrix inverse)
		{
			double det = Determinant();
			if (det != 0)
			{
				inverse = (1 / det) * Transpose().Minors().Adjugate();
				return true;
			}
			inverse = null;
			return false;
		}
		public Matrix Minors()
		{
			Matrix cofactor = new Matrix();
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					int leftCol = j == 0 ? 1 : 0;
					int rightCol = j == 2 ? 1 : 2;
					int topRow = i == 0 ? 1 : 0;
					int bottomRow = i == 2 ? 1 : 2;
					cofactor.Set(i, j, Get(topRow, leftCol) * Get(bottomRow, rightCol) - Get(topRow, rightCol) * Get(bottomRow, leftCol));
				}
			}
			return cofactor;
		}
		public static Vector3 operator *(Matrix matrix, Vector3 vector)
		{
			Vector3 result = new Vector3();
			result.X += matrix.Get(0, 0) * vector.X;
			result.X += matrix.Get(0, 1) * vector.Y;
			result.X += matrix.Get(0, 2) * vector.Z;
			result.Y += matrix.Get(1, 0) * vector.X;
			result.Y += matrix.Get(1, 1) * vector.Y;
			result.Y += matrix.Get(1, 2) * vector.Z;
			result.Z += matrix.Get(2, 0) * vector.X;
			result.Z += matrix.Get(2, 1) * vector.Y;
			result.Z += matrix.Get(2, 2) * vector.Z;
			return result;
		}
		public static Matrix operator *(Matrix a, Matrix b)
		{
			Matrix result = new Matrix();
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					double dot = 0;
					for (int k = 0; k < 3; k++)
					{
						dot += a.Get(i, k) * b.Get(k, j);
					}
					result.Set(i, j, dot);
				}
			}
			return result;
		}
		public static Matrix operator +(Matrix a, Matrix b)
		{
			Matrix result = new Matrix();
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					result.data[i, j] = a.data[i, j] + b.data[i, j];
				}
			}
			return result;
		}
		public static Matrix operator -(Matrix a, Matrix b)
		{
			Matrix result = new Matrix();
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					result.data[i, j] = a.data[i, j] - b.data[i, j];
				}
			}
			return result;
		}
		public static Matrix operator *(Matrix a, double scalar)
		{
			Matrix result = new Matrix();
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					result.data[i, j] = a.data[i, j] * scalar;
				}
			}
			return result;
		}
		public static Matrix operator *(double scalar, Matrix a)
		{
			return a * scalar;
		}

		public static Matrix AxisAngle(Vector3 axis, double angle)
		{
			double cos = Math.Cos(angle);
			double oneMinCos = 1 - cos;
			double sin = Math.Sin(angle);
			return new Matrix(new double[,]{
				{ cos + axis.X * axis.X * oneMinCos, axis.X * axis.Y * oneMinCos - axis.Z * sin, axis.X * axis.Z * oneMinCos + axis.Y * sin },
				{ axis.Y * axis.X * oneMinCos + axis.Z * sin, cos + axis.Y * axis.Y * oneMinCos, axis.Y * axis.Z * oneMinCos - axis.X * sin },
				{ axis.Z * axis.X * oneMinCos - axis.Y * sin, axis.Z * axis.Y * oneMinCos + axis.X * sin, cos + axis.Z * axis.Z * oneMinCos }
			});
		}
		public static Matrix YawPitchRoll(Vector3 rotation)
		{
			double ca = Math.Cos(rotation.X);
			double sa = Math.Sin(rotation.X);
			double cb = Math.Cos(rotation.Y);
			double sb = Math.Sin(rotation.Y);
			double cg = Math.Cos(rotation.Z);
			double sg = Math.Sin(rotation.Z);
			return new Matrix(new double[,]{
				{ca * cb, ca * sb * sg - sa * cg, ca * sb * cg + sa * sg},
				{sa * cb, sa * sb * sg + ca * cg, sa * sb * cg - ca * sg},
				{-sb, cb * sg, cb * cg}
			});
		}
		public static Matrix Identity
		{
			get
			{
				return new Matrix(new double[,]{
					{1,0,0 },
					{0,1,0 },
					{0,0,1 }
				});
			}
		}
	}
}