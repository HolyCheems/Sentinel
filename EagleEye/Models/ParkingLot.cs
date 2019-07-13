﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using EagleEye.Models.Geometry;
namespace EagleEye.Models
{
	public class ParkingLot : IID
	{
		/// <summary>
		//Developer:
		//	Gage Coates

		//Purpose:
		//	Stores accociated annotation data, baseline,
		//	and camera reference. Updates annotation state
		//	based on camera state

		//Dependencies:
		//	Bitmap:
		//		Defines the baseline image
		//	Camera:
		//		A parking lot must have an associated camera
		//		to compare against the baseline
		//	Annotation:
		//		Defines regions in the baseline that should be
		//		included in calculations
		/// </summary>
		public ParkingLot(int id,string name, Camera camera)
		{
			ID = id;
			Name = name;
			Camera = camera;
			Camera.Changed += CameraChangeHandler;
		}
		/// <summary>
		//Purpose:
		//	Subsribes to the Camera.Changed event and
		//	kicks off an update
		/// </summary>
		private void CameraChangeHandler(object sender, EventArgs e)
		{
			Update();
		}

		// A unique identifier
		public int ID { get; private set; } = -1;
		// A human readable identifier (not unique)
		public string Name { get; set; }
		// The assocative camera that monitors this parking lot
		public Camera Camera { get; private set; }
		// An image that defines the empty state of the parking lot
		private Bitmap m_baseline;
		public Bitmap Baseline { get
			{
				if (m_baseline == null)
					return null;

				lock (m_baseline)
				{
					return new Bitmap(m_baseline);
				}
			}
			set
			{
				if (m_baseline != null)
				{
					lock (m_baseline)
					{
						m_baseline = value;
					}
				} else
				{
					m_baseline = value;
				}
			}
		}
		// A list of annotations that define regions of Parking or Isle space
		public List<Annotation> Annotations { get; private set; } = new List<Annotation>();
		// Only annotations of Parking type
		public IEnumerable<Annotation> ParkingSpaces {
			get
			{
				return Annotations.Where(a => a.Type == Annotation.AnnotationType.Parking);
			}
		}
		// Only annotations of Isle type
		public IEnumerable<Annotation> Isles {
			get
			{
				return Annotations.Where(a => a.Type == Annotation.AnnotationType.Isle);
			}
		}
		/// <summary>
		//Purpose:
		//	Encapsulates the entire parking lot state
		//	change, triggerd by Camera changes
		/// </summary>
		public void Update()
		{
			if (Baseline != null && Baseline.SameSize(Camera.CurrentImage))
			{
				lock (this)
				{
					// Reset percentDifferences to zero
					Annotations.ForEach(a => a.PercentDifference = 0);
					// Maps annotations to their area in pixels
					Dictionary<Annotation, int> annotationPixelAreas = Annotations.ToDictionary(a => a, a => 0);
					// Get the raw pixel difference (color sensitive)
					Bitmap baseline = Baseline;
					Bitmap current = Camera.CurrentImage;
					Bitmap difference = baseline.Difference(
						current.Add(
							// Adjust the current image by the average light difference
							// between the baseline and current image
							AverageDifference(
								baseline,
								current,
								Annotations.Where(a => 
									a.Type == Annotation.AnnotationType.Constant
								)
							)
						)
					);
					// Sum up difference percentage contained by each annotation
					for (int x = 0; x < difference.Width; x++)
					{
						for (int y = 0; y < difference.Height; y++)
						{
							Vector2 pixelPoint = new Vector2((double)x / difference.Width, (double)y / difference.Height);
							foreach (Annotation annotation in Annotations.Where(a => a.Contains(pixelPoint)))
							{
								annotationPixelAreas[annotation]++;
								annotation.PercentDifference += difference.GetPixel(x, y).Value();
							}
						}
					}
					// Normalize the difference percentages by dividing out the total pixel area
					foreach (var pixelArea in annotationPixelAreas.Where(p => p.Value > 0))
					{
						pixelArea.Key.PercentDifference /= pixelArea.Value;
					}
				}
			}
		}
		/// <summary>
		//Purpose:
		//	Returns the average difference for each color channel
		//	between two images bounded by the specified
		//	annotations

		//Returns:
		//	A Tuple of signed integers that represent an average
		//	difference in R G B channels
		//Notes:
		//	The returned values can be negative if averageB
		//	is "brighter" than averageA
		/// </summary>
		private Tuple<int, int, int> AverageDifference(Bitmap a, Bitmap b, IEnumerable<Annotation> clip)
		{
			var averageA = a.Average((x, y) => clip.Any(c => c.Contains(new Vector2((double)x / a.Width, (double)y / a.Height))));
			var averageB = b.Average((x, y) => clip.Any(c => c.Contains(new Vector2((double)x / b.Width, (double)y / b.Height))));
			return new Tuple<int, int, int>(averageA.R - averageB.R, averageA.G - averageB.G, averageA.B - averageB.B);
		}
		
	}
}