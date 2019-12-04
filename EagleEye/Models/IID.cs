using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Models
{
	/// <summary>
	/// Defines a common interface for Repository to
	/// interact with models that are uniquely identified
	/// </summary>
	/// <remarks>
	/// .NET naming convention calls for interfaces to
	/// begin with 'I' indicating that this is an
	/// interface
	/// </remarks>
	public interface IID
	{
		int ID { get; }
	}
}
