using System;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.HollowKnight {
	public partial class HollowKnightSettings : UserControl {
		public bool FalseKnight { get; set; }
		public bool MothwingCloak { get; set; }
		public bool ThornsOfAgony { get; set; }
		public bool MantisClaw { get; set; }
		public bool DistantVillageStag { get; set; }
		public bool CrystalHeart { get; set; }
		public bool GruzMother { get; set; }
		public bool DreamNail { get; set; }
		public bool NailUpgrade1 { get; set; }
		public bool WatcherKnight { get; set; }
		public bool Lurien { get; set; }
		public bool Hegemol { get; set; }
		public bool Monomon { get; set; }
		public bool Uumuu { get; set; }
		private bool isLoading;

		public HollowKnightSettings() {
			isLoading = true;
			InitializeComponent();

			//Defaults
			FalseKnight = true;
			MothwingCloak = true;
			ThornsOfAgony = true;
			MantisClaw = true;
			DistantVillageStag = true;
			CrystalHeart = true;
			GruzMother = true;
			DreamNail = true;
			NailUpgrade1 = true;
			WatcherKnight = true;
			Lurien = true;
			Hegemol = true;
			Monomon = true;
			Uumuu = true;

			isLoading = false;
		}

		private void Settings_Load(object sender, EventArgs e) {
			isLoading = true;
			LoadSettings();
			isLoading = false;
		}
		public void LoadSettings() {
			chkFalseKnight.Checked = FalseKnight;
			chkMothwingCloak.Checked = MothwingCloak;
			chkThornsOfAgony.Checked = ThornsOfAgony;
			chkMantisClaw.Checked = MantisClaw;
			chkDistantVillageStation.Checked = DistantVillageStag;
			chkCrystalHeart.Checked = CrystalHeart;
			chkGruzMother.Checked = GruzMother;
			chkDreamNail.Checked = DreamNail;
			chkNailUpgrade1.Checked = NailUpgrade1;
			chkWatcherKnight.Checked = WatcherKnight;
			chkLurien.Checked = Lurien;
			chkHegemol.Checked = Hegemol;
			chkMonomon.Checked = Monomon;
			chkUumuu.Checked = Uumuu;
		}
		private void chkBox_CheckedChanged(object sender, EventArgs e) {
			UpdateSplits();
		}
		public void UpdateSplits() {
			if (isLoading) return;

			FalseKnight = chkFalseKnight.Checked;
			MothwingCloak = chkMothwingCloak.Checked;
			ThornsOfAgony = chkThornsOfAgony.Checked;
			MantisClaw = chkMantisClaw.Checked;
			DistantVillageStag = chkDistantVillageStation.Checked;
			CrystalHeart = chkCrystalHeart.Checked;
			GruzMother = chkGruzMother.Checked;
			DreamNail = chkDreamNail.Checked;
			NailUpgrade1 = chkNailUpgrade1.Checked;
			WatcherKnight = chkWatcherKnight.Checked;
			Lurien = chkLurien.Checked;
			Hegemol = chkHegemol.Checked;
			Monomon = chkMonomon.Checked;
			Uumuu = chkUumuu.Checked;
		}
		public XmlNode UpdateSettings(XmlDocument document) {
			XmlElement xmlSettings = document.CreateElement("Settings");

			SetSetting(document, xmlSettings, FalseKnight, "FalseKnight");
			SetSetting(document, xmlSettings, MothwingCloak, "MothwingCloak");
			SetSetting(document, xmlSettings, ThornsOfAgony, "ThornsOfAgony");
			SetSetting(document, xmlSettings, MantisClaw, "MantisClaw");
			SetSetting(document, xmlSettings, DistantVillageStag, "DistantVillageStag");
			SetSetting(document, xmlSettings, CrystalHeart, "CrystalHeart");
			SetSetting(document, xmlSettings, GruzMother, "GruzMother");
			SetSetting(document, xmlSettings, DreamNail, "DreamNail");
			SetSetting(document, xmlSettings, NailUpgrade1, "NailUpgrade1");
			SetSetting(document, xmlSettings, WatcherKnight, "WatcherKnight");
			SetSetting(document, xmlSettings, Lurien, "Lurien");
			SetSetting(document, xmlSettings, Hegemol, "Hegemol");
			SetSetting(document, xmlSettings, Monomon, "Monomon");
			SetSetting(document, xmlSettings, Uumuu, "Uumuu");

			return xmlSettings;
		}
		private void SetSetting(XmlDocument document, XmlElement settings, bool val, string name) {
			XmlElement xmlOption = document.CreateElement(name);
			xmlOption.InnerText = val.ToString();
			settings.AppendChild(xmlOption);
		}
		public void SetSettings(XmlNode settings) {
			FalseKnight = GetSetting(settings, "//FalseKnight", true);
			MothwingCloak = GetSetting(settings, "//MothwingCloak", true);
			ThornsOfAgony = GetSetting(settings, "//ThornsOfAgony", true);
			MantisClaw = GetSetting(settings, "//MantisClaw", true);
			DistantVillageStag = GetSetting(settings, "//DistantVillageStag", true);
			CrystalHeart = GetSetting(settings, "//CrystalHeart", true);
			GruzMother = GetSetting(settings, "//GruzMother", true);
			DreamNail = GetSetting(settings, "//DreamNail", true);
			NailUpgrade1 = GetSetting(settings, "//NailUpgrade1", true);
			WatcherKnight = GetSetting(settings, "//WatcherKnight", true);
			Lurien = GetSetting(settings, "//Lurien", true);
			Hegemol = GetSetting(settings, "//Hegemol", true);
			Monomon = GetSetting(settings, "//Monomon", true);
			Uumuu = GetSetting(settings, "//Uumuu", true);
		}
		private bool GetSetting(XmlNode settings, string name, bool defaultVal = false) {
			XmlNode option = settings.SelectSingleNode(name);
			if (option != null && option.InnerText != "") {
				return bool.Parse(option.InnerText);
			}
			return defaultVal;
		}
	}
}