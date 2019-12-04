using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EagleEye.Models;
namespace EagleEye.Tests
{
	public static class Tests
	{
		public static void RunTests() {
			EagleEye.Models.Geometry.Vector2 vector = new Models.Geometry.Vector2(1, 0);
			//JsonImport();\
		}
		static ParkingLot TestLot()
		{
			ParkingLot lot = new ParkingLot(0, "Test Lot", new Camera(0,"Test Camera"));
			Annotation Aisle = new Annotation(0, Annotation.AnnotationType.Aisle);
			Aisle.Points.Add(new Models.Geometry.Vector2(0, 0));
			Aisle.Points.Add(new Models.Geometry.Vector2(0.5, 0));
			Aisle.Points.Add(new Models.Geometry.Vector2(0.5, 0.5));
			Aisle.Points.Add(new Models.Geometry.Vector2(0, 0.5));
			lot.Annotations.Add(Aisle);
			Annotation Aisle2 = new Annotation(0, Annotation.AnnotationType.Aisle);
			Aisle2.Points.Add(new Models.Geometry.Vector2(0, 0.5));
			Aisle2.Points.Add(new Models.Geometry.Vector2(1, 0.5));
			Aisle2.Points.Add(new Models.Geometry.Vector2(1, 1));
			Aisle2.Points.Add(new Models.Geometry.Vector2(0, 1));
			lot.Annotations.Add(Aisle2);
			Annotation space = new Annotation(1, Annotation.AnnotationType.Parking);
			space.Points.Add(new Models.Geometry.Vector2(0.5, 0));
			space.Points.Add(new Models.Geometry.Vector2(1, 0));
			space.Points.Add(new Models.Geometry.Vector2(1, 0.25));
			space.Points.Add(new Models.Geometry.Vector2(0.5, 0.25));
			lot.Annotations.Add(space);
			Annotation space2 = new Annotation(1, Annotation.AnnotationType.Parking);
			space2.Points.Add(new Models.Geometry.Vector2(0.75, 0));
			space2.Points.Add(new Models.Geometry.Vector2(1, 0));
			space2.Points.Add(new Models.Geometry.Vector2(1, 0.25));
			space2.Points.Add(new Models.Geometry.Vector2(0.75, 0.25));
			lot.Annotations.Add(space2);
			return lot;
		}
		static void JsonImport()
		{
			Json json = Json.Import(new System.IO.StreamReader(HttpRuntime.AppDomainAppPath + "/App_Data/test.json"));
			if (json.Type != Models.Json.JsonType.Object)
			{
				throw new Exception("Json did not parse root as Object type");
			} else
			{
				if (!json.ContainsKey("test"))
				{
					throw new Exception("Json did not contain the correct key");
				} else
				{
					Json value = json["test"];
					if (value == null)
					{
						throw new Exception();
					} else
					{
						if (value.Count != 5)
						{
							throw new Exception();
						}
					}
				}
			}
			using (System.IO.StreamWriter stream = new System.IO.StreamWriter(HttpRuntime.AppDomainAppPath + "/App_Data/test_export.json")) {
				json.Export(stream);
			}
		}
	}
}