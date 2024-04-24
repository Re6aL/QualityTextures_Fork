using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;
//using SettingsHelper;





namespace QualityOfBuilding
{
    public enum QualityCategoryWithDefault
    {
        Awful = 0,
        Poor = 1,
        Normal = 2,
        Good = 3,
        Excellent = 4,
        Masterwork = 5,
        Legendary = 6,
        Default = 7
    }

    public class QualityOfBuildingSetting : ModSettings
    {
        public static bool DebugLog = false;

        public static bool ApplyToNonBuildingThings = true;
        public static bool ApplyToWornApparels = true;

        public static bool UseAwfulTexture = true;
        public static bool UsePoorTexture = true;
        public static bool UseNormalTexture = true;
        public static bool UseGoodTexture = true;
        public static bool UseExcellentTexture = true;
        public static bool UseMasterworkTexture = true;
        public static bool UseLegendaryTexture = true;



        //public bool exampleBool;
        //public float exampleFloat = 200f;
        //public List<Pawn> exampleListOfPawns = new List<Pawn>();
        /*

        

        public static QualityCategoryWithDefault targetForAwful = QualityCategoryWithDefault.Awful;
        public static QualityCategoryWithDefault targetForAwfulSecond = QualityCategoryWithDefault.Poor;
        public static QualityCategoryWithDefault targetForAwfulThird = QualityCategoryWithDefault.Normal;

        public static QualityCategoryWithDefault targetForPoor = QualityCategoryWithDefault.Poor;
        public static QualityCategoryWithDefault targetForPoorSecond = QualityCategoryWithDefault.Awful;
        public static QualityCategoryWithDefault targetForPoorThird = QualityCategoryWithDefault.Awful;

        public static QualityCategoryWithDefault targetForNormal = QualityCategoryWithDefault.Normal;
        public static QualityCategoryWithDefault targetForNormalSecond = QualityCategoryWithDefault.Default;
        public static QualityCategoryWithDefault targetForNormalThird = QualityCategoryWithDefault.Default;


        public static QualityCategoryWithDefault targetForGood = QualityCategoryWithDefault.Good;
        public static QualityCategoryWithDefault targetForGoodSecond = QualityCategoryWithDefault.Normal;
        public static QualityCategoryWithDefault targetForGoodThird = QualityCategoryWithDefault.Default;

        public static QualityCategoryWithDefault targetForExcellent = QualityCategoryWithDefault.Excellent;
        public static QualityCategoryWithDefault targetForExcellentSecond = QualityCategoryWithDefault.Masterwork;
        public static QualityCategoryWithDefault targetForExcellentThird = QualityCategoryWithDefault.Normal;

        public static QualityCategoryWithDefault targetForMasterwork = QualityCategoryWithDefault.Masterwork;
        public static QualityCategoryWithDefault targetForMasterworkSecond = QualityCategoryWithDefault.Excellent;
        public static QualityCategoryWithDefault targetForMasterworkThird = QualityCategoryWithDefault.Normal;


        public static QualityCategoryWithDefault targetForLegendary = QualityCategoryWithDefault.Legendary;
        public static QualityCategoryWithDefault targetForLegendarySecond = QualityCategoryWithDefault.Masterwork;
        public static QualityCategoryWithDefault targetForLegendaryThird = QualityCategoryWithDefault.Excellent;
        public static QualityCategoryWithDefault targetForLegendaryFourth = QualityCategoryWithDefault.Normal;

        */
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref DebugLog, "DebugLog", false);

            Scribe_Values.Look(ref ApplyToNonBuildingThings, "ApplyToNonBuildingThings", true);
            Scribe_Values.Look(ref ApplyToWornApparels, "ApplyToWornApparels", true);

            Scribe_Values.Look(ref UseAwfulTexture, "UseAwfulTexture", true);
            Scribe_Values.Look(ref UsePoorTexture, "UsePoorTexture", true);
            Scribe_Values.Look(ref UseNormalTexture, "UseNormalTexture", true);
            Scribe_Values.Look(ref UseGoodTexture, "UseGoodTexture", true);
            Scribe_Values.Look(ref UseExcellentTexture, "UseExcellentTexture", true);
            Scribe_Values.Look(ref UseMasterworkTexture, "UseMasterworkTexture", true);
            Scribe_Values.Look(ref UseLegendaryTexture, "UseLegendaryTexture", true);
            /*
            Scribe_Values.Look(ref targetForAwful, "targetForAwful", QualityCategoryWithDefault.Awful);
            Scribe_Values.Look(ref targetForAwfulSecond, "targetForAwfulSecond", QualityCategoryWithDefault.Poor);
            Scribe_Values.Look(ref targetForAwfulThird, "targetForAwfulThird", QualityCategoryWithDefault.Normal);
            */
            //Scribe_Collections.Look(ref exampleListOfPawns, "exampleListOfPawns", LookMode.Reference);

        }

    }
    public class QualityOFBuildingMod : Mod
    {
        public static QualityOfBuildingSetting settings;

        public QualityOFBuildingMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<QualityOfBuildingSetting>();
        }


        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            Rect newRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            listingStandard.Begin(newRect);
            //string test = "hello";
            //listingStandard.TextFieldNumericLabeled<QualityCategory>("Quality", ref QualityOfBuildingSetting.targetQualityForOriginal, ref test);
            //listingStandard.
            listingStandard.CheckboxLabeled("Debug Log", ref QualityOfBuildingSetting.DebugLog, "Log Warnings");
            listingStandard.Gap();
            listingStandard.Gap();
            listingStandard.Gap();


            listingStandard.Label("If you change option, you need to clear cache and then Reload Savefile", -1);
            if (listingStandard.ButtonText("Clear Quality Graphic Cache"))
            {
                QualityOfBuildingDatabase.ClearAll();
            }
            listingStandard.GapLine();
            listingStandard.Label("Non Building Things Support",-1);
            listingStandard.CheckboxLabeled("Apply Non Building Things (such as weapons, droped apparels)", ref QualityOfBuildingSetting.ApplyToNonBuildingThings, "Apply Quality Graphic System to more things!\nif it doesn't have quality texture, it will work as original");
            listingStandard.CheckboxLabeled("Apply Worn Apparel Graphics", ref QualityOfBuildingSetting.ApplyToWornApparels, "Apply Quality Graphic System worn Apparel Graphic!\nif it doesn't have quality texture, it will work as original");
            listingStandard.GapLine();
            listingStandard.Gap();

            //listingStandard.Label("Use Settings");
            listingStandard.Label("Using Quality Of Building Texture Setting", -1, "If you want to use Original Texture(from vanilla or other texture replacer), check these settings.\nNeed to reload save for Update Option");
            listingStandard.Gap();
            listingStandard.CheckboxLabeled("Apply Awful Quality Texture", ref QualityOfBuildingSetting.UseAwfulTexture, "Awful buildings will use awful quality texture for it.\nDisable it if you want to use original texture for this quality.");
            if (QualityOfBuildingSetting.UseAwfulTexture)
            {
                
            }
            listingStandard.Gap(6);
            listingStandard.CheckboxLabeled("Apply Poor Quality Texture", ref QualityOfBuildingSetting.UsePoorTexture, "Poor buildings will use awful quality texture for it.\nDisable it if you want to use original texture for this quality.");
            if (QualityOfBuildingSetting.UsePoorTexture)
            {

            }
            listingStandard.Gap(6);
            listingStandard.CheckboxLabeled("Apply Normal Quality Texture", ref QualityOfBuildingSetting.UseNormalTexture, "Normal buildings will use awful quality texture for it.\nDisable it if you want to use original texture for this quality.");
            if (QualityOfBuildingSetting.UseNormalTexture)
            {

            }
            listingStandard.Gap(6);
            listingStandard.CheckboxLabeled("Apply Good Quality Texture", ref QualityOfBuildingSetting.UseGoodTexture, "Good buildings will use awful quality texture for it.\nDisable it if you want to use original texture for this quality.");
            if (QualityOfBuildingSetting.UseGoodTexture)
            {

            }
            listingStandard.Gap(6);
            listingStandard.CheckboxLabeled("Apply Excellent Quality Texture", ref QualityOfBuildingSetting.UseExcellentTexture, "Excellent buildings will use awful quality texture for it.\nDisable it if you want to use original texture for this quality.");
            if (QualityOfBuildingSetting.UseExcellentTexture)
            {

            }
            listingStandard.Gap(6);
            listingStandard.CheckboxLabeled("Masterwork Quality Texture", ref QualityOfBuildingSetting.UseMasterworkTexture, "Masterwork buildings will use awful quality texture for it.\nDisable it if you want to use original texture for this quality.");
            if (QualityOfBuildingSetting.UseMasterworkTexture)
            {

            }
            listingStandard.Gap(6);
            listingStandard.CheckboxLabeled("Apply Legendary Quality Texture", ref QualityOfBuildingSetting.UseLegendaryTexture, "Legendary buildings will use awful quality texture for it.\nDisable it if you want to use original texture for this quality.");
            if (QualityOfBuildingSetting.UseLegendaryTexture)
            {

            }
            listingStandard.Gap(6);
            //listingStandard.CheckboxLabeled("  Poor Texture for a temporary expedient", ref QualityOfBuildingSetting.useAwfulFromPoor, "Use Poor Texture for Awful when Awful Texture don't exist");
            //listingStandard.CheckboxLabeled("  Force to use a temporary expedient", ref QualityOfBuildingSetting.useAwfulFromPoor, "Force to use Poor Texture for Awful");
            /*
            listingStandard.Label("Awful Building");
            if (listingStandard.ButtonText("Awful Building use "+ QualityOfBuildingSetting.targetForAwful.ToString()+" texture"))
            {
                Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()
                {
                    new FloatMenuOption("Set Awful",(()=>QualityOfBuildingSetting.targetForAwful = QualityCategoryWithDefault.Awful)),
                    new FloatMenuOption("Set Poor",(()=>QualityOfBuildingSetting.targetForAwful = QualityCategoryWithDefault.Poor)),
                    new FloatMenuOption("Set Normal",(()=>QualityOfBuildingSetting.targetForAwful = QualityCategoryWithDefault.Normal)),
                    new FloatMenuOption("Set Good",(()=>QualityOfBuildingSetting.targetForAwful = QualityCategoryWithDefault.Good)),
                    new FloatMenuOption("Set Excellent",(()=>QualityOfBuildingSetting.targetForAwful = QualityCategoryWithDefault.Excellent)),
                    new FloatMenuOption("Set MasterWork",(()=>QualityOfBuildingSetting.targetForAwful = QualityCategoryWithDefault.Masterwork)),
                    new FloatMenuOption("Set Legendary",(()=>QualityOfBuildingSetting.targetForAwful = QualityCategoryWithDefault.Legendary)),
                    new FloatMenuOption("Use Default Texture",(()=>QualityOfBuildingSetting.targetForAwful = QualityCategoryWithDefault.Default))
                }
                ));
            }
            if(QualityOfBuildingSetting.targetForAwful != QualityCategoryWithDefault.Default)
            {
                listingStandard.Gap(6);
                listingStandard.Label("Secondary Texture for Awful Building");
                if (listingStandard.ButtonText("Awful Building use " + QualityOfBuildingSetting.targetForAwfulSecond.ToString() + " texture for Secondary"))
                {
                    Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()
                {
                    new FloatMenuOption("Set Awful",(()=>QualityOfBuildingSetting.targetForAwfulSecond = QualityCategoryWithDefault.Awful)),
                    new FloatMenuOption("Set Poor",(()=>QualityOfBuildingSetting.targetForAwfulSecond = QualityCategoryWithDefault.Poor)),
                    new FloatMenuOption("Set Normal",(()=>QualityOfBuildingSetting.targetForAwfulSecond = QualityCategoryWithDefault.Normal)),
                    new FloatMenuOption("Set Good",(()=>QualityOfBuildingSetting.targetForAwfulSecond = QualityCategoryWithDefault.Good)),
                    new FloatMenuOption("Set Excellent",(()=>QualityOfBuildingSetting.targetForAwfulSecond = QualityCategoryWithDefault.Excellent)),
                    new FloatMenuOption("Set MasterWork",(()=>QualityOfBuildingSetting.targetForAwfulSecond = QualityCategoryWithDefault.Masterwork)),
                    new FloatMenuOption("Set Legendary",(()=>QualityOfBuildingSetting.targetForAwfulSecond = QualityCategoryWithDefault.Legendary)),
                    new FloatMenuOption("Use Default Texture",(()=>QualityOfBuildingSetting.targetForAwfulSecond = QualityCategoryWithDefault.Default))                }
                    ));
                }
                if (QualityOfBuildingSetting.targetForAwfulSecond != QualityCategoryWithDefault.Default)
                {
                    listingStandard.Gap(6);
                    listingStandard.Label("Third Texture for Awful Building");
                    if (listingStandard.ButtonText("Awful Building use " + QualityOfBuildingSetting.targetForAwfulThird.ToString() + " texture for Third"))
                    {
                        Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()
                    {
                        new FloatMenuOption("Set Awful",(()=>QualityOfBuildingSetting.targetForAwfulThird = QualityCategoryWithDefault.Awful)),
                        new FloatMenuOption("Set Poor",(()=>QualityOfBuildingSetting.targetForAwfulThird = QualityCategoryWithDefault.Poor)),
                        new FloatMenuOption("Set Normal",(()=>QualityOfBuildingSetting.targetForAwfulThird = QualityCategoryWithDefault.Normal)),
                        new FloatMenuOption("Set Good",(()=>QualityOfBuildingSetting.targetForAwfulThird = QualityCategoryWithDefault.Good)),
                        new FloatMenuOption("Set Excellent",(()=>QualityOfBuildingSetting.targetForAwfulThird = QualityCategoryWithDefault.Excellent)),
                        new FloatMenuOption("Set MasterWork",(()=>QualityOfBuildingSetting.targetForAwfulThird = QualityCategoryWithDefault.Masterwork)),
                        new FloatMenuOption("Set Legendary",(()=>QualityOfBuildingSetting.targetForAwfulThird = QualityCategoryWithDefault.Legendary)),
                        new FloatMenuOption("Use Default Texture",(()=>QualityOfBuildingSetting.targetForAwfulThird = QualityCategoryWithDefault.Default))                }
                        ));
                    }
                }
                
            }

            listingStandard.Gap(12);


            listingStandard.Label("Poor Building");
            if (listingStandard.ButtonText("Poor Building use " + QualityOfBuildingSetting.targetForPoor.ToString() + " texture"))
            {
                Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()
                {
                    new FloatMenuOption("Set Awful",(()=>QualityOfBuildingSetting.targetForPoor = QualityCategoryWithDefault.Awful)),
                    new FloatMenuOption("Set Poor",(()=>QualityOfBuildingSetting.targetForPoor = QualityCategoryWithDefault.Poor)),
                    new FloatMenuOption("Set Normal",(()=>QualityOfBuildingSetting.targetForPoor = QualityCategoryWithDefault.Normal)),
                    new FloatMenuOption("Set Good",(()=>QualityOfBuildingSetting.targetForPoor = QualityCategoryWithDefault.Good)),
                    new FloatMenuOption("Set Excellent",(()=>QualityOfBuildingSetting.targetForPoor = QualityCategoryWithDefault.Excellent)),
                    new FloatMenuOption("Set MasterWork",(()=>QualityOfBuildingSetting.targetForPoor = QualityCategoryWithDefault.Masterwork)),
                    new FloatMenuOption("Set Legendary",(()=>QualityOfBuildingSetting.targetForPoor = QualityCategoryWithDefault.Legendary)),
                    new FloatMenuOption("Use Default Texture",(()=>QualityOfBuildingSetting.targetForPoor = QualityCategoryWithDefault.Default))
                }
                ));
            }
            if (QualityOfBuildingSetting.targetForPoor != QualityCategoryWithDefault.Default)
            {
                listingStandard.Gap(6);
                listingStandard.Label("Secondary Texture for Awful Building");
                if (listingStandard.ButtonText("Poor Building use " + QualityOfBuildingSetting.targetForPoorSecond.ToString() + " texture for Secondary"))
                {
                    Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()
                {
                    new FloatMenuOption("Set Awful",(()=>QualityOfBuildingSetting.targetForPoorSecond = QualityCategoryWithDefault.Awful)),
                    new FloatMenuOption("Set Poor",(()=>QualityOfBuildingSetting.targetForPoorSecond = QualityCategoryWithDefault.Poor)),
                    new FloatMenuOption("Set Normal",(()=>QualityOfBuildingSetting.targetForPoorSecond = QualityCategoryWithDefault.Normal)),
                    new FloatMenuOption("Set Good",(()=>QualityOfBuildingSetting.targetForPoorSecond = QualityCategoryWithDefault.Good)),
                    new FloatMenuOption("Set Excellent",(()=>QualityOfBuildingSetting.targetForPoorSecond = QualityCategoryWithDefault.Excellent)),
                    new FloatMenuOption("Set MasterWork",(()=>QualityOfBuildingSetting.targetForPoorSecond = QualityCategoryWithDefault.Masterwork)),
                    new FloatMenuOption("Set Legendary",(()=>QualityOfBuildingSetting.targetForPoorSecond = QualityCategoryWithDefault.Legendary)),
                    new FloatMenuOption("Use Default Texture",(()=>QualityOfBuildingSetting.targetForPoorSecond = QualityCategoryWithDefault.Default))                }
                    ));
                }
                if (QualityOfBuildingSetting.targetForPoorSecond != QualityCategoryWithDefault.Default)
                {
                    listingStandard.Gap(6);
                    listingStandard.Label("Third Texture for Awful Building");
                    if (listingStandard.ButtonText("Poor Building use " + QualityOfBuildingSetting.targetForPoorThird.ToString() + " texture for Third"))
                    {
                        Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()
                    {
                        new FloatMenuOption("Set Awful",(()=>QualityOfBuildingSetting.targetForPoorThird = QualityCategoryWithDefault.Awful)),
                        new FloatMenuOption("Set Poor",(()=>QualityOfBuildingSetting.targetForPoorThird = QualityCategoryWithDefault.Poor)),
                        new FloatMenuOption("Set Normal",(()=>QualityOfBuildingSetting.targetForPoorThird = QualityCategoryWithDefault.Normal)),
                        new FloatMenuOption("Set Good",(()=>QualityOfBuildingSetting.targetForPoorThird = QualityCategoryWithDefault.Good)),
                        new FloatMenuOption("Set Excellent",(()=>QualityOfBuildingSetting.targetForPoorThird = QualityCategoryWithDefault.Excellent)),
                        new FloatMenuOption("Set MasterWork",(()=>QualityOfBuildingSetting.targetForPoorThird = QualityCategoryWithDefault.Masterwork)),
                        new FloatMenuOption("Set Legendary",(()=>QualityOfBuildingSetting.targetForPoorThird = QualityCategoryWithDefault.Legendary)),
                        new FloatMenuOption("Use Default Texture",(()=>QualityOfBuildingSetting.targetForPoorThird = QualityCategoryWithDefault.Default))                }
                        ));
                    }
                }
            }
            listingStandard.Gap(12);


            listingStandard.Label("Normal Building");
            if (listingStandard.ButtonText("Normal Building use " + QualityOfBuildingSetting.targetForNormal.ToString() + " texture"))
            {
                Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()
                {
                    new FloatMenuOption("Set Awful",(()=>QualityOfBuildingSetting.targetForNormal = QualityCategoryWithDefault.Awful)),
                    new FloatMenuOption("Set Poor",(()=>QualityOfBuildingSetting.targetForNormal = QualityCategoryWithDefault.Poor)),
                    new FloatMenuOption("Set Normal",(()=>QualityOfBuildingSetting.targetForNormal = QualityCategoryWithDefault.Normal)),
                    new FloatMenuOption("Set Good",(()=>QualityOfBuildingSetting.targetForNormal = QualityCategoryWithDefault.Good)),
                    new FloatMenuOption("Set Excellent",(()=>QualityOfBuildingSetting.targetForNormal = QualityCategoryWithDefault.Excellent)),
                    new FloatMenuOption("Set MasterWork",(()=>QualityOfBuildingSetting.targetForNormal = QualityCategoryWithDefault.Masterwork)),
                    new FloatMenuOption("Set Legendary",(()=>QualityOfBuildingSetting.targetForNormal = QualityCategoryWithDefault.Legendary)),
                    new FloatMenuOption("Use Default Texture",(()=>QualityOfBuildingSetting.targetForNormal = QualityCategoryWithDefault.Default))
                }
                ));
            }
            if (QualityOfBuildingSetting.targetForNormal != QualityCategoryWithDefault.Default)
            {
                listingStandard.Gap(6);
                listingStandard.Label("Secondary Texture for Normal Building");
                if (listingStandard.ButtonText("Normal Building use " + QualityOfBuildingSetting.targetForNormalSecond.ToString() + " texture for Secondary"))
                {
                    Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()
                {
                    new FloatMenuOption("Set Awful",(()=>QualityOfBuildingSetting.targetForNormalSecond = QualityCategoryWithDefault.Awful)),
                    new FloatMenuOption("Set Poor",(()=>QualityOfBuildingSetting.targetForNormalSecond = QualityCategoryWithDefault.Poor)),
                    new FloatMenuOption("Set Normal",(()=>QualityOfBuildingSetting.targetForNormalSecond = QualityCategoryWithDefault.Normal)),
                    new FloatMenuOption("Set Good",(()=>QualityOfBuildingSetting.targetForNormalSecond = QualityCategoryWithDefault.Good)),
                    new FloatMenuOption("Set Excellent",(()=>QualityOfBuildingSetting.targetForNormalSecond = QualityCategoryWithDefault.Excellent)),
                    new FloatMenuOption("Set MasterWork",(()=>QualityOfBuildingSetting.targetForNormalSecond = QualityCategoryWithDefault.Masterwork)),
                    new FloatMenuOption("Set Legendary",(()=>QualityOfBuildingSetting.targetForNormalSecond = QualityCategoryWithDefault.Legendary)),
                    new FloatMenuOption("Use Default Texture",(()=>QualityOfBuildingSetting.targetForNormalSecond = QualityCategoryWithDefault.Default))                }
                    ));
                }
                if (QualityOfBuildingSetting.targetForPoorSecond != QualityCategoryWithDefault.Default)
                {
                    listingStandard.Gap(6);
                    listingStandard.Label("Third Texture for Normal Building");
                    if (listingStandard.ButtonText("Normal Building use " + QualityOfBuildingSetting.targetForNormalThird.ToString() + " texture for Third"))
                    {
                        Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()
                    {
                        new FloatMenuOption("Set Awful",(()=>QualityOfBuildingSetting.targetForNormalThird = QualityCategoryWithDefault.Awful)),
                        new FloatMenuOption("Set Poor",(()=>QualityOfBuildingSetting.targetForNormalThird = QualityCategoryWithDefault.Poor)),
                        new FloatMenuOption("Set Normal",(()=>QualityOfBuildingSetting.targetForNormalThird = QualityCategoryWithDefault.Normal)),
                        new FloatMenuOption("Set Good",(()=>QualityOfBuildingSetting.targetForNormalThird = QualityCategoryWithDefault.Good)),
                        new FloatMenuOption("Set Excellent",(()=>QualityOfBuildingSetting.targetForNormalThird = QualityCategoryWithDefault.Excellent)),
                        new FloatMenuOption("Set MasterWork",(()=>QualityOfBuildingSetting.targetForNormalThird = QualityCategoryWithDefault.Masterwork)),
                        new FloatMenuOption("Set Legendary",(()=>QualityOfBuildingSetting.targetForNormalThird = QualityCategoryWithDefault.Legendary)),
                        new FloatMenuOption("Use Default Texture",(()=>QualityOfBuildingSetting.targetForNormalThird = QualityCategoryWithDefault.Default))                }
                        ));
                    }
                }
            }
            listingStandard.Gap(12);
            */
            //settings.exampleFloat = listingStandard.Slider(settings.exampleFloat, 100f, 300f);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            //return "Quality Of Building";
            return "Quality Of Building";
        }

       


    }
    //LoadedModManager.GetMod<QualityOFBuildingMod>().GetSettings<ExampleSettings>().exampleBool
}