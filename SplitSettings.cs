using System;
using System.Windows.Forms;

namespace LiveSplit.HollowKnight {
	public partial class SplitSettings : UserControl {

		public SplitInfo Split { get; private set; }

		public SplitSettings() {
			InitializeComponent();
		}

		public void SetSplit(SplitInfo split) {
			foreach (var item in cboName.Items) {
				if ((item as ComboBoxItem).Tag.Equals(split)) {
					Split = split;
					cboName.SelectedItem = item;
				}
			}
		}

		private void cboName_SelectedIndexChanged(object sender, EventArgs e) {
			var splitDescription = (cboName.SelectedItem as ComboBoxItem).Text;
			Split = (cboName.SelectedItem as ComboBoxItem).Tag as SplitInfo;
			
			ToolTips.SetToolTip(cboName, Split.ToolTip);
		}
	}
}