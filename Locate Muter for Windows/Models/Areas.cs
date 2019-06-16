using LocateMuter_for_Windows.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Locate_Muter_for_Windows.Models
{
	class Areas : ObservableCollection<Area>
	{
		public bool Muting {get; set;}

		public string Title
		{
			get
			{
				if (Muting)
				{
					return "ミュートするエリア";
				}
				else
				{
					return "ミュート解除するエリア";
				}
			}
		}

		public Color Color1
		{
			get
			{
				if (Muting)
				{
					return Colors.Red;
				}
				else
				{
					return Colors.Blue;
				}
			}
		}

		public Color Color2
		{
			get
			{
				if (Muting)
				{
					return Colors.DarkRed;
				}
				else
				{
					return Colors.DarkBlue;
				}
			}
		}

		public Areas() {}

		public Areas(bool muting)
		{
			Muting = muting;
		}
	}
}
