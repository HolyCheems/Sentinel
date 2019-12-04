using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using EagleEye.Models;
using System.Drawing;
using System.Threading;

namespace EagleEye
{
    public class EagleEyeConfig
    {
        private static string DatabasePath = $"{HttpRuntime.AppDomainAppPath}/App_Data/database.json";
        static ReaderWriterLock locker = new ReaderWriterLock();
        public static void ImportDatabase()
        {
            try
            {
                locker.AcquireReaderLock(Int32.MaxValue);
                using (var stream = new StreamReader(DatabasePath))
                {
                    Json database = Json.Import(stream);
                    Json cameras = database["Cameras"];
                    for (int i = 0; i < cameras.Count; i++)
                    {
                        Json cameraJson = cameras[i];
                        Camera camera = new Camera(cameraJson["ID"], cameraJson["Name"]);
                        Repository<Camera>.Add(camera);
                    }
                    Json zoneManagers = database["ZoneManagers"];
                    for (int i = 0; i < zoneManagers.Count; i++)
                    {
                        Json zmJson = zoneManagers[i];
                        ZoneManager zm = new ZoneManager(zmJson["ID"], zmJson["Name"]);
                        for (int p = 0; p < zmJson["Lots"].Count; p++)
                        {
                            Json lotJson = zmJson["Lots"][i];
                            ParkingLot lot = new ParkingLot(lotJson["ID"], lotJson["Name"], Repository<Camera>.Get(lotJson["Camera"]));
                            string bitmapPath = $"{HttpRuntime.AppDomainAppPath}App_Data\\{(int)lotJson["ID"]}.jpg";
                            if (File.Exists(bitmapPath))
                                lot.Baseline = new Bitmap(Bitmap.FromFile(bitmapPath) as Bitmap);
                            for (int a = 0; a < lotJson["Annotations"].Count; a++)
                            {
                                Json ann = lotJson["Annotations"][a];
                                Annotation annotation = new Annotation(ann["ID"], (Annotation.AnnotationType)(int)ann["Type"]);
                                for (int n = 0; n < ann["Points"].Count; n++)
                                {
                                    Json point = ann["Points"][n];
                                    Models.Geometry.Vector2 pnt = new Models.Geometry.Vector2(point["X"], point["Y"]);
                                    annotation.Add(pnt);
                                }
                                lot.Annotations.Add(annotation);
                            }
                            zm.Lots.Add(lot);
                        }
                        Repository<ZoneManager>.Add(zm);

                    }
                }
            }
            finally
            {
                locker.ReleaseReaderLock();
            }
        }
        public static void ExportDatabase()
        {
            try
            {
                locker.AcquireWriterLock(Int32.MaxValue);
                Json database = Json.Object;
                Json cameras = Json.Array;
                foreach (Camera camera in Repository<Camera>.Models.Values)
                {
                    Json cameraJson = Json.Object;
                    cameraJson["ID"] = camera.ID;
                    cameraJson["Name"] = camera.Name;
                    cameras.Add(cameraJson);
                }
                database["Cameras"] = cameras;
                Json zms = Json.Array;
                foreach (ZoneManager zm in Repository<ZoneManager>.Models.Values)
                {
                    Json zmJson = Json.Object;
                    zmJson["ID"] = zm.ID;
                    zmJson["Name"] = zm.Name;
                    Json lots = Json.Array;
                    foreach (var lot in zm.Lots)
                    {
                        Json lotJson = Json.Object;
                        lotJson["ID"] = lot.ID;
                        lotJson["Name"] = lot.Name;
                        lotJson["Camera"] = lot.Camera != null ? lot.Camera.ID : -1;
                        Json annotations = Json.Array;
                        foreach (var annotation in lot.Annotations)
                        {
                            Json ann = Json.Object;
                            ann["ID"] = annotation.ID;
                            ann["Type"] = (int)annotation.Type;
                            Json points = Json.Array;
                            foreach (var point in annotation.Points)
                            {
                                Json p = Json.Object;
                                p["X"] = point.X;
                                p["Y"] = point.Y;
                                points.Add(p);
                            }
                            ann["Points"] = points;
                            annotations.Add(ann);
                        }
                        lotJson["Annotations"] = annotations;
                        lots.Add(lotJson);
                        if (lot.Baseline != null)
                            try
                            {
                                lot.Baseline.Save($"{HttpRuntime.AppDomainAppPath}App_Data\\{lot.ID}.jpg");
                            }
                            catch (Exception ex)
                            {

                            }
                    }
                    zmJson["Lots"] = lots;
                    zms.Add(zmJson);
                }

                database["ZoneManagers"] = zms;

                using (StreamWriter stream = new StreamWriter(DatabasePath))
                {
                    database.Export(stream);
                }
            }
            finally
            {
                locker.ReleaseWriterLock();
            }
        }
        public static int WebImageWidth { get => 400; }
        public static Mutex Mutex { get; } = new Mutex();
    }
}