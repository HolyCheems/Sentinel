using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
namespace EagleEye.Models
{
	/// <summary>
	/// Parses a json formated stream of characters into
	/// an tree structure of Json instances
	/// </summary>
	/// <remarks>
	/// Author: Gage Coates.
	/// Every node in the tree, including primives, are
	/// stored as Json instances differentiated by
	/// the Type property
	/// </remarks>
	public class Json
	{
		/// <summary>
		/// Parses a stream into a json tree
		/// </summary>
		/// <param name="stream">The stream reader from which to pull characters</param>
		/// <returns>A json node representing the entire stream as a JSON tree structure</returns>
		static public Json Import(StreamReader stream) {
			int i = 0;
			string json = stream.ReadToEnd();
			return ParseTree(ref json, ref i);
		}
		/// <summary>
		/// Serializes the Json tree to a stream
		/// </summary>
		/// <param name="stream">The stream writer to which the tree is serialized</param>
		public void Export(StreamWriter stream)
		{
			switch (Type)
			{
				case JsonType.Array:
					{
						stream.Write('[');
						int i = 0;
						foreach (Json child in Children)
						{
							child.Export(stream);
							if (i != Children.Count - 1)
							{
								stream.Write(',');
							}
							i++;
						}
						stream.Write(']');
						break;
					}
				case JsonType.Object:
					{
						stream.Write('{');
						int i = 0;
						foreach (Json child in Children)
						{
							child.Export(stream);
							stream.Write(':');
							child.Children.First().Export(stream);
							if (i != Children.Count - 1)
							{
								stream.Write(',');
							}
							i++;
						}
						stream.Write('}');
						break;
					}
				case JsonType.String:
					stream.Write('"' + Data + '"');
					break;
				default:
					stream.Write(Data);
					break;
			}
		}
		/// <summary>
		/// Constructs a Json String instance
		/// </summary>
		/// <param name="str">The value to be stored</param>
		public Json(string str)
		{
			Type = JsonType.String;
			Data = str;
		}
		/// <summary>
		/// Constructs a Json Number instance
		/// </summary>
		/// <param name="number">The value to be stored</param>
		public Json(int number)
		{
			Type = JsonType.Number;
			Data = number.ToString();
		}
		/// <summary>
		/// Constructs a Json Number instance
		/// </summary>
		/// <param name="number">The value to be stored</param>
		public Json(double number)
		{
			Type = JsonType.Number;
			Data = number.ToString();
		}
		/// <summary>
		/// Constructs a Json Boolean instance
		/// </summary>
		/// <param name="boolean">The value to be stored</param>
		/// <remarks>boolean values in JSON are encoded as "true" or "false" strings</remarks>
		public Json(bool boolean)
		{
			Type = JsonType.Boolean;
			Data = boolean ? "true" : "false";
		}
		/// <summary>
		/// Constructs a default Json instance
		/// </summary>
		/// <remarks>This is for internal use only</remarks>
		private Json()
		{
			
		}
		/// <summary>
		/// Constructs a Json Null instance
		/// </summary>
		/// <remarks>JSON null is encoded as "null"</remarks>
		public static Json Null
		{
			get
			{
				Json json = new Json();
				json.Type = JsonType.Null;
				json.Data = "null";
				return json;
			}
		}
		/// <summary>
		/// Creates a Json Array instance
		/// </summary>
		public static Json Array
		{
			get
			{
				Json json = new Json();
				json.Type = JsonType.Array;
				json.Children = new List<Json>();
				return json;
			}
		}
		/// <summary>
		/// Creates a Json Object instance
		/// </summary>
		public static Json Object
		{
			get
			{
				Json json = new Json();
				json.Type = JsonType.Object;
				json.Children = new List<Json>();
				return json;
			}
		}
		/// <summary>
		/// The raw data associated with this Json node
		/// This is only used for primitive types
		/// </summary>
		private string Data { get; set; }
		/// <summary>
		/// Differentiates Json instance types
		/// </summary>
		public JsonType Type { get; private set; }
		/// <summary>
		/// The child instances, primitives do not use this
		/// </summary>
		private List<Json> Children { get; set; }
		/// <summary>
		/// Defines different parsing states
		/// </summary>
		private enum ParsingState
		{
			/// <summary>
			/// The default parsing state until an actionable delimeter is found
			/// </summary>
			None,
			/// <summary>
			/// The state for parsing a JSON string, delimited by an opening and closing double quote
			/// </summary>
			String,
			/// <summary>
			/// The state for parsing a JSON number, which can include decimal points
			/// </summary>
			Number,
			/// <summary>
			/// The state for parsing a "true" value
			/// </summary>
			True,
			/// <summary>
			/// The state for parsing a "false" value
			/// </summary>
			False,
			/// <summary>
			/// The state for parsing a "null" value
			/// </summary>
			Null,
			/// <summary>
			/// The state for parsing a JSON object, which contains key value pairs in the form: "key1":value,"key2":value,etc..
			/// </summary>
			Object,
			/// <summary>
			/// The state for parsing a JSON array, which contains comma separated values in the form: value1,value2,etc..
			/// </summary>
			Array,
		}
		/// <summary>
		/// Determines the type of a Json node in the tree
		/// </summary>
		public enum JsonType
		{
			/// <summary>
			/// Represents a non existant value
			/// </summary>
			Null,
			/// <summary>
			/// Represents a string literal
			/// </summary>
			String,
			/// <summary>
			/// Represents a number literal; can be an integer or floating point
			/// </summary>
			Number,
			/// <summary>
			/// Represents a boolean value
			/// </summary>
			Boolean,
			/// <summary>
			/// Represents a map of key value pairs
			/// </summary>
			Object,
			/// <summary>
			/// Represents a list of values
			/// </summary>
			Array,
		}
		/// <summary>
		/// Recursively parses the input string as a Json node starting at
		/// index i
		/// </summary>
		/// <returns>A Json node</returns>
		static private Json ParseTree(ref string json, ref int i)
		{
			ParsingState state = ParsingState.None;
			Json root = new Json();
			for (; i < json.Length;)
			{
				char ch = json[i];
				if (state == ParsingState.None)
				{
					i++;
					switch (ch)
					{
						case '"':
							state = ParsingState.String;
							root.Type = JsonType.String;
							root.Data = "";
							break;
						case char c when c >= '0' && c <= '9':
							state = ParsingState.Number;
							root.Type = JsonType.Number;
							root.Data = c.ToString();
							break;
						case 't':
							state = ParsingState.True;
							root.Type = JsonType.Boolean;
							root.Data = ch.ToString();
							break;
						case 'f':
							state = ParsingState.False;
							root.Type = JsonType.Boolean;
							root.Data = ch.ToString();
							break;
						case 'n':
							state = ParsingState.Null;
							root.Type = JsonType.Null;
							root.Data = ch.ToString();
							break;
						case '{':
							state = ParsingState.Object;
							root.Type = JsonType.Object;
							root.Children = new List<Json>();
							break;
						case '[':
							state = ParsingState.Array;
							root.Type = JsonType.Array;
							root.Children = new List<Json>();
							break;
						case ']':
							return null;
						case '}':
							return null;
					}
				}
				else if (state == ParsingState.String)
				{
					i++;
					if (ch != '"')
					{
						root.Data += ch;
					}
					else
					{
						return root;
					}
				}
				else if (state == ParsingState.Number)
				{
					if (ch >= '0' && ch <= '9' || ch == '.')
					{
						root.Data += ch;
						i++;
					}
					else
					{
						return root;
					}
				}
				else if (state == ParsingState.Array)
				{
					Json child = ParseTree(ref json, ref i);
					if (child != null)
					{
						root.Children.Add(child);
					}
					else
					{
						return root;
					}
				}
				else if (state == ParsingState.Object)
				{
					Json child = ParseTree(ref json, ref i);
					if (child != null)
					{
						child.Children = new List<Json> { ParseTree(ref json, ref i) };
						root.Children.Add(child);
					}
					else
					{
						return root;
					}
				}
				else if (Char.IsLetter(ch))
				{
					root.Data += ch;
					i++;
				}
				else
				{
					return root;
				}
			}
			return root;
		}
		/// <summary>
		/// Tests whether this Object instance contains a key
		/// </summary>
		/// <param name="key">The key to test for</param>
		/// <returns>true if the key is present, else false</returns>
		public bool ContainsKey(string key)
		{
			if (Type != JsonType.Object)
				return false;

			return Children.Any(c => c.Data == key);
		}

		/// <summary>
		/// An accessor for Object nodes by key
		/// </summary>
		/// <param name="key">The key used for access</param>
		/// <returns>
		/// A Json node if the value is found, 
		///	else a null (not a Null instance)
		///	</returns>
		public Json this[string key]
		{
			get
			{
				if (Type == JsonType.Object)
					return Children.First(c => c.Data == key).Children.First();
				return null;
			}
			set
			{
				if (Type == JsonType.Object)
				{
					Json jsonKey = Children.Find(c => c.Data == key);
					if (jsonKey != null)
					{
						jsonKey.Children[0] = value;
					}
					else
					{
						jsonKey = new Json(key);
						jsonKey.Children = new List<Json> { value };
						Children.Add(jsonKey);
					}
				}
			}
		}
		/// <summary>
		/// An accessor for Array nodes by index
		/// </summary>
		/// <param name="index">The index used for access</param>
		/// <returns>A Json node if found, else null (not a Null instance)</returns>
		public Json this[int index]
		{
			get
			{
				if (Type == JsonType.Array)
					return Children.ElementAt(index);
				return null;
			}
			set
			{
				if (Type == JsonType.Array)
					Children[index] = value;
			}
		}
		public int Count
		{
			get
			{
				if (Type == JsonType.Array || Type == JsonType.Object)
					return Children.Count;
				return 0;
			}
		}
		/// <summary>
		//Purpose:
		//	Adds the Json node to an Array
		/// </summary>
		public void Add(Json json)
		{
			if (Type == JsonType.Array)
			{
				Children.Add(json);
			}
		}
		//--------------------------------------------------
		// Implict conversions from .NET primitives 
		// to Json primitives
		public static implicit operator Json(int number)
		{
			return new Json(number);
		}
		public static implicit operator Json(double number)
		{
			return new Json(number);
		}
		public static implicit operator Json(string str)
		{
			return new Json(str);
		}
		public static implicit operator Json(bool boolean)
		{
			return new Json(boolean);
		}
		//--------------------------------------------------
		// Implict conversions from Json primitives
		// to .NET primitives
		public static implicit operator int(Json json)
		{
			return Int32.Parse(json.Data);
		}
		public static implicit operator double(Json json)
		{
			return Double.Parse(json.Data);
		}
		public static implicit operator string(Json json)
		{
			return json.Data;
		}
		public static implicit operator bool(Json json)
		{
			return Boolean.Parse(json.Data);
		}
	}
}