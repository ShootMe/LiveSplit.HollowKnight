using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.HollowKnight {
	public partial class HollowKnightSettings : UserControl {
		public List<SplitName> Splits { get; private set; }
		public bool ShowMapDisplay { get; set; }
		public bool RainbowDash { get; set; }
		private bool isLoading;
		public HollowKnightSettings() {
			isLoading = true;
			InitializeComponent();

			Splits = new List<SplitName>();
			isLoading = false;
		}

		public bool HasSplit(SplitName split) {
			return Splits.Contains(split);
		}

		private void Settings_Load(object sender, EventArgs e) {
			LoadSettings();
		}
		public void LoadSettings() {
			isLoading = true;
			this.flowMain.SuspendLayout();

			for (int i = flowMain.Controls.Count - 1; i > 0; i--) {
				flowMain.Controls.RemoveAt(i);
			}

			foreach (SplitName split in Splits) {
				MemberInfo info = typeof(SplitName).GetMember(split.ToString())[0];
				DescriptionAttribute description = (DescriptionAttribute)info.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];

				HollowKnightSplitSettings setting = new HollowKnightSplitSettings();
				setting.cboName.DataSource = GetAvailableSplits();
				setting.cboName.Text = description.Description;
				AddHandlers(setting);

				flowMain.Controls.Add(setting);
			}

			isLoading = false;
			this.flowMain.ResumeLayout(true);
		}
		private void AddHandlers(HollowKnightSplitSettings setting) {
			setting.cboName.SelectedIndexChanged += new EventHandler(cboName_SelectedIndexChanged);
			setting.btnRemove.Click += new EventHandler(btnRemove_Click);
		}
		private void RemoveHandlers(HollowKnightSplitSettings setting) {
			setting.cboName.SelectedIndexChanged -= cboName_SelectedIndexChanged;
			setting.btnRemove.Click -= btnRemove_Click;
		}
		public void btnRemove_Click(object sender, EventArgs e) {
			for (int i = flowMain.Controls.Count - 1; i > 1; i--) {
				if (flowMain.Controls[i].Contains((Control)sender)) {
					RemoveHandlers((HollowKnightSplitSettings)((Button)sender).Parent);

					flowMain.Controls.RemoveAt(i);
					break;
				}
			}
			UpdateSplits();
		}
		public void cboName_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateSplits();
		}
		public void UpdateSplits() {
			if (isLoading) return;

			Splits.Clear();
			foreach (Control c in flowMain.Controls) {
				if (c is HollowKnightSplitSettings) {
					HollowKnightSplitSettings setting = (HollowKnightSplitSettings)c;
					if (!string.IsNullOrEmpty(setting.cboName.Text)) {
						SplitName split = HollowKnightSplitSettings.GetSplitName(setting.cboName.Text);
						Splits.Add(split);
					}
				}
			}
		}
		public XmlNode UpdateSettings(XmlDocument document) {
			XmlElement xmlSettings = document.CreateElement("Settings");

			XmlElement xmlSplits = document.CreateElement("Splits");
			xmlSettings.AppendChild(xmlSplits);

			foreach (SplitName split in Splits) {
				XmlElement xmlSplit = document.CreateElement("Split");
				xmlSplit.InnerText = split.ToString();

				xmlSplits.AppendChild(xmlSplit);
			}
			return xmlSettings;
		}
		public void SetSettings(XmlNode settings) {
			Splits.Clear();
			XmlNodeList splitNodes = settings.SelectNodes("//Splits/Split");
			foreach (XmlNode splitNode in splitNodes) {
				string splitDescription = splitNode.InnerText;
				SplitName split = HollowKnightSplitSettings.GetSplitName(splitDescription);
				Splits.Add(split);
			}
		}
		private void btnAddSplit_Click(object sender, EventArgs e) {
			HollowKnightSplitSettings setting = new HollowKnightSplitSettings();
			List<string> splitNames = GetAvailableSplits();
			setting.cboName.DataSource = splitNames;
			setting.cboName.Text = splitNames[0];
			AddHandlers(setting);

			flowMain.Controls.Add(setting);
			UpdateSplits();
		}
		private List<string> GetAvailableSplits() {
			List<string> splits = new List<string>();
			foreach (SplitName split in Enum.GetValues(typeof(SplitName))) {
				MemberInfo info = typeof(SplitName).GetMember(split.ToString())[0];
				DescriptionAttribute description = (DescriptionAttribute)info.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
				splits.Add(description.Description);
			}
			if (rdAlpha.Checked) {
				splits.Sort(delegate (string one, string two) {
					return one.CompareTo(two);
				});
			}
			return splits;
		}
		private void radio_CheckedChanged(object sender, EventArgs e) {
			foreach (Control c in flowMain.Controls) {
				if (c is HollowKnightSplitSettings) {
					HollowKnightSplitSettings setting = (HollowKnightSplitSettings)c;
					string text = setting.cboName.Text;
					setting.cboName.DataSource = GetAvailableSplits();
					setting.cboName.Text = text;
				}
			}
		}
	}
}