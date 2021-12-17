using System;
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
        [Description("深渊尖啸（技能）"), ToolTip("Splits when obtaining Abyss Shriek (Shadow Scream)")]
        AbyssShriek,
        [Description("水晶之心（技能）"), ToolTip("Splits when obtaining Crystal Heart (Super Dash)")]
        CrystalHeart,
        [Description("旋风劈砍（技能）"), ToolTip("Splits when obtaining Cyclone Slash (Nail Art)")]
        CycloneSlash,
        [Description("冲刺劈砍（技能）"), ToolTip("Splits when obtaining Dash Slash (Nail Art)")]
        DashSlash,
        [Description("黑暗降临（技能）"), ToolTip("Splits when obtaining Descending Dark (Shadow Dive)")]
        DescendingDark,
        [Description("荒芜俯冲（技能）"), ToolTip("Splits when obtaining Desolate Dive")]
        DesolateDive,
        [Description("梦之钉（技能）"), ToolTip("Splits when obtaining Dream Nail")]
        DreamNail,
        [Description("梦之钉-觉醒的（技能）"), ToolTip("Splits when Awkening the Dream Nail")]
        DreamNail2,
        [Description("梦之门（技能）"), ToolTip("Splits when obtaining Dream Gate")]
        DreamGate,
        [Description("强力劈砍（技能）"), ToolTip("Splits when obtaining Great Slash (Nail Art)")]
        GreatSlash,
        [Description("嚎叫幽灵（技能）"), ToolTip("Splits when obtaining Howling Wraiths (Scream)")]
        HowlingWraiths,
        [Description("伊思玛的眼泪（技能）"), ToolTip("Splits when obtaining Isma's Tear (Acid Armour)")]
        IsmasTear,
        [Description("螳螂爪（技能）"), ToolTip("Splits when obtaining Mantis Claw (Wall Jump)")]
        MantisClaw,
        [Description("帝王之翼（技能）"), ToolTip("Splits when obtaining Monarch Wings (Double Jump)")]
        MonarchWings,
        [Description("蛾翼披风（技能）"), ToolTip("Splits when obtaining Mothwing Cloak (Dash)")]
        MothwingCloak,
        [Description("暗影披风（技能）"), ToolTip("Splits when obtaining Shade Cloak (Shadow Dash)")]
        ShadeCloak,
        [Description("暗影之魂（技能）"), ToolTip("Splits when obtaining Shade Soul (Vengeful Spirit 2)")]
        ShadeSoul,
        [Description("复仇之魂（技能）"), ToolTip("Splits when obtaining Vengeful Spirit")]
        VengefulSpirit,

        [Description("城市纹章（物品）"), ToolTip("Splits when obtaining the City Crest")]
        CityKey,
        [Description("娇嫩的花（物品）"), ToolTip("Splits when flower is in inventory")]
        HasDelicateFlower,
        [Description("典雅的钥匙（物品）"), ToolTip("Splits when obtaining the Elegant Key")]
        ElegantKey,
        [Description("典雅的钥匙和面具碎片1（物品）"), ToolTip("Splits when obtaining both mask shard 1 and Elegant Key")]
        ElegantKeyShoptimised,
        [Description("神明调谐器（物品）"), ToolTip("Splits when obtaining the God Tuner")]
        GodTuner,
        [Description("猎人的印记（物品）"), ToolTip("Splits when obtaining the Hunter's Mark")]
        HuntersMark,
        [Description("王之印记（物品）"), ToolTip("Splits when obtaining the Kings Brand")]
        KingsBrand,
        [Description("爱之钥（物品）"), ToolTip("Splits when obtaining the Love Key")]
        LoveKey,
        [Description("光蝇灯笼（物品）"), ToolTip("Splits when obtaining the Lumafly Lantern")]
        LumaflyLantern,
        [Description("苍白潜伏者钥匙（物品）"), ToolTip("Splits when obtaining the Simple Key from the Pale Lurker")]
        PaleLurkerKey,
        [Description("苍白矿石-任意（物品）"), ToolTip("Splits if you've obtained any Pale Ore")]
        PaleOre,
        [Description("萨鲁巴的祝福（物品）"), ToolTip("Splits when obtaining Salubra's Blessing")]
        SalubrasBlessing,
        [Description("简单钥匙-第一把（物品）"), ToolTip("Splits when obtaining the first Simple Key")]
        SimpleKey,
        [Description("店主的钥匙（物品）"), ToolTip("Splits when obtaining the Shopkeeper's Key from the Crystal Peak key room")]
        SlyKey,
        [Description("电车票（物品）"), ToolTip("Splits when obtaining the Tram Pass")]
        TramPass,
        
        


        [Description("面具碎片1（碎片）"), ToolTip("Splits when getting 1st Mask fragment")]
        MaskFragment1,
        [Description("面具碎片2（碎片）"), ToolTip("Splits when getting 2nd Mask fragment")]
        MaskFragment2,
        [Description("面具碎片3（碎片）"), ToolTip("Splits when getting 3rd Mask fragment")]
        MaskFragment3,
        [Description("面具碎片4（升级）"), ToolTip("Splits when getting 4th Mask fragment (6 HP)")]
        Mask1,
        [Description("面具碎片5（碎片）"), ToolTip("Splits when getting 5th Mask fragment")]
        MaskFragment5,
        [Description("面具碎片6（碎片）"), ToolTip("Splits when getting 6th Mask fragment")]
        MaskFragment6,
        [Description("面具碎片7（碎片）"), ToolTip("Splits when getting 7th Mask fragment")]
        MaskFragment7,
        [Description("面具碎片8（升级）"), ToolTip("Splits when getting 8th Mask fragment (7 HP)")]
        Mask2,
        [Description("面具碎片9（碎片）"), ToolTip("Splits when getting 9th Mask fragment")]
        MaskFragment9,
        [Description("面具碎片10（碎片）"), ToolTip("Splits when getting 10th Mask fragment")]
        MaskFragment10,
        [Description("面具碎片11（碎片）"), ToolTip("Splits when getting 11th Mask fragment")]
        MaskFragment11,
        [Description("面具碎片12（升级）"), ToolTip("Splits when getting 12th Mask fragment (8 HP)")]
        Mask3,
        [Description("面具碎片13（碎片）"), ToolTip("Splits when getting 13th Mask fragment")]
        MaskFragment13,
        [Description("面具碎片14（碎片）"), ToolTip("Splits when getting 14th Mask fragment")]
        MaskFragment14,
        [Description("面具碎片15（碎片）"), ToolTip("Splits when getting 15th Mask fragment")]
        MaskFragment15,
        [Description("面具碎片16（升级）"), ToolTip("Splits when getting 16th Mask fragment (9 HP)")]
        Mask4,
        [Description("骨钉1（升级）"), ToolTip("Splits when getting Nail Upgrade 1")]
        NailUpgrade1,
        [Description("骨钉2（升级）"), ToolTip("Splits when getting Nail Upgrade 2")]
        NailUpgrade2,
        [Description("骨钉3（升级）"), ToolTip("Splits when getting Coil Nail (Upgrade 3)")]
        NailUpgrade3,
        [Description("骨钉4（升级）"), ToolTip("Splits when getting Pure Nail (Upgrade 4)")]
        NailUpgrade4,
        [Description("容器碎片1（碎片）"), ToolTip("Splits when getting 1st Vessel fragment")]
        VesselFragment1,
        [Description("容器碎片2（碎片）"), ToolTip("Splits when getting 2nd Vessel fragment")]
        VesselFragment2,
        [Description("容器碎片3（升级）"), ToolTip("Splits when getting 3rd Vessel fragment (1 extra MP)")]
        Vessel1,
        [Description("容器碎片4（碎片）"), ToolTip("Splits when getting 4th Vessel fragment")]
        VesselFragment4,
        [Description("容器碎片5（碎片）"), ToolTip("Splits when getting 5th Vessel fragment")]
        VesselFragment5,
        [Description("容器碎片6（升级）"), ToolTip("Splits when getting 6th Vessel fragment (2 extra MP)")]
        Vessel2,
        [Description("容器碎片7（碎片）"), ToolTip("Splits when getting 7th Vessel fragment")]
        VesselFragment7,
        [Description("容器碎片8（碎片）"), ToolTip("Splits when getting 8th Vessel fragment")]
        VesselFragment8,
        [Description("容器碎片9（升级）"), ToolTip("Splits when getting 9th Vessel fragment (3 extra MP)")]
        Vessel3,

        [Description("残破容器（Boss）"), ToolTip("Splits when killing Broken Vessel")]
        BrokenVessel,
        [Description("躁欲的毛里克（Boss）"), ToolTip("Splits when killing Brooding Mawlek")]
        BroodingMawlek,
        [Description("收藏家（Boss）"), ToolTip("Splits when killing Collector")]
        Collector,
        [Description("水晶守卫1（Boss）"), ToolTip("Splits when killing the Crystal Guardian for the first time")]
        CrystalGuardian1,
        [Description("水晶守卫2（Boss）"), ToolTip("Splits when killing the Crystal Guardian for the second time")]
        CrystalGuardian2,
        [Description("粪虫防御者（Boss）"), ToolTip("Splits when killing Dung Defender")]
        DungDefender,
        [Description("粪虫防御者国王神像（装饰品）"), ToolTip("Splits when picking up Dung Defender idol as the first idol")]
        DungDefenderIdol,
        [Description("胡长老（Boss）"), ToolTip("Splits when killing Elder Hu")]
        ElderHu,
        [Description("胡长老（精华）"), ToolTip("Splits when absorbing essence from Elder Hu")]
        ElderHuEssence,
        [Description("假骑士（Boss）"), ToolTip("Splits when killing False Knight")]
        FalseKnight,
        [Description("失败冠军（Boss）"), ToolTip("Splits when killing Failed Champion (False Knight Dream)")]
        FailedKnight,
        [Description("失败冠军（精华）"), ToolTip("Splits when getting Failed Champion essence (False Knight Dream)")]
        FailedChampionEssence,
        [Description("吸虫之母（Boss）"), ToolTip("Splits when killing Flukemarm")]
        Flukemarm,
        [Description("加利安（Boss）"), ToolTip("Splits when killing Galien")]
        Galien,
        [Description("加利安（精华）"), ToolTip("Splits when absorbing essence from Galien")]
        GalienEssence,
        [Description("神之驯服者（Boss）"), ToolTip("Splits when killing the God Tamer")]
        GodTamer,
        [Description("戈布（Boss）"), ToolTip("Splits when killing Gorb")]
        Gorb,
        [Description("戈布（精华）"), ToolTip("Splits when absorbing essence from Gorb")]
        GorbEssence,
        [Description("灰色王子左特（Boss）"), ToolTip("Splits when killing Grey Prince")]
        GreyPrince,
        [Description("灰色王子左特（精华）"), ToolTip("Splits when getting Grey Prince Zote essence")]
        GreyPrinceEssence,
        [Description("格鲁兹之母（Boss）"), ToolTip("Splits when killing Gruz Mother")]
        GruzMother,
        [Description("蜂巢骑士（Boss）"), ToolTip("Splits when killing Hive Knight")]
        HiveKnight,
        [Description("大黄蜂1（Boss）"), ToolTip("Splits when killing Hornet for the first time")]
        Hornet1,
        [Description("大黄蜂2（Boss）"), ToolTip("Splits when killing Hornet for the second time")]
        Hornet2,
        [Description("失落近亲（Boss）"), ToolTip("Splits when killing Lost Kin (Broken Vessel Dream)")]
        LostKin,
        [Description("失落近亲（精华）"), ToolTip("Splits when getting Lost Kin essence (Broken Vessel Dream)")]
        LostKinEssence,
        [Description("螳螂领主（Boss）"), ToolTip("Splits when killing Mantis Lords")]
        MantisLords,
        [Description("马科斯（Boss）"), ToolTip("Splits when killing Markoth")]
        Markoth,
        [Description("马科斯（精华）"), ToolTip("Splits when absorbing essence from Markoth")]
        MarkothEssence,
        [Description("马尔穆（Boss）"), ToolTip("Splits when killing Marmu")]
        Marmu,
        [Description("马尔穆（精华）"), ToolTip("Splits when absorbing essence from Marmu")]
        MarmuEssence,
        [Description("大型苔藓冲锋者（Boss）"), ToolTip("Splits when killing Mega Moss Charger")]
        MegaMossCharger,
        [Description("梦魇之王格林（Boss）"), ToolTip("Splits when killing Nightmare King Grimm")]
        NightmareKingGrimm,
        [Description("无眼（Boss）"), ToolTip("Splits when killing No Eyes")]
        NoEyes,
        [Description("无眼（精华）"), ToolTip("Splits when absorbing essence from No Eyes")]
        NoEyesEssence,
        [Description("诺斯克（Boss）"), ToolTip("Splits when killing Nosk")]
        Nosk,
        [Description("奥波路波（Boss）"), ToolTip("Splits when 2 Oblobbles are deafeated (ideally the first pair you encounter in Colo 2)")]
        KilledOblobbles,
        [Description("骨钉兄弟奥罗&马托（Boss）"), ToolTip("Splits when killing the Nail Bros (Pantheon)")]
        MatoOroNailBros,
        [Description("纯粹容器（Boss）"), ToolTip("Splits when killing Pure Vessel")]
        PureVessel,
        [Description("片段练习-辐光（Boss）"), ToolTip("Splits when killing The Radiance")]
        RadianceBoss,
        [Description("片段练习-空洞骑士（Boss）"), ToolTip("Splits when killing The Hollow Knight")]
        HollowKnightBoss,
        [Description("绘画大师席奥（Boss）"), ToolTip("Splits when killing Sheo (Pantheon)")]
        SheoPaintmaster,
        [Description("骨钉贤者斯莱（Boss）"), ToolTip("Splits when killing Nailsage Sly (Pantheon)")]
        SlyNailsage,
        [Description("灵魂大师（Boss）"), ToolTip("Splits when killing Soul Master")]
        SoulMaster,
        [Description("灵魂暴君（Boss）"), ToolTip("Splits when killing Soul Tyrant (Soul Master Dream)")]
        SoulTyrant,
        [Description("灵魂暴君（精华）"), ToolTip("Splits when getting Soul Tyrant essence (Soul Master Dream)")]
        SoulTyrantEssence,
        [Description("灵魂暴君和圣所幼虫（精华）"), ToolTip("Splits when getting Soul Tyrant essence and Sanctum fakedive grub")]
        SoulTyrantEssenceWithSanctumGrub,
        [Description("叛徒领主（Boss）"), ToolTip("Splits when killing Traitor Lord")]
        TraitorLord,
        [Description("剧团团长格林（Boss）"), ToolTip("Splits when killing Troupe Master Grimm")]
        TroupeMasterGrimm,
        [Description("乌姆（Boss）"), ToolTip("Splits when killing Uumuu")]
        Uumuu,
        [Description("守望者骑士（Boss）"), ToolTip("Splits when killing Watcher Knight")]
        BlackKnight,
        [Description("白色防御者（Boss）"), ToolTip("Splits when killing White Defender")]
        WhiteDefender,
        [Description("白色防御者（精华）"), ToolTip("Splits when getting White Defender essence")]
        WhiteDefenderEssence,
        [Description("泽若（Boss）"), ToolTip("Splits when killing Xero")]
        Xero,
        [Description("泽若（精华）"), ToolTip("Splits when absorbing essence from Xero")]
        XeroEssence,
        
        [Description("反击蝇之王（万神殿）"), ToolTip("Splits after killing Vengefly King in Pantheon 1")]
        VengeflyKingP,
        [Description("格鲁兹之母（万神殿）"), ToolTip("Splits after killing Gruz Mother in Pantheon 1")]
        GruzMotherP,
        [Description("假骑士（万神殿）"), ToolTip("Splits after killing False Knight in Pantheon 1")]
        FalseKnightP,
        [Description("大型苔藓冲锋者（万神殿）"), ToolTip("Splits after killing Massive Moss Charger in Pantheon 1")]
        MassiveMossChargerP,
        [Description("大黄蜂1（万神殿）"), ToolTip("Splits after killing Hornet 1 in Pantheon 1")]
        Hornet1P,
        [Description("戈布（万神殿）"), ToolTip("Splits after killing Gorb in Pantheon 1")]
        GorbP,
        [Description("粪虫防御者（万神殿）"), ToolTip("Splits after killing Dung Defender in Pantheon 1")]
        DungDefenderP,
        [Description("灵魂战士（万神殿）"), ToolTip("Splits after killing Soul Warrior in Pantheon 1")]
        SoulWarriorP,
        [Description("躁欲的毛里克（万神殿）"), ToolTip("Splits after killing Brooding Mawlek in Pantheon 1")]
        BroodingMawlekP,
        [Description("骨钉兄弟奥罗&马托（万神殿）"), ToolTip("Splits after killing Oro & Mato in Pantheon 1")]
        OroMatoNailBrosP,

        [Description("泽若（万神殿）"), ToolTip("Splits after killing Xero in Pantheon 2")]
        XeroP,
        [Description("水晶守卫（万神殿）"), ToolTip("Splits after killing Crystal Guardian in Pantheon 2")]
        CrystalGuardianP,
        [Description("灵魂大师（万神殿）"), ToolTip("Splits after killing Soul Master in Pantheon 2")]
        SoulMasterP,
        [Description("奥波路波（万神殿）"), ToolTip("Splits after killing Oblobbles in Pantheon 2")]
        OblobblesP,
        [Description("螳螂领主（万神殿）"), ToolTip("Splits after killing Mantis Lords in Pantheon 2")]
        MantisLordsP,
        [Description("马尔穆（万神殿）"), ToolTip("Splits after killing Marmu in Pantheon 2")]
        MarmuP,
        [Description("诺斯克（万神殿）"), ToolTip("Splits after killing Nosk in Pantheon 2")]
        NoskP,
        [Description("吸虫之母（万神殿）"), ToolTip("Splits after killing Flukemarm in Pantheon 2")]
        FlukemarmP,
        [Description("残破容器（万神殿）"), ToolTip("Splits after killing Broken Vessel in Pantheon 2")]
        BrokenVesselP,
        [Description("绘画大师席奥（万神殿）"), ToolTip("Splits after killing Sheo Paintmaster in Pantheon 2")]
        SheoPaintmasterP,

        [Description("蜂巢骑士（万神殿）"), ToolTip("Splits after killing Hive Knight in Pantheon 3")]
        HiveKnightP,
        [Description("胡长老（万神殿）"), ToolTip("Splits after killing Elder Hu in Pantheon 3")]
        ElderHuP,
        [Description("收藏家（万神殿）"), ToolTip("Splits after killing The Collector in Pantheon 3")]
        CollectorP,
        [Description("神之驯服者（万神殿）"), ToolTip("Splits after killing God Tamer in Pantheon 3")]
        GodTamerP,
        [Description("剧团团长格林（万神殿）"), ToolTip("Splits after killing Troupe Master Grimm in Pantheon 3")]
        TroupeMasterGrimmP,
        [Description("加利安（万神殿）"), ToolTip("Splits after killing Galien in Pantheon 3")]
        GalienP,
        [Description("灰色王子左特（万神殿）"), ToolTip("Splits after killing Grey Prince Zote in Pantheon 3")]
        GreyPrinceZoteP,
        [Description("乌姆（万神殿）"), ToolTip("Splits after killing Uumuu in Pantheon 3")]
        UumuuP,
        [Description("大黄蜂2（万神殿）"), ToolTip("Splits after killing Hornet 2 in Pantheon 3")]
        Hornet2P,
        [Description("骨钉贤者斯莱（万神殿）"), ToolTip("Splits after killing Sly Nailsage in Pantheon 3")]
        SlyP,

        [Description("暴怒守卫（万神殿）"), ToolTip("Splits after killing Enraged Guardian in Pantheon 4")]
        EnragedGuardianP,
        [Description("失落近亲（万神殿）"), ToolTip("Splits after killing Lost Kin in Pantheon 4")]
        LostKinP,
        [Description("无眼（万神殿）"), ToolTip("Splits after killing No Eyes in Pantheon 4")]
        NoEyesP,
        [Description("叛徒领主（万神殿）"), ToolTip("Splits after killing Traitor Lord in Pantheon 4")]
        TraitorLordP,
        [Description("白色防御者（万神殿）"), ToolTip("Splits after killing White Defender in Pantheon 4")]
        WhiteDefenderP,
        [Description("失败冠军（万神殿）"), ToolTip("Splits after killing Failed Champion in Pantheon 4")]
        FailedChampionP,
        [Description("马科斯（万神殿）"), ToolTip("Splits after killing Markoth in Pantheon 4")]
        MarkothP,
        [Description("守望者骑士（万神殿）"), ToolTip("Splits after killing Watcher Knights in Pantheon 4")]
        WatcherKnightsP,
        [Description("灵魂暴君（万神殿）"), ToolTip("Splits after killing Soul Tyrant in Pantheon 4")]
        SoulTyrantP,
        [Description("纯粹容器（万神殿）"), ToolTip("Splits after killing Pure Vessel in Pantheon 4")]
        PureVesselP,

        [Description("诺斯克大黄蜂（万神殿）"), ToolTip("Splits after killing Nosk Hornet in Pantheon 5")]
        NoskHornetP,
        [Description("梦魇之王格林（万神殿）"), ToolTip("Splits after killing Nightmare King Grimm in Pantheon 5")]
        NightmareKingGrimmP,

        [Description("野兽赫拉（守梦者）"), ToolTip("Splits when you see the mask for Herrah (In Spider Area)")]
        Hegemol,
        [Description("守望者卢瑞恩（守梦者）"), ToolTip("Splits when you see the mask for Lurien (After killing Watcher Knight)")]
        Lurien,
        [Description("教师莫诺蒙（守梦者）"), ToolTip("Splits when you see the mask for Monomon (After killing Uumuu)")]
        Monomon,
        [Description("第一位守梦者（守梦者）"), ToolTip("Splits when you see the mask for the first dreamer killed")]
        Dreamer1,
        [Description("第二位守梦者（守梦者）"), ToolTip("Splits when you see the mask for the second dreamer killed")]
        Dreamer2,
        [Description("第三位守梦者（守梦者）"), ToolTip("Splits when you see the mask for the third dreamer killed")]
        Dreamer3,

        [Description("106%格林前购物（事件）"), ToolTip("Splits when Lantern + Vessel Fragment(5) + Mask Shard(4) have been acquired")]
        PreGrimmShop,
        [Description("1xx%斯莱最终购物（事件）"), ToolTip("Splits on leaving Sly's shop after having bough Sprintmaster and Vessel Frag 8")]
        SlyShopFinished,
        [Description("解锁过载（事件）"), ToolTip("Splits when overcharming is enabled")]
        CanOvercharm,
        [Description("锁链劈断-空洞骑士（事件）"), ToolTip("Splits at the end of the first Hollow Knight scream after the chains are broken")]
        UnchainedHollowKnight,
        [Description("吊灯-守望者骑士（事件）"), ToolTip("Splits when dropping the chandelier on one of the watcher knights")]
        WatcherChandelier,
        [Description("泪水之城大门（事件）"), ToolTip("Splits when using the City Key to open the gate")] 
        CityGateOpen,
        [Description("死亡（事件）"), ToolTip("Splits when player hp is 0")]
        PlayerDeath,
        [Description("结束（全成就速通重开用）"), ToolTip("Splits on any credits rolling")]
        EndingSplit,
        [Description("送花任务（事件）"), ToolTip("Splits when placing the flower at the grave of the Traitors' Child")]
        FlowerQuest,
        [Description("送花任务奖励（事件）"), ToolTip("Splits when Grey Mourner gives you the Flower Quest reward")]
        FlowerRewardGiven,
        [Description("幸福的夫妇（事件）"), ToolTip("Splits when talking to Nailsmith in Sheo's hut for the first time")]
        HappyCouplePlayerDataEvent,
        [Description("骨钉匠-击杀（事件）"), ToolTip("Splits when Nailsmith is killed")]
        NailsmithKilled,
        [Description("梦魇火炬点亮（事件）"), ToolTip("Splits when initially lighting the Nightmare Lantern")]
        NightmareLantern,
        [Description("梦魇火炬破坏（事件）"), ToolTip("Splits when destroying the Nightmare Lantern")]
        NightmareLanternDestroyed,
        [Description("辐光梦境进入（事件）"), ToolTip("Splits when going into the dream world for Hollow Knight to fight Radiance")]
        HollowKnightDreamnail,
        [Description("先知升天（事件）"), ToolTip("Splits when the Seer Departs after bringing back 2400 essence")]
        SeerDeparts,
        [Description("灵魂沼地大门（事件）"), ToolTip("Splits when the Seer open Spirit Glade after bringing back 200 essence")]
        SpiritGladeOpen,
        [Description("陷阱椅子-野兽巢穴（事件）"), ToolTip("Splits when getting the trap bench in Beasts Den")]
        BeastsDenTrapBench,
        [Description("无尽折磨解锁（事件）"), ToolTip("Splits when breaking the wall to the Zote statue in Godhome")]
        EternalOrdealUnlocked,
        [Description("无尽折磨完成（事件）"), ToolTip("Splits when achieving the ordeal (57th Zote killed)")]
        EternalOrdealAchieved,

        [Description("斗兽场解锁1（试炼）"), ToolTip("Splits when the knight unlocks the trial at Little Fool")]
        ColosseumBronzeUnlocked,
        [Description("斗兽场解锁2（试炼）"), ToolTip("Splits when the knight unlocks the trial at Little Fool")]
        ColosseumSilverUnlocked,
        [Description("斗兽场解锁3（试炼）"), ToolTip("Splits when the knight unlocks the trial at Little Fool")]
        ColosseumGoldUnlocked,
        [Description("斗兽场完成1（试炼）"), ToolTip("Splits when beating the first Colosseum trial")]
        ColosseumBronze,
        [Description("斗兽场完成2（试炼）"), ToolTip("Splits when beating the second Colosseum trial")]
        ColosseumSilver,
        [Description("斗兽场完成3（试炼）"), ToolTip("Splits when beating the third Colosseum trial")]
        ColosseumGold,
        [Description("斗兽场离开1（切换场景）"), ToolTip("Splits on the transition out of the trial, or in the load-in after quitout")]
        ColosseumBronzeExit,
        [Description("斗兽场离开2（切换场景）"), ToolTip("Splits on the transition out of the trial, or in the load-in after quitout")]
        ColosseumSilverExit,
        [Description("斗兽场离开3（切换场景）"), ToolTip("Splits on the transition out of the trial, or in the load-in after quitout")]
        ColosseumGoldExit,
        [Description("万神殿1（试炼）"), ToolTip("Splits when beating the first Pantheon")]
        Pantheon1,
        [Description("万神殿2（试炼）"), ToolTip("Splits when beating the second Pantheon")]
        Pantheon2,
        [Description("万神殿3（试炼）"), ToolTip("Splits when beating the third Pantheon")]
        Pantheon3,
        [Description("万神殿4（试炼）"), ToolTip("Splits when beating the fourth Pantheon")]
        Pantheon4,
        [Description("万神殿5（试炼）"), ToolTip("Splits when beating the fifth Pantheon")]
        Pantheon5,
        [Description("苦痛之路完成"), ToolTip("Splits when completing the Path of Pain section in White Palace")]
        PathOfPain,

        [Description("阿司匹德猎手（遭遇战）"), ToolTip("Splits when killing the final Aspid Hunter")]
        AspidHunter,
        [Description("阿鲁巴（击杀）"), ToolTip("Splits when killing an Aluba")]
        Aluba,
        //[Description("Al2ba（击杀）"), ToolTip("Splits when killing two Alubas")]
        //Al2ba,
        [Description("米拉（击杀）"), ToolTip("Splits when killing a Husk Miner")]
        HuskMiner,
        [Description("大跳虫（击杀）"), ToolTip("Splits when killing a Great Hopper")]
        GreatHopper,
        [Description("华丽躯壳（击杀）"), ToolTip("Splits when killing Gorgeous Husk")]
        GorgeousHusk,
        [Description("维修虫（击杀）"), ToolTip("Splits when killing Menderbug")]
        MenderBug,
        [Description("圣所战士（击杀）"), ToolTip("Splits on first Sanctum Warrior kill")]
        killedSanctumWarrior,
        [Description("灵魂扭曲者（击杀）"), ToolTip("Splits on first Soul Twister kill")]
        killedSoulTwister,
        //[Description("Revek（击杀）"), ToolTip("Splits when talking to Revek after clearing all other Glade ghosts")]
        //Revek,
        [Description("苔藓骑士（遭遇战）"), ToolTip("Splits when killing Moss Knight")]
        MossKnight,
        [Description("拜年菇（遭遇战）"), ToolTip("Splits when killing the final Shrumal Ogre")]
        MushroomBrawler,
        [Description("左特营救反击蝇之王（遭遇战）"), ToolTip("Splits when rescuing Zote from the Vengefly King")]
        Zote1,
        [Description("左特营救深邃巢穴（遭遇战）"), ToolTip("Splits when rescuing Zote in Deepnest")]
        Zote2,
        [Description("左特击杀斗兽场（遭遇战）"), ToolTip("Splits when killing Zote in the Colosseum")]
        ZoteKilled,
        
        [Description("遗忘的十字路（鹿角站）"), ToolTip("Splits when opening the Forgotten Crossroads Stag Station")]
        CrossroadsStation,
        [Description("苍绿之径（鹿角站）"), ToolTip("Splits when obtaining Greenpath Stag Station")]
        GreenpathStation,
        [Description("王后驿站（鹿角站）"), ToolTip("Splits when obtaining Queens Station Stag Station")]
        QueensStationStation,
        [Description("王后花园（鹿角站）"), ToolTip("Splits when obtaining Queens Gardens Stag Station")]
        QueensGardensStation,
        [Description("城市仓库（鹿角站）"), ToolTip("Splits when obtaining City Storerooms Stag Station")]
        StoreroomsStation,
        [Description("国王驿站（鹿角站）"), ToolTip("Splits when obtaining Kings Station Stag Station")]
        KingsStationStation,
        [Description("安息之地（鹿角站）"), ToolTip("Splits when obtaining Resting Grounds Stag Station")]
        RestingGroundsStation,
        [Description("遥远的村庄（鹿角站）"), ToolTip("Splits when obtaining Distant Village Stag Station")]
        DeepnestStation,
        [Description("隐藏的鹿角站（鹿角站）"), ToolTip("Splits when obtaining to Hidden Station Stag Station")]
        HiddenStationStation,
        [Description("鹿角虫巢穴（鹿角站）"), ToolTip("Splits when traveling to Stagnest (Requires Ordered Splits)")]
        StagnestStation,


        [Description("蘑菇先生1（地点）"), ToolTip("Splits when talking to Mr. Mushroom")]
        MrMushroom1,
        [Description("蘑菇先生2（地点）"), ToolTip("Splits when talking to Mr. Mushroom")]
        MrMushroom2,
        [Description("蘑菇先生3（地点）"), ToolTip("Splits when talking to Mr. Mushroom")]
        MrMushroom3,
        [Description("蘑菇先生4（地点）"), ToolTip("Splits when talking to Mr. Mushroom")]
        MrMushroom4,
        [Description("蘑菇先生5（地点）"), ToolTip("Splits when talking to Mr. Mushroom")]
        MrMushroom5,
        [Description("蘑菇先生6（地点）"), ToolTip("Splits when talking to Mr. Mushroom")]
        MrMushroom6,
        [Description("蘑菇先生7（地点）"), ToolTip("Splits when talking to Mr. Mushroom")]
        MrMushroom7,

        [Description("古老盆地（出现标题文字）"), ToolTip("Splits when entering Ancient Basin text first appears")]
        Abyss,
        [Description("泪水之城（出现标题文字）"), ToolTip("Splits when entering City Of Tears text first appears")]
        CityOfTears,
        [Description("斗兽场（出现标题文字）"), ToolTip("Splits when entering Colosseum text first appears")]
        Colosseum,
        [Description("水晶山峰（出现标题文字）"), ToolTip("Splits when entering Crystal Peak text first appears")]
        CrystalPeak,
        [Description("深邃巢穴（出现标题文字）"), ToolTip("Splits when entering Deepnest text first appears")]
        Deepnest,
        [Description("深邃巢穴温泉（区域）"), ToolTip("Splits when entering the Deepnest Spa area with bench")]
        DeepnestSpa,
        [Description("德特茅斯（出现标题文字）"), ToolTip("Splits when entering Dirtmouth text first appears")]
        Dirtmouth,
        [Description("雾之峡谷（出现标题文字）"), ToolTip("Splits when entering Fog Canyon text first appears")]
        FogCanyon,
        [Description("遗忘的十字路（出现标题文字）"), ToolTip("Splits when entering Forgotten Crossroads text first appears")]
        ForgottenCrossroads,
        [Description("真菌荒地（出现标题文字）"), ToolTip("Splits when entering Fungal Wastes text first appears")]
        FungalWastes,
        [Description("神居（出现标题文字）"), ToolTip("Splits when entering Godhome text first appears")]
        Godhome,
        [Description("苍绿之径（出现标题文字）"), ToolTip("Splits when entering Greenpath text first appears")]
        Greenpath,
        [Description("蜂巢（出现标题文字）"), ToolTip("Splits when entering Hive text first appears")]
        Hive,
        [Description("感染的十字路（出现标题文字）"), ToolTip("Splits when entering Infected Crossroads text first appears")]
        InfectedCrossroads,
        [Description("王国边境（出现标题文字）"), ToolTip("Splits when entering Kingdom's Edge text first appears")]
        KingdomsEdge,
        [Description("王后花园（出现标题文字）"), ToolTip("Splits when entering Queen's Gardens text first appears")]
        QueensGardens,
        [Description("安息之地（出现标题文字）"), ToolTip("Splits when entering Resting Grounds text first appears")]
        RestingGrounds,
        [Description("皇家水道（出现标题文字）"), ToolTip("Splits when entering Royal Waterways text first appears")]
        RoyalWaterways,
        [Description("教师档案馆（区域）"), ToolTip("Splits when entering Teachers Archive for the first time")]
        TeachersArchive,
        [Description("白色宫殿（区域）"), ToolTip("Splits when entering White Palace text for the first time")]
        WhitePalace,
        [Description("白色宫殿-工作室（区域）"), ToolTip("Splits when visiting the secret room in White Palace")] 
        WhitePalaceSecretRoom,

        [Description("古老盆地（切换场景）"), ToolTip("Splits on transition to Basin, alternative to the (Area) split")]
        BasinEntry,
        [Description("蓝湖（切换场景）"), ToolTip("Splits on transition to Blue Lake from Gruz Mother scene (requires Ordered Splits)")]
        BlueLake,
        [Description("水晶山峰（切换场景）"), ToolTip("Splits on transition to the room where the dive and toll entrances meet")]
        CrystalPeakEntry,
        //[Description("水晶Mound离开（切换场景）"), ToolTip("Splits on transition from Crystal Mound")]
        //CrystalMoundExit,
        [Description("进入任意梦境（切换场景）"), ToolTip("Splits when entering any dream world")]
        EnterAnyDream,
        [Description("雾之峡谷（切换场景）"), ToolTip("Splits on transition to Fog Canyon")]
        FogCanyonEntry,
        [Description("华丽躯壳击杀（切换场景）"), ToolTip("Splits on transition after Gorgeous Husk defeated")]
        TransGorgeousHusk,
        [Description("苍绿之径（切换场景）"), ToolTip("Splits when entering Greenpath")]
        EnterGreenpath,
        [Description("过载进入苍绿之径（切换场景）"), ToolTip("Splits when entering Greenpath with overcharming unlocked")]
        EnterGreenpathWithOvercharm,
        [Description("拥有螳螂爪（切换场景）"), ToolTip("Splits on transition after Mantis Claw acquired")]
        TransClaw,
        [Description("拥有黑暗降临（切换场景）"), ToolTip("Splits on transition after Descending Dark acquired")]
        TransDescendingDark,
        [Description("蜂巢（切换场景）"), ToolTip("Splits on transition to Hive")]
        HiveEntry,
        [Description("国王山道（切换场景）"), ToolTip("Splits when leaving King's pass")]
        KingsPass,
        [Description("国王山道和德特茅斯（切换场景）"), ToolTip("Splits on transition between Dirtmouth and King's Pass")]
        KingsPassEnterFromTown,
        [Description("王国边境（切换场景）"), ToolTip("Splits on transition to Kingdom's Edge from Kings Station")]
        KingdomsEdgeEntry,
        [Description("王国边境过载（切换场景）"), ToolTip("Splits on transition to Kingdom's Edge from Kings Station while overcharmed")]
        KingdomsEdgeOvercharmedEntry,
        [Description("梦魇之王格林梦境（切换场景）"), ToolTip("Splits on transition to NKG dream")]
        EnterNKG,
        [Description("王后花园-酸冲（切换场景）"), ToolTip("Splits on transition to QG scene following QGA")]
        QueensGardensEntry,
        [Description("王后花园-雾谷（切换场景）"), ToolTip("Splits on transition to QG frogs scene")]
        QueensGardensFrogsTrans,
        [Description("王后花园-左上遭遇战（切换场景）"), ToolTip("Splits on transition to room after upper arena in QG. Useful for TE, 1xx, and other categories that go to upper QG")]
        QueensGardensPostArenaTransition,
        [Description("圣所（切换场景）"), ToolTip("Splits when entering Sanctum")]
        EnterSanctum,
        [Description("暗影之魂后进入圣所（切换场景）"), ToolTip("Splits when entering Sanctum after obtaining shade soul")]
        EnterSanctumWithShadeSoul,
        [Description("皇家水道井（切换场景）"), ToolTip("Splits on transition to Waterways through Waterways")]
        WaterwaysEntry,
        [Description("白色宫殿入口（切换场景）"), ToolTip("Splits when entering the first White Palace scene")]
        WhitePalaceEntry,

        [Description("巴德尔之壳（护符）"), ToolTip("Splits when obtaining the Baldur Shell charm")]
        BaldurShell,
        [Description("冲刺大师（护符）"), ToolTip("Splits when obtaining the Dashmaster charm")]
        Dashmaster,
        [Description("深度聚集（护符）"), ToolTip("Splits when obtaining the Deep Focus charm")]
        DeepFocus,
        [Description("防御者纹章（护符）"), ToolTip("Splits when obtaining the Defenders Crest charm")]
        DefendersCrest,
        [Description("梦之盾（护符）"), ToolTip("Splits when obtaining the Dreamshield charm")]
        Dreamshield,
        [Description("舞梦者（护符）"), ToolTip("Splits when obtaining the Dream Wielder charm")]
        DreamWielder,
        [Description("吸虫之巢（护符）"), ToolTip("Splits when obtaining the Flukenest charm")]
        Flukenest,
        [Description("易碎贪婪（护符）"), ToolTip("Splits when obtaining the Fragile Greed charm")]
        FragileGreed,
        [Description("易碎心脏（护符）"), ToolTip("Splits when obtaining the Fragile Heart charm")]
        FragileHeart,
        [Description("易碎力量（护符）"), ToolTip("Splits when obtaining the Fragile Strength charm")]
        FragileStrength,
        [Description("亡者之怒（护符）"), ToolTip("Splits when obtaining the Fury of the Fallen charm")]
        FuryOfTheFallen,
        [Description("采集虫群（护符）"), ToolTip("Splits when obtaining the Gathering Swarm charm")]
        GatheringSwarm,
        [Description("发光子宫（护符）"), ToolTip("Splits when obtaining the Glowing Womb charm")]
        GlowingWomb,
        [Description("格林之子（护符）"), ToolTip("Splits when obtaining the Grimmchild charm")]
        Grimmchild,
        [Description("格林之子等级2（护符）"), ToolTip("Splits when obtaining the Grimmchild Lvl 2 charm")]
        Grimmchild2,
        [Description("格林之子等级3（护符）"), ToolTip("Splits when obtaining the Grimmchild Lvl 3 charm")]
        Grimmchild3,
        [Description("格林之子等级4（护符）"), ToolTip("Splits when obtaining the Grimmchild Lvl 4 charm")]
        Grimmchild4,
        [Description("蜕变挽歌（护符）"), ToolTip("Splits when obtaining the Grubberfly's Elegy charm")]
        GrubberflysElegy,
        [Description("幼虫之歌（护符）"), ToolTip("Splits when obtaining the Grubsong charm")]
        Grubsong,
        [Description("沉重之击（护符）"), ToolTip("Splits when obtaining the Heavy Blow charm")]
        HeavyBlow,
        [Description("蜂巢之血（护符）"), ToolTip("Splits when obtaining the Hiveblood charm")]
        Hiveblood,
        [Description("乔尼的祝福（护符）"), ToolTip("Splits when obtaining the Joni's Blessing charm")]
        JonisBlessing,
        [Description("国王之魂碎片-王后（护符）"), ToolTip("Splits on picking up the left Kingsoul White Fragment")] 
        WhiteFragmentLeft,
        [Description("国王之魂碎片-国王（护符）"), ToolTip("Splits on picking up the right Kingsoul White Fragment")] 
        WhiteFragmentRight,
        [Description("国王之魂（护符）"), ToolTip("Splits when obtaining the completed Kingsoul charm")]
        Kingsoul,
        [Description("生命血核心（护符）"), ToolTip("Splits when obtaining the Lifeblood Core charm")]
        LifebloodCore,
        [Description("生命血之心（护符）"), ToolTip("Splits when obtaining the Lifeblood Heart charm")]
        LifebloodHeart,
        [Description("修长之钉（护符）"), ToolTip("Splits when obtaining the Longnail charm")]
        Longnail,
        [Description("骄傲印记（护符）"), ToolTip("Splits when obtaining the Mark Of Pride charm")]
        MarkOfPride,
        [Description("骨钉大师的荣耀（护符）"), ToolTip("Splits when obtaining the Nailmaster's Glory charm")]
        NailmastersGlory,
        [Description("快速聚集（护符）"), ToolTip("Splits when obtaining the Quick Focus charm")]
        QuickFocus,
        [Description("快速劈砍（护符）"), ToolTip("Splits when obtaining the Quick Slash charm")]
        QuickSlash,
        [Description("萨满之石（护符）"), ToolTip("Splits when obtaining Shaman Stone charm")]
        ShamanStone,
        [Description("乌恩之形（护符）"), ToolTip("Splits when obtaining Shape of Unn charm")]
        ShapeOfUnn,
        [Description("锋利之影（护符）"), ToolTip("Splits when obtaining Sharp Shadow charm")]
        SharpShadow,
        [Description("灵魂捕手（护符）"), ToolTip("Splits when obtaining the Soul Catcher charm")]
        SoulCatcher,
        [Description("噬魂者（护符）"), ToolTip("Splits when obtaining the Soul Eater charm")]
        SoulEater,
        [Description("法术扭曲者（护符）"), ToolTip("Splits when obtaining the Spell Twister charm")]
        SpellTwister,
        [Description("蘑菇孢子（护符）"), ToolTip("Splits when obtaining the Spore Shroom charm")]
        SporeShroom,
        [Description("飞毛腿（护符）"), ToolTip("Splits when obtaining the Sprintmaster charm")]
        Sprintmaster,
        [Description("坚硬外壳（护符）"), ToolTip("Splits when obtaining Stalwart Shell charm")]
        StalwartShell,
        [Description("稳定之体（护符）"), ToolTip("Splits when obtaining the Steady Body charm")]
        SteadyBody,
        [Description("苦痛荆棘（护符）"), ToolTip("Splits when obtaining Thorns of Agony charm")]
        ThornsOfAgony,
        [Description("坚固贪婪（护符）"), ToolTip("Splits when obtaining the Unbreakable Greed charm")]
        UnbreakableGreed,
        [Description("坚固心脏（护符）"), ToolTip("Splits when obtaining the Unbreakable Heart charm")]
        UnbreakableHeart,
        [Description("坚固力量（护符）"), ToolTip("Splits when obtaining the Unbreakable Strength charm")]
        UnbreakableStrength,
        [Description("虚空之心（护符）"), ToolTip("Splits when changing the Kingsoul to the Void Heart charm")]
        VoidHeart,
        [Description("任性的指南针（护符）"), ToolTip("Splits when obtaining Wayward Compass charm")]
        WaywardCompass,
        [Description("编织者之歌（护符）"), ToolTip("Splits when obtaining the Weaversong charm")]
        Weaversong,
        [Description("拜年菇（护符槽）"), ToolTip("Splits when obtaining the charm notch after defeating the Shrumal Ogres")]
        NotchShrumalOgres,
        [Description("雾之峡谷（护符槽）"), ToolTip("Splits when obtaining the charm notch in Fog Canyon")]
        NotchFogCanyon,
        [Description("格林（护符槽）"), ToolTip("Splits when obtaining the charm notch after Grimm")]
        NotchGrimm,
        [Description("萨鲁巴1（护符槽）"), ToolTip("Splits when obtaining the first charm notch from Salubra")]
        NotchSalubra1,
        [Description("萨鲁巴2（护符槽）"), ToolTip("Splits when obtaining the second charm notch from Salubra")]
        NotchSalubra2,
        [Description("萨鲁巴3（护符槽）"), ToolTip("Splits when obtaining the third charm notch from Salubra")]
        NotchSalubra3,
        [Description("萨鲁巴4（护符槽）"), ToolTip("Splits when obtaining the fourth charm notch from Salubra")]
        NotchSalubra4,

        [Description("里姆商店对话（NPC）"), ToolTip("Splits when talking to Lemm in the shop for the first time")]
        Lemm2,
        [Description("里姆全护符槽速通（事件）"), ToolTip("Splits on having sold a total: 1 journal, 6 seals, and 4 idols to Lemm")]
        AllCharmNotchesLemm2CP,
        [Description("见到灰色哀悼者（NPC）"), ToolTip("Splits when talking to Grey Mourner for the first time")]
        MetGreyMourner,
        [Description("先知升天后对话灰色哀悼者（NPC）"), ToolTip("Splits when both talked to Grey Mourner and Seer has ascended")]
        GreyMournerSeerAscended,
        [Description("虫长者送花（NPC）"), ToolTip("Splits when giving the flower to the Elderbug")]
        ElderbugFlower,
        [Description("寻神者送花（NPC）"), ToolTip("Splits when giving Godseeker a flower")]
        givenGodseekerFlower,
        [Description("奥罗送花（NPC）"), ToolTip("Splits when giving Oro a flower")]
        givenOroFlower,
        [Description("白色夫人送花（NPC）"), ToolTip("Splits when giving White Lady a flower")]
        givenWhiteLadyFlower,
        [Description("艾米丽塔送花（NPC）"), ToolTip("Splits when giving Emilita a flower")]
        givenEmilitiaFlower,
        [Description("布蕾塔营救（NPC）"), ToolTip("Splits when saving Bretta")]
        BrettaRescued,
        [Description("布鲁姆火焰（NPC）"), ToolTip("Splits when collecting Brumm's flame in Deepnest")]
        BrummFlame,
        [Description("小愚人（NPC）"), ToolTip("Splits when talking to the Little Fool for the first time")]
        LittleFool,
        [Description("斯莱营救（NPC）"), ToolTip("Splits when saving Sly")]
        SlyRescued,

        [Description("格林火焰1（火焰）"), ToolTip("Splits after obtaining the first flame.")]
        Flame1,
        [Description("格林火焰2（火焰）"), ToolTip("Splits after obtaining the second flame.")]
        Flame2,
        [Description("格林火焰3（火焰）"), ToolTip("Splits after obtaining the third flame.")]
        Flame3,

        [Description("苍白矿石1（矿石）"), ToolTip("Splits after obtaining the first pale ore.")]
        Ore1,
        [Description("苍白矿石2（矿石）"), ToolTip("Splits after obtaining the second pale ore.")]
        Ore2,
        [Description("苍白矿石3（矿石）"), ToolTip("Splits after obtaining the third pale ore.")]
        Ore3,
        [Description("苍白矿石4（矿石）"), ToolTip("Splits after obtaining the fourth pale ore.")]
        Ore4,
        [Description("苍白矿石5（矿石）"), ToolTip("Splits after obtaining the fifth pale ore.")]
        Ore5,
        [Description("苍白矿石6（矿石）"), ToolTip("Splits after obtaining the sixth pale ore.")]
        Ore6,

        [Description("营救幼虫1（幼虫）"), ToolTip("Splits when rescuing grub #1")]
        Grub1,
        [Description("营救幼虫2（幼虫）"), ToolTip("Splits when rescuing grub #2")]
        Grub2,
        [Description("营救幼虫3（幼虫）"), ToolTip("Splits when rescuing grub #3")]
        Grub3,
        [Description("营救幼虫4（幼虫）"), ToolTip("Splits when rescuing grub #4")]
        Grub4,
        [Description("营救幼虫5（幼虫）"), ToolTip("Splits when rescuing grub #5")]
        Grub5,
        [Description("营救幼虫6（幼虫）"), ToolTip("Splits when rescuing grub #6")]
        Grub6,
        [Description("营救幼虫7（幼虫）"), ToolTip("Splits when rescuing grub #7")]
        Grub7,
        [Description("营救幼虫8（幼虫）"), ToolTip("Splits when rescuing grub #8")]
        Grub8,
        [Description("营救幼虫9（幼虫）"), ToolTip("Splits when rescuing grub #9")]
        Grub9,
        [Description("营救幼虫10（幼虫）"), ToolTip("Splits when rescuing grub #10")]
        Grub10,
        [Description("营救幼虫11（幼虫）"), ToolTip("Splits when rescuing grub #11")]
        Grub11,
        [Description("营救幼虫12（幼虫）"), ToolTip("Splits when rescuing grub #12")]
        Grub12,
        [Description("营救幼虫13（幼虫）"), ToolTip("Splits when rescuing grub #13")]
        Grub13,
        [Description("营救幼虫14（幼虫）"), ToolTip("Splits when rescuing grub #14")]
        Grub14,
        [Description("营救幼虫15（幼虫）"), ToolTip("Splits when rescuing grub #15")]
        Grub15,
        [Description("营救幼虫16（幼虫）"), ToolTip("Splits when rescuing grub #16")]
        Grub16,
        [Description("营救幼虫17（幼虫）"), ToolTip("Splits when rescuing grub #17")]
        Grub17,
        [Description("营救幼虫18（幼虫）"), ToolTip("Splits when rescuing grub #18")]
        Grub18,
        [Description("营救幼虫19（幼虫）"), ToolTip("Splits when rescuing grub #19")]
        Grub19,
        [Description("营救幼虫20（幼虫）"), ToolTip("Splits when rescuing grub #20")]
        Grub20,
        [Description("营救幼虫21（幼虫）"), ToolTip("Splits when rescuing grub #21")]
        Grub21,
        [Description("营救幼虫22（幼虫）"), ToolTip("Splits when rescuing grub #22")]
        Grub22,
        [Description("营救幼虫23（幼虫）"), ToolTip("Splits when rescuing grub #23")]
        Grub23,
        [Description("营救幼虫24（幼虫）"), ToolTip("Splits when rescuing grub #24")]
        Grub24,
        [Description("营救幼虫25（幼虫）"), ToolTip("Splits when rescuing grub #25")]
        Grub25,
        [Description("营救幼虫26（幼虫）"), ToolTip("Splits when rescuing grub #26")]
        Grub26,
        [Description("营救幼虫27（幼虫）"), ToolTip("Splits when rescuing grub #27")]
        Grub27,
        [Description("营救幼虫28（幼虫）"), ToolTip("Splits when rescuing grub #28")]
        Grub28,
        [Description("营救幼虫29（幼虫）"), ToolTip("Splits when rescuing grub #29")]
        Grub29,
        [Description("营救幼虫30（幼虫）"), ToolTip("Splits when rescuing grub #30")]
        Grub30,
        [Description("营救幼虫31（幼虫）"), ToolTip("Splits when rescuing grub #31")]
        Grub31,
        [Description("营救幼虫32（幼虫）"), ToolTip("Splits when rescuing grub #32")]
        Grub32,
        [Description("营救幼虫33（幼虫）"), ToolTip("Splits when rescuing grub #33")]
        Grub33,
        [Description("营救幼虫34（幼虫）"), ToolTip("Splits when rescuing grub #34")]
        Grub34,
        [Description("营救幼虫35（幼虫）"), ToolTip("Splits when rescuing grub #35")]
        Grub35,
        [Description("营救幼虫36（幼虫）"), ToolTip("Splits when rescuing grub #36")]
        Grub36,
        [Description("营救幼虫37（幼虫）"), ToolTip("Splits when rescuing grub #37")]
        Grub37,
        [Description("营救幼虫38（幼虫）"), ToolTip("Splits when rescuing grub #38")]
        Grub38,
        [Description("营救幼虫39（幼虫）"), ToolTip("Splits when rescuing grub #39")]
        Grub39,
        [Description("营救幼虫40（幼虫）"), ToolTip("Splits when rescuing grub #40")]
        Grub40,
        [Description("营救幼虫41（幼虫）"), ToolTip("Splits when rescuing grub #41")]
        Grub41,
        [Description("营救幼虫42（幼虫）"), ToolTip("Splits when rescuing grub #42")]
        Grub42,
        [Description("营救幼虫43（幼虫）"), ToolTip("Splits when rescuing grub #43")]
        Grub43,
        [Description("营救幼虫44（幼虫）"), ToolTip("Splits when rescuing grub #44")]
        Grub44,
        [Description("营救幼虫45（幼虫）"), ToolTip("Splits when rescuing grub #45")]
        Grub45,
        [Description("营救幼虫46（幼虫）"), ToolTip("Splits when rescuing grub #46")]
        Grub46,

        [Description("营救幼虫盆地下砸（幼虫）"), ToolTip("Splits when rescuing the grub in Abyss_17")]
        GrubBasinDive,
        [Description("营救幼虫盆地二段跳（幼虫）"), ToolTip("Splits when rescuing the grub in Abyss_19")]
        GrubBasinWings,
        [Description("营救幼虫爱之塔下（幼虫）"), ToolTip("Splits when rescuing the grub in Ruins2_07")]
        GrubCityBelowLoveTower,
        [Description("营救幼虫圣所下（幼虫）"), ToolTip("Splits when rescuing the grub in Ruins1_05")]
        GrubCityBelowSanctum,
        [Description("营救幼虫收藏家（幼虫）"), ToolTip("Splits when rescuing all three grubs in Ruins2_11. (On 1221, splits for right grub)")]
        GrubCityCollectorAll,
        [Description("营救幼虫泪城守卫房间（幼虫）"), ToolTip("Splits when rescuing the grub in Ruins_House_01")]
        GrubCityGuardHouse,
        [Description("营救幼虫圣所（幼虫）"), ToolTip("Splits when rescuing the grub in Ruins1_32")]
        GrubCitySanctum,
        [Description("营救幼虫守望者的尖塔（幼虫）"), ToolTip("Splits when rescuing the grub in Ruins2_03")]
        GrubCitySpire,
        [Description("营救幼虫巴德尔之壳（幼虫）"), ToolTip("Splits when rescuing the grub in Fungus1_28")]
        GrubCliffsBaldurShell,
        [Description("营救幼虫十字路酸水（幼虫）"), ToolTip("Splits when rescuing the grub in Crossroads_35")]
        GrubCrossroadsAcid,
        [Description("营救幼虫十字路龙牙哥（幼虫）"), ToolTip("Splits when rescuing the grub in Crossroads_48")]
        GrubCrossroadsGuarded,
        [Description("营救幼虫十字路尖刺（幼虫）"), ToolTip("Splits when rescuing the grub in Crossroads_31")]
        GrubCrossroadsSpikes,
        [Description("营救幼虫十字路反击蝇（幼虫）"), ToolTip("Splits when rescuing the grub in Crossroads_05")]
        GrubCrossroadsVengefly,
        [Description("营救幼虫十字路隐藏墙（幼虫）"), ToolTip("Splits when rescuing the grub in Crossroads_03")]
        GrubCrossroadsWall,
        [Description("营救幼虫水晶山峰下层（幼虫）"), ToolTip("Splits when rescuing the grub in Mines_04")]
        GrubCrystalPeaksBottomLever,
        [Description("营救幼虫圣巢之巅（幼虫）"), ToolTip("Splits when rescuing the grub in Mines_24")]
        GrubCrystalPeaksCrown,
        [Description("营救幼虫水晶山峰粉碎机（幼虫）"), ToolTip("Splits when rescuing the grub in Mines_19")]
        GrubCrystalPeaksCrushers,
        [Description("营救幼虫水晶山峰水晶之心（幼虫）"), ToolTip("Splits when rescuing the grub in Mines_31")]
        GrubCrystalPeaksCrystalHeart,
        [Description("营救幼虫水晶山峰真假幼虫（幼虫）"), ToolTip("Splits when rescuing the grub in Mines_16")]
        GrubCrystalPeaksMimics,
        [Description("营救幼虫结晶山丘（幼虫）"), ToolTip("Splits when rescuing the grub in Mines_35")]
        GrubCrystalPeaksMound,
        [Description("营救幼虫水晶山峰尖刺（幼虫）"), ToolTip("Splits when rescuing the grub in Mines_03")]
        GrubCrystalPeaksSpikes,
        [Description("营救幼虫深邃巢穴野兽巢穴（幼虫）"), ToolTip("Splits when rescuing the grub in Deepnest_Spider_Town")]
        GrubDeepnestBeastsDen,
        [Description("营救幼虫深邃巢穴摸黑房间（幼虫）"), ToolTip("Splits when rescuing the grub in Deepnest_39")]
        GrubDeepnestDark,
        [Description("营救幼虫深邃巢穴真假幼虫（幼虫）"), ToolTip("Splits when rescuing the grub in Deepnest_36")]
        GrubDeepnestMimics,
        [Description("营救幼虫深邃巢穴诺斯克（幼虫）"), ToolTip("Splits when rescuing the grub in Deepnest_31")]
        GrubDeepnestNosk,
        [Description("营救幼虫深邃巢穴尖刺（幼虫）"), ToolTip("Splits when rescuing the grub in Deepnest_03")]
        GrubDeepnestSpikes,
        [Description("营救幼虫雾之峡谷档案馆（幼虫）"), ToolTip("Splits when rescuing the grub in Fungus3_47")]
        GrubFogCanyonArchives,
        [Description("营救幼虫真菌荒地弹跳（幼虫）"), ToolTip("Splits when rescuing the grub in Fungus2_18")]
        GrubFungalBouncy,
        [Description("营救幼虫真菌荒地蘑菇孢子（幼虫）"), ToolTip("Splits when rescuing the grub in Fungus2_20")]
        GrubFungalSporeShroom,
        [Description("营救幼虫苍绿之径柯尼法（幼虫）"), ToolTip("Splits when rescuing the grub in Fungus1_06")]
        GrubGreenpathCornifer,
        [Description("营救幼虫苍绿之径猎人（幼虫）"), ToolTip("Splits when rescuing the grub in Fungus1_07")]
        GrubGreenpathHunter,
        [Description("营救幼虫苍绿之径苔藓骑士（幼虫）"), ToolTip("Splits when rescuing the grub in Fungus1_21")]
        GrubGreenpathMossKnight,
        [Description("营救幼虫苍绿之径容器碎片（幼虫）"), ToolTip("Splits when rescuing the grub in Fungus1_13")]
        GrubGreenpathVesselFragment,
        [Description("营救幼虫蜂巢外（幼虫）"), ToolTip("Splits when rescuing the grub in Hive_03")]
        GrubHiveExternal,
        [Description("营救幼虫蜂巢内（幼虫）"), ToolTip("Splits when rescuing the grub in Hive_04")]
        GrubHiveInternal,
        [Description("营救幼虫王国边境中间（幼虫）"), ToolTip("Splits when rescuing the grub in Deepnest_East_11")]
        GrubKingdomsEdgeCenter,
        [Description("营救幼虫王国边境奥罗（幼虫）"), ToolTip("Splits when rescuing the grub in Deepnest_East_14")]
        GrubKingdomsEdgeOro,
        [Description("营救幼虫王后花园鹿角站下（幼虫）"), ToolTip("Splits when rescuing the grub in Fungus3_10")]
        GrubQueensGardensBelowStag,
        [Description("营救幼虫王后花园顶（幼虫）"), ToolTip("Splits when rescuing the grub in Fungus3_22")]
        GrubQueensGardensUpper,
        [Description("营救幼虫王后花园白色夫人（幼虫）"), ToolTip("Splits when rescuing the grub in Fungus3_48")]
        GrubQueensGardensWhiteLady,
        [Description("营救幼虫安息之地地窖（幼虫）"), ToolTip("Splits when rescuing the grub in RestingGrounds_10")]
        GrubRestingGroundsCrypts,
        [Description("营救幼虫皇家水道中间（幼虫）"), ToolTip("Splits when rescuing the grub in Waterways_04")]
        GrubWaterwaysCenter,
        [Description("营救幼虫皇家水道王国边境（幼虫）"), ToolTip("Splits when rescuing the grub in Waterways_14")]
        GrubWaterwaysHwurmps,
        [Description("营救幼虫伊思玛（幼虫）"), ToolTip("Splits when rescuing the grub in Waterways_13")]
        GrubWaterwaysIsma,

        [Description("低语之根（祖先山丘）"), ToolTip("Splits upon completing the whispering root in the Ancestral Mound")]
        TreeMound,
        [Description("低语之根（泪水之城）"), ToolTip("Splits upon completing the whispering root in the City of Tears")]
        TreeCity,
        [Description("低语之根（水晶山峰）"), ToolTip("Splits upon completing the whispering root in Crystal Peak")]
        TreePeak,
        [Description("低语之根（深邃巢穴）"), ToolTip("Splits upon completing the whispering root in Deepnest")]
        TreeDeepnest,
        [Description("低语之根（十字路）"), ToolTip("Splits upon completing the whispering root in the Forgotten Crossroads")]
        TreeCrossroads,
        [Description("低语之根（食腿者）"), ToolTip("Splits upon completing the whispering root left from Leg Eater")]
        TreeLegEater,
        [Description("低语之根（螳螂村）"), ToolTip("Splits upon completing the whispering root above the Mantis Village")]
        TreeMantisVillage,
        [Description("低语之根（苍绿之径）"), ToolTip("Splits upon completing the whispering root in Greenpath")]
        TreeGreenpath,
        [Description("低语之根（蜂巢）"), ToolTip("Splits upon completing the whispering root in the Hive")]
        TreeHive,
        [Description("低语之根（呼啸悬崖）"), ToolTip("Splits upon completing the whispering root in the Howling Cliifs")]
        TreeCliffs,
        [Description("低语之根（王国边境）"), ToolTip("Splits upon completing the whispering root in the Kingdom's Edge")]
        TreeKingdomsEdge,
        [Description("低语之根（王后花园）"), ToolTip("Splits upon completing the whispering root in the Queen's Gardens")]
        TreeQueensGardens,
        [Description("低语之根（安息之地）"), ToolTip("Splits upon completing the whispering root in the Resting Grounds")]
        TreeRestingGrounds,
        [Description("低语之根（皇家水道）"), ToolTip("Splits upon completing the whispering root in the Royal Waterways")]
        TreeWaterways,
        [Description("低语之根（灵魂沼地）"), ToolTip("Splits upon completing the whispering root in the Spirits' Glade")]
        TreeGlade,
        
        [Description("王后花园椅子（收费机）"), ToolTip("Splits when buying Queen's Garden toll bench")] 
        TollBenchQG,
        [Description("圣所椅子（收费机）"), ToolTip("Splits when buying City/Sanctum toll bench by Cornifer's location")] 
        TollBenchCity,
        [Description("古老盆地椅子（收费机）"), ToolTip("Splits when buying Ancient Basin toll bench")] 
        TollBenchBasin,
        [Description("皇家水道井（收费机）"), ToolTip("Splits when opening the Waterways Manhole")]
        WaterwaysManhole,
        [Description("解锁深邃巢穴电车"), ToolTip("Splits when unlocking the tram in Deepnest")]
        TramDeepnest,

        [Description("白色宫殿-下层（点灯）"), ToolTip("Splits when lighting the orb in White Palace lowest floor")] 
        WhitePalaceOrb1,
        [Description("白色宫殿-中层左（点灯）"), ToolTip("Splits when lighting the orb in White Palace left wing")] 
        WhitePalaceOrb3,
        [Description("白色宫殿-中层右（点灯）"), ToolTip("Splits when lighting the orb in White Palace right wing")] 
        WhitePalaceOrb2,


        [Description("白色宫殿-下层入口（房间）"), ToolTip("Splits on transition to White_Palace_01")]
        WhitePalaceLowerEntry,
        [Description("白色宫殿-下层灯（房间）"), ToolTip("Splits on transition to White_Palace_02")]
        WhitePalaceLowerOrb,
        [Description("白色宫殿-中层左入口（房间）"), ToolTip("Splits on transition to White_Palace_04")]
        WhitePalaceLeftEntry,
        [Description("白色宫殿-中层左中间（房间）"), ToolTip("Splits on transition between White_Palace_04 and _14")]
        WhitePalaceLeftWingMid,
        [Description("白色宫殿-中层右入口（房间）"), ToolTip("Splits on transition between White_Palace_03_Hub and _15")]
        WhitePalaceRightEntry,
        [Description("白色宫殿-中层右攀爬房间（房间）"), ToolTip("Splits on transition between White_Palace_05 and _16")]
        WhitePalaceRightClimb,
        [Description("白色宫殿-中层右锯挤房间（房间）"), ToolTip("Splits on transition between White_Palace_16 and _05")]
        WhitePalaceRightSqueeze,
        [Description("白色宫殿-中层右出口（房间）"), ToolTip("Splits on transition between White_Palace_05 and _15")]
        WhitePalaceRightDone,
        [Description("白色宫殿-上层入口（房间）"), ToolTip("Splits on transition between White_Palace_03_Hub and _06")]
        WhitePalaceTopEntry,
        [Description("白色宫殿-上层诅咒循环（房间）"), ToolTip("Splits on transition between White_Palace_06 and _07")]
        WhitePalaceTopClimb,
        [Description("白色宫殿-上层上（房间）"), ToolTip("Splits on transition between White_Palace_07 and _12")]
        WhitePalaceTopLeverRoom,
        [Description("白色宫殿-上层最后平台（房间）"), ToolTip("Splits on transition between White_Palace_12 and _13")]
        WhitePalaceTopLastPlats,
        [Description("白色宫殿-上层王座（房间）"), ToolTip("Splits on transition between White_Palace_13 and _09")]
        WhitePalaceThroneRoom,
        [Description("白色宫殿-中庭（房间）"), ToolTip("Splits on any transition to White_Palace_03_Hub")]
        WhitePalaceAtrium,

        [Description("苦痛之路进入房间1（房间）"), ToolTip("Splits on transition to the first room in PoP (entry to PoP)")]
        PathOfPainEntry,
        [Description("苦痛之路进入房间2（房间）"), ToolTip("Splits on transition to the second room in PoP")]
        PathOfPainTransition1,
        [Description("苦痛之路进入房间3（房间）"), ToolTip("Splits on transition to the third room in PoP")]
        PathOfPainTransition2,
        [Description("苦痛之路进入房间4（房间）"), ToolTip("Splits on transition to the fourth room in PoP (Final room)")]
        PathOfPainTransition3,

        [Description("王国边境酸水旁放梦门（不一定行）"), ToolTip("Splits when placing Dreamgate by KE Acid (hopefully)")]
        DgateKingdomsEdgeAcid,

        [Description("灵魂大师遭遇（试炼）"), ToolTip("Splits when Soul Master is activated the first time and the gate closes")]
        SoulMasterEncountered,

        [Description("万神殿离开椅子房间（切换场景）"), ToolTip("Splits when leaving a Godhome Bench room")]
        GodhomeBench,
        [Description("万神殿离开寻神者房间（切换场景）"), ToolTip("Splits when leaving a Godhome lore room")]
        GodhomeLoreRoom,
        [Description("进入万神殿任意一到四（切换场景）"), ToolTip("Splits on entry to any of pantheon 1 - 4")]
        Pantheon1to4Entry,
        [Description("进入万神殿五"), ToolTip("Splits on entry to pantheon 5")]
        Pantheon5Entry,

        [Description("回主菜单"), ToolTip("Splits on main menu")]
        Menu,
        [Description("回主菜单（有螳螂爪）"), ToolTip("Splits on transition to menu after Mantis Claw acquired")]
        MenuClaw,
        [Description("回主菜单（击杀华丽躯壳后）"), ToolTip("Splits on transition to menu after Gorgeous Husk defeated")]
        MenuGorgeousHusk,

        [Description("柯尼法在家（切换场景）"), ToolTip("Splits when entering Iselda's hut while Cornifer is sleeping")]
        CorniferAtHome,
        [Description("全部圣巢印章（物品）"), ToolTip("Splits when 17 Hallownest Seals have been collected")]
        AllSeals,
        [Description("全部腐臭蛋（物品）"), ToolTip("Splits when 21 Rancid Eggs have been collected")]
        AllEggs,
        [Description("斯莱商店的简单钥匙（物品）"), ToolTip("Splits when buying the simple key from Sly")]
        SlySimpleKey,
        [Description("易碎护符全损坏（事件）"), ToolTip("Splits when all 3 fragile charms are broken")]
        AllBreakables,
        [Description("坚固护符全获得（护符）"), ToolTip("Splits when all 3 unbreakable charms are obtained")]
        AllUnbreakables,
        [Description("见到艾米丽塔（事件）"), ToolTip("Splits when talking to Emilitia for the first time")]
        MetEmilitia,

        [Description("地图德特茅斯（物品）"), ToolTip("Splits when acquiring the Dirtmouth map")]
        mapDirtmouth,
        [Description("地图十字路（物品）"), ToolTip("Splits when acquiring the Crossroads map")]
        mapCrossroads,
        [Description("地图苍绿之径（物品）"), ToolTip("Splits when acquiring the Greenpath map")]
        mapGreenpath,
        [Description("地图雾之峡谷（物品）"), ToolTip("Splits when acquiring the Fog Canyon map")]
        mapFogCanyon,
        [Description("地图王后花园（物品）"), ToolTip("Splits when acquiring the QG map")]
        mapRoyalGardens,
        [Description("地图真菌荒地（物品）"), ToolTip("Splits when acquiring the Fungal Wastes map")]
        mapFungalWastes,
        [Description("地图泪水之城（物品）"), ToolTip("Splits when acquiring the City map")]
        mapCity,
        [Description("地图皇家水道（物品）"), ToolTip("Splits when acquiring the Waterways map")]
        mapWaterways,
        [Description("地图水晶山峰（物品）"), ToolTip("Splits when acquiring the Crystal Peak map")]
        mapMines,
        [Description("地图深邃巢穴（物品）"), ToolTip("Splits when acquiring the Deepnest map")]
        mapDeepnest,
        [Description("地图呼啸悬崖（物品）"), ToolTip("Splits when acquiring the Howling Cliffs map")]
        mapCliffs,
        [Description("地图王国边境（物品）"), ToolTip("Splits when acquiring the KE map")]
        mapOutskirts,
        [Description("地图安息之地（物品）"), ToolTip("Splits when acquiring the Resting Grounds map")]
        mapRestingGrounds,
        [Description("地图古老盆地（物品）"), ToolTip("Splits when acquiring the Abyss map")]
        mapAbyss,
       
        [Description("梦之钉吸收玛丽莎（获得）"), ToolTip("Splits when obtaining the essence from Marissa")]
        OnObtainGhostMarissa,
        [Description("梦之钉吸收凯利福与菲拉（获得）"), ToolTip("Splits when obtaining the essence from Caelif and Fera Orthop")]
        OnObtainGhostCaelifFera,
        [Description("梦之钉吸收波奇（获得）"), ToolTip("Splits when obtaining the essence from Poggy Thorax")]
        OnObtainGhostPoggy,
        [Description("梦之钉吸收掘墓者（获得）"), ToolTip("Splits when obtaining the essence from Gravedigger")]
        OnObtainGhostGravedigger,
        [Description("梦之钉吸收乔尼（获得）"), ToolTip("Splits when obtaining the essence from Joni")]
        OnObtainGhostJoni,
        [Description("梦之钉吸收阿布（获得）"), ToolTip("Splits when obtaining the essence from Cloth")]
        OnObtainGhostCloth,
        [Description("梦之钉吸收维斯帕（获得）"), ToolTip("Splits when obtaining the essence from Vespa")]
        OnObtainGhostVespa,
        [Description("梦之钉吸收瑞维克（获得）"), ToolTip("Splits when 19 essence has been obtained in Spirits' Glade")]
        OnObtainGhostRevek,

        [Description("漫游者日记（获得）"), ToolTip("Splits when obtaining a Wanderer's Journal")]
        OnObtainWanderersJournal,
        [Description("圣巢印章（获得）"), ToolTip("Splits when obtaining a Hallownest Seal")]
        OnObtainHallownestSeal,
        [Description("国王神像（获得）"), ToolTip("Splits when obtaining a King's Idol")]
        OnObtainKingsIdol,
        [Description("神秘蛋（获得）"), ToolTip("Splits when obtaining an Arcane Egg")]
        OnObtainArcaneEgg,
        [Description("腐臭蛋（获得）"), ToolTip("Splits when obtaining a Rancid Egg")]
        OnObtainRancidEgg,
        [Description("面具碎片（获得）"), ToolTip("Splits when obtaining a Mask Shard (splits on upgrade for full mask)")]
        OnObtainMaskShard,
        [Description("容器碎片（获得）"), ToolTip("Splits when obtaining a Vessel Fragment (splits on upgrade for full vessel)")]
        OnObtainVesselFragment,
        [Description("简单钥匙（获得）"), ToolTip("Splits when obtaining a Simple Key")]
        OnObtainSimpleKey,
        [Description("使用简单钥匙"), ToolTip("Splits when using a Simple Key")]
        OnUseSimpleKey,
        [Description("幼虫（获得）"), ToolTip("Splits when obtaining a Grub")]
        OnObtainGrub,

        [Description("任意切换场景"), ToolTip("Splits when the knight enters a transition (only one will split per transition)")]
        AnyTransition,
        [Description("任意切换场景（不含SaveState）"), ToolTip("Splits when the knight enters a transition (excludes save states and Sly's basement)")]
        TransitionAfterSaveState,
        [Description("手动分段"), ToolTip("Never splits. Use this when you need to manually split while using ordered splits")]
        ManualSplit,




        /*
        [Description("Mage Door (Test)"), ToolTip("Splits when Nailsmith is spared")] 
        MageDoor,
        [Description("圣所战士Window(Test)"), ToolTip("Splits when Nailsmith is killed")] 
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
