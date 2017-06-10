using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Linq;

namespace LiveSplit.HollowKnight {
	public partial class Settings : UserControl {
		public List<SplitInfo> Splits { get; private set; }
		public bool OldGameTime { get; set; }
		private bool isLoading;
		public Dictionary<string, SplitInfo> reference = SplitInfo.RegisteredSplits;
		public Settings() {
			isLoading = true;
			InitializeComponent();

			OldGameTime = false;
			Splits = new List<SplitInfo>();
			isLoading = false;
		}

		public bool HasSplit(SplitInfo split) {
			return Splits.Contains(split);
		}

		private void Settings_Load(object sender, EventArgs e) {
			LoadSettings();
		}
		public void LoadSettings() {
			isLoading = true;
			flowMain.SuspendLayout();

			for (int i = flowMain.Controls.Count - 1; i > 0; i--) {
				flowMain.Controls.RemoveAt(i);
			}

			foreach (var split in Splits) {

				var setting = new SplitSettings();

				foreach (var item in SplitInfo.RegisteredSplits) {
					setting.cboName.Items.Add(new ComboBoxItem(item.Value.Description, item.Value));
				}
				AddHandlers(setting);
				for (int i = 0; i < setting.cboName.Items.Count; i++) {
					if ((setting.cboName.Items[i] as ComboBoxItem).Tag.Equals(split)) {
						setting.cboName.SelectedIndex = i;
						break;
					}
				}

				flowMain.Controls.Add(setting);
			}
			chkOldGameTime.Checked = OldGameTime;

			isLoading = false;
			flowMain.ResumeLayout(true);
		}
		private void AddHandlers(SplitSettings setting) {
			setting.cboName.SelectedIndexChanged += new EventHandler(ControlChanged);
			setting.btnRemove.Click += new EventHandler(btnRemove_Click);
			setting.btnMoveUp.Click += new EventHandler(btnMoveUp_Click);
			setting.btnMoveDown.Click += new EventHandler(btnMoveDown_Click);
		}
		private void RemoveHandlers(SplitSettings setting) {
			setting.cboName.SelectedIndexChanged -= ControlChanged;
			setting.btnRemove.Click -= btnRemove_Click;
		}
		public void btnRemove_Click(object sender, EventArgs e) {
			for (int i = flowMain.Controls.Count - 1; i > 0; i--) {
				var currentControl = flowMain.Controls[i];
				if (currentControl.Contains((Control)sender)) {
					RemoveHandlers((SplitSettings)currentControl);

					flowMain.Controls.RemoveAt(i);
					break;
				}
			}
			UpdateSplits();
		}
		public void btnMoveUp_Click(object sender, EventArgs e) {

			// Dont need to move up if this item is already at the top of the list.
			if (flowMain.Controls.Count <= 1) {
				return;
			}

			// We're basically doing a single swap on the two splits.
			SplitInfo currentSplit, aboveSplit;
			for (var i = flowMain.Controls.Count - 1; i > 0; i--) {
				var currentControl = flowMain.Controls[i];

				if (currentControl.Contains((Control)sender)) {
					var currentSplitSettings = (currentControl as SplitSettings);
					var aboveSplitSettings = (flowMain.Controls[i - 1] as SplitSettings);

					// get the current settings for each split setting
					currentSplit = currentSplitSettings.Split;
					aboveSplit = aboveSplitSettings.Split;

					// swap them
					currentSplitSettings.SetSplit(aboveSplit);
					aboveSplitSettings.SetSplit(currentSplit);
					break;
				}
			}
			UpdateSplits();
		}
		public void btnMoveDown_Click(object sender, EventArgs e) {

			// We're basically doing a single swap on the two splits.
			SplitInfo currentSplit, belowSplit;
			for (var i = flowMain.Controls.Count - 1; i > 0; i--) {
				var currentControl = flowMain.Controls[i];

				if (currentControl.Contains((Control)sender)) {

					// Dont need to move down if this item is already at the bottom of the list.
					if (i == flowMain.Controls.Count - 1) {
						break;
					}

					var currentSplitSettings = (currentControl as SplitSettings);
					var belowSplitSettings = (flowMain.Controls[i + 1] as SplitSettings);

					// get the current settings for each split setting
					currentSplit = currentSplitSettings.Split;
					belowSplit = belowSplitSettings.Split;

					// swap them
					currentSplitSettings.SetSplit(belowSplit);
					belowSplitSettings.SetSplit(currentSplit);
					break;
				}
			}
			UpdateSplits();
		}
		public void ControlChanged(object sender, EventArgs e) {
			UpdateSplits();
		}
		public void UpdateSplits() {
			if (isLoading) return;

			Splits.Clear();
			foreach (var c in flowMain.Controls) {
				if (c is SplitSettings splitSettings) {

					if (!string.IsNullOrEmpty(splitSettings.cboName.Text)) {
						var split = (splitSettings.cboName.SelectedItem as ComboBoxItem).Tag as SplitInfo;
						Splits.Add(split);
					}
				}
			}

			OldGameTime = chkOldGameTime.Checked;
		}
		public XmlNode UpdateSettings(XmlDocument document) {
			var xmlSettings = document.CreateElement("Settings");

			var xmlSplits = document.CreateElement("Splits");
			xmlSettings.AppendChild(xmlSplits);

			foreach (var split in Splits) {
				var xmlSplit = document.CreateElement("Split");
				xmlSplit.InnerText = split.ID;

				xmlSplits.AppendChild(xmlSplit);
			}

			var xmlGameTime = document.CreateElement("OldGameTime");
			xmlGameTime.InnerText = OldGameTime.ToString();
			xmlSettings.AppendChild(xmlGameTime);

			return xmlSettings;
		}
		public void SetSettings(XmlNode settings) {
			Splits.Clear();
			var splitNodes = settings.SelectNodes(".//Splits/Split");
			foreach (XmlNode splitNode in splitNodes) {

				var split = SplitInfo.FromID(splitNode.InnerText);
				Splits.Add(split);
			}

			var gameTime = settings.SelectSingleNode(".//OldGameTime");
			OldGameTime = gameTime == null || string.IsNullOrEmpty(gameTime.InnerText) ? false : bool.Parse(gameTime.InnerText);
		}
		private void btnAddSplit_Click(object sender, EventArgs e) {
			var setting = new SplitSettings();

			foreach (var item in SplitInfo.RegisteredSplits) {
				setting.cboName.Items.Add(new ComboBoxItem(item.Value.Description, item.Value));
			}
			AddHandlers(setting);
			setting.cboName.SelectedIndex = 0;

			flowMain.Controls.Add(setting);
			UpdateSplits();
		}
		private List<string> GetAvailableSplits() {
			var splits = new List<string>();

			SplitInfo.RegisteredSplits.Values.ToList().ForEach(x => splits.Add(x.Description));

			if (rdAlpha.Checked) {
				splits.Sort(delegate (string one, string two) {
					return one.CompareTo(two);
				});
			}
			return splits;
		}
		private void radio_CheckedChanged(object sender, EventArgs e) {
			foreach (var c in flowMain.Controls) {
				if (c is SplitSettings splitSettings) {
					var index = splitSettings.cboName.SelectedIndex;
					foreach (var item in SplitInfo.RegisteredSplits) {
						splitSettings.cboName.Items.Add(new ComboBoxItem(item.Value.Description, item.Value));
					}
					splitSettings.cboName.SelectedIndex = index;
				}
			}
		}
	}
}