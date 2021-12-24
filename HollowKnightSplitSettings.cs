﻿using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace LiveSplit.HollowKnight {
    public partial class HollowKnightSplitSettings : UserControl {
        public string Split { get; set; } = "";
        private int mX = 0;
        private int mY = 0;
        private bool isDragging = false;
        public HollowKnightSplitSettings() {
            InitializeComponent();
            cboName.MouseWheel += (o, e) => ((HandledMouseEventArgs)e).Handled = true;
        }

        private void cboName_SelectedIndexChanged(object sender, EventArgs e) {
            string splitDescription = cboName.SelectedValue.ToString();
            SplitName split = GetSplitName(splitDescription);
            Split = split.ToString();

            MemberInfo info = typeof(SplitName).GetMember(split.ToString())[0];
            DescriptionAttribute description = (DescriptionAttribute)info.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
            ToolTipAttribute tooltip = (ToolTipAttribute)info.GetCustomAttributes(typeof(ToolTipAttribute), false)[0];
            ToolTips.SetToolTip(cboName, tooltip.ToolTip);
        }
        public static SplitName GetSplitName(string text) {
            foreach (SplitName split in Enum.GetValues(typeof(SplitName))) {
                string name = split.ToString();
                MemberInfo info = typeof(SplitName).GetMember(name)[0];
                DescriptionAttribute description = (DescriptionAttribute)info.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];

                if (name.Equals(text, StringComparison.OrdinalIgnoreCase) || description.Description.Equals(text, StringComparison.OrdinalIgnoreCase)) {
                    return split;
                }
            }
            return SplitName.ForgottenCrossroads;
        }

        private void picHandle_MouseMove(object sender, MouseEventArgs e) {
            if (!isDragging) {
                if (e.Button == MouseButtons.Left) {
                    int num1 = mX - e.X;
                    int num2 = mY - e.Y;
                    if (((num1 * num1) + (num2 * num2)) > 20) {
                        DoDragDrop(this, DragDropEffects.All);
                        isDragging = true;
                        return;
                    }
                }
            }
        }
        private void picHandle_MouseDown(object sender, MouseEventArgs e) {
            mX = e.X;
            mY = e.Y;
            isDragging = false;
        }
    }
    public enum SplitName {
        [Description("Abyss Shriek (Skill)"), ToolTip("Splits when obtaining Abyss Shriek (Shadow Scream)")]
        AbyssShriek,
        [Description("Crystal Heart (Skill)"), ToolTip("Splits when obtaining Crystal Heart (Super Dash)")]
        CrystalHeart,
        [Description("Cyclone Slash (Skill)"), ToolTip("Splits when obtaining Cyclone Slash (Nail Art)")]
        CycloneSlash,
        [Description("Dash Slash (Skill)"), ToolTip("Splits when obtaining Dash Slash (Nail Art)")]
        DashSlash,
        [Description("Descending Dark (Skill)"), ToolTip("Splits when obtaining Descending Dark (Shadow Dive)")]
        DescendingDark,
        [Description("Desolate Dive (Skill)"), ToolTip("Splits when obtaining Desolate Dive")]
        DesolateDive,
        [Description("Dream Nail (Skill)"), ToolTip("Splits when obtaining Dream Nail")]
        DreamNail,
        [Description("Dream Nail - Awoken (Skill)"), ToolTip("Splits when Awkening the Dream Nail")]
        DreamNail2,
        [Description("Dream Gate (Skill)"), ToolTip("Splits when obtaining Dream Gate")]
        DreamGate,
        [Description("Great Slash (Skill)"), ToolTip("Splits when obtaining Great Slash (Nail Art)")]
        GreatSlash,
        [Description("Howling Wraiths (Skill)"), ToolTip("Splits when obtaining Howling Wraiths (Scream)")]
        HowlingWraiths,
        [Description("Isma's Tear (Skill)"), ToolTip("Splits when obtaining Isma's Tear (Acid Armour)")]
        IsmasTear,
        [Description("Mantis Claw (Skill)"), ToolTip("Splits when obtaining Mantis Claw (Wall Jump)")]
        MantisClaw,
        [Description("Monarch Wings (Skill)"), ToolTip("Splits when obtaining Monarch Wings (Double Jump)")]
        MonarchWings,
        [Description("Mothwing Cloak (Skill)"), ToolTip("Splits when obtaining Mothwing Cloak (Dash)")]
        MothwingCloak,
        [Description("Shade Cloak (Skill)"), ToolTip("Splits when obtaining Shade Cloak (Shadow Dash)")]
        ShadeCloak,
        [Description("Shade Soul (Skill)"), ToolTip("Splits when obtaining Shade Soul (Vengeful Spirit 2)")]
        ShadeSoul,
        [Description("Vengeful Spirit (Skill)"), ToolTip("Splits when obtaining Vengeful Spirit")]
        VengefulSpirit,

        [Description("City Crest (Item)"), ToolTip("Splits when obtaining the City Crest")]
        CityKey,
        [Description("Delicate Flower (Item)"), ToolTip("Splits when flower is in inventory")]
        HasDelicateFlower,
        [Description("Elegant Key (Item)"), ToolTip("Splits when obtaining the Elegant Key")]
        ElegantKey,
        [Description("Elegant Key Shoptimisation (Item)"), ToolTip("Splits when obtaining both mask shard 1 and Elegant Key")]
        ElegantKeyShoptimised,
        [Description("God Tuner (Item)"), ToolTip("Splits when obtaining the God Tuner")]
        GodTuner,
        [Description("Hunter's Mark (Item)"), ToolTip("Splits when obtaining the Hunter's Mark")]
        HuntersMark,
        [Description("Kings Brand (Item)"), ToolTip("Splits when obtaining the Kings Brand")]
        KingsBrand,
        [Description("Love Key (Item)"), ToolTip("Splits when obtaining the Love Key")]
        LoveKey,
        [Description("Lumafly Lantern (Item)"), ToolTip("Splits when obtaining the Lumafly Lantern")]
        LumaflyLantern,
        [Description("Pale Lurker Key (Item)"), ToolTip("Splits when obtaining the Simple Key from the Pale Lurker")]
        PaleLurkerKey,
        [Description("Pale Ore - Any (Item)"), ToolTip("Splits if you've obtained any Pale Ore")]
        PaleOre,
        [Description("Salubra's Blessing (Item)"), ToolTip("Splits when obtaining Salubra's Blessing")]
        SalubrasBlessing,
        [Description("Simple Key - First (Item)"), ToolTip("Splits when obtaining the first Simple Key")]
        SimpleKey,
        [Description("Shopkeeper's Key (Item)"), ToolTip("Splits when obtaining the Shopkeeper's Key from the Crystal Peak key room")]
        SlyKey,
        [Description("Tram Pass (Item)"), ToolTip("Splits when obtaining the Tram Pass")]
        TramPass,




        [Description("Mask Fragment 1 (Fragment)"), ToolTip("Splits when getting 1st Mask fragment")]
        MaskFragment1,
        [Description("Mask Fragment 2 (Fragment)"), ToolTip("Splits when getting 2nd Mask fragment")]
        MaskFragment2,
        [Description("Mask Fragment 3 (Fragment)"), ToolTip("Splits when getting 3rd Mask fragment")]
        MaskFragment3,
        [Description("Mask Fragment 4 (Upgrade)"), ToolTip("Splits when getting 4th Mask fragment (6 HP)")]
        Mask1,
        [Description("Mask Fragment 5 (Fragment)"), ToolTip("Splits when getting 5th Mask fragment")]
        MaskFragment5,
        [Description("Mask Fragment 6 (Fragment)"), ToolTip("Splits when getting 6th Mask fragment")]
        MaskFragment6,
        [Description("Mask Fragment 7 (Fragment)"), ToolTip("Splits when getting 7th Mask fragment")]
        MaskFragment7,
        [Description("Mask Fragment 8 (Upgrade)"), ToolTip("Splits when getting 8th Mask fragment (7 HP)")]
        Mask2,
        [Description("Mask Fragment 9 (Fragment)"), ToolTip("Splits when getting 9th Mask fragment")]
        MaskFragment9,
        [Description("Mask Fragment 10 (Fragment)"), ToolTip("Splits when getting 10th Mask fragment")]
        MaskFragment10,
        [Description("Mask Fragment 11 (Fragment)"), ToolTip("Splits when getting 11th Mask fragment")]
        MaskFragment11,
        [Description("Mask Fragment 12 (Upgrade)"), ToolTip("Splits when getting 12th Mask fragment (8 HP)")]
        Mask3,
        [Description("Mask Fragment 13 (Fragment)"), ToolTip("Splits when getting 13th Mask fragment")]
        MaskFragment13,
        [Description("Mask Fragment 14 (Fragment)"), ToolTip("Splits when getting 14th Mask fragment")]
        MaskFragment14,
        [Description("Mask Fragment 15 (Fragment)"), ToolTip("Splits when getting 15th Mask fragment")]
        MaskFragment15,
        [Description("Mask Fragment 16 (Upgrade)"), ToolTip("Splits when getting 16th Mask fragment (9 HP)")]
        Mask4,
        [Description("Nail 1 (Upgrade)"), ToolTip("Splits when getting Nail Upgrade 1")]
        NailUpgrade1,
        [Description("Nail 2 (Upgrade)"), ToolTip("Splits when getting Nail Upgrade 2")]
        NailUpgrade2,
        [Description("Nail 3 (Upgrade)"), ToolTip("Splits when getting Coil Nail (Upgrade 3)")]
        NailUpgrade3,
        [Description("Nail 4 (Upgrade)"), ToolTip("Splits when getting Pure Nail (Upgrade 4)")]
        NailUpgrade4,
        [Description("Vessel Fragment 1 (Fragment)"), ToolTip("Splits when getting 1st Vessel fragment")]
        VesselFragment1,
        [Description("Vessel Fragment 2 (Fragment)"), ToolTip("Splits when getting 2nd Vessel fragment")]
        VesselFragment2,
        [Description("Vessel Fragment 3 (Upgrade)"), ToolTip("Splits when getting 3rd Vessel fragment (1 extra MP)")]
        Vessel1,
        [Description("Vessel Fragment 4 (Fragment)"), ToolTip("Splits when getting 4th Vessel fragment")]
        VesselFragment4,
        [Description("Vessel Fragment 5 (Fragment)"), ToolTip("Splits when getting 5th Vessel fragment")]
        VesselFragment5,
        [Description("Vessel Fragment 6 (Upgrade)"), ToolTip("Splits when getting 6th Vessel fragment (2 extra MP)")]
        Vessel2,
        [Description("Vessel Fragment 7 (Fragment)"), ToolTip("Splits when getting 7th Vessel fragment")]
        VesselFragment7,
        [Description("Vessel Fragment 8 (Fragment)"), ToolTip("Splits when getting 8th Vessel fragment")]
        VesselFragment8,
        [Description("Vessel Fragment 9 (Upgrade)"), ToolTip("Splits when getting 9th Vessel fragment (3 extra MP)")]
        Vessel3,

        [Description("Broken Vessel (Boss)"), ToolTip("Splits when killing Broken Vessel")]
        BrokenVessel,
        [Description("Brooding Mawlek (Boss)"), ToolTip("Splits when killing Brooding Mawlek")]
        BroodingMawlek,
        [Description("Collector (Boss)"), ToolTip("Splits when killing Collector")]
        Collector,
        [Description("Crystal Guardian 1 (Boss)"), ToolTip("Splits when killing the Crystal Guardian for the first time")]
        CrystalGuardian1,
        [Description("Crystal Guardian 2 (Boss)"), ToolTip("Splits when killing the Crystal Guardian for the second time")]
        CrystalGuardian2,
        [Description("Dung Defender (Boss)"), ToolTip("Splits when killing Dung Defender")]
        DungDefender,
        [Description("Dung Defender Idol (Trinket)"), ToolTip("Splits when picking up Dung Defender idol as the first idol")]
        DungDefenderIdol,
        [Description("Elder Hu (Boss)"), ToolTip("Splits when killing Elder Hu")]
        ElderHu,
        [Description("Elder Hu (Essence)"), ToolTip("Splits when absorbing essence from Elder Hu")]
        ElderHuEssence,
        [Description("False Knight (Boss)"), ToolTip("Splits when killing False Knight")]
        FalseKnight,
        [Description("Failed Champion (Boss)"), ToolTip("Splits when killing Failed Champion (False Knight Dream)")]
        FailedKnight,
        [Description("Failed Champion (Essence)"), ToolTip("Splits when getting Failed Champion essence (False Knight Dream)")]
        FailedChampionEssence,
        [Description("Flukemarm (Boss)"), ToolTip("Splits when killing Flukemarm")]
        Flukemarm,
        [Description("Galien (Boss)"), ToolTip("Splits when killing Galien")]
        Galien,
        [Description("Galien (Essence)"), ToolTip("Splits when absorbing essence from Galien")]
        GalienEssence,
        [Description("God Tamer (Boss)"), ToolTip("Splits when killing the God Tamer")]
        GodTamer,
        [Description("Gorb (Boss)"), ToolTip("Splits when killing Gorb")]
        Gorb,
        [Description("Gorb (Essence)"), ToolTip("Splits when absorbing essence from Gorb")]
        GorbEssence,
        [Description("Grey Prince Zote (Boss)"), ToolTip("Splits when killing Grey Prince")]
        GreyPrince,
        [Description("Grey Prince Zote (Essence)"), ToolTip("Splits when getting Grey Prince Zote essence")]
        GreyPrinceEssence,
        [Description("Grey Prince Zote Level (Boss)"), ToolTip("Splits each time defeating Grey Prince Zote in Bretta's dream")]
        OnDefeatGPZ,
        [Description("Gruz Mother (Boss)"), ToolTip("Splits when killing Gruz Mother")]
        GruzMother,
        [Description("Hive Knight (Boss)"), ToolTip("Splits when killing Hive Knight")]
        HiveKnight,
        [Description("Hornet 1 (Boss)"), ToolTip("Splits when killing Hornet for the first time")]
        Hornet1,
        [Description("Hornet 2 (Boss)"), ToolTip("Splits when killing Hornet for the second time")]
        Hornet2,
        [Description("Lost Kin (Boss)"), ToolTip("Splits when killing Lost Kin (Broken Vessel Dream)")]
        LostKin,
        [Description("Lost Kin (Essence)"), ToolTip("Splits when getting Lost Kin essence (Broken Vessel Dream)")]
        LostKinEssence,
        [Description("Mantis Lords (Boss)"), ToolTip("Splits when killing Mantis Lords")]
        MantisLords,
        [Description("Markoth (Boss)"), ToolTip("Splits when killing Markoth")]
        Markoth,
        [Description("Markoth (Essence)"), ToolTip("Splits when absorbing essence from Markoth")]
        MarkothEssence,
        [Description("Marmu (Boss)"), ToolTip("Splits when killing Marmu")]
        Marmu,
        [Description("Marmu (Essence)"), ToolTip("Splits when absorbing essence from Marmu")]
        MarmuEssence,
        [Description("Massive Moss Charger (Boss)"), ToolTip("Splits when killing Massive Moss Charger")]
        MegaMossCharger,
        [Description("Nightmare King Grimm (Boss)"), ToolTip("Splits when killing Nightmare King Grimm")]
        NightmareKingGrimm,
        [Description("No Eyes (Boss)"), ToolTip("Splits when killing No Eyes")]
        NoEyes,
        [Description("No Eyes (Essence)"), ToolTip("Splits when absorbing essence from No Eyes")]
        NoEyesEssence,
        [Description("Nosk (Boss)"), ToolTip("Splits when killing Nosk")]
        Nosk,
        [Description("Oblobbles (Boss)"), ToolTip("Splits when 2 Oblobbles are deafeated (ideally the first pair you encounter in Colo 2)")]
        KilledOblobbles,
        [Description("Oro & Mato Nail Bros (Boss)"), ToolTip("Splits when killing the Nail Bros (Pantheon)")]
        MatoOroNailBros,
        [Description("Pure Vessel (Boss)"), ToolTip("Splits when killing Pure Vessel")]
        PureVessel,
        [Description("Segment Practice - Radiance (Boss)"), ToolTip("Splits when killing The Radiance")]
        RadianceBoss,
        [Description("Segment Practice - THK (Boss)"), ToolTip("Splits when killing The Hollow Knight")]
        HollowKnightBoss,
        [Description("Sheo Paintmaster (Boss)"), ToolTip("Splits when killing Sheo (Pantheon)")]
        SheoPaintmaster,
        [Description("Sly Nailsage (Boss)"), ToolTip("Splits when killing Nailsage Sly (Pantheon)")]
        SlyNailsage,
        [Description("Soul Master (Boss)"), ToolTip("Splits when killing Soul Master")]
        SoulMaster,
        [Description("Soul Tyrant (Boss)"), ToolTip("Splits when killing Soul Tyrant (Soul Master Dream)")]
        SoulTyrant,
        [Description("Soul Tyrant (Essence)"), ToolTip("Splits when getting Soul Tyrant essence (Soul Master Dream)")]
        SoulTyrantEssence,
        [Description("Soul Tyrant w/ Sanctum Grub (Essence)"), ToolTip("Splits when getting Soul Tyrant essence and Sanctum fakedive grub")]
        SoulTyrantEssenceWithSanctumGrub,
        [Description("Traitor Lord (Boss)"), ToolTip("Splits when killing Traitor Lord")]
        TraitorLord,
        [Description("Troupe Master Grimm (Boss)"), ToolTip("Splits when killing Troupe Master Grimm")]
        TroupeMasterGrimm,
        [Description("Uumuu (Boss)"), ToolTip("Splits when killing Uumuu")]
        Uumuu,
        [Description("Watcher Knight (Boss)"), ToolTip("Splits when killing Watcher Knight")]
        BlackKnight,
        [Description("White Defender (Boss)"), ToolTip("Splits when killing White Defender")]
        WhiteDefender,
        [Description("White Defender (Essence)"), ToolTip("Splits when getting White Defender essence")]
        WhiteDefenderEssence,
        [Description("White Defender Level (Boss)"), ToolTip("Splits each time defeating White Defender in Dung Defender's dream")]
        OnDefeatWhiteDefender,
        [Description("Xero (Boss)"), ToolTip("Splits when killing Xero")]
        Xero,
        [Description("Xero (Essence)"), ToolTip("Splits when absorbing essence from Xero")]
        XeroEssence,

        [Description("Vengefly King (Pantheon)"), ToolTip("Splits after killing Vengefly King in Pantheon 1")]
        VengeflyKingP,
        [Description("Gruz Mother (Pantheon)"), ToolTip("Splits after killing Gruz Mother in Pantheon 1")]
        GruzMotherP,
        [Description("False Knight (Pantheon)"), ToolTip("Splits after killing False Knight in Pantheon 1")]
        FalseKnightP,
        [Description("Massive Moss Charger (Pantheon)"), ToolTip("Splits after killing Massive Moss Charger in Pantheon 1")]
        MassiveMossChargerP,
        [Description("Hornet 1 (Pantheon)"), ToolTip("Splits after killing Hornet 1 in Pantheon 1")]
        Hornet1P,
        [Description("Gorb (Pantheon)"), ToolTip("Splits after killing Gorb in Pantheon 1")]
        GorbP,
        [Description("Dung Defender (Pantheon)"), ToolTip("Splits after killing Dung Defender in Pantheon 1")]
        DungDefenderP,
        [Description("Soul Warrior (Pantheon)"), ToolTip("Splits after killing Soul Warrior in Pantheon 1")]
        SoulWarriorP,
        [Description("Brooding Mawlek (Pantheon)"), ToolTip("Splits after killing Brooding Mawlek in Pantheon 1")]
        BroodingMawlekP,
        [Description("Oro & Mato Nail Bros (Pantheon)"), ToolTip("Splits after killing Oro & Mato in Pantheon 1")]
        OroMatoNailBrosP,

        [Description("Xero (Pantheon)"), ToolTip("Splits after killing Xero in Pantheon 2")]
        XeroP,
        [Description("Crystal Guardian (Pantheon)"), ToolTip("Splits after killing Crystal Guardian in Pantheon 2")]
        CrystalGuardianP,
        [Description("Soul Master (Pantheon)"), ToolTip("Splits after killing Soul Master in Pantheon 2")]
        SoulMasterP,
        [Description("Oblobbles (Pantheon)"), ToolTip("Splits after killing Oblobbles in Pantheon 2")]
        OblobblesP,
        [Description("Mantis Lords (Pantheon)"), ToolTip("Splits after killing Mantis Lords in Pantheon 2")]
        MantisLordsP,
        [Description("Marmu (Pantheon)"), ToolTip("Splits after killing Marmu in Pantheon 2")]
        MarmuP,
        [Description("Nosk (Pantheon)"), ToolTip("Splits after killing Nosk in Pantheon 2")]
        NoskP,
        [Description("Flukemarm (Pantheon)"), ToolTip("Splits after killing Flukemarm in Pantheon 2")]
        FlukemarmP,
        [Description("Broken Vessel (Pantheon)"), ToolTip("Splits after killing Broken Vessel in Pantheon 2")]
        BrokenVesselP,
        [Description("Sheo Paintmaster (Pantheon)"), ToolTip("Splits after killing Sheo Paintmaster in Pantheon 2")]
        SheoPaintmasterP,

        [Description("Hive Knight (Pantheon)"), ToolTip("Splits after killing Hive Knight in Pantheon 3")]
        HiveKnightP,
        [Description("Elder Hu (Pantheon)"), ToolTip("Splits after killing Elder Hu in Pantheon 3")]
        ElderHuP,
        [Description("Collector (Pantheon)"), ToolTip("Splits after killing The Collector in Pantheon 3")]
        CollectorP,
        [Description("God Tamer (Pantheon)"), ToolTip("Splits after killing God Tamer in Pantheon 3")]
        GodTamerP,
        [Description("Troupe Master Grimm (Pantheon)"), ToolTip("Splits after killing Troupe Master Grimm in Pantheon 3")]
        TroupeMasterGrimmP,
        [Description("Galien (Pantheon)"), ToolTip("Splits after killing Galien in Pantheon 3")]
        GalienP,
        [Description("Grey Prince Zote (Pantheon)"), ToolTip("Splits after killing Grey Prince Zote in Pantheon 3")]
        GreyPrinceZoteP,
        [Description("Uumuu (Pantheon)"), ToolTip("Splits after killing Uumuu in Pantheon 3")]
        UumuuP,
        [Description("Hornet 2 (Pantheon)"), ToolTip("Splits after killing Hornet 2 in Pantheon 3")]
        Hornet2P,
        [Description("Sly Nailsage (Pantheon)"), ToolTip("Splits after killing Sly Nailsage in Pantheon 3")]
        SlyP,

        [Description("Enraged Guardian (Pantheon)"), ToolTip("Splits after killing Enraged Guardian in Pantheon 4")]
        EnragedGuardianP,
        [Description("Lost Kin (Pantheon)"), ToolTip("Splits after killing Lost Kin in Pantheon 4")]
        LostKinP,
        [Description("No Eyes (Pantheon)"), ToolTip("Splits after killing No Eyes in Pantheon 4")]
        NoEyesP,
        [Description("Traitor Lord (Pantheon)"), ToolTip("Splits after killing Traitor Lord in Pantheon 4")]
        TraitorLordP,
        [Description("White Defender (Pantheon)"), ToolTip("Splits after killing White Defender in Pantheon 4")]
        WhiteDefenderP,
        [Description("Failed Champion (Pantheon)"), ToolTip("Splits after killing Failed Champion in Pantheon 4")]
        FailedChampionP,
        [Description("Markoth (Pantheon)"), ToolTip("Splits after killing Markoth in Pantheon 4")]
        MarkothP,
        [Description("Watcher Knights (Pantheon)"), ToolTip("Splits after killing Watcher Knights in Pantheon 4")]
        WatcherKnightsP,
        [Description("Soul Tyrant (Pantheon)"), ToolTip("Splits after killing Soul Tyrant in Pantheon 4")]
        SoulTyrantP,
        [Description("Pure Vessel (Pantheon)"), ToolTip("Splits after killing Pure Vessel in Pantheon 4")]
        PureVesselP,

        [Description("Nosk Hornet (Pantheon)"), ToolTip("Splits after killing Nosk Hornet in Pantheon 5")]
        NoskHornetP,
        [Description("Nightmare King Grimm (Pantheon)"), ToolTip("Splits after killing Nightmare King Grimm in Pantheon 5")]
        NightmareKingGrimmP,

        [Description("Herrah the Beast (Dreamer)"), ToolTip("Splits when you see the mask for Herrah (In Spider Area)")]
        Hegemol,
        [Description("Lurien the Watcher (Dreamer)"), ToolTip("Splits when you see the mask for Lurien (After killing Watcher Knight)")]
        Lurien,
        [Description("Monomon the Teacher (Dreamer)"), ToolTip("Splits when you see the mask for Monomon (After killing Uumuu)")]
        Monomon,
        [Description("First Dreamer (Dreamer)"), ToolTip("Splits when you see the mask for the first dreamer killed")]
        Dreamer1,
        [Description("Second Dreamer (Dreamer)"), ToolTip("Splits when you see the mask for the second dreamer killed")]
        Dreamer2,
        [Description("Third Dreamer (Dreamer)"), ToolTip("Splits when you see the mask for the third dreamer killed")]
        Dreamer3,

        [Description("106% Pre-Grimm Shop (Event)"), ToolTip("Splits when Lantern + Vessel Fragment(5) + Mask Shard(4) have been acquired")]
        PreGrimmShop,
        [Description("1xx% Sly Final Shop (Event)"), ToolTip("Splits on leaving Sly's shop after having bough Sprintmaster and Vessel Frag 8")]
        SlyShopFinished,
        [Description("Can Overcharm (Event)"), ToolTip("Splits when overcharming is enabled")]
        CanOvercharm,
        [Description("Chains Broken - Hollow Knight (Event)"), ToolTip("Splits at the end of the first Hollow Knight scream after the chains are broken")]
        UnchainedHollowKnight,
        [Description("Chandelier - Watcher Knights (Event)"), ToolTip("Splits when dropping the chandelier on one of the watcher knights")]
        WatcherChandelier,
        [Description("City Gate (Event)"), ToolTip("Splits when using the City Key to open the gate")]
        CityGateOpen,
        [Description("Death (Event)"), ToolTip("Splits when player hp is 0")]
        PlayerDeath,
        [Description("Ending (Event)"), ToolTip("Splits on any credits rolling")]
        EndingSplit,
        [Description("Flower Quest (Event)"), ToolTip("Splits when placing the flower at the grave of the Traitors' Child")]
        FlowerQuest,
        [Description("Flower Quest Reward (Event)"), ToolTip("Splits when Grey Mourner gives you the Flower Quest reward")]
        FlowerRewardGiven,
        [Description("Happy Couple (Event)"), ToolTip("Splits when talking to Nailsmith in Sheo's hut for the first time")]
        HappyCouplePlayerDataEvent,
        [Description("Nailsmith - Killed (Event)"), ToolTip("Splits when Nailsmith is killed")]
        NailsmithKilled,
        [Description("Nightmare Lantern Lit (Event)"), ToolTip("Splits when initially lighting the Nightmare Lantern")]
        NightmareLantern,
        [Description("Nightmare Lantern Destroyed (Event)"), ToolTip("Splits when destroying the Nightmare Lantern")]
        NightmareLanternDestroyed,
        [Description("Radiance Dream Entry (Event)"), ToolTip("Splits when going into the dream world for Hollow Knight to fight Radiance")]
        HollowKnightDreamnail,
        [Description("Seer Departs (Event)"), ToolTip("Splits when the Seer Departs after bringing back 2400 essence")]
        SeerDeparts,
        [Description("Spirit Glade Door (Event)"), ToolTip("Splits when the Seer open Spirit Glade after bringing back 200 essence")]
        SpiritGladeOpen,
        [Description("Trap Bench - Beasts Den (Event)"), ToolTip("Splits when getting the trap bench in Beasts Den")]
        BeastsDenTrapBench,
        [Description("Eternal Ordeal Unlocked (Event)"), ToolTip("Splits when breaking the wall to the Zote statue in Godhome")]
        EternalOrdealUnlocked,
        [Description("Eternal Ordeal Achieved (Event)"), ToolTip("Splits when achieving the ordeal (57th Zote killed)")]
        EternalOrdealAchieved,
        [Description("Riding Stag (Event)"), ToolTip("Splits while riding the stag")]
        RidingStag,
        [Description("Saved Cloth (Event)"), ToolTip("Splits when saving Cloth in Ancient Basin")]
        SavedCloth,
        [Description("Colosseum Unlocked 1 (Trial)"), ToolTip("Splits when the knight unlocks the trial at Little Fool")]
        ColosseumBronzeUnlocked,
        [Description("Colosseum Unlocked 2 (Trial)"), ToolTip("Splits when the knight unlocks the trial at Little Fool")]
        ColosseumSilverUnlocked,
        [Description("Colosseum Unlocked 3 (Trial)"), ToolTip("Splits when the knight unlocks the trial at Little Fool")]
        ColosseumGoldUnlocked,
        [Description("Colosseum Fight 1 (Trial)"), ToolTip("Splits when beating the first Colosseum trial")]
        ColosseumBronze,
        [Description("Colosseum Fight 2 (Trial)"), ToolTip("Splits when beating the second Colosseum trial")]
        ColosseumSilver,
        [Description("Colosseum Fight 3 (Trial)"), ToolTip("Splits when beating the third Colosseum trial")]
        ColosseumGold,
        [Description("Colosseum Exit 1 (Transition)"), ToolTip("Splits on the transition out of the trial, or in the load-in after quitout")]
        ColosseumBronzeExit,
        [Description("Colosseum Exit 2 (Transition)"), ToolTip("Splits on the transition out of the trial, or in the load-in after quitout")]
        ColosseumSilverExit,
        [Description("Colosseum Exit 3 (Transition)"), ToolTip("Splits on the transition out of the trial, or in the load-in after quitout")]
        ColosseumGoldExit,
        [Description("Pantheon 1 (Trial)"), ToolTip("Splits when beating the first Pantheon")]
        Pantheon1,
        [Description("Pantheon 2 (Trial)"), ToolTip("Splits when beating the second Pantheon")]
        Pantheon2,
        [Description("Pantheon 3 (Trial)"), ToolTip("Splits when beating the third Pantheon")]
        Pantheon3,
        [Description("Pantheon 4 (Trial)"), ToolTip("Splits when beating the fourth Pantheon")]
        Pantheon4,
        [Description("Pantheon 5 (Trial)"), ToolTip("Splits when beating the fifth Pantheon")]
        Pantheon5,
        [Description("Path of Pain (Completed)"), ToolTip("Splits when completing the Path of Pain section in White Palace")]
        PathOfPain,

        [Description("Aspid Hunter (Mini Boss)"), ToolTip("Splits when killing the final Aspid Hunter")]
        AspidHunter,
        [Description("Aluba (Killed)"), ToolTip("Splits when killing an Aluba")]
        Aluba,
        //[Description("Al2ba (Killed)"), ToolTip("Splits when killing two Alubas")]
        //Al2ba,
        [Description("Husk Miner (Killed)"), ToolTip("Splits when killing a Husk Miner")]
        HuskMiner,
        [Description("Great Hopper (Killed)"), ToolTip("Splits when killing a Great Hopper")]
        GreatHopper,
        [Description("Gorgeous Husk (Killed)"), ToolTip("Splits when killing Gorgeous Husk")]
        GorgeousHusk,
        [Description("Menderbug (Killed)"), ToolTip("Splits when killing Menderbug")]
        MenderBug,
        [Description("Sanctum Warrior (Killed)"), ToolTip("Splits on first Sanctum Warrior kill")]
        killedSanctumWarrior,
        [Description("Soul Twister (Killed)"), ToolTip("Splits on first Soul Twister kill")]
        killedSoulTwister,
        //[Description("Revek (Killed)"), ToolTip("Splits when talking to Revek after clearing all other Glade ghosts")]
        //Revek,
        [Description("Moss Knight (Mini Boss)"), ToolTip("Splits when killing Moss Knight")]
        MossKnight,
        [Description("Shrumal Ogre (Mini Boss)"), ToolTip("Splits when killing the final Shrumal Ogre")]
        MushroomBrawler,
        [Description("Zote Rescued Vengefly King (Mini Boss)"), ToolTip("Splits when rescuing Zote from the Vengefly King")]
        Zote1,
        [Description("Zote Rescued Deepnest (Mini Boss)"), ToolTip("Splits when rescuing Zote in Deepnest")]
        Zote2,
        [Description("Zote Killed Colosseum (Mini Boss)"), ToolTip("Splits when killing Zote in the Colosseum")]
        ZoteKilled,

        [Description("Forgotten Crossroads (Stag Station)"), ToolTip("Splits when opening the Forgotten Crossroads Stag Station")]
        CrossroadsStation,
        [Description("Greenpath (Stag Station)"), ToolTip("Splits when obtaining Greenpath Stag Station")]
        GreenpathStation,
        [Description("Queen's Station (Stag Station)"), ToolTip("Splits when obtaining Queens Station Stag Station")]
        QueensStationStation,
        [Description("Queen's Gardens (Stag Station)"), ToolTip("Splits when obtaining Queens Gardens Stag Station")]
        QueensGardensStation,
        [Description("City Storerooms (Stag Station)"), ToolTip("Splits when obtaining City Storerooms Stag Station")]
        StoreroomsStation,
        [Description("Kings Station (Stag Station)"), ToolTip("Splits when obtaining Kings Station Stag Station")]
        KingsStationStation,
        [Description("Resting Grounds (Stag Station)"), ToolTip("Splits when obtaining Resting Grounds Stag Station")]
        RestingGroundsStation,
        [Description("Distant Village (Stag Station)"), ToolTip("Splits when obtaining Distant Village Stag Station")]
        DeepnestStation,
        [Description("Hidden Station (Stag Station)"), ToolTip("Splits when obtaining to Hidden Station Stag Station")]
        HiddenStationStation,
        [Description("Stagnest (Stag Station)"), ToolTip("Splits when traveling to Stagnest (Requires Ordered Splits)")]
        StagnestStation,


        [Description("Mr. Mushroom 1 (Spot)"), ToolTip("Splits when talking to Mr. Mushroom")]
        MrMushroom1,
        [Description("Mr. Mushroom 2 (Spot)"), ToolTip("Splits when talking to Mr. Mushroom")]
        MrMushroom2,
        [Description("Mr. Mushroom 3 (Spot)"), ToolTip("Splits when talking to Mr. Mushroom")]
        MrMushroom3,
        [Description("Mr. Mushroom 4 (Spot)"), ToolTip("Splits when talking to Mr. Mushroom")]
        MrMushroom4,
        [Description("Mr. Mushroom 5 (Spot)"), ToolTip("Splits when talking to Mr. Mushroom")]
        MrMushroom5,
        [Description("Mr. Mushroom 6 (Spot)"), ToolTip("Splits when talking to Mr. Mushroom")]
        MrMushroom6,
        [Description("Mr. Mushroom 7 (Spot)"), ToolTip("Splits when talking to Mr. Mushroom")]
        MrMushroom7,

        [Description("Ancient Basin (Area)"), ToolTip("Splits when entering Ancient Basin text first appears")]
        Abyss,
        [Description("City Of Tears (Area)"), ToolTip("Splits when entering City Of Tears text first appears")]
        CityOfTears,
        [Description("Colosseum (Area)"), ToolTip("Splits when entering Colosseum text first appears")]
        Colosseum,
        [Description("Crystal Peak (Area)"), ToolTip("Splits when entering Crystal Peak text first appears")]
        CrystalPeak,
        [Description("Deepnest (Area)"), ToolTip("Splits when entering Deepnest text first appears")]
        Deepnest,
        [Description("Deepnest Spa (Area)"), ToolTip("Splits when entering the Deepnest Spa area with bench")]
        DeepnestSpa,
        [Description("Dirtmouth (Area)"), ToolTip("Splits when entering Dirtmouth text first appears")]
        Dirtmouth,
        [Description("Fog Canyon (Area)"), ToolTip("Splits when entering Fog Canyon text first appears")]
        FogCanyon,
        [Description("Forgotten Crossroads (Area)"), ToolTip("Splits when entering Forgotten Crossroads text first appears")]
        ForgottenCrossroads,
        [Description("Fungal Wastes (Area)"), ToolTip("Splits when entering Fungal Wastes text first appears")]
        FungalWastes,
        [Description("Godhome (Area)"), ToolTip("Splits when entering Godhome text first appears")]
        Godhome,
        [Description("Greenpath (Area)"), ToolTip("Splits when entering Greenpath text first appears")]
        Greenpath,
        [Description("Hive (Area)"), ToolTip("Splits when entering Hive text first appears")]
        Hive,
        [Description("Infected Crossroads (Area)"), ToolTip("Splits when entering Infected Crossroads text first appears")]
        InfectedCrossroads,
        [Description("Kingdom's Edge (Area)"), ToolTip("Splits when entering Kingdom's Edge text first appears")]
        KingdomsEdge,
        [Description("Queen's Gardens (Area)"), ToolTip("Splits when entering Queen's Gardens text first appears")]
        QueensGardens,
        [Description("Resting Grounds (Area)"), ToolTip("Splits when entering Resting Grounds text first appears")]
        RestingGrounds,
        [Description("Royal Waterways (Area)"), ToolTip("Splits when entering Royal Waterways text first appears")]
        RoyalWaterways,
        [Description("Teachers Archive (Area)"), ToolTip("Splits when entering Teachers Archive for the first time")]
        TeachersArchive,
        [Description("White Palace (Area)"), ToolTip("Splits when entering White Palace text for the first time")]
        WhitePalace,
        [Description("White Palace - Workshop (Area)"), ToolTip("Splits when visiting the secret room in White Palace")]
        WhitePalaceSecretRoom,

        [Description("Ancient Basin (Transition)"), ToolTip("Splits on transition to Basin, alternative to the (Area) split")]
        BasinEntry,
        [Description("Blue Lake (Transition)"), ToolTip("Splits on transition to Blue Lake from Gruz Mother scene (requires Ordered Splits)")]
        BlueLake,
        [Description("Crystal Peak Entry (Transition)"), ToolTip("Splits on transition to the room where the dive and toll entrances meet, or the room right of Dirtmouth")]
        CrystalPeakEntry,
        [Description("Crystal Mound Exit (Transition)"), ToolTip("Splits on transition from Crystal Mound")]
        CrystalMoundExit,
        [Description("Enter Any Dream (Transition)"), ToolTip("Splits when entering any dream world")]
        EnterAnyDream,
        [Description("Fog Canyon (Transition)"), ToolTip("Splits on transition to East Fog Canyon")]
        FogCanyonEntry,
        [Description("Fungal Wastes Entry (Transition)"), ToolTip("Splits on transition to Fungal Wastes (scene below Crossroads, right of QS, left of Waterways or with Spore Shroom")]
        FungalWastesEntry,
        [Description("Gorgeous Husk Killed (Transition)"), ToolTip("Splits on transition after Gorgeous Husk defeated")]
        TransGorgeousHusk,
        [Description("Greenpath (Transition)"), ToolTip("Splits when entering Greenpath")]
        EnterGreenpath,
        [Description("Greenpath w/ Unlocked Overcharm (Transition)"), ToolTip("Splits when entering Greenpath with overcharming unlocked")]
        EnterGreenpathWithOvercharm,
        [Description("Has Claw (Transition)"), ToolTip("Splits on transition after Mantis Claw acquired")]
        TransClaw,
        [Description("Has Descending Dark (Transition)"), ToolTip("Splits on transition after Descending Dark acquired")]
        TransDescendingDark,
        [Description("Hive (Transition)"), ToolTip("Splits on transition to Hive")]
        HiveEntry,
        [Description("King's Pass (Transition)"), ToolTip("Splits when leaving King's pass")]
        KingsPass,
        [Description("King's Pass From Town (Transition)"), ToolTip("Splits on transition between Dirtmouth and King's Pass")]
        KingsPassEnterFromTown,
        [Description("Kingdom's Edge (Transition)"), ToolTip("Splits on transition to Kingdom's Edge from Kings Station")]
        KingdomsEdgeEntry,
        [Description("Kingdom's Edge Overcharmed (Transition)"), ToolTip("Splits on transition to Kingdom's Edge from Kings Station while overcharmed")]
        KingdomsEdgeOvercharmedEntry,
        [Description("NKG Dream (Transition)"), ToolTip("Splits on transition to NKG dream")]
        EnterNKG,
        [Description("Queen's Garden Entry (Transition)"), ToolTip("Splits on transition to QG scene following QGA or above Deepnest")]
        QueensGardensEntry,
        [Description("Queen's Garden - Frogs (Transition)"), ToolTip("Splits on transition to QG frogs scene")]
        QueensGardensFrogsTrans,
        [Description("Queen's Garden - Post-Upper Arena (Transition)"), ToolTip("Splits on transition to room after upper arena in QG. Useful for TE, 1xx, and other categories that go to upper QG")]
        QueensGardensPostArenaTransition,
        [Description("Sanctum (Transition)"), ToolTip("Splits when entering Sanctum")]
        EnterSanctum,
        [Description("Sanctum w/ Shade Soul (Transition)"), ToolTip("Splits when entering Sanctum after obtaining shade soul")]
        EnterSanctumWithShadeSoul,
        [Description("Waterways Manhole (Transition)"), ToolTip("Splits on transition to Waterways through Waterways")]
        WaterwaysEntry,
        [Description("White Palace Entry (Transition)"), ToolTip("Splits when entering the first White Palace scene")]
        WhitePalaceEntry,

        [Description("Baldur Shell (Charm)"), ToolTip("Splits when obtaining the Baldur Shell charm")]
        BaldurShell,
        [Description("Dashmaster (Charm)"), ToolTip("Splits when obtaining the Dashmaster charm")]
        Dashmaster,
        [Description("Deep Focus (Charm)"), ToolTip("Splits when obtaining the Deep Focus charm")]
        DeepFocus,
        [Description("Defenders Crest (Charm)"), ToolTip("Splits when obtaining the Defenders Crest charm")]
        DefendersCrest,
        [Description("Dreamshield (Charm)"), ToolTip("Splits when obtaining the Dreamshield charm")]
        Dreamshield,
        [Description("Dream Wielder (Charm)"), ToolTip("Splits when obtaining the Dream Wielder charm")]
        DreamWielder,
        [Description("Flukenest (Charm)"), ToolTip("Splits when obtaining the Flukenest charm")]
        Flukenest,
        [Description("Fragile Greed (Charm)"), ToolTip("Splits when obtaining the Fragile Greed charm")]
        FragileGreed,
        [Description("Fragile Heart (Charm)"), ToolTip("Splits when obtaining the Fragile Heart charm")]
        FragileHeart,
        [Description("Fragile Strength (Charm)"), ToolTip("Splits when obtaining the Fragile Strength charm")]
        FragileStrength,
        [Description("Fury of the Fallen (Charm)"), ToolTip("Splits when obtaining the Fury of the Fallen charm")]
        FuryOfTheFallen,
        [Description("Gathering Swarm (Charm)"), ToolTip("Splits when obtaining the Gathering Swarm charm")]
        GatheringSwarm,
        [Description("Glowing Womb (Charm)"), ToolTip("Splits when obtaining the Glowing Womb charm")]
        GlowingWomb,
        [Description("Grimmchild (Charm)"), ToolTip("Splits when obtaining the Grimmchild charm")]
        Grimmchild,
        [Description("Grimmchild Lvl 2 (Charm)"), ToolTip("Splits when obtaining the Grimmchild Lvl 2 charm")]
        Grimmchild2,
        [Description("Grimmchild Lvl 3 (Charm)"), ToolTip("Splits when obtaining the Grimmchild Lvl 3 charm")]
        Grimmchild3,
        [Description("Grimmchild Lvl 4 (Charm)"), ToolTip("Splits when obtaining the Grimmchild Lvl 4 charm")]
        Grimmchild4,
        [Description("Grubberfly's Elegy (Charm)"), ToolTip("Splits when obtaining the Grubberfly's Elegy charm")]
        GrubberflysElegy,
        [Description("Grubsong (Charm)"), ToolTip("Splits when obtaining the Grubsong charm")]
        Grubsong,
        [Description("Heavy Blow (Charm)"), ToolTip("Splits when obtaining the Heavy Blow charm")]
        HeavyBlow,
        [Description("Hiveblood (Charm)"), ToolTip("Splits when obtaining the Hiveblood charm")]
        Hiveblood,
        [Description("Joni's Blessing (Charm)"), ToolTip("Splits when obtaining the Joni's Blessing charm")]
        JonisBlessing,
        [Description("Kingsoul Fragment - Queen's (Charm)"), ToolTip("Splits on picking up the left Kingsoul White Fragment")]
        WhiteFragmentLeft,
        [Description("Kingsoul Fragment - King's (Charm)"), ToolTip("Splits on picking up the right Kingsoul White Fragment")]
        WhiteFragmentRight,
        [Description("Kingsoul (Charm)"), ToolTip("Splits when obtaining the completed Kingsoul charm")]
        Kingsoul,
        [Description("Lifeblood Core (Charm)"), ToolTip("Splits when obtaining the Lifeblood Core charm")]
        LifebloodCore,
        [Description("Lifeblood Heart (Charm)"), ToolTip("Splits when obtaining the Lifeblood Heart charm")]
        LifebloodHeart,
        [Description("Longnail (Charm)"), ToolTip("Splits when obtaining the Longnail charm")]
        Longnail,
        [Description("Mark Of Pride (Charm)"), ToolTip("Splits when obtaining the Mark Of Pride charm")]
        MarkOfPride,
        [Description("Nailmaster's Glory (Charm)"), ToolTip("Splits when obtaining the Nailmaster's Glory charm")]
        NailmastersGlory,
        [Description("Quick Focus (Charm)"), ToolTip("Splits when obtaining the Quick Focus charm")]
        QuickFocus,
        [Description("Quick Slash (Charm)"), ToolTip("Splits when obtaining the Quick Slash charm")]
        QuickSlash,
        [Description("Shaman Stone (Charm)"), ToolTip("Splits when obtaining Shaman Stone charm")]
        ShamanStone,
        [Description("Shape of Unn (Charm)"), ToolTip("Splits when obtaining Shape of Unn charm")]
        ShapeOfUnn,
        [Description("Sharp Shadow (Charm)"), ToolTip("Splits when obtaining Sharp Shadow charm")]
        SharpShadow,
        [Description("Soul Catcher (Charm)"), ToolTip("Splits when obtaining the Soul Catcher charm")]
        SoulCatcher,
        [Description("Soul Eater (Charm)"), ToolTip("Splits when obtaining the Soul Eater charm")]
        SoulEater,
        [Description("Spell Twister (Charm)"), ToolTip("Splits when obtaining the Spell Twister charm")]
        SpellTwister,
        [Description("Spore Shroom (Charm)"), ToolTip("Splits when obtaining the Spore Shroom charm")]
        SporeShroom,
        [Description("Sprintmaster (Charm)"), ToolTip("Splits when obtaining the Sprintmaster charm")]
        Sprintmaster,
        [Description("Stalwart Shell (Charm)"), ToolTip("Splits when obtaining Stalwart Shell charm")]
        StalwartShell,
        [Description("Steady Body (Charm)"), ToolTip("Splits when obtaining the Steady Body charm")]
        SteadyBody,
        [Description("Thorns Of Agony (Charm)"), ToolTip("Splits when obtaining Thorns of Agony charm")]
        ThornsOfAgony,
        [Description("Unbreakable Greed (Charm)"), ToolTip("Splits when obtaining the Unbreakable Greed charm")]
        UnbreakableGreed,
        [Description("Unbreakable Heart (Charm)"), ToolTip("Splits when obtaining the Unbreakable Heart charm")]
        UnbreakableHeart,
        [Description("Unbreakable Strength (Charm)"), ToolTip("Splits when obtaining the Unbreakable Strength charm")]
        UnbreakableStrength,
        [Description("Void Heart (Charm)"), ToolTip("Splits when changing the Kingsoul to the Void Heart charm")]
        VoidHeart,
        [Description("Wayward Compass (Charm)"), ToolTip("Splits when obtaining Wayward Compass charm")]
        WaywardCompass,
        [Description("Weaversong (Charm)"), ToolTip("Splits when obtaining the Weaversong charm")]
        Weaversong,
        [Description("Shrumal Ogres (Charm Notch)"), ToolTip("Splits when obtaining the charm notch after defeating the Shrumal Ogres")]
        NotchShrumalOgres,
        [Description("Fog Canyon (Charm Notch)"), ToolTip("Splits when obtaining the charm notch in Fog Canyon")]
        NotchFogCanyon,
        [Description("Grimm (Charm Notch)"), ToolTip("Splits when obtaining the charm notch after Grimm")]
        NotchGrimm,
        [Description("Salubra 1 (Charm Notch)"), ToolTip("Splits when obtaining the first charm notch from Salubra")]
        NotchSalubra1,
        [Description("Salubra 2 (Charm Notch)"), ToolTip("Splits when obtaining the second charm notch from Salubra")]
        NotchSalubra2,
        [Description("Salubra 3 (Charm Notch)"), ToolTip("Splits when obtaining the third charm notch from Salubra")]
        NotchSalubra3,
        [Description("Salubra 4 (Charm Notch)"), ToolTip("Splits when obtaining the fourth charm notch from Salubra")]
        NotchSalubra4,

        [Description("Lemm Shop (NPC)"), ToolTip("Splits when talking to Lemm in the shop for the first time")]
        Lemm2,
        [Description("Lemm - ACN (Event)"), ToolTip("Splits on having sold a total: 1 journal, 6 seals, and 4 idols to Lemm")]
        AllCharmNotchesLemm2CP,
        [Description("Met Grey Mourner (NPC)"), ToolTip("Splits when talking to Grey Mourner for the first time")]
        MetGreyMourner,
        [Description("Mourner w/ Seer Ascended (NPC)"), ToolTip("Splits when both talked to Grey Mourner and Seer has ascended")]
        GreyMournerSeerAscended,
        [Description("Elderbug Flower Quest (NPC)"), ToolTip("Splits when giving the flower to the Elderbug")]
        ElderbugFlower,
        [Description("Godseeker Flower (NPC)"), ToolTip("Splits when giving Godseeker a flower")]
        givenGodseekerFlower,
        [Description("Oro Flower (NPC)"), ToolTip("Splits when giving Oro a flower")]
        givenOroFlower,
        [Description("White Lady Flower (NPC)"), ToolTip("Splits when giving White Lady a flower")]
        givenWhiteLadyFlower,
        [Description("Emilitia Flower (NPC)"), ToolTip("Splits when giving Emilita a flower")]
        givenEmilitiaFlower,
        [Description("Bretta Rescued (NPC)"), ToolTip("Splits when saving Bretta")]
        BrettaRescued,
        [Description("Brumm Flame (NPC)"), ToolTip("Splits when collecting Brumm's flame in Deepnest")]
        BrummFlame,
        [Description("Little Fool (NPC)"), ToolTip("Splits when talking to the Little Fool for the first time")]
        LittleFool,
        [Description("Sly Rescued (NPC)"), ToolTip("Splits when saving Sly")]
        SlyRescued,

        [Description("Grimm Flame 1 (Flame)"), ToolTip("Splits after obtaining the first flame.")]
        Flame1,
        [Description("Grimm Flame 2 (Flame)"), ToolTip("Splits after obtaining the second flame.")]
        Flame2,
        [Description("Grimm Flame 3 (Flame)"), ToolTip("Splits after obtaining the third flame.")]
        Flame3,

        [Description("Pale Ore 1 (Ore)"), ToolTip("Splits after obtaining the first pale ore.")]
        Ore1,
        [Description("Pale Ore 2 (Ore)"), ToolTip("Splits after obtaining the second pale ore.")]
        Ore2,
        [Description("Pale Ore 3 (Ore)"), ToolTip("Splits after obtaining the third pale ore.")]
        Ore3,
        [Description("Pale Ore 4 (Ore)"), ToolTip("Splits after obtaining the fourth pale ore.")]
        Ore4,
        [Description("Pale Ore 5 (Ore)"), ToolTip("Splits after obtaining the fifth pale ore.")]
        Ore5,
        [Description("Pale Ore 6 (Ore)"), ToolTip("Splits after obtaining the sixth pale ore.")]
        Ore6,

        [Description("Rescued Grub 1 (Grub)"), ToolTip("Splits when rescuing grub #1")]
        Grub1,
        [Description("Rescued Grub 2 (Grub)"), ToolTip("Splits when rescuing grub #2")]
        Grub2,
        [Description("Rescued Grub 3 (Grub)"), ToolTip("Splits when rescuing grub #3")]
        Grub3,
        [Description("Rescued Grub 4 (Grub)"), ToolTip("Splits when rescuing grub #4")]
        Grub4,
        [Description("Rescued Grub 5 (Grub)"), ToolTip("Splits when rescuing grub #5")]
        Grub5,
        [Description("Rescued Grub 6 (Grub)"), ToolTip("Splits when rescuing grub #6")]
        Grub6,
        [Description("Rescued Grub 7 (Grub)"), ToolTip("Splits when rescuing grub #7")]
        Grub7,
        [Description("Rescued Grub 8 (Grub)"), ToolTip("Splits when rescuing grub #8")]
        Grub8,
        [Description("Rescued Grub 9 (Grub)"), ToolTip("Splits when rescuing grub #9")]
        Grub9,
        [Description("Rescued Grub 10 (Grub)"), ToolTip("Splits when rescuing grub #10")]
        Grub10,
        [Description("Rescued Grub 11 (Grub)"), ToolTip("Splits when rescuing grub #11")]
        Grub11,
        [Description("Rescued Grub 12 (Grub)"), ToolTip("Splits when rescuing grub #12")]
        Grub12,
        [Description("Rescued Grub 13 (Grub)"), ToolTip("Splits when rescuing grub #13")]
        Grub13,
        [Description("Rescued Grub 14 (Grub)"), ToolTip("Splits when rescuing grub #14")]
        Grub14,
        [Description("Rescued Grub 15 (Grub)"), ToolTip("Splits when rescuing grub #15")]
        Grub15,
        [Description("Rescued Grub 16 (Grub)"), ToolTip("Splits when rescuing grub #16")]
        Grub16,
        [Description("Rescued Grub 17 (Grub)"), ToolTip("Splits when rescuing grub #17")]
        Grub17,
        [Description("Rescued Grub 18 (Grub)"), ToolTip("Splits when rescuing grub #18")]
        Grub18,
        [Description("Rescued Grub 19 (Grub)"), ToolTip("Splits when rescuing grub #19")]
        Grub19,
        [Description("Rescued Grub 20 (Grub)"), ToolTip("Splits when rescuing grub #20")]
        Grub20,
        [Description("Rescued Grub 21 (Grub)"), ToolTip("Splits when rescuing grub #21")]
        Grub21,
        [Description("Rescued Grub 22 (Grub)"), ToolTip("Splits when rescuing grub #22")]
        Grub22,
        [Description("Rescued Grub 23 (Grub)"), ToolTip("Splits when rescuing grub #23")]
        Grub23,
        [Description("Rescued Grub 24 (Grub)"), ToolTip("Splits when rescuing grub #24")]
        Grub24,
        [Description("Rescued Grub 25 (Grub)"), ToolTip("Splits when rescuing grub #25")]
        Grub25,
        [Description("Rescued Grub 26 (Grub)"), ToolTip("Splits when rescuing grub #26")]
        Grub26,
        [Description("Rescued Grub 27 (Grub)"), ToolTip("Splits when rescuing grub #27")]
        Grub27,
        [Description("Rescued Grub 28 (Grub)"), ToolTip("Splits when rescuing grub #28")]
        Grub28,
        [Description("Rescued Grub 29 (Grub)"), ToolTip("Splits when rescuing grub #29")]
        Grub29,
        [Description("Rescued Grub 30 (Grub)"), ToolTip("Splits when rescuing grub #30")]
        Grub30,
        [Description("Rescued Grub 31 (Grub)"), ToolTip("Splits when rescuing grub #31")]
        Grub31,
        [Description("Rescued Grub 32 (Grub)"), ToolTip("Splits when rescuing grub #32")]
        Grub32,
        [Description("Rescued Grub 33 (Grub)"), ToolTip("Splits when rescuing grub #33")]
        Grub33,
        [Description("Rescued Grub 34 (Grub)"), ToolTip("Splits when rescuing grub #34")]
        Grub34,
        [Description("Rescued Grub 35 (Grub)"), ToolTip("Splits when rescuing grub #35")]
        Grub35,
        [Description("Rescued Grub 36 (Grub)"), ToolTip("Splits when rescuing grub #36")]
        Grub36,
        [Description("Rescued Grub 37 (Grub)"), ToolTip("Splits when rescuing grub #37")]
        Grub37,
        [Description("Rescued Grub 38 (Grub)"), ToolTip("Splits when rescuing grub #38")]
        Grub38,
        [Description("Rescued Grub 39 (Grub)"), ToolTip("Splits when rescuing grub #39")]
        Grub39,
        [Description("Rescued Grub 40 (Grub)"), ToolTip("Splits when rescuing grub #40")]
        Grub40,
        [Description("Rescued Grub 41 (Grub)"), ToolTip("Splits when rescuing grub #41")]
        Grub41,
        [Description("Rescued Grub 42 (Grub)"), ToolTip("Splits when rescuing grub #42")]
        Grub42,
        [Description("Rescued Grub 43 (Grub)"), ToolTip("Splits when rescuing grub #43")]
        Grub43,
        [Description("Rescued Grub 44 (Grub)"), ToolTip("Splits when rescuing grub #44")]
        Grub44,
        [Description("Rescued Grub 45 (Grub)"), ToolTip("Splits when rescuing grub #45")]
        Grub45,
        [Description("Rescued Grub 46 (Grub)"), ToolTip("Splits when rescuing grub #46")]
        Grub46,

        [Description("Rescued Grub Basin Dive (Grub)"), ToolTip("Splits when rescuing the grub in Abyss_17")]
        GrubBasinDive,
        [Description("Rescued Grub Basin Wings (Grub)"), ToolTip("Splits when rescuing the grub in Abyss_19")]
        GrubBasinWings,
        [Description("Rescued Grub City Below Love Tower (Grub)"), ToolTip("Splits when rescuing the grub in Ruins2_07")]
        GrubCityBelowLoveTower,
        [Description("Rescued Grub City Below Sanctum (Grub)"), ToolTip("Splits when rescuing the grub in Ruins1_05")]
        GrubCityBelowSanctum,
        [Description("Rescued Grub City Collector All (Grub)"), ToolTip("Splits when rescuing all three grubs in Ruins2_11. (On 1221, splits for right grub)")]
        GrubCityCollectorAll,
        [Description("Rescued Grub City Collector (Grub)"), ToolTip("Splits when rescuing any grub in Ruins2_11")]
        GrubCityCollector,
        [Description("Rescued Grub City Guard House (Grub)"), ToolTip("Splits when rescuing the grub in Ruins_House_01")]
        GrubCityGuardHouse,
        [Description("Rescued Grub City Sanctum (Grub)"), ToolTip("Splits when rescuing the grub in Ruins1_32")]
        GrubCitySanctum,
        [Description("Rescued Grub City Spire (Grub)"), ToolTip("Splits when rescuing the grub in Ruins2_03")]
        GrubCitySpire,
        [Description("Rescued Grub Cliffs Baldur Shell (Grub)"), ToolTip("Splits when rescuing the grub in Fungus1_28")]
        GrubCliffsBaldurShell,
        [Description("Rescued Grub Crossroads Acid (Grub)"), ToolTip("Splits when rescuing the grub in Crossroads_35")]
        GrubCrossroadsAcid,
        [Description("Rescued Grub Crossroads Guarded (Grub)"), ToolTip("Splits when rescuing the grub in Crossroads_48")]
        GrubCrossroadsGuarded,
        [Description("Rescued Grub Crossroads Spikes (Grub)"), ToolTip("Splits when rescuing the grub in Crossroads_31")]
        GrubCrossroadsSpikes,
        [Description("Rescued Grub Crossroads Vengefly (Grub)"), ToolTip("Splits when rescuing the grub in Crossroads_05")]
        GrubCrossroadsVengefly,
        [Description("Rescued Grub Crossroads Wall (Grub)"), ToolTip("Splits when rescuing the grub in Crossroads_03")]
        GrubCrossroadsWall,
        [Description("Rescued Grub Crystal Peaks Bottom Lever (Grub)"), ToolTip("Splits when rescuing the grub in Mines_04")]
        GrubCrystalPeaksBottomLever,
        [Description("Rescued Grub Crystal Peaks Crown (Grub)"), ToolTip("Splits when rescuing the grub in Mines_24")]
        GrubCrystalPeaksCrown,
        [Description("Rescued Grub Crystal Peaks Crushers (Grub)"), ToolTip("Splits when rescuing the grub in Mines_19")]
        GrubCrystalPeaksCrushers,
        [Description("Rescued Grub Crystal Peaks Crystal Heart (Grub)"), ToolTip("Splits when rescuing the grub in Mines_31")]
        GrubCrystalPeaksCrystalHeart,
        [Description("Rescued Grub Crystal Peaks Mimic (Grub)"), ToolTip("Splits when rescuing the grub in Mines_16")]
        GrubCrystalPeaksMimics,
        [Description("Rescued Grub Crystal Peaks Mound (Grub)"), ToolTip("Splits when rescuing the grub in Mines_35")]
        GrubCrystalPeaksMound,
        [Description("Rescued Grub Crystal Peaks Spikes (Grub)"), ToolTip("Splits when rescuing the grub in Mines_03")]
        GrubCrystalPeaksSpikes,
        [Description("Rescued Grub Deepnest Beast's Den (Grub)"), ToolTip("Splits when rescuing the grub in Deepnest_Spider_Town")]
        GrubDeepnestBeastsDen,
        [Description("Rescued Grub Deepnest Dark (Grub)"), ToolTip("Splits when rescuing the grub in Deepnest_39")]
        GrubDeepnestDark,
        [Description("Rescued Grub Deepnest Mimics (Grub)"), ToolTip("Splits when rescuing the grub in Deepnest_36")]
        GrubDeepnestMimics,
        [Description("Rescued Grub Deepnest Nosk (Grub)"), ToolTip("Splits when rescuing the grub in Deepnest_31")]
        GrubDeepnestNosk,
        [Description("Rescued Grub Deepnest Spikes (Grub)"), ToolTip("Splits when rescuing the grub in Deepnest_03")]
        GrubDeepnestSpikes,
        [Description("Rescued Grub Fog Canyon Archives (Grub)"), ToolTip("Splits when rescuing the grub in Fungus3_47")]
        GrubFogCanyonArchives,
        [Description("Rescued Grub Fungal Bouncy (Grub)"), ToolTip("Splits when rescuing the grub in Fungus2_18")]
        GrubFungalBouncy,
        [Description("Rescued Grub Fungal Spore Shroom (Grub)"), ToolTip("Splits when rescuing the grub in Fungus2_20")]
        GrubFungalSporeShroom,
        [Description("Rescued Grub Greenpath Cornifer (Grub)"), ToolTip("Splits when rescuing the grub in Fungus1_06")]
        GrubGreenpathCornifer,
        [Description("Rescued Grub Greenpath Hunter (Grub)"), ToolTip("Splits when rescuing the grub in Fungus1_07")]
        GrubGreenpathHunter,
        [Description("Rescued Grub Greenpath Moss Knight (Grub)"), ToolTip("Splits when rescuing the grub in Fungus1_21")]
        GrubGreenpathMossKnight,
        [Description("Rescued Grub Greenpath Vessel Fragment (Grub)"), ToolTip("Splits when rescuing the grub in Fungus1_13")]
        GrubGreenpathVesselFragment,
        [Description("Rescued Grub Hive External (Grub)"), ToolTip("Splits when rescuing the grub in Hive_03")]
        GrubHiveExternal,
        [Description("Rescued Grub Hive Internal (Grub)"), ToolTip("Splits when rescuing the grub in Hive_04")]
        GrubHiveInternal,
        [Description("Rescued Grub Kingdom's Edge Center (Grub)"), ToolTip("Splits when rescuing the grub in Deepnest_East_11")]
        GrubKingdomsEdgeCenter,
        [Description("Rescued Grub Kingdom's Edge Oro (Grub)"), ToolTip("Splits when rescuing the grub in Deepnest_East_14")]
        GrubKingdomsEdgeOro,
        [Description("Rescued Grub Queen's Gardens Below Stag (Grub)"), ToolTip("Splits when rescuing the grub in Fungus3_10")]
        GrubQueensGardensBelowStag,
        [Description("Rescued Grub Queen's Gardens Upper (Grub)"), ToolTip("Splits when rescuing the grub in Fungus3_22")]
        GrubQueensGardensUpper,
        [Description("Rescued Grub Queen's Gardens White Lady (Grub)"), ToolTip("Splits when rescuing the grub in Fungus3_48")]
        GrubQueensGardensWhiteLady,
        [Description("Rescued Grub Resting Grounds Crypts (Grub)"), ToolTip("Splits when rescuing the grub in RestingGrounds_10")]
        GrubRestingGroundsCrypts,
        [Description("Rescued Grub Waterways Center (Grub)"), ToolTip("Splits when rescuing the grub in Waterways_04")]
        GrubWaterwaysCenter,
        [Description("Rescued Grub Waterways Hwurmps (Grub)"), ToolTip("Splits when rescuing the grub in Waterways_14")]
        GrubWaterwaysHwurmps,
        [Description("Rescued Grub Waterways Isma (Grub)"), ToolTip("Splits when rescuing the grub in Waterways_13")]
        GrubWaterwaysIsma,

        [Description("Whispering Root (Ancestral Mound)"), ToolTip("Splits upon completing the whispering root in the Ancestral Mound")]
        TreeMound,
        [Description("Whispering Root (City of Tears)"), ToolTip("Splits upon completing the whispering root in the City of Tears")]
        TreeCity,
        [Description("Whispering Root (Crystal Peak)"), ToolTip("Splits upon completing the whispering root in Crystal Peak")]
        TreePeak,
        [Description("Whispering Root (Deepnest)"), ToolTip("Splits upon completing the whispering root in Deepnest")]
        TreeDeepnest,
        [Description("Whispering Root (Forgotten Crossroads)"), ToolTip("Splits upon completing the whispering root in the Forgotten Crossroads")]
        TreeCrossroads,
        [Description("Whispering Root (Leg Eater)"), ToolTip("Splits upon completing the whispering root left from Leg Eater")]
        TreeLegEater,
        [Description("Whispering Root (Mantis Village)"), ToolTip("Splits upon completing the whispering root above the Mantis Village")]
        TreeMantisVillage,
        [Description("Whispering Root (Greenpath)"), ToolTip("Splits upon completing the whispering root in Greenpath")]
        TreeGreenpath,
        [Description("Whispering Root (Hive)"), ToolTip("Splits upon completing the whispering root in the Hive")]
        TreeHive,
        [Description("Whispering Root (Howling Cliffs)"), ToolTip("Splits upon completing the whispering root in the Howling Cliifs")]
        TreeCliffs,
        [Description("Whispering Root (Kingdom's Edge)"), ToolTip("Splits upon completing the whispering root in the Kingdom's Edge")]
        TreeKingdomsEdge,
        [Description("Whispering Root (Queen's Gardens)"), ToolTip("Splits upon completing the whispering root in the Queen's Gardens")]
        TreeQueensGardens,
        [Description("Whispering Root (Resting Grounds)"), ToolTip("Splits upon completing the whispering root in the Resting Grounds")]
        TreeRestingGrounds,
        [Description("Whispering Root (Royal Waterways)"), ToolTip("Splits upon completing the whispering root in the Royal Waterways")]
        TreeWaterways,
        [Description("Whispering Root (Spirits' Glade)"), ToolTip("Splits upon completing the whispering root in the Spirits' Glade")]
        TreeGlade,

        [Description("100 Essence (Essence)"), ToolTip("Essence")]
        Essence100,
        [Description("200 Essence (Essence)"), ToolTip("Essence")]
        Essence200,
        [Description("300 Essence (Essence)"), ToolTip("Essence")]
        Essence300,
        [Description("400 Essence (Essence)"), ToolTip("Essence")]
        Essence400,
        [Description("500 Essence (Essence)"), ToolTip("Essence")]
        Essence500,
        [Description("600 Essence (Essence)"), ToolTip("Essence")]
        Essence600,
        [Description("700 Essence (Essence)"), ToolTip("Essence")]
        Essence700,
        [Description("800 Essence (Essence)"), ToolTip("Essence")]
        Essence800,
        [Description("900 Essence (Essence)"), ToolTip("Essence")]
        Essence900,
        [Description("1000 Essence (Essence)"), ToolTip("Essence")]
        Essence1000,
        [Description("1100 Essence (Essence)"), ToolTip("Essence")]
        Essence1100,
        [Description("1200 Essence (Essence)"), ToolTip("Essence")]
        Essence1200,
        [Description("1300 Essence (Essence)"), ToolTip("Essence")]
        Essence1300,
        [Description("1400 Essence (Essence)"), ToolTip("Essence")]
        Essence1400,
        [Description("1500 Essence (Essence)"), ToolTip("Essence")]
        Essence1500,
        [Description("1600 Essence (Essence)"), ToolTip("Essence")]
        Essence1600,
        [Description("1700 Essence (Essence)"), ToolTip("Essence")]
        Essence1700,
        [Description("1800 Essence (Essence)"), ToolTip("Essence")]
        Essence1800,
        [Description("1900 Essence (Essence)"), ToolTip("Essence")]
        Essence1900,
        [Description("2000 Essence (Essence)"), ToolTip("Essence")]
        Essence2000,
        [Description("2100 Essence (Essence)"), ToolTip("Essence")]
        Essence2100,
        [Description("2200 Essence (Essence)"), ToolTip("Essence")]
        Essence2200,
        [Description("2300 Essence (Essence)"), ToolTip("Essence")]
        Essence2300,
        [Description("2400 Essence (Essence)"), ToolTip("Essence")]
        Essence2400,

        [Description("Queen's Garden Bench (Toll)"), ToolTip("Splits when buying Queen's Garden toll bench")]
        TollBenchQG,
        [Description("Sanctum Bench (Toll)"), ToolTip("Splits when buying City/Sanctum toll bench by Cornifer's location")]
        TollBenchCity,
        [Description("Basin Bench (Toll)"), ToolTip("Splits when buying Ancient Basin toll bench")]
        TollBenchBasin,
        [Description("Waterways Manhole (Toll)"), ToolTip("Splits when opening the Waterways Manhole")]
        WaterwaysManhole,
        [Description("Tram Deepnest (Tram)"), ToolTip("Splits when unlocking the tram in Deepnest")]
        TramDeepnest,

        [Description("White Palace - Lower Orb (Lever)"), ToolTip("Splits when lighting the orb in White Palace lowest floor")]
        WhitePalaceOrb1,
        [Description("White Palace - Left Orb (Lever)"), ToolTip("Splits when lighting the orb in White Palace left wing")]
        WhitePalaceOrb3,
        [Description("White Palace - Right Orb (Lever)"), ToolTip("Splits when lighting the orb in White Palace right wing")]
        WhitePalaceOrb2,


        [Description("White Palace - Lower Entry (Room)"), ToolTip("Splits on transition to White_Palace_01")]
        WhitePalaceLowerEntry,
        [Description("White Palace - Lower Orb (Room)"), ToolTip("Splits on transition to White_Palace_02")]
        WhitePalaceLowerOrb,
        [Description("White Palace - Left Entry (Room)"), ToolTip("Splits on transition to White_Palace_04")]
        WhitePalaceLeftEntry,
        [Description("White Palace - Left Midpoint (Room)"), ToolTip("Splits on transition between White_Palace_04 and _14")]
        WhitePalaceLeftWingMid,
        [Description("White Palace - Right Side Entry (Room)"), ToolTip("Splits on transition between White_Palace_03_Hub and _15")]
        WhitePalaceRightEntry,
        [Description("White Palace - Right Side Climb (Room)"), ToolTip("Splits on transition between White_Palace_05 and _16")]
        WhitePalaceRightClimb,
        [Description("White Palace - Right Side Saw Squeeze (Room)"), ToolTip("Splits on transition between White_Palace_16 and _05")]
        WhitePalaceRightSqueeze,
        [Description("White Palace - Right Side Exit (Room)"), ToolTip("Splits on transition between White_Palace_05 and _15")]
        WhitePalaceRightDone,
        [Description("White Palace - Top Entry (Room)"), ToolTip("Splits on transition between White_Palace_03_Hub and _06")]
        WhitePalaceTopEntry,
        [Description("White Palace - Top Cursed Cycle (Room)"), ToolTip("Splits on transition between White_Palace_06 and _07")]
        WhitePalaceTopClimb,
        [Description("White Palace - Top Lever (Room)"), ToolTip("Splits on transition between White_Palace_07 and _12")]
        WhitePalaceTopLeverRoom,
        [Description("White Palace - Top Final Platforming (Room)"), ToolTip("Splits on transition between White_Palace_12 and _13")]
        WhitePalaceTopLastPlats,
        [Description("White Palace - Throne Room (Room)"), ToolTip("Splits on transition between White_Palace_13 and _09")]
        WhitePalaceThroneRoom,
        [Description("White Palace - Atrium (Room)"), ToolTip("Splits on any transition to White_Palace_03_Hub")]
        WhitePalaceAtrium,

        [Description("Path of Pain Room 1 (Room)"), ToolTip("Splits on transition to the first room in PoP (entry to PoP)")]
        PathOfPainEntry,
        [Description("Path of Pain Room 2 (Room)"), ToolTip("Splits on transition to the second room in PoP")]
        PathOfPainTransition1,
        [Description("Path of Pain Room 3 (Room)"), ToolTip("Splits on transition to the third room in PoP")]
        PathOfPainTransition2,
        [Description("Path of Pain Room 4 (Room)"), ToolTip("Splits on transition to the fourth room in PoP (Final room)")]
        PathOfPainTransition3,

        [Description("Kingdom's Edge Acid (Dreamgate)"), ToolTip("Splits when placing Dreamgate by KE Acid (hopefully)")]
        DgateKingdomsEdgeAcid,

        [Description("Soul Master Encountered (Practice)"), ToolTip("Splits when Soul Master is activated the first time and the gate closes")]
        SoulMasterEncountered,

        [Description("Godhome Bench (Transition)"), ToolTip("Splits when leaving a Godhome Bench room")]
        GodhomeBench,
        [Description("Godhome Lore Room (Transition)"), ToolTip("Splits when leaving a Godhome lore room")]
        GodhomeLoreRoom,
        [Description("Pantheon 1-4 (Transition)"), ToolTip("Splits on entry to any of pantheon 1 - 4")]
        Pantheon1to4Entry,
        [Description("Pantheon 5 (Transition)"), ToolTip("Splits on entry to pantheon 5")]
        Pantheon5Entry,

        [Description("Main Menu (Menu)"), ToolTip("Splits on main menu")]
        Menu,
        [Description("Main Menu w/ Claw (Menu)"), ToolTip("Splits on transition to menu after Mantis Claw acquired")]
        MenuClaw,
        [Description("Main Menu w/ Ghusk (Menu)"), ToolTip("Splits on transition to menu after Gorgeous Husk defeated")]
        MenuGorgeousHusk,

        [Description("Cornifer at Home (Transition)"), ToolTip("Splits when entering Iselda's hut while Cornifer is sleeping")]
        CorniferAtHome,
        [Description("All Seals (Item)"), ToolTip("Splits when 17 Hallownest Seals have been collected")]
        AllSeals,
        [Description("All Eggs (Item)"), ToolTip("Splits when 21 Rancid Eggs have been collected")]
        AllEggs,
        [Description("Sly Simple Key (Item)"), ToolTip("Splits when buying the simple key from Sly")]
        SlySimpleKey,
        [Description("All Breakables (Event)"), ToolTip("Splits when all 3 fragile charms are broken")]
        AllBreakables,
        [Description("All Unbreakables (Charm)"), ToolTip("Splits when all 3 unbreakable charms are obtained")]
        AllUnbreakables,
        [Description("Met Emilitia (Event)"), ToolTip("Splits when talking to Emilitia for the first time")]
        MetEmilitia,

        [Description("Map Dirtmouth (Item)"), ToolTip("Splits when acquiring the Dirtmouth map")]
        mapDirtmouth,
        [Description("Map Crossroads (Item)"), ToolTip("Splits when acquiring the Crossroads map")]
        mapCrossroads,
        [Description("Map Greenpath (Item)"), ToolTip("Splits when acquiring the Greenpath map")]
        mapGreenpath,
        [Description("Map Fog Canyon (Item)"), ToolTip("Splits when acquiring the Fog Canyon map")]
        mapFogCanyon,
        [Description("Map Queen's Gardens (Item)"), ToolTip("Splits when acquiring the QG map")]
        mapRoyalGardens,
        [Description("Map Fungal Wastes (Item)"), ToolTip("Splits when acquiring the Fungal Wastes map")]
        mapFungalWastes,
        [Description("Map City of Tears (Item)"), ToolTip("Splits when acquiring the City map")]
        mapCity,
        [Description("Map Waterways (Item)"), ToolTip("Splits when acquiring the Waterways map")]
        mapWaterways,
        [Description("Map Crystal Peak (Item)"), ToolTip("Splits when acquiring the Crystal Peak map")]
        mapMines,
        [Description("Map Deepnest (Item)"), ToolTip("Splits when acquiring the Deepnest map")]
        mapDeepnest,
        [Description("Map Howling Cliffs (Item)"), ToolTip("Splits when acquiring the Howling Cliffs map")]
        mapCliffs,
        [Description("Map Kingdom's Edge (Item)"), ToolTip("Splits when acquiring the KE map")]
        mapOutskirts,
        [Description("Map Resting Grounds (Item)"), ToolTip("Splits when acquiring the Resting Grounds map")]
        mapRestingGrounds,
        [Description("Map Ancient Basin (Item)"), ToolTip("Splits when acquiring the Abyss map")]
        mapAbyss,

        [Description("Dream Nail Marissa (Obtain)"), ToolTip("Splits when obtaining the essence from Marissa")]
        OnObtainGhostMarissa,
        [Description("Dream Nail Caelif and Fera (Obtain)"), ToolTip("Splits when obtaining the essence from Caelif and Fera Orthop")]
        OnObtainGhostCaelifFera,
        [Description("Dream Nail Poggy (Obtain)"), ToolTip("Splits when obtaining the essence from Poggy Thorax")]
        OnObtainGhostPoggy,
        [Description("Dream Nail Gravedigger (Obtain)"), ToolTip("Splits when obtaining the essence from Gravedigger")]
        OnObtainGhostGravedigger,
        [Description("Dream Nail Joni (Obtain)"), ToolTip("Splits when obtaining the essence from Joni")]
        OnObtainGhostJoni,
        [Description("Dream Nail Cloth (Obtain)"), ToolTip("Splits when obtaining the essence from Cloth")]
        OnObtainGhostCloth,
        [Description("Dream Nail Vespa (Obtain)"), ToolTip("Splits when obtaining the essence from Vespa")]
        OnObtainGhostVespa,
        [Description("Dream Nail Revek (Obtain)"), ToolTip("Splits when 19 essence has been obtained in Spirits' Glade")]
        OnObtainGhostRevek,

        [Description("Wanderer's Journal (Obtain)"), ToolTip("Splits when obtaining a Wanderer's Journal")]
        OnObtainWanderersJournal,
        [Description("Hallownest Seal (Obtain)"), ToolTip("Splits when obtaining a Hallownest Seal")]
        OnObtainHallownestSeal,
        [Description("King's Idol (Obtain)"), ToolTip("Splits when obtaining a King's Idol")]
        OnObtainKingsIdol,
        [Description("Arcane Egg (Obtain)"), ToolTip("Splits when obtaining an Arcane Egg")]
        OnObtainArcaneEgg,
        [Description("Rancid Egg (Obtain)"), ToolTip("Splits when obtaining a Rancid Egg")]
        OnObtainRancidEgg,
        [Description("Mask Shard (Obtain)"), ToolTip("Splits when obtaining a Mask Shard (splits on upgrade for full mask)")]
        OnObtainMaskShard,
        [Description("Vessel Fragment (Obtain)"), ToolTip("Splits when obtaining a Vessel Fragment (splits on upgrade for full vessel)")]
        OnObtainVesselFragment,
        [Description("Simple Key (Obtain)"), ToolTip("Splits when obtaining a Simple Key")]
        OnObtainSimpleKey,
        [Description("Use Simple Key (Obtain)"), ToolTip("Splits when using a Simple Key")]
        OnUseSimpleKey,
        [Description("Grub (Obtain)"), ToolTip("Splits when obtaining a Grub")]
        OnObtainGrub,
        [Description("Pale Ore (Obtain)"), ToolTip("Splits when obtaining a Pale Ore")]
        OnObtainPaleOre,

        [Description("Any Transition (Transition)"), ToolTip("Splits when the knight enters a transition (only one will split per transition)")]
        AnyTransition,
        [Description("Transition excluding Save State (Transition)"), ToolTip("Splits when the knight enters a transition (excludes save states and Sly's basement)")]
        TransitionAfterSaveState,
        [Description("Manual Split (Misc)"), ToolTip("Never splits. Use this when you need to manually split while using ordered splits")]
        ManualSplit,




        /*
        [Description("Mage Door (Test)"), ToolTip("Splits when Nailsmith is spared")]
        MageDoor,
        [Description("Sanctum Warrior Window (Test)"), ToolTip("Splits when Nailsmith is killed")]
        MageWindow,
        [Description("Mage Lord Enc. (Test)"), ToolTip("Splits when Nailsmith is spared")]
        MageLordEncountered,
        [Description("Mage Lord 2 Enc. (Test)"), ToolTip("Splits when Nailsmith is killed")]
        MageDoor2,
        [Description("Mage Window (Test)"), ToolTip("Splits when Nailsmith is spared")]
        MageWindowGlass,
        [Description("Mage Window Glass (Test)"), ToolTip("Splits when Nailsmith is killed")]
        MageLordEncountered2,
        */



        /*
        [Description("Equipped fr. health (menu testing)"), ToolTip("Splits when equipping charm23, for timing menuing")]
        EquippedFragileHealth,
        */

    }
    public class ToolTipAttribute : Attribute {
        public string ToolTip { get; set; }
        public ToolTipAttribute(string text) {
            ToolTip = text;
        }
    }
}
