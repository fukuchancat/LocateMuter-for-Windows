using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using Locate_Muter_for_Windows.Models;
using System.Collections.ObjectModel;

namespace LocateMuter_for_Windows.Models
{
	public class AreasCollection : ObservableCollection<ObservableCollection<Area>>
	{
		public AreasCollection() {
			this.Add(new Areas(false));
			this.Add(new Areas(true));
		}

		public void addMutingArea(Area area)
		{
			this[1].Add(area);
		}

		public void addUnMutingArea(Area area)
		{
			this[0].Add(area);
		}

		public AreasCollection getReversed()
		{
			AreasCollection ac = new AreasCollection();

			for (int i = Count - 1; i >= 0; i--)
			{
				ac.Add(this[i]);
			}

			return ac;
		}
	}
}
