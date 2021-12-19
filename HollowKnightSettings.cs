using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.HollowKnight {
    public partial class HollowKnightSettings : UserControl {
        public List<SplitName> Splits { get; private set; }
        public bool Ordered { get; set; }
        public bool AutosplitEndRuns { get; set; }
        public SplitName? AutosplitStartRuns { get; set; }
        private bool isLoading;
        private List<string> availableSplits = new List<string>();
        private List<string> availableSplitsAlphaSorted = new List<string>();

        public HollowKnightSettings() {
            isLoading = true;
            InitializeComponent();
            string version = typeof(HollowKnightComponent).Assembly.GetName().Version.ToString();
#if DEBUG
            version += "-dev";
#endif
            this.versionLabel.Text = "Autosplitter Version: " + version;

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

            chkOrdered.Checked = Ordered;
            chkAutosplitEndRuns.Checked = AutosplitEndRuns;
            chkAutosplitStartRuns.Checked = AutosplitStartRuns != null;

            foreach (SplitName split in Splits) {
                MemberInfo info = typeof(SplitName).GetMember(split.ToString())[0];
                DescriptionAttribute description = (DescriptionAttribute)info.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];

                HollowKnightSplitSettings setting = new HollowKnightSplitSettings();
                setting.cboName.DataSource = new List<string>() { description.Description };
                setting.cboName.Enabled = false;
                setting.cboName.Text = description.Description;
                AddHandlers(setting);

                flowMain.Controls.Add(setting);
            }

            isLoading = false;
            this.flowMain.ResumeLayout(true);
        }
        private void AddHandlers(HollowKnightSplitSettings setting) {
            setting.cboName.SelectedIndexChanged += new EventHandler(ControlChanged);
            setting.btnRemove.Click += new EventHandler(btnRemove_Click);
            setting.btnEdit.Click += new EventHandler(btnEdit_Click);
            setting.btnAddAbove.Click += new EventHandler(btnAddAbove_Click);
            setting.btnAddBelow.Click += new EventHandler(btnAddBelow_Click);
        }
        private void RemoveHandlers(HollowKnightSplitSettings setting) {
            setting.cboName.SelectedIndexChanged -= ControlChanged;
            setting.btnRemove.Click -= btnRemove_Click;
            setting.btnEdit.Click -= btnEdit_Click;
            setting.btnAddAbove.Click -= btnAddAbove_Click;
            setting.btnAddBelow.Click -= btnAddBelow_Click;
        }
        public void btnRemove_Click(object sender, EventArgs e) {
            for (int i = flowMain.Controls.Count - 1; i > 0; i--) {
                if (flowMain.Controls[i].Contains((Control)sender)) {
                    RemoveHandlers((HollowKnightSplitSettings)((Button)sender).Parent);

                    flowMain.Controls.RemoveAt(i);
                    break;
                }
            }
            UpdateSplits();
        }
        public void btnEdit_Click(object sender, EventArgs e) {
            for (int i = flowMain.Controls.Count - 1; i > 0; i--) {
                if (flowMain.Controls[i].Contains((Control)sender)) {
                    HollowKnightSplitSettings setting = (HollowKnightSplitSettings)((Button)sender).Parent;
                    if (setting.cboName.Enabled) {
                        disableEdit(setting);
                    } else {
                        enableEdit(setting);
                    }
                    break;
                }
            }
        }
        private void btnAddAbove_Click(object sender, EventArgs e) {
            for (int i = flowMain.Controls.Count - 1; i > 0; i--) {
                if (flowMain.Controls[i].Contains((Control)sender)) {
                    HollowKnightSplitSettings setting = (HollowKnightSplitSettings)((Button)sender).Parent;
                    int index = setting.Parent.Controls.GetChildIndex(setting);
                    addSplitAtIndex(index);
                }
            }
        }
        private void btnAddBelow_Click(object sender, EventArgs e) {
            for (int i = flowMain.Controls.Count - 1; i > 0; i--) {
                if (flowMain.Controls[i].Contains((Control)sender)) {
                    HollowKnightSplitSettings setting = (HollowKnightSplitSettings)((Button)sender).Parent;
                    int index = setting.Parent.Controls.GetChildIndex(setting);
                    addSplitAtIndex(index + 1);
                }
            }
        }
        private void enableEdit(HollowKnightSplitSettings setting) {
            string currentText = setting.cboName.Text;
            setting.btnEdit.Text = "✔";
            setting.cboName.DataSource = GetAvailableSplits();
            setting.cboName.Text = currentText;
            setting.cboName.Enabled = true;
        }
        private void disableEdit(HollowKnightSplitSettings setting) {
            setting.btnEdit.Text = "✏";
            setting.cboName.Enabled = false;
        }
        public void ControlChanged(object sender, EventArgs e) {
            UpdateSplits();
        }
        public void UpdateSplits() {
            if (isLoading) return;

            Ordered = chkOrdered.Checked;
            AutosplitEndRuns = chkAutosplitEndRuns.Checked;
            AutosplitStartRuns = chkAutosplitStartRuns.Checked ?
                HollowKnightSplitSettings.GetSplitName(cboStartTriggerName.Text) : null;

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

            XmlElement xmlOrdered = document.CreateElement("Ordered");
            xmlOrdered.InnerText = Ordered.ToString();
            xmlSettings.AppendChild(xmlOrdered);

            XmlElement xmlAutosplitEndRuns = document.CreateElement("AutosplitEndRuns");
            xmlAutosplitEndRuns.InnerText = AutosplitEndRuns.ToString();
            xmlSettings.AppendChild(xmlAutosplitEndRuns);

            XmlElement xmlAutosplitStartRuns = document.CreateElement("AutosplitStartRuns");
            xmlAutosplitStartRuns.InnerText = AutosplitStartRuns.ToString();
            xmlSettings.AppendChild(xmlAutosplitStartRuns);

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
            XmlNode orderedNode = settings.SelectSingleNode(".//Ordered");
            XmlNode AutosplitEndRunsNode = settings.SelectSingleNode(".//AutosplitEndRuns");
            XmlNode AutosplitStartRunsNode = settings.SelectSingleNode(".//AutosplitStartRuns");
            bool isOrdered = false;
            bool isAutosplitEndRuns = false;

            if (orderedNode != null) {
                bool.TryParse(orderedNode.InnerText, out isOrdered);
            }
            if (AutosplitEndRunsNode != null) {
                bool.TryParse(AutosplitEndRunsNode.InnerText, out isAutosplitEndRuns);
            }
            if (AutosplitStartRunsNode != null) {
                string splitDescription = AutosplitStartRunsNode.InnerText.Trim();
                if (!string.IsNullOrEmpty(splitDescription)) {
                    cboStartTriggerName.DataSource = GetAvailableSplits();
                    AutosplitStartRuns = HollowKnightSplitSettings.GetSplitName(splitDescription);
                    MemberInfo info = typeof(SplitName).GetMember(AutosplitStartRuns.ToString())[0];
                    DescriptionAttribute description = (DescriptionAttribute)info.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
                    cboStartTriggerName.Text = description.Description;
                }
            }
            Ordered = isOrdered;
            AutosplitEndRuns = isAutosplitEndRuns;

            Splits.Clear();
            XmlNodeList splitNodes = settings.SelectNodes(".//Splits/Split");
            foreach (XmlNode splitNode in splitNodes) {
                string splitDescription = splitNode.InnerText;
                SplitName split = HollowKnightSplitSettings.GetSplitName(splitDescription);
                Splits.Add(split);
            }
        }
        private HollowKnightSplitSettings createSetting() {
            HollowKnightSplitSettings setting = new HollowKnightSplitSettings();
            List<string> splitNames = GetAvailableSplits();
            setting.cboName.DataSource = splitNames;
            setting.cboName.Text = splitNames[0];
            setting.btnEdit.Text = "✔";
            AddHandlers(setting);
            return setting;
        }
        private void btnAddSplit_Click(object sender, EventArgs e) {
            HollowKnightSplitSettings setting = createSetting();
            flowMain.Controls.Add(setting);
            UpdateSplits();
        }
        private void addSplitAtIndex(int index) {
            HollowKnightSplitSettings setting = createSetting();
            flowMain.Controls.Add(setting);
            flowMain.Controls.SetChildIndex(setting, index);
            UpdateSplits();
        }
        private List<string> GetAvailableSplits() {
            if (availableSplits.Count == 0) {
                foreach (SplitName split in Enum.GetValues(typeof(SplitName))) {
                    MemberInfo info = typeof(SplitName).GetMember(split.ToString())[0];
                    DescriptionAttribute description = (DescriptionAttribute)info.GetCustomAttributes(typeof(DescriptionAttribute), false)[0];
                    availableSplits.Add(description.Description);
                    availableSplitsAlphaSorted.Add(description.Description);
                }
                availableSplitsAlphaSorted.Sort(delegate (string one, string two) {
                    return one.CompareTo(two);
                });
            }
            return rdAlpha.Checked ? availableSplitsAlphaSorted : availableSplits;
        }
        private void radio_CheckedChanged(object sender, EventArgs e) {
            foreach (Control c in flowMain.Controls) {
                if (c is HollowKnightSplitSettings) {
                    HollowKnightSplitSettings setting = (HollowKnightSplitSettings)c;
                    if (setting.cboName.Enabled) {
                        string text = setting.cboName.Text;
                        setting.cboName.DataSource = GetAvailableSplits();
                        setting.cboName.Text = text;
                    }
                }
            }

            if (chkAutosplitStartRuns.Checked) {
                string text = cboStartTriggerName.Text;
                cboStartTriggerName.DataSource = GetAvailableSplits();
                cboStartTriggerName.Text = text;
            }
        }
        private void flowMain_DragDrop(object sender, DragEventArgs e) {
            UpdateSplits();
        }
        private void flowMain_DragEnter(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.Move;
        }
        private void flowMain_DragOver(object sender, DragEventArgs e) {
            HollowKnightSplitSettings data = (HollowKnightSplitSettings)e.Data.GetData(typeof(HollowKnightSplitSettings));
            FlowLayoutPanel destination = (FlowLayoutPanel)sender;
            Point p = destination.PointToClient(new Point(e.X, e.Y));
            var item = destination.GetChildAtPoint(p);
            int index = destination.Controls.GetChildIndex(item, false);
            if (index == 0) {
                e.Effect = DragDropEffects.None;
            } else {
                e.Effect = DragDropEffects.Move;
                int oldIndex = destination.Controls.GetChildIndex(data);
                if (oldIndex != index) {
                    enableEdit(data);
                    destination.Controls.SetChildIndex(data, index);
                    destination.Invalidate();
                }
            }
        }
        private void AutosplitEndChanged(object sender, EventArgs e) {
            UpdateSplits();
        }

        private void AutosplitStartChanged(object sender, EventArgs e) {
            if (chkAutosplitStartRuns.Checked) {
                cboStartTriggerName.Enabled = true;
                cboStartTriggerName.DataSource = GetAvailableSplits();
            }
            else {
                cboStartTriggerName.Text = "";
                cboStartTriggerName.Enabled = false;
                cboStartTriggerName.DataSource = new List<string>();
            }
            UpdateSplits();
        }

        private void cboStartTriggerName_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateSplits();
        }
    }
}
