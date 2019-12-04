using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;

namespace EagleEye.Models
{
	/// <summary>
	/// Developer:
	/// 	Gage Coates
	///
	/// Purpose:
	/// 	Creates a standard interface for CRUD operations
	/// 	on classes that implement the IID interface
	///
	/// Dependencies:
	/// 	IID:
	/// 		Allows for uniquely identifying objects
	/// 	T:
	/// 		The subject class that implements IID

	/// Notes:
	/// 	All methods are static and state is static
	/// </summary>
	public class Repository<T> where T : class, IID
	{
		/// <summary>
		/// Adds an object to the internal collection
		/// </summary>
		public static void Add(T model)
		{
			Models.TryAdd(model.ID, model);
		}
		/// <summary>
		/// Gets an object by id
		/// </summary>
		/// <returns>
		/// An object T associated with the given id
		/// </returns>
		public static T Get(int id)
		{
			T model;
			Models.TryGetValue(id, out model);
			return model;
		}
		/// <summary>
		/// Tests if an object is contained with the given id
		/// </summary>
		/// <param name="id">The id to test</param>
		/// <returns>true if contained, else false</returns>
		public static bool Contains(int id)
		{
			return Models.ContainsKey(id);
		}
		/// <summary>
		/// removes an object with the given id, if it exists
		/// </summary>
		/// <param name="id">The id of the object to delete</param>
		public static void Delete(int id)
		{
			T model;
			Models.TryRemove(id, out model);
		}
		/// <summary>
		/// Provides a unique ID relative to the current set of known ids
		/// </summary>
		/// <returns>
		/// A unique integer id
		/// </returns>
		public static int NextID
		{
			get
			{
				return Models.Count > 0 ? Models.Keys.Max() + 1 : 0;
			}
		}
		/// <summary>
		/// The static thread-safe mapping of objects to their id
		/// Operations are made in constant time due to hashing
		/// </summary>
		public static ConcurrentDictionary<int, T> Models { get; private set; }  = new ConcurrentDictionary<int, T>();

	}
}