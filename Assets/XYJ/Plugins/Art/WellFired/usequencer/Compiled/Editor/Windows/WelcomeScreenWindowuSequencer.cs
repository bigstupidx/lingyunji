using UnityEngine;
using System.Collections;

namespace WellFired
{
	public class WelcomeScreenWindowuSequencer : WellFired.Shared.WelcomeScreenWindow
	{
		public override string ProductName 
		{ 
			get { return "usequencer"; }
		}

		public override string ReadableProductName 
		{ 
			get { return "uSequencer"; }
		}
		
		public override string DocumentationURL 
		{ 
			get { return "http://www.wellfired.com/usequencer.html#documentation_tab"; }
		}

		public override string ProductDescription 
		{ 
			get { return "You have in your hands Unity's most powerful sequencer, \nwe cannot wait to see what you produce."; }
		}

		public override string DocumentationText
		{
			get { return "For a full overview of uSequencer, including a \nreference for adding your own events, check out our \nndocumentation"; }
		}
	}
}