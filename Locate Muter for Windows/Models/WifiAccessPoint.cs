namespace Locate_Muter_for_Windows.Models
{
	class WifiAccessPoint
	{
		public string macAddress;
		public double signalStrength;
		
		public WifiAccessPoint(string macAddress, double signalStrength)
		{
			this.macAddress = macAddress;
			this.signalStrength = signalStrength;
		}
	}
}
