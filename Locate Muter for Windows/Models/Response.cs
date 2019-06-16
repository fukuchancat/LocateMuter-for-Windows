using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locate_Muter_for_Windows.Models
{
	class Response
	{
		public ResponseLocation location;
		public double accuracy;
	}

	class ResponseLocation
	{
		public double lat;
		public double lng;
	}
}
