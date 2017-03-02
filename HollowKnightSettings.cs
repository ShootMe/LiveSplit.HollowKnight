using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.HollowKnight {
	public partial class HollowKnightSettings : UserControl {
		public List<string> Splits { get; private set; }
		public bool ShowMapDisplay { get; set; }
		public bool RainbowDash { get; set; }
		private bool isLoading;
		public HollowKnightSettings() {
			isLoading = true;
			InitializeComponent();

			Splits = new List<string>();
			isLoading = false;
		}

		public bool HasSplit(SplitName split) {
			MemberInfo info = typeof(SplitName).GetMember(split.ToString())[0];
			DescriptionAttribute description = (DescriptionAttribute)info.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
			return Splits.Contains(description.Description);
		}

		private void OriSettings_Load(object sender, EventArgs e) {
			LoadSettings();
		}
		public void LoadSettings() {
			isLoading = true;
			this.flowMain.SuspendLayout();

			for (int i = flowMain.Controls.Count - 1; i > 0; i--) {
				flowMain.Controls.RemoveAt(i);
			}

			foreach (string split in Splits) {
				HollowKnightSplitSettings setting = new HollowKnightSplitSettings();
				setting.cboName.DataSource = GetAvailableSplits();
				setting.cboName.Text = split;
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
						Splits.Add(setting.cboName.Text);
					}
				}
			}
		}
		public XmlNode UpdateSettings(XmlDocument document) {
			XmlElement xmlSettings = document.CreateElement("Settings");

			XmlElement xmlSplits = document.CreateElement("Splits");
			xmlSettings.AppendChild(xmlSplits);

			foreach (string split in Splits) {
				XmlElement xmlSplit = document.CreateElement("Split");
				xmlSplit.InnerText = split;

				xmlSplits.AppendChild(xmlSplit);
			}
			return xmlSettings;
		}
		public void SetSettings(XmlNode settings) {
			Splits.Clear();
			XmlNodeList splitNodes = settings.SelectNodes("//Splits/Split");
			foreach (XmlNode splitNode in splitNodes) {
				Splits.Add(splitNode.InnerText);
			}
		}
		private void btnAddSplit_Click(object sender, EventArgs e) {
			HollowKnightSplitSettings setting = new HollowKnightSplitSettings();
			setting.cboName.DataSource = GetAvailableSplits();
			setting.cboName.Text = "False Knight";
			AddHandlers(setting);

			flowMain.Controls.Add(setting);
			UpdateSplits();
		}
		private List<string> GetAvailableSplits() {
			List<string> splits = new List<string>();
			foreach(SplitName split in Enum.GetValues(typeof(SplitName))) {
				MemberInfo info = typeof(SplitName).GetMember(split.ToString())[0];
				DescriptionAttribute description = (DescriptionAttribute)info.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
				splits.Add(description.Description);
			}
			return splits;
		}
	}
}