using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.HollowKnight {
	public class ComboBoxItem {

		public string Text { get; private set; }
		public SplitInfo Tag { get; private set; }

		public ComboBoxItem(string text, SplitInfo tag) {
			Text = text;
			Tag = tag;
		}

		public override string ToString() {
			return Text;
		}
	}
}
