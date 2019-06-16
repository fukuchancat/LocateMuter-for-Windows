using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Media;

namespace LocateMuter_for_Windows.Models
{
	public class Area : NotificationObject
	{
		#region Name変更通知プロパティ
		private string _Name;

		public string Name
		{
			get
			{ return _Name; }
			set
			{
				if (_Name == value)
					return;
				_Name = value;
				RaisePropertyChanged("Name");
			}
		}
		#endregion

		#region Location変更通知プロパティ
		private Location _Location;

		public Location Location
		{
			get
			{ return _Location; }
			set
			{
				if (_Location == value)
					return;
				_Location = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region Radius変更通知プロパティ
		private double _Radius;

		public double Radius
		{
			get
			{ return _Radius; }
			set
			{
				if (_Radius == value)
					return;
				_Radius = value;
				RaisePropertyChanged("Radius");
			}
		}
		#endregion

		[System.Xml.Serialization.XmlIgnoreAttribute]
		public LocationCollection EdgeLocations
		{
			get
			{
				return getCircledLocations(Location, Radius);
			}
		}

		public Area() {}

		public Area(string name, Location location, double radius)
		{
			Name = name;
			Location = location;
			Radius = radius;
		}

		LocationCollection getCircledLocations(Location location, double length)
		{
			LocationCollection locations = new LocationCollection();
			for (int r = 0; r < 360; r += 10)
			{
				Location l = getMovedLocation(location, length, r);
				locations.Add(l);
			}

			return locations;
		}

		Location getMovedLocation(Location location, double distance, double heading)
		{
			double latitude = location.Latitude;
			double longitude = location.Longitude;

			double earth_radius = 6378150;

			double latitude_distance = distance * Math.Cos(heading * Math.PI / 180);
			double earth_circle = 2 * Math.PI * earth_radius;

			double latitude_per_meter = 360 / earth_circle;
			double latitude_delta = latitude_distance * latitude_per_meter;

			double new_latitude = latitude + latitude_delta;

			double longitude_distance = distance * Math.Sin(heading * Math.PI / 180);
			double earth_radius_at_longitude = earth_radius * Math.Cos(new_latitude * Math.PI / 180);
			double earth_circle_at_longitude = 2 * Math.PI * earth_radius_at_longitude;

			double longitude_per_meter = 360 / earth_circle_at_longitude;
			double longitude_delta = longitude_distance * longitude_per_meter;

			double new_longtitude = longitude + longitude_delta;

			return new Location(new_latitude, new_longtitude);
		}
	}
}
