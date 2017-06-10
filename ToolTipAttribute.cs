using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.HollowKnight {

	public class ToolTipAttribute : Attribute {

		public string ToolTip { get; set; }

		public ToolTipAttribute(string text) {
			ToolTip = text;
		}
	}
}
