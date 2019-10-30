using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Drawing;
using System.Collections.Concurrent;

namespace EagleEye.Models
{
	/// <summary>
	/// Defines a unique camera instance that maintains
	/// a current image
	/// </summary>
	/// <remarks>Author: Gage Coates</remarks>
	public class Camera : IID
	{
		/// <summary>
		/// Constructs a camera from an id and name
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <param name="name">A name</param>
		public Camera(int id,string name)
		{
			ID = id;
			Name = name;
			CurrentImage = new Bitmap(400, 300);
			CurrentImage.Fill(Color.Black);
		}
		/// <summary>
		/// A unique identifier
		/// </summary>
		public int ID { get; private set; } = -1;
		/// <summary>
		/// A human readable identifier (not unique)
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// The most recent image from this camera
		/// </summary>
		private Bitmap m_currentImage;
		/// <summary>
		/// A property for accessing m_currentImage
		/// </summary>
		/// <see cref="m_currentImage"/>
		public Bitmap CurrentImage {
			get
			{
				if (m_currentImage == null)
					return null;

				lock (m_currentImage) {
					return new Bitmap(m_currentImage);
				}
				
			}
			set
			{
				if (m_currentImage != null)
				{
					lock (m_currentImage)
					{
						m_currentImage = value;
						LastUpdate = DateTime.Now;
						Changed(this,new EventArgs());
					}
				} else
				{
					m_currentImage = value;
					LastUpdate = DateTime.Now;
					Changed(this, new EventArgs());
				}
			}
		}
		/// <summary>
		/// An event that fires when changes have occured to the CurrentImage property
		/// </summary>
		public event EventHandler Changed = delegate { };
		/// <summary>
		/// The time CurrentImage was last updated
		/// </summary>
		public DateTime LastUpdate { get; set; }
	}
}