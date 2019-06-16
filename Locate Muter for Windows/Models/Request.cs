using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locate_Muter_for_Windows.Models
{
	class Request
	{
		public bool considerIp = true;
		public List<WifiAccessPoint> wifiAccessPoints = new List<WifiAccessPoint>();
	}
}
