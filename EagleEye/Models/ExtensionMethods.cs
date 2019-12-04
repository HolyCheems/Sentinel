using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
namespace EagleEye.Models
{
	/// <summary>
	/// Provides extension methods for the Bitmap, Color, and IEnumerable<Geometry.Vector2> classes
	/// </summary>
	static class ExtensionMethods
	{
		//--------------------------------------------------
		// Bitmap extensions

		/// <summary>
		/// Tests whether two Bitmaps share a common size
		/// </summary>
		/// <param name="a">The first Bitmap</param>
		/// <param name="b">The second Bitmap</param>
		/// <returns>true if equal in width and height, else false</returns>
		public static bool SameSize(this Bitmap a, Bitmap b)
		{
			return a.Width == b.Width && a.Height == b.Height;
		}
		/// <summary>
		/// Finds the smallest set of adjacent points (determined by count) that have a color value above the given threshold
		/// </summary>
		/// <param name="source">The Bitmap to be used as the source</param>
		/// <param name="count">The size of the island - this will be equal to the size of the returned set of points</param>
		/// <param name="thresholdArea">The value threshold to search for</param>
		/// <returns>A set of points</returns>
		static public List<Point> FindIslands(this Bitmap source, int count, int thresholdArea)
		{
			int gridSize = (int)Math.Floor(Math.Sqrt((double)thresholdArea));
			List<HashSet<Point>> islands = new List<HashSet<Point>>();
			for (int level = 255; level >= 0; level--)
			{
				islands.Clear();
				for (int x = 0; x < source.Width; x += gridSize)
				{
					for (int y = 0; y < source.Height; y += gridSize)
					{
						Point point = new Point(x, y);
						if (!islands.Any(island => island.Contains(point)))
						{
							int value = source.GetPixel(x, y).R;
							if (value >= level)
							{
								HashSet<Point> island = new HashSet<Point>();
								Queue<Point> newPoints = new Queue<Point>();
								newPoints.Enqueue(point);
								while (newPoints.Count > 0)
								{
									var seed = newPoints.Dequeue();
									if (seed.X >= 0 && seed.X < source.Width && seed.Y >= 0 && seed.Y < source.Height && source.GetPixel(seed.X, seed.Y).R >= level)
									{
										if (!island.Contains(seed))
										{
											island.Add(seed);
											newPoints.Enqueue(new Point(seed.X, seed.Y - 1));
											newPoints.Enqueue(new Point(seed.X - 1, seed.Y));
											newPoints.Enqueue(new Point(seed.X + 1, seed.Y));
											newPoints.Enqueue(new Point(seed.X, seed.Y + 1));
										}
										else
										{
											var neighbor = islands.Find(i => i.Contains(seed));
											if (neighbor != null)
											{
												// Union islands
												islands.Remove(neighbor);
												island.UnionWith(neighbor);
											}
										}
									}
								}
								islands.Add(island);
								if (islands.Count(i => i.Count >= thresholdArea) >= count)
									goto done;
							}
						}
					}
				}

			}
			done:;
			List<Point> points = new List<Point>();
			foreach (var island in islands.Where(i => i.Count >= thresholdArea))
			{
				Point average = new Point(0, 0);
				foreach (var point in island)
				{
					average.X += point.X;
					average.Y += point.Y;
				}
				average.X /= island.Count;
				average.Y /= island.Count;
				points.Add(average);
			}
			return points;
		}
		/// <summary>
		/// Normalizes the green channel as a percent of the total
		/// </summary>
		/// <param name="source">The source Bitmap</param>
		/// <returns>A green shifted version of the source Bitmap</returns>
		static public Bitmap GreenShift(this Bitmap source)
		{
			Bitmap result = new Bitmap(source.Width, source.Height);
			for (int x = 0; x < result.Width; x++)
			{
				for (int y = 0; y < result.Height; y++)
				{
					Color color = source.GetPixel(x, y);
					int total = (color.R + color.B + color.G);
					int g = total != 0 ? (int)Math.Min(255, (double)(color.G + color.B) * 255 / total) : 0;
					result.SetPixel(x, y, Color.FromArgb(g, g, g));
				}
			}
			return result;
		}
		/// <summary>
		/// Creates a 2 color black and white image based on source color values
		/// </summary>
		/// <param name="source">The source Bitmap</param>
		/// <param name="threshold">The value threshold that determines Black/White for each pixel</param>
		/// <returns>A posturized image</returns>
		static public Bitmap Posterize(this Bitmap source, int threshold = 382)
		{
			Bitmap result = new Bitmap(source.Width, source.Height);
			for (int x = 0; x < source.Width; x++)
			{
				for (int y = 0; y < source.Height; y++)
				{
					var pixel = source.GetPixel(x, y);
					if (pixel.R + pixel.G + pixel.B > threshold)
					{
						result.SetPixel(x, y, Color.White);
					}
					else
					{
						result.SetPixel(x, y, Color.Black);
					}
				}
			}
			return result;
		}
		/// <summary>
		/// Returns a grayscale bitmap that represents the value difference in two Bitmaps
		/// </summary>
		/// <param name="a">The first bitmap</param>
		/// <param name="b">The second bitmap</param>
		/// <returns>A difference bitmap</returns>
		/// <remarks>White represents more difference, Black represents no difference</remarks>
		static public Bitmap Difference(this Bitmap a, Bitmap b)
		{
			Bitmap result = new Bitmap(a.Width, a.Height);
			for (int x = 0; x < result.Width; x++)
			{
				for (int y = 0; y < result.Height; y++)
				{
					Color colorA = a.GetPixel(x, y);
					Color colorB = b.GetPixel(x, y);
					result.SetPixel(x, y, Color.FromArgb(Math.Abs(colorA.R - colorB.R), Math.Abs(colorA.G - colorB.G), Math.Abs(colorA.B - colorB.B)));

				}
			}
			return result;
		}
		/// <summary>
		/// Calculates the average difference for all three color channels
		/// </summary>
		/// <param name="a">The first bitmap</param>
		/// <param name="b">The second bitmap</param>
		/// <returns>A Tuple of three signed integers that represent average difference for each channel</returns>
		static public Tuple<int,int,int> AverageDifference(this Bitmap a, Bitmap b)
		{
			int red = 0, green = 0, blue = 0;
			
			for (int x = 0; x < a.Width; x++)
			{
				for (int y = 0; y < a.Height; y++)
				{
					Color colorA = a.GetPixel(x, y);
					Color colorB = b.GetPixel(x, y);
					red += colorA.R - colorB.R;
					green += colorA.G - colorB.G;
					blue += colorA.B - colorB.B;
				}
			}
			int pixelCount = a.Width * a.Height;
			return new Tuple<int,int,int>(red / pixelCount, green / pixelCount, blue / pixelCount);
		}
		/// <summary>
		/// Creates a new bitmap with the given offsets applied to each pixel of the source image
		/// </summary>
		/// <param name="source">The source image</param>
		/// <param name="offset">The offset for each color channel to apply</param>
		/// <returns>A new bitmap with the offsets applied</returns>
		static public Bitmap Add(this Bitmap source, Tuple<int,int,int> offset)
		{
			Bitmap result = new Bitmap(source);
			for (int x = 0; x < result.Width; x++)
			{
				for (int y = 0; y < result.Height; y++)
				{
					Color baseColor = source.GetPixel(x, y);
					result.SetPixel(x, y, Color.FromArgb(
						Math.Max(0, Math.Min(255, baseColor.R + offset.Item1)),
						Math.Max(0, Math.Min(255, baseColor.G + offset.Item2)),
						Math.Max(0, Math.Min(255, baseColor.B + offset.Item3))
					));
				}
			}
			return result;
		}
		/// <summary>
		/// Calculates the average color for pixels that satisfy the pixelSelector predicate
		/// </summary>
		/// <param name="source">The source image</param>
		/// <param name="pixelSelector">A predicate that takes in x,y pixel coordinates and returns true or false</param>
		/// <returns>An average Color</returns>
		static public Color Average(this Bitmap source, Func<int,int,bool> pixelSelector)
		{
			int r = 0;
			int g = 0;
			int b = 0;
			int total = 0;
			for (int x = 0; x < source.Width; x++)
			{
				for (int y = 0; y < source.Height; y++)
				{
					if (pixelSelector(x,y))
					{
						total++;
						var color = source.GetPixel(x, y);
						r += color.R;
						g += color.G;
						b += color.B;
					}
				}
			}
			if (total > 0)
			{
				return Color.FromArgb(r / total, g / total, b / total);
			} else
			{
				return Color.Black;
			}
		}
		/// <summary>
		/// Fills the source bitmap with the given color
		/// </summary>
		/// <param name="source">The source Bitmap to be modified</param>
		/// <param name="color">The color to fill with</param>
		static public void Fill(this Bitmap source, Color color)
		{
			for (int x = 0; x < source.Width; x++)
			{
				for (int y = 0; y < source.Height; y++)
				{
					source.SetPixel(x, y, color);
				}
			}
		}
		/// <summary>
		/// Calculates the amount of overlap by summing overlap for each color channel
		/// </summary>
		/// <param name="a">The first image</param>
		/// <param name="b">The second image</param>
		/// <returns>The sum total of overlap across all channels</returns>
		static public int Overlap(this Bitmap a, Bitmap b)
		{
			int overlap = 0;
			for (int x = 0; x < a.Width; x++)
			{
				for (int y = 0; y < a.Height; y++)
				{
					var pixelA = a.GetPixel(x, y);
					var pixelB = b.GetPixel(x, y);
					overlap += Math.Min(pixelA.R, pixelB.R) + Math.Min(pixelA.G, pixelB.G) + Math.Min(pixelA.B, pixelB.B);
				}
			}
			return overlap;
		}
		/// <summary>
		/// Returns a new image cropped by the given percentage of the origional
		/// </summary>
		/// <param name="source">The source image</param>
		/// <param name="percent">The percentage (in width/height) to crop by</param>
		/// <returns>A cropped version of the source</returns>
		static public Bitmap Crop(this Bitmap source, double percent)
		{
			int width = (int)(source.Width * percent);
			int height = (int)(source.Height * percent);
			return source.Clone(new Rectangle((source.Width - width) / 2, (source.Height - height) / 2, width, height), source.PixelFormat);
		}
		/// <summary>
		/// Creates a scaled version of the source
		/// </summary>
		/// <param name="source">The source image</param>
		/// <param name="scale">The scale factor</param>
		/// <returns>A scaled version of the source</returns>
		static public Bitmap Scale(this Bitmap source, double scale)
		{
			Bitmap result = new Bitmap((int)(source.Width * scale), (int)(source.Height * scale));
			for (int x = 0; x < result.Width; x++)
			{
				for (int y = 0; y < result.Height; y++)
				{
					result.SetPixel(x, y, source.GetPixel((int)Math.Max(0, Math.Min(source.Width, x / scale)), (int)Math.Max(0, Math.Min(source.Height, y / scale))));
				}
			}
			return result;
		}
		/// <summary>
		/// Creates a scaled version of the source
		/// </summary>
		/// <param name="source">The source image</param>
		/// <param name="scale">The width of the returned image</param>
		/// <returns>A scaled version of the source</returns>
		static public Bitmap Scale(this Bitmap source, int width)
		{
			double scale = (double)width / (double)source.Width;
			return Scale(source, scale);
		}
		/// <summary>
		/// Calculates the sum of all channel values in the source
		/// </summary>
		/// <param name="source">The source image</param>
		/// <returns>A sum of all color channels</returns>
		static public int Sum(this Bitmap source)
		{
			int sum = 0;
			for (int x = 0; x < source.Width; x++)
			{
				for (int y = 0; y < source.Height; y++)
				{
					var pixel = source.GetPixel(x, y);
					sum += pixel.R + pixel.G + pixel.B;
				}
			}
			return sum;
		}
		/// <summary>
		/// Transforms an image by the given transormation matrix (an inverse transform is applied)
		/// e.g. take the destination pixel and find the corresponding source pixel
		/// </summary>
		/// <param name="source">The source image</param>
		/// <param name="matrix">The transformation matrix</param>
		/// <returns>A transformed version of the source</returns>
		static public Bitmap Transform(this Bitmap source, Geometry.Matrix matrix)
		{
			Bitmap result = new Bitmap(source.Width, source.Height);
			for (int x = 0; x < result.Width; x++)
			{
				for (int y = 0; y < result.Height; y++)
				{
					// Inverse transform. e.g. take the destination pixel and find the corresponding source pixel
					Geometry.Vector3 sourcePoint = matrix * new Geometry.Vector3((double)x, (double)y, 1);
					int sx = (int)Math.Round(sourcePoint.X / sourcePoint.Z);
					int sy = (int)Math.Round(sourcePoint.Y / sourcePoint.Z);
					if (sx >= 0 && sx < source.Width && sy >= 0 && sy < source.Height)
						result.SetPixel(x, y, source.GetPixel(sx, sy));
				}
			}
			return result;
		}

		//--------------------------------------------------
		// Color extensions

		/// <summary>
		/// Calculates the normalized value of a given color by
		/// summing all channels and dividing by the highest possible
		/// value of (255 + 255 + 255)
		/// </summary>
		/// <param name="color">The color to find the value of</param>
		/// <returns>A normalized value in the range [0,1]</returns>
		static public double Value(this Color color)
		{
			// 765.0 is 255 * 3, e.g. the maximum value
			return (color.R + color.G + color.B) / 765.0;
		}

		//--------------------------------------------------
		// IEnumerable<Vector2> extensions
		
		/// <summary>
		/// Calculates the average center point from a collection
		/// of Vector2 instances
		/// </summary>
		/// <param name="vertices">A collection of vertices to be factored into the centroid</param>
		/// <returns>A Vector2 instance representing the center point</returns>
		static public Geometry.Vector2 Centroid(this IEnumerable<Geometry.Vector2> vertices)
		{
			return new Geometry.Vector2(vertices.Average(v => v.X), vertices.Average(v => v.Y));
		}
	}
}
