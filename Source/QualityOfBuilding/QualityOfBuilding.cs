using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace QualityOfBuilding
{
    public class QualityCheckComp : ThingComp
    {
        public bool QualityChecked = false;
        public bool needToUpdate = true;
        public bool forceToUpdate = false;
        //TODO
        public Graphic qualityGraphic = null; //caching graphic is not working now
        public Graphic qualityStyleGraphic = null; //caching graphic is not working now

        public void Clear()
        {
            QualityChecked = false;
            needToUpdate = true;
            forceToUpdate = false;
            qualityGraphic = null;
            qualityStyleGraphic = null;
        }
        public void Update(Graphic graphicInput)
        {

        }

    }


    public static class QualityOfBuildingDatabase
    {
        public struct PathWithQuality
        {
            public string originalGraphicPath;
            public QualityCategory quality;

            public PathWithQuality(string originalGraphicPath, QualityCategory quality)
            {
                this.originalGraphicPath = originalGraphicPath;
                this.quality = quality;
            }
        }
        public struct PathWithQualityComparer : IEqualityComparer<PathWithQuality>
        {
            public bool Equals(PathWithQuality x, PathWithQuality y)
            {
                return (x.originalGraphicPath == y.originalGraphicPath) && (x.quality == y.quality);
            }

            public int GetHashCode(PathWithQuality obj)
            {
                return obj.GetHashCode();
            }
        }

        private static Dictionary<PathWithQuality, Graphic> qualityGraphics = new Dictionary<PathWithQuality, Graphic>(new PathWithQualityComparer());
        private static Dictionary<PathWithQuality, Graphic> qualityGraphicsSingle = new Dictionary<PathWithQuality, Graphic>(new PathWithQualityComparer());
        private static Dictionary<PathWithQuality, Graphic> qualityGraphicsMulti = new Dictionary<PathWithQuality, Graphic>(new PathWithQualityComparer());

        public static bool GetQualityGraphic(PathWithQuality info, Type graphicType , out Graphic graphic)
        {
            Graphic result;

            if(graphicType == typeof(Graphic_Single))
            {
                if (QualityOfBuildingDatabase.qualityGraphicsSingle.TryGetValue(info, out result))
                {
                    graphic = result;
                    return true;
                }
                graphic = null;
                return false;
            }
            else if (graphicType == typeof(Graphic_Multi))
            {
                if (QualityOfBuildingDatabase.qualityGraphicsMulti.TryGetValue(info, out result))
                {
                    graphic = result;
                    return true;
                }
                graphic = null;
                return false;
            }
            else
            {
                if (QualityOfBuildingDatabase.qualityGraphics.TryGetValue(info, out result))
                {
                    graphic = result;
                    return true;
                }
                graphic = null;
                return false;
            }


        }

        public static void SetQualityGraphic(string originalPath, QualityCategory quality,Type graphicType , Graphic graphic)
        {
            if (graphicType == typeof(Graphic_Single))
            {
                QualityOfBuildingDatabase.qualityGraphicsSingle.SetOrAdd(new PathWithQuality(originalPath, quality), graphic);
                return;
            }
            else if (graphicType == typeof(Graphic_Multi))
            {
                QualityOfBuildingDatabase.qualityGraphicsMulti.SetOrAdd(new PathWithQuality(originalPath, quality), graphic);
                return;
            }
            else
            {
                QualityOfBuildingDatabase.qualityGraphics.SetOrAdd(new PathWithQuality(originalPath, quality), graphic);
            }

        }

        public static void ClearAll()
        {
            qualityGraphics.Clear();
            qualityGraphicsSingle.Clear();
            qualityGraphicsMulti.Clear();
        }


    }


    public class QualityOfBuildingUtility
    {
        public static List<string> QualityStrings = new List<string>() { "_Quality0", "_Quality1", "_Quality2", "_Quality3", "_Quality4", "_Quality5", "_Quality6" };
        //public static List<string> QualityStrings = new List<string>() {  };

        public static bool IsValidTexture(string path)
        {
            if (path == null || path == string.Empty)
                return false;
            if (ContentFinder<Texture2D>.Get(path, false) != null)
                return true;
            if (ContentFinder<Texture2D>.Get(path + "_north", false) != null)
                return true;
            if (ContentFinder<Texture2D>.Get(path + "_east", false) != null)
                return true;
            if (ContentFinder<Texture2D>.Get(path + "_south", false) != null)
                return true;
            if (ContentFinder<Texture2D>.Get(path + "_west", false) != null)
                return true;

            if (QualityOfBuildingSetting.DebugLog)
                Log.Warning("[Quality of Building]There is no texture for: " + path);
            return false;
        }

        public static string QualityToString(QualityCategory quality)
        {

            if (quality == QualityCategory.Awful)
                return QualityStrings[0]; // missing Awful texture will use Poor texture.
            if (quality == QualityCategory.Poor)
                return QualityStrings[1]; // missing poor texture will use awful texture.
            if (quality == QualityCategory.Normal)
                return QualityStrings[2]; // missing normal texture will use original texture.
            if (quality == QualityCategory.Good)
                return QualityStrings[3]; // missing good texture will use normal texture.
            if (quality == QualityCategory.Excellent)
                return QualityStrings[4]; // missing Excellent texture will use Masterwork texture.
            if (quality == QualityCategory.Masterwork)
                return QualityStrings[5]; // missing masterwork texture will use exellent texture.
            if (quality == QualityCategory.Legendary)
                return QualityStrings[6]; // missing legendery texture will use masterwork texture.
            return string.Empty;
        }
        public static string MakePath(string path ,bool useSubFolder, QualityCategory quality)//1.1 and 1.2 but may be broken
        {
            
            string fileName = System.IO.Path.GetFileName(path);
            string folderPath = useSubFolder? path.Substring(0, Math.Max(path.Length - fileName.Length - 1, 0)):"";
            string folderName = System.IO.Path.GetFileName(folderPath);
            //string upperFolder = path.Substring(0, Math.Max(path.Length - fileName.Length - folderName.Length - 2, 0));
            //string targetPath = useSubFolder ? System.IO.Path.Combine(folder + "_Quality", filename) : path;
            string targetPath = useSubFolder ? path.Insert(Math.Max(path.Length - fileName.Length - folderName.Length - 1,0),"Quality_") : path;
            //Log.Message(targetPath);

            string newPath = string.Empty;
            if(!targetPath.EndsWith(QualityToString(quality)))
                newPath = targetPath + QualityToString(quality);
            if (IsValidTexture(newPath))
                return newPath;
            else
            {
                //checking lower or upper quality texture for missing
                if (quality == QualityCategory.Awful)
                {

                    newPath = targetPath + QualityToString(QualityCategory.Poor);
                    if (IsValidTexture(newPath))
                        return newPath;
                }
                if (quality == QualityCategory.Poor)
                {

                    newPath = targetPath + QualityToString(QualityCategory.Awful);
                    if (IsValidTexture(newPath))
                        return newPath;
                }
                if (quality == QualityCategory.Normal)
                {
                    return string.Empty;
                }
                if (quality == QualityCategory.Good)
                {

                    newPath = targetPath + QualityToString(QualityCategory.Normal);
                    if (IsValidTexture(newPath))
                        return newPath;
                }
                if (quality == QualityCategory.Excellent)
                {

                    newPath = targetPath + QualityToString(QualityCategory.Masterwork);
                    if (IsValidTexture(newPath))
                        return newPath;
                }
                if (quality == QualityCategory.Masterwork)
                {

                    newPath = targetPath + QualityToString(QualityCategory.Excellent);
                    if (IsValidTexture(newPath))
                        return newPath;
                }
                if (quality == QualityCategory.Legendary)
                {
                    newPath = targetPath + QualityToString(QualityCategory.Excellent);
                    if (IsValidTexture(newPath))
                        return newPath;

                    newPath = targetPath + QualityToString(QualityCategory.Masterwork);
                    if (IsValidTexture(newPath))
                        return newPath;


                }
            }
            return string.Empty;
        }
        /*
        public static string MakePath(string pathname, string filename, QualityCategory quality, bool rotatable)
        {
            string newPath = pathname + "/Quality/" + filename + QualityToString(quality);
            if (ContentFinder<Texture2D>.Get(rotatable ? newPath + "_north" : newPath, true) != null)
                return newPath;
            else
            {
                if (quality == QualityCategory.Awful)
                {
                    newPath = pathname + "/Quality/" + filename + QualityToString(QualityCategory.Poor);
                    if (ContentFinder<Texture2D>.Get(rotatable ? newPath + "_north" : newPath, true) != null)
                        return newPath;
                }
                if (quality == QualityCategory.Poor)
                {
                    newPath = pathname + "/Quality/" + filename + QualityToString(QualityCategory.Awful);
                    if (ContentFinder<Texture2D>.Get(rotatable ? newPath + "_north" : newPath, true) != null)
                        return newPath;
                }
                if (quality == QualityCategory.Normal)
                {
                    return pathname + "/" + filename;
                }
                if (quality == QualityCategory.Good)
                {
                    newPath = pathname + "/Quality/" + filename + QualityToString(QualityCategory.Normal);
                    if (ContentFinder<Texture2D>.Get(rotatable ? newPath + "_north" : newPath, false) != null)
                        return newPath;
                }
                if (quality == QualityCategory.Excellent)
                {
                    newPath = pathname + "/Quality/" + filename + QualityToString(QualityCategory.Masterwork);
                    if (ContentFinder<Texture2D>.Get(rotatable ? newPath + "_north" : newPath, false) != null)
                        return newPath;
                }
                if (quality == QualityCategory.Masterwork)
                {
                    newPath = pathname + "/Quality/" + filename + QualityToString(QualityCategory.Excellent);
                    if (ContentFinder<Texture2D>.Get(rotatable ? newPath + "_north" : newPath, false) != null)
                        return newPath;
                }
                if (quality == QualityCategory.Legendary)
                {
                    newPath = pathname + "/Quality/" + filename + QualityToString(QualityCategory.Masterwork);
                    if (ContentFinder<Texture2D>.Get(rotatable ? newPath + "_north" : newPath, false) != null)
                        return newPath;
                    newPath = pathname + "/Quality/" + filename + QualityToString(QualityCategory.Excellent);
                    if (ContentFinder<Texture2D>.Get(rotatable ? newPath + "_north" : newPath, false) != null)
                        return newPath;
                }
            }
            return pathname + "/" + filename;
        }
        */
        public static string MakePathForQuality(string path, bool useSubFolder, QualityCategory quality)//1.3
        {

            string fileName = System.IO.Path.GetFileName(path);
            //string folderPath = useSubFolder ? path.Substring(0, Math.Max(path.Length - fileName.Length - 1, 0)) : "";
            string folderPath = !useSubFolder ? path.Substring(0, Math.Max(path.Length - fileName.Length - 1, 0)) : "";
            string folderName = System.IO.Path.GetFileName(folderPath);
            //string upperFolder = path.Substring(0, Math.Max(path.Length - fileName.Length - folderName.Length - 2, 0));
            //string targetPath = useSubFolder ? System.IO.Path.Combine(folder + "_Quality", filename) : path;
            string targetPath = useSubFolder ? path.Insert(Math.Max(path.Length - fileName.Length - folderName.Length - 1, 0), "Quality_") : path;

            //Log.Message(targetPath);

            string newPath = string.Empty;
            if (!targetPath.EndsWith(QualityToString(quality)))
                newPath = targetPath + QualityToString(quality);
            if (IsValidTexture(newPath))
                return newPath;
            else
            {
                //checking lower or upper quality texture for missing
                if (quality == QualityCategory.Awful)
                {
                    if (!newPath.EndsWith(QualityToString(QualityCategory.Poor)))
                        newPath = targetPath + QualityToString(QualityCategory.Poor);
                    if (IsValidTexture(newPath))
                        return newPath;
                }
                if (quality == QualityCategory.Poor)
                {
                    if (!newPath.EndsWith(QualityToString(QualityCategory.Awful)))
                        newPath = targetPath + QualityToString(QualityCategory.Awful);
                    if (IsValidTexture(newPath))
                        return newPath;
                }
                if (quality == QualityCategory.Normal)
                {
                    return string.Empty;
                }
                if (quality == QualityCategory.Good)
                {
                    if (!newPath.EndsWith(QualityToString(QualityCategory.Good)))
                        newPath = targetPath + QualityToString(QualityCategory.Normal);
                    if (IsValidTexture(newPath))
                        return newPath;
                }
                if (quality == QualityCategory.Excellent)
                {
                    if (!newPath.EndsWith(QualityToString(QualityCategory.Masterwork)))
                        newPath = targetPath + QualityToString(QualityCategory.Masterwork);
                    if (IsValidTexture(newPath))
                        return newPath;
                }
                if (quality == QualityCategory.Masterwork)
                {
                    if (!newPath.EndsWith(QualityToString(QualityCategory.Excellent)))
                        newPath = targetPath + QualityToString(QualityCategory.Excellent);
                    if (IsValidTexture(newPath))
                        return newPath;
                }
                if (quality == QualityCategory.Legendary)
                {
                    if (!newPath.EndsWith(QualityToString(QualityCategory.Masterwork)))
                        newPath = targetPath + QualityToString(QualityCategory.Masterwork);
                    if (IsValidTexture(newPath))
                        return newPath;
                    if (!newPath.EndsWith(QualityToString(QualityCategory.Excellent)))
                        newPath = targetPath + QualityToString(QualityCategory.Excellent);
                    if (IsValidTexture(newPath))
                        return newPath;
                }
            }
            return string.Empty;
        }

        //1.1 and 1.2
        public static void DoPrefix(ThingWithComps __instance, ref Graphic ___graphicInt,bool useSubFolder ,bool ignorCache, bool writeCache)
        {
            //Log.Message("Hello!");

            if (__instance == null)
                return;

            var compQuality = __instance.TryGetComp<CompQuality>();
            if (compQuality != null)
            {
                //Log.Message("QualityCompFounded");

                var qualityCheckComp = __instance.TryGetComp<QualityCheckComp>();
                if (qualityCheckComp != null)
                {


                    //Log.Message("QualityCheckCompFounded");
                    if (___graphicInt == null)
                    {
                        //Log.Message("___graphicInt Null");
                        qualityCheckComp.QualityChecked = false;
                        /*
                        if (__instance.def == null)
                            return;
                        if (__instance.def.graphicData == null)
                        {
                            return;
                        }
                        */
                        //___graphicInt = __instance.def.graphicData.GraphicColoredFor(__instance);
                        //Log.Message("___graphicInt filled");
                    }

                    if (ignorCache==true || qualityCheckComp.QualityChecked == false)
                    {
                        /*
                        if(___graphicInt.data != null || ___graphicInt.data.graphicClass != null)
                            Log.Message(___graphicInt.data.graphicClass.ToString());
                        */
                        if (___graphicInt==null|| ___graphicInt.path ==null || ___graphicInt.GetType() == typeof(Graphic_Random))
                        {
                            
                            //Log.Message("GraphicRandom or Null");
                            //TODO :: sculptures
                            /*
                             * ___graphicInt.data != null
                            if(___graphicInt.data.graphicClass == typeof(Graphic_Random))
                            {

                            }
                            var targetQuality = compQuality.Quality;
                            bool flag = false;
                            var fixgraphic = __instance.DefaultGraphic;// this is for avoid null graphicInt
                            string path = __instance.Graphic.path;
                            string filename;
                            string pathname;
                            pathname = System.IO.Path.GetDirectoryName(path);
                            filename = System.IO.Path.GetFileName(path);
                            var graphicRandom = ___graphicInt as Graphic_Random;
                            graphicRandom.SubGraphicFor(__instance);
                            ___graphicInt = GraphicDatabase.Get<Graphic_Random>(___graphicInt.path, ___graphicInt.Shader, ___graphicInt.drawSize, ___graphicInt.color, ___graphicInt.colorTwo);
                            */
                        }
                        else
                        {
                            var targetQuality = compQuality.Quality;



                            switch (targetQuality)
                            {
                                case QualityCategory.Awful:
                                    if (QualityOfBuildingSetting.UseAwfulTexture == false)
                                        return;
                                    break;
                                case QualityCategory.Poor:
                                    if (QualityOfBuildingSetting.UsePoorTexture == false)
                                        return;
                                    break;
                                case QualityCategory.Normal:
                                    if (QualityOfBuildingSetting.UseNormalTexture == false)
                                        return;
                                    break;
                                case QualityCategory.Good:
                                    if (QualityOfBuildingSetting.UseGoodTexture == false)
                                        return;
                                    break;
                                case QualityCategory.Excellent:
                                    if (QualityOfBuildingSetting.UseExcellentTexture == false)
                                        return;
                                    break;
                                case QualityCategory.Masterwork:
                                    if (QualityOfBuildingSetting.UseMasterworkTexture == false)
                                        return;
                                    break;
                                case QualityCategory.Legendary:
                                    if (QualityOfBuildingSetting.UseLegendaryTexture == false)
                                        return;
                                    break;
                            }

                            bool flag = false;
                            //var fixgraphic = __instance.DefaultGraphic;// this is for avoid null graphicInt
                            //string path = __instance.Graphic.path;
                            //Log.Message(___graphicInt.path.ToString());
                            //string path = ___graphicInt == null ? __instance.Graphic.path:___graphicInt.path;
                            string path = ___graphicInt.path;
                            
                            
                            //string filename;
                            //string pathname;
                            //pathname = System.IO.Path.GetDirectoryName(path);
                            //filename = System.IO.Path.GetFileName(path);
                            
                            string newPath = MakePath(path, useSubFolder, targetQuality);
                            flag = (IsValidTexture(newPath));
                            if (flag)
                            {
                                //Log.Message(___graphicInt.path.ToString() + " : texturefound :: Quality: "+ targetQuality.ToString());
                                //Log.Message(___graphicInt.GetType().ToString());
                                //Log.Message(newPath);
                                if (___graphicInt.GetType() == typeof(Graphic_Multi))
                                    ___graphicInt = GraphicDatabase.Get<Graphic_Multi>(newPath, ___graphicInt.Shader, ___graphicInt.drawSize, ___graphicInt.color, ___graphicInt.colorTwo);
                                else if (___graphicInt.GetType() == typeof(Graphic_Single))
                                    ___graphicInt = GraphicDatabase.Get<Graphic_Single>(newPath, ___graphicInt.Shader, ___graphicInt.drawSize, ___graphicInt.color);
                                else
                                {
                                    //___graphicInt.data.graphicClass
                                    ___graphicInt = GraphicDatabase.Get(___graphicInt.GetType(), newPath, ___graphicInt.Shader, ___graphicInt.drawSize, ___graphicInt.color, ___graphicInt.colorTwo);
                                }
                                /*
                                if (__instance.def.coversFloor)
                                {
                                    __instance.Map.mapDrawer.MapMeshDirty(__instance.Position, MapMeshFlag.Terrain, true, false);
                                }
                                */
                                //Log.Message(__instance.Map.ToString());
                                //__instance.DirtyMapMesh(__instance.Map);

                                /*
                                CellRect cellRect = __instance.OccupiedRect();
                                for (int i = cellRect.minZ; i <= cellRect.maxZ; i++)
                                {
                                    for (int j = cellRect.minX; j <= cellRect.maxX; j++)
                                    {
                                        //__instance.
                                        IntVec3 intVec = new IntVec3(j, 0, i);
                                        __instance.Map.mapDrawer.MapMeshDirty(intVec, MapMeshFlag.Buildings);
                                        __instance.Map.glowGrid.MarkGlowGridDirty(intVec);
                                        if (!SnowGrid.CanCoexistWithSnow(__instance.def))
                                        {
                                            __instance.Map.snowGrid.SetDepth(intVec, 0f);
                                        }
                                    }
                                }*/

                            }
                            else
                            {
                                if(QualityOfBuildingSetting.DebugLog)
                                    Log.Warning("[Quality of Building] missing quality texture for: " + __instance.ToString() + "\nIt will use original Texture");
                            }
                            //qualityCheckComp.QualityChecked = true;
                        }
                        if(writeCache)
                        qualityCheckComp.QualityChecked = true;
                    }

                    //if(ContentFinder<Texture2D>.Get(pathname+"/Quality/"+filename+))
                    //__instance.Graphic
                }
            }
        }


        //1.3
        public static Graphic GetQualityGraphicAndCache(ThingWithComps instance,  Graphic graphic, bool forceToUseSubFolder = false, bool isStyleGraphic = false, bool forceUpate = false, bool doCache = true)
        {
            //Log.Message("Hello!");

            

            if (instance == null)
                return graphic;

            if (graphic == null)
                return null;

            var compQuality = instance.TryGetComp<CompQuality>();
            if (compQuality == null)
                return graphic;
            var qualityCheckComp = instance.TryGetComp<QualityCheckComp>();
            if (qualityCheckComp == null)
                return graphic;

            bool flag = qualityCheckComp.needToUpdate;
            if (forceUpate)
                flag = true;

            flag = true;//for testing   //caching graphic is not working now


            bool useSubForder = forceToUseSubFolder;

            QualityCategory targetQuality = compQuality.Quality;
            /*
            if (instance.def.graphicData != null)
            {
                if (instance.def.graphicData.graphicClass == typeof(Graphic_Random) || instance.def.graphicData.graphicClass == typeof(Graphic_RandomRotated))
                {
                    return graphic;//todo
                }

            }*/

            Graphic result = graphic;
            string path = graphic.path;

            Graphic cachedGraphic = null;
            if (graphic as Graphic_Collection != null)
            {
                /*
                if (cachedGraphic != null)
                {
                    if (QualityOfBuildingSetting.DebugLog)
                        Log.Message("[Quality of Building] it is cached Graphic_Collection :" + path);
                    return graphic; //Get InnerGraphic will handle it.
                }*/

                //return graphic; //GraphicDatabase is caching graphic in dictionary
                FieldInfo subGraphicsFieldInfo = AccessTools.Field(typeof(Graphic_Collection), "subGraphics");
                Graphic[] subGraphics = (Graphic[])subGraphicsFieldInfo.GetValue(graphic);
                List<Graphic> newSubGraphics = new List<Graphic>();
                //Graphic graphicCopy = GraphicDatabase.Get(graphic.GetType(), graphic.path, graphic.Shader, graphic.drawSize, graphic.color, graphic.colorTwo, graphic.data, graphic.data.shaderParameters);
                //Graphic graphicCopy = graphic.GetCopy(graphic.drawSize); // not clone...
                
                //Graphic graphicCopy = new Graphic();


                //Graphic[] newSubGraphics = new Graphic[subGraphics.Length];
                foreach (Graphic subGraphic in subGraphics)
                {
                    //cachedGraphic = QualityOfBuildingDatabase.GetQualityGraphic(new QualityOfBuildingDatabase.PathWithQuality(subGraphic.path, targetQuality));
                    Graphic newSubGraphic = QualityOfBuildingUtility.GetQualityGraphicAndCache(instance, subGraphic, true, isStyleGraphic, forceUpate, doCache);
                    //newSubGraphics.Add(newSubGraphic);
                }

                return graphic;

                //Graphic graphicCollectionCopied = graphic.
                //subGraphicsFieldInfo.SetValue(graphicCopy, newSubGraphics.ToArray());
                //graphicCopy.path = ??;//need to change! because of caching in dictionary
                //return graphicCopy;

                if (graphic as Graphic_Random != null)
                    return graphic; //skip if it is Graphic_Random
            }

            if (graphic as Graphic_RandomRotated != null)
            {
                FieldInfo subGraphicFieldInfo = null;
                Graphic subGraphic = null;
                subGraphicFieldInfo = AccessTools.Field(typeof(Graphic_RandomRotated), "subGraphic");
                subGraphic = (Graphic)subGraphicFieldInfo.GetValue((Graphic_RandomRotated)graphic);
                float rot = (float)AccessTools.Field(typeof(Graphic_RandomRotated), "maxAngle").GetValue(graphic);


                Graphic newSubGraphic;
                newSubGraphic = QualityOfBuildingUtility.GetQualityGraphicAndCache(instance, subGraphic, useSubForder, isStyleGraphic, forceUpate, doCache);
                Graphic rotatedGrahpic = new Graphic_RandomRotated(newSubGraphic, rot);
                //subGraphicFieldInfo.SetValue(rotatedGrahpic, newSubGraphic);
                return rotatedGrahpic;
                /*
                path = subGraphic.path;
                //rangedWeapon Bug Fix.... I know this is bad way to fix but...
                string fileName = System.IO.Path.GetFileName(path);
                if (fileName.Contains("_Quality"))
                {
                    if (QualityOfBuildingSetting.DebugLog)
                        Log.Warning("[Quality of Builing] alredy patched graphic. trying to get def graphic path for:" + path);
                    if (instance.def.graphicData != null)
                    {
                        //instance.def.graphicData.Init();
                        path = instance.def.graphicData.texPath;
                        if (QualityOfBuildingSetting.DebugLog)
                            Log.Message("[Quality of Builing] using Default Def Graphic path for:" + path);
                    }
                }*/
            }



            if (!flag)
            {
                return graphic;
                //not using cahce for now
                if (isStyleGraphic)
                {
                    if (qualityCheckComp.qualityStyleGraphic == null)
                        return graphic;
                    return qualityCheckComp.qualityStyleGraphic;
                }
                if (qualityCheckComp.qualityGraphic == null)
                    return graphic;
                return qualityCheckComp.qualityGraphic;
            }



            //Graphic_Random graphic_Random = graphic as Graphic_Random;



            



            switch (targetQuality)
            {
                case QualityCategory.Awful:
                    if (QualityOfBuildingSetting.UseAwfulTexture == false)
                        return graphic;
                    break;
                case QualityCategory.Poor:
                    if (QualityOfBuildingSetting.UsePoorTexture == false)
                        return graphic;
                    break;
                case QualityCategory.Normal:
                    if (QualityOfBuildingSetting.UseNormalTexture == false)
                        return graphic;
                    break;
                case QualityCategory.Good:
                    if (QualityOfBuildingSetting.UseGoodTexture == false)
                        return graphic;
                    break;
                case QualityCategory.Excellent:
                    if (QualityOfBuildingSetting.UseExcellentTexture == false)
                        return graphic;
                    break;
                case QualityCategory.Masterwork:
                    if (QualityOfBuildingSetting.UseMasterworkTexture == false)
                        return graphic;
                    break;
                case QualityCategory.Legendary:
                    if (QualityOfBuildingSetting.UseLegendaryTexture == false)
                        return graphic;
                    break;
            }


            if (path == null)//other things like weapons
            {
                if (!QualityOfBuildingSetting.ApplyToNonBuildingThings)
                {
                    if(doCache)
                        qualityCheckComp.needToUpdate = false;
                    return graphic;
                }

                /*
                if (instance.def.graphicData != null)
                {
                    if (instance.def.graphicData.graphicClass == typeof(Graphic_Random))
                    {
                        return graphic;//todo
                    }

                }*/




                if (path == null)
                {
                    if(doCache)
                        qualityCheckComp.needToUpdate = false;
                    return graphic;
                }
            }


            

            if(QualityOfBuildingDatabase.GetQualityGraphic(new QualityOfBuildingDatabase.PathWithQuality(path, targetQuality), graphic.GetType(), out cachedGraphic))
            {
                if (doCache)
                    qualityCheckComp.needToUpdate = false;
                if (cachedGraphic != null)
                {
                    if (QualityOfBuildingSetting.DebugLog)
                        Log.Message("[Quality of Building] it is cached Graphic :" + path + " Quality :" + targetQuality.ToString());
                    return cachedGraphic.GetColoredVersion(graphic.Shader, graphic.Color , graphic.ColorTwo); //Get InnerGraphic will handle it.
                }
                if (QualityOfBuildingSetting.DebugLog)
                    Log.Message("[Quality of Building] it is cached Graphic But Not Supported :" + path + " Quality :" + targetQuality.ToString());
                return graphic;
            }

            /*
            string fileName = System.IO.Path.GetFileName(path);
            if (fileName.Contains("_Quality"))
            {
                if (QualityOfBuildingSetting.DebugLog)
                    Log.Message("[Quality of Builing] already Patched Item:" + path);
            }
            */


            if (QualityOfBuildingSetting.DebugLog)
                Log.Message("[Quality of Builing] trying to make new Path for Graphic:" + path);
            path = MakePath(path, useSubForder, targetQuality);

            if (!IsValidTexture(path))
            {

                if (QualityOfBuildingSetting.DebugLog)
                    Log.Warning("[Quality of Builing] Invalid Texture Path for Graphic:" + graphic.path);
                if (doCache)
                    qualityCheckComp.needToUpdate = false;
                if(path == string.Empty)
                {
                    QualityOfBuildingDatabase.SetQualityGraphic(graphic.path, targetQuality, graphic.GetType() , null);
                    return graphic;
                }


            }
            if (doCache)
                qualityCheckComp.needToUpdate = false;



            if (QualityOfBuildingSetting.DebugLog)
            {
                Log.Message("[Quality of Builing] trying to get Graphic:" + path);
                Log.Message("[Quality of Builing] graphic's type:" + graphic.GetType());
            }

            /*
            if(graphic as Graphic_Collection != null)
            {
                Log.Warning("[Quality of Builing] Grapic_Collection type doesn't Support! :" + path);
                return graphic;
            }*/

            if (graphic.GetType() == typeof(Graphic_Single) || graphic.GetType() == typeof(Graphic_Multi))
            {
                if(graphic.GetType() == typeof(Graphic_Single))
                    result = GraphicDatabase.Get<Graphic_Single>(path, graphic.Shader, graphic.drawSize, graphic.color, graphic.colorTwo, graphic.data);
                else if(graphic.GetType() == typeof(Graphic_Multi))
                    result = GraphicDatabase.Get<Graphic_Multi>(path, graphic.Shader, graphic.drawSize, graphic.color, graphic.colorTwo, graphic.data);

                QualityOfBuildingDatabase.SetQualityGraphic(graphic.path, targetQuality, graphic.GetType(), result);
                if (doCache)
                    qualityCheckComp.needToUpdate = false;
            }

            else
            {
                if (doCache)
                    qualityCheckComp.needToUpdate = false;
                if (QualityOfBuildingSetting.DebugLog)
                    Log.Warning("[Quality of Builing] the type of graphic is not graphic_single or graphic_multi. Skipping Quality Graphic for :" + path);
                return graphic;
            }
            
            /*
            else // graphic is Graphic_RandomRotated
            {
                if(subGraphic != null && subGraphicFieldInfo != null)
                {
                    Graphic subGraphicResult = null;
                    if (subGraphic.GetType() == typeof(Graphic_Single) || subGraphic.GetType() == typeof(Graphic_Multi))
                    {
                        subGraphicResult = GraphicDatabase.Get(subGraphic.GetType(), path, subGraphic.Shader, subGraphic.drawSize, subGraphic.color, subGraphic.colorTwo);
                        //result = graphic.GetCopy(graphic.drawSize);
                        //result = new Graphic_RandomRotated(subGraphicResult, onGroundRandomRotateAngle)

                    }
                    else
                    {
                        if (QualityOfBuildingSetting.DebugLog)
                            Log.Warning("[Quality of Builing] the type of graphic is not graphic_single or graphic_multi. Skipping Quality Graphic for :" + path);
                        return graphic;
                    }
                    subGraphicFieldInfo.SetValue((Graphic_RandomRotated)result, subGraphicResult);
                    if (QualityOfBuildingSetting.DebugLog)
                        Log.Message("[Quality of Builing] Set Subgraphic :" + path);
                }
                else
                {
                    if (QualityOfBuildingSetting.DebugLog)
                        Log.Warning("[Quality of Builing] Failed To Set Subgraphic :" + path);
                    return graphic;
                }
            }*/

            if (result == null)
            {
                if (QualityOfBuildingSetting.DebugLog)
                    Log.Error("[Quality of Builing] Fail to Get Graphic in path:" + path);//mabe it doesn't return null but crash.
                return graphic;
            }

            //cache to component
            if (isStyleGraphic)
                if (doCache)
                    qualityCheckComp.qualityStyleGraphic = result;
            else
                if (doCache)
                    qualityCheckComp.qualityGraphic = result;



            if (QualityOfBuildingSetting.DebugLog)
                Log.Message("  [Quality of Builing]Quality Graphic Patched:" + path);//mabe it doesn't return null but crash.
            return result;

        }





        //1.3 TODO
        public static Graphic GetQulityGraphicAndCacheForWornApparel(ThingWithComps instance, Graphic graphic, bool forceToUseSubFolder = false, bool isStyleGraphic = false, bool forceUpate = false)
        {
            return null;
        }
    }


    [StaticConstructorOnStartup]
    public class QualityOfBuildingMain
    {
        static QualityOfBuildingMain()
        {
            Log.Message("[Quality Of Building] patching");
            var harmony = new Harmony("QualityOfBuilding");
            harmony.PatchAll();
        }
    }



    /*
    //for sculptures
    [HarmonyPatch(typeof(Graphic_Collection))]
    [HarmonyPatch("Init")]
    public class QualityOfBuildingGraphicCollectionPatch
    {
        public static void Postfix(Graphic_Collection __instance, GraphicRequest req, ref Graphic[] ___subGraphics)
        {
            //Graphic[] newSubGraphics = new Graphic[] { };
            List<Graphic> newSubGraphics = new List<Graphic>();
            foreach(Graphic subGraphic in ___subGraphics)
            {
                //Log.Message(subGraphic.path);
                if (subGraphic.path.Contains("_Quality") == false)
                {
                    //Log.Message("_Quality not found");
                    newSubGraphics.Add(subGraphic);
                }

            }
            ___subGraphics = new Graphic[newSubGraphics.Count];
            for(int i = 0; i< newSubGraphics.Count; i++)
            {
                ___subGraphics[i] = GraphicDatabase.Get(typeof(Graphic_Single), newSubGraphics[i].path, req.shader, __instance.drawSize, __instance.color, __instance.colorTwo, null, req.shaderParameters);
            }

        }
    }
    */


    //for sculptures
    [HarmonyPatch(typeof(Graphic_Random))]
    [HarmonyPatch("SubGraphicFor")]
    public class QualityOfBuildingGraphicRandomPatch
    {
        //1.1 and 1.2
        public static void OldPostfix(Thing thing, ref Graphic __result)
        {
            
            ThingWithComps thingWithComp = thing as ThingWithComps;
            if (thingWithComp != null)
            {
                //Log.Message("SubGraphicFor");
                //Log.Message(thing.ToString() + "is Graphic Random");
                //Log.Message(__result.ToString());

                //Log.Message(__result.data.ToString()); // null
                //Log.Message(__result.data.graphicClass.ToString()); //null
                QualityOfBuildingUtility.DoPrefix(thingWithComp , ref __result,true,true,true);
            }

            else
            {
                //Log.Message(thing.ToString() + "is not ThingWithComp");
            }

        }
        //1.3
        public static void Postfix(Thing thing, ref Graphic __result)
        {

            ThingWithComps thingWithComps = thing as ThingWithComps;

            if (thingWithComps == null)
                return;
            var qualityCheckComp = thing.TryGetComp<QualityCheckComp>();
            if (qualityCheckComp == null)
                return;

            __result = QualityOfBuildingUtility.GetQualityGraphicAndCache(thingWithComps, __result, true,false,true,false);
            


        }
    }



    /*
[HarmonyPatch(typeof(Thing))]
[HarmonyPatch("get_DefaultGraphic")]
public class QualityOfBuildingThingPatch
{

    public static void Postfix(ref Graphic __result, Thing __instance)
    {
        ThingWithComps thingWithComp = __instance as ThingWithComps;
        if (thingWithComp != null)
        {
            //Log.Message(thing.ToString() + "is Graphic Random");
            //Log.Message(__result.ToString());

            //Log.Message(__result.data.ToString()); // null
            //Log.Message(__result.data.graphicClass.ToString()); //null
            QualityOfBuildingUtility.DoPrefix(thingWithComp, ref __result, true, true);
        }

        else
        {
            //QualityOfBuildingUtility.DoPrefix(__instance, ref __result, true);
        }


    }

}*/




    [HarmonyPatch(typeof(ThingWithComps))]
    [HarmonyPatch("Print")]
    public class QualityOfBuildingPrintPatch
    {

        //1.1 and 1.2
        public static void OldPrefix(ThingWithComps __instance, ref Graphic ___graphicInt)
        {
            if (___graphicInt == null)
            {
                if (__instance.def.graphicData != null)
                {
                   
                    Graphic_Random graphicRandom = ___graphicInt as Graphic_Random;
                    if (graphicRandom == null)
                    {
                        ___graphicInt = __instance.def.graphicData.GraphicColoredFor(__instance);
                    }
                    else
                    {
                        ___graphicInt = graphicRandom.SubGraphicFor(__instance);
                        return;
                    }
                        
                }
                else
                {
                    if (__instance.def.graphicData != null)
                    {

                        Graphic_Random graphicRandom = ___graphicInt as Graphic_Random;
                        if (graphicRandom == null)
                        {
                            ___graphicInt = __instance.def.graphicData.GraphicColoredFor(__instance);
                        }
                        else
                        {
                            ___graphicInt = graphicRandom.SubGraphicFor(__instance);
                            return;
                        }

                    }
                }
            }
            //Log.Message("Print");
            QualityOfBuildingUtility.DoPrefix(__instance, ref ___graphicInt,false, false, true);
        }


    }

    /*
    [HarmonyPatch(typeof(MinifiedThing))]
    [HarmonyPatch("DrawAt")]
    public class QualityOfBuildingMinifiedThingPatch
    {
        
        public static void Prefix(ThingWithComps __instance, ref Graphic ___cachedGraphic)
        {
            QualityOfBuildingUtility.DoPrefix(__instance, ref ___cachedGraphic, false);
        }

    }
    */

    
    [HarmonyPatch(typeof(GraphicUtility))]
    [HarmonyPatch("ExtractInnerGraphicFor")]
    public class QualityOfBuildingGraphicUtilityPatch
    {
        //1.1 and 1.2
        public static void OldPostfix(ref Graphic __result, Thing thing)
        {
            ThingWithComps thingWithComp = thing as ThingWithComps;
            if (thingWithComp != null)
            {
                //Log.Message(thing.ToString() + "is Graphic Random");
                //Log.Message(__result.ToString());

                //Log.Message(__result.data.ToString()); // null
                //Log.Message(__result.data.graphicClass.ToString()); //null
                //Log.Message("ExtractInnerGraphicFor");

                QualityOfBuildingUtility.DoPrefix(thingWithComp, ref __result, false, true, false);
                //QualityOfBuildingUtility.DoPrefix(thingWithComp, ref __result, false, false, false);
            }

            else
            {
                //QualityOfBuildingUtility.DoPrefix(__instance, ref __result, true);
            }

            
        }
        public static void testPostfix(ref Graphic __result, Thing thing)
        {
            ThingWithComps thingWithComps = thing as ThingWithComps;

            if (thingWithComps == null)
                return;
            var qualityCheckComp = thing.TryGetComp<QualityCheckComp>();
            if (qualityCheckComp == null)
                return;

            __result = QualityOfBuildingUtility.GetQualityGraphicAndCache(thingWithComps, __result, true);
        }


    }

    [HarmonyPatch(typeof(Thing))]
    [HarmonyPatch("Notify_ColorChanged")]
    public class QualityOfBuildingColorChangePatch
    {
        //1.2
        public static void oldPrefix(Thing __instance)
        {
            ThingWithComps thingWithComp = __instance as ThingWithComps;
            if (thingWithComp != null)
            {
                var qualityCheckComp = __instance.TryGetComp<QualityCheckComp>();
                if (qualityCheckComp != null)
                {
                    qualityCheckComp.QualityChecked = false;
                    qualityCheckComp.needToUpdate = true;
                }

            }


        }

    }
    //this may have heavy performance
    /*
    [HarmonyPatch(typeof(Thing))]
    [HarmonyPatch("get_DefaultGraphic")]
    public class QualityOfBuildingThingGraphicPatch
    {

        public static void Postfix(ref Graphic __result, Thing __instance)
        {

            ThingWithComps thingWithComp = __instance as ThingWithComps;
            if (thingWithComp != null)
            {
                //Log.Message(thing.ToString() + "is Graphic Random");
                //Log.Message(__result.ToString());

                //Log.Message(__result.data.ToString()); // null
                //Log.Message(__result.data.graphicClass.ToString()); //null
                //Log.Message("get_Graphic");
                if (thingWithComp.def.graphicData.graphicClass != typeof(Graphic_Random))
                    QualityOfBuildingUtility.DoPrefix(thingWithComp, ref __result, true, true, false);
                else
                    QualityOfBuildingUtility.DoPrefix(thingWithComp, ref __result, false, true, false);
            }

            else
            {
                //QualityOfBuildingUtility.DoPrefix(__instance, ref __result, true);
            }


        }

    }*/





    [HarmonyPatch(typeof(Thing))]
    [HarmonyPatch("get_Graphic")]
    public class ThingGetGraphicPatch
    {

        //1.3Test
        public static void Prefix(Thing __instance, Graphic ___graphicInt, Graphic ___styleGraphicInt)
        {

            ThingWithComps thingWithComps = __instance as ThingWithComps;

            if (thingWithComps == null)
                return;

            var qualityCheckComp = __instance.TryGetComp<QualityCheckComp>();
            if (qualityCheckComp == null)
                return;

            ThingStyleDef styleDef = __instance.StyleDef;
            if (styleDef != null && styleDef.Graphic != null) // StyleGraphic
            {
                if (___styleGraphicInt == null)
                {
                    qualityCheckComp.needToUpdate = true;
                    return;
                }

            }
            else //DefaultGraphic
            {
                if (___graphicInt == null)
                {
                    qualityCheckComp.needToUpdate = true;
                    return;
                }
            }
            qualityCheckComp.needToUpdate = false; ;
        }
        public static void Postfix(ref Graphic __result, Thing __instance, ref Graphic ___graphicInt, ref Graphic ___styleGraphicInt)
        {
            ThingWithComps thingWithComps = __instance as ThingWithComps;

            if (thingWithComps == null)
                return;
            var qualityCheckComp = __instance.TryGetComp<QualityCheckComp>();
            if (qualityCheckComp == null)
                return;

            if (__instance as Building == null)
                if (!QualityOfBuildingSetting.ApplyToNonBuildingThings)
                    return;

            if (qualityCheckComp.needToUpdate != true)
                return;
            ThingStyleDef styleDef = __instance.StyleDef;
            if (styleDef != null && styleDef.Graphic != null)
            {
                if (___styleGraphicInt == null)
                    return;
                ___styleGraphicInt = QualityOfBuildingUtility.GetQualityGraphicAndCache(thingWithComps, ___styleGraphicInt, false,true,true,true);
                __result = ___styleGraphicInt;
            }
            else
            {
                if (___graphicInt == null)
                    return;
                ___graphicInt = QualityOfBuildingUtility.GetQualityGraphicAndCache(thingWithComps, ___graphicInt, false, false, true, true);
                __result = ___graphicInt;
            }


        }

    }
    [HarmonyPatch(typeof(Thing))]
    [HarmonyPatch("get_DefaultGraphic")]
    public class ThingGetDefaultGraphicPatch
    {

        //1.3Test
        public static void testPrefix(Thing __instance, Graphic ___graphicInt)
        {
            if (___graphicInt != null)
                return;

            ThingWithComps thingWithComps = __instance as ThingWithComps;

            if (thingWithComps == null)
                return;

            var qualityCheckComp = __instance.TryGetComp<QualityCheckComp>();
            if (qualityCheckComp == null)
                return;

            qualityCheckComp.needToUpdate = true;
        }

        //TODO:: check in prefix and do patch in postfix
        //does need change praphicInt for optimization?



        public static void testPostfix(ref Graphic __result, Thing __instance, Graphic ___graphicInt)
        {
            ThingWithComps thingWithComps = __instance as ThingWithComps;

            if (thingWithComps == null)
                return;
            var qualityCheckComp = __instance.TryGetComp<QualityCheckComp>();
            if (qualityCheckComp == null)
                return;

            __result = QualityOfBuildingUtility.GetQualityGraphicAndCache(thingWithComps, __result);
        }

    }
    [HarmonyPatch(typeof(ThingStyleDef))]
    [HarmonyPatch("get_Graphic")]
    public class ThingStyleDefGetGraphicPatch
    {

        //1.3Test for DLC... TODO
        public static void testPostfix(ref Graphic __result, Thing __instance)
        {
            ThingWithComps thingWithComps = __instance as ThingWithComps;

            if (thingWithComps == null)
                return;

            __result = QualityOfBuildingUtility.GetQualityGraphicAndCache(thingWithComps, __result, false);
        }

    }

    //1.3
    [HarmonyPatch(typeof(ApparelGraphicRecordGetter), "TryGetGraphicApparel")]
    public class WornApparelGraphicPatch
    {
        public static void Postfix(Apparel apparel, BodyTypeDef bodyType, ref ApparelGraphicRecord rec, ref bool __result)
        {
            if (!__result)
                return;

            if (apparel.Wearer == null)
                return;

            if (!QualityOfBuildingSetting.ApplyToWornApparels)
                return;

            rec.graphic = QualityOfBuildingUtility.GetQualityGraphicAndCache(apparel, rec.graphic,false,false,true,false);

        }
    }



    //
    [HarmonyPatch(typeof(Blueprint_Install))]
    [HarmonyPatch("get_Graphic")]
    public class QualityOfBuildingBlueprint_InstallPatch
    {
        //1.1 and 1.2
        [Obsolete]
        public static void  OldPostfix(ref Graphic __result, Blueprint_Install __instance)
        {
            
            ThingWithComps thingWithComp = __instance.ThingToInstall as ThingWithComps;
            if (thingWithComp != null)
            {
                //Log.Message(thing.ToString() + "is Graphic Random");
                //Log.Message(__result.ToString());

                //Log.Message(__result.data.ToString()); // null
                //Log.Message(__result.data.graphicClass.ToString()); //null
                //Log.Message("get_Graphic");
                if(thingWithComp.def.graphicData.graphicClass != typeof(Graphic_Random))
                    QualityOfBuildingUtility.DoPrefix(thingWithComp, ref __result, true, true, false);
                else
                    QualityOfBuildingUtility.DoPrefix(thingWithComp, ref __result, false, true, false);
            }

            else
            {
                //QualityOfBuildingUtility.DoPrefix(__instance, ref __result, true);
            }


        }



    }


    /*
    [HarmonyPatch(typeof(GhostUtility))]
    [HarmonyPatch("GhostGraphicFor")]
    public class QualityOfBuildingGhostUtilityPatch
    {

        public static void Postfix(ref Graphic __result, Thing thing)
        {
            ThingWithComps thingWithComp = thing as ThingWithComps;
            if (thingWithComp != null)
            {
                //Log.Message(thing.ToString() + "is Graphic Random");
                //Log.Message(__result.ToString());

                //Log.Message(__result.data.ToString()); // null
                //Log.Message(__result.data.graphicClass.ToString()); //null
                QualityOfBuildingUtility.DoPrefix(thingWithComp, ref __result, false, false);
            }

            else
            {
                //QualityOfBuildingUtility.DoPrefix(__instance, ref __result, true);
            }


        }

    }*/

    /*
    [HarmonyPatch(typeof(Blueprint))]
    [HarmonyPatch("Draw")]
    public class QualityOfBuildingBlueprintInstallPatch
    {

        public static void Prefix(ThingWithComps __instance, ref Graphic ___cachedGraphic)
        {
            QualityOfBuildingUtility.DoPrefix(__instance, ref ___cachedGraphic);
        }

    }
    */

    /*

    [HarmonyPatch(typeof(Thing))]
    [HarmonyPatch("Notify_ColorChanged")]
    public class ThingPatch
    {
        public static void Postfix(Thing __instance)
        {
            var qualityCheckComp = __instance.TryGetComp<QualityCheckComp>();
            if (qualityCheckComp != null)
            {
                qualityCheckComp.QualityChecked = false;
            }
        }
    }
    */
}
