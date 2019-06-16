using System;
using System.Text;
using System.Device.Location;
using System.Web.Script.Serialization;
using Livet;

using LocateMuter_for_Windows.Models;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using Locate_Muter_for_Windows.Models;
using System.Net.Http;
using NativeWifi;
using System.Timers;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace LocateMuter_for_Windows.ViewModels
{
	public class MainWindowViewModel : ViewModel
	{
		/* コマンド、プロパティの定義にはそれぞれ 
         * 
         *  lvcom   : ViewModelCommand
         *  lvcomn  : ViewModelCommand(CanExecute無)
         *  llcom   : ListenerCommand(パラメータ有のコマンド)
         *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
         *  lprop   : 変更通知プロパティ(.NET4.5ではlpropn)
         *  
         * を使用してください。
         * 
         * Modelが十分にリッチであるならコマンドにこだわる必要はありません。
         * View側のコードビハインドを使用しないMVVMパターンの実装を行う場合でも、ViewModelにメソッドを定義し、
         * LivetCallMethodActionなどから直接メソッドを呼び出してください。
         * 
         * ViewModelのコマンドを呼び出せるLivetのすべてのビヘイビア・トリガー・アクションは
         * 同様に直接ViewModelのメソッドを呼び出し可能です。
         */

		/* ViewModelからViewを操作したい場合は、View側のコードビハインド無で処理を行いたい場合は
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信する事を検討してください。
         */

		/* Modelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedEventListenerや
         * CollectionChangedEventListenerを使うと便利です。各種ListenerはViewModelに定義されている
         * CompositeDisposableプロパティ(LivetCompositeDisposable型)に格納しておく事でイベント解放を容易に行えます。
         * 
         * ReactiveExtensionsなどを併用する場合は、ReactiveExtensionsのCompositeDisposableを
         * ViewModelのCompositeDisposableプロパティに格納しておくのを推奨します。
         * 
         * LivetのWindowテンプレートではViewのウィンドウが閉じる際にDataContextDisposeActionが動作するようになっており、
         * ViewModelのDisposeが呼ばれCompositeDisposableプロパティに格納されたすべてのIDisposable型のインスタンスが解放されます。
         * 
         * ViewModelを使いまわしたい時などは、ViewからDataContextDisposeActionを取り除くか、発動のタイミングをずらす事で対応可能です。
         */

		/* UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         * 
         * LivetのViewModelではプロパティ変更通知(RaisePropertyChanged)やDispatcherCollectionを使ったコレクション変更通知は
         * 自動的にUIDispatcher上での通知に変換されます。変更通知に際してUIDispatcherを操作する必要はありません。
         */

		private bool firstMoved = false;
		private GeoCoordinateWatcher wtc = new GeoCoordinateWatcher();
		private Timer timer = new Timer(10000);
		private string jsonPrevious = "";

		[System.Xml.Serialization.XmlIgnoreAttribute]
		public Map Map;

		#region PriorMode変更通知プロパティ
		private int _PriorMode;

		public int PriorMode
		{
			get
			{
				if (Map != null)
					reloadMap();
				return _PriorMode;
			}
			set
			{
				if (_PriorMode == value)
					return;
				_PriorMode = value;
				checkNow();
				RaisePropertyChanged();
				RaisePropertyChanged("AreasCollection");
			}
		}
		#endregion

		#region GapFillMode変更通知プロパティ
		private int _GapMuteMode;

		public int GapMuteMode
		{
			get
			{ return _GapMuteMode; }
			set
			{
				if (_GapMuteMode == value)
					return;
				_GapMuteMode = value;
				checkNow();
				RaisePropertyChanged();
			}
		}
		#endregion

		#region GpsSourceMode変更通知プロパティ
		private int _GpsSourceMode;

		public int GpsSourceMode
		{
			get
			{ return _GpsSourceMode; }
			set
			{ 
				if (_GpsSourceMode == value)
					return;
				_GpsSourceMode = value;
				checkStart();
				RaisePropertyChanged();
			}
		}
		#endregion

		#region AreasCollectionOrigin変更通知プロパティ
		private AreasCollection _AreasCollectionOrigin;

		public AreasCollection AreasCollectionOrigin
		{
			get
			{
				return _AreasCollectionOrigin;
			}
			set
			{
				if (_AreasCollectionOrigin == value)
					return;
				_AreasCollectionOrigin = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region AreasCollection変更通知プロパティ
		public AreasCollection AreasCollection
		{
			get
			{
				if (PriorMode == 0)
				{
					return AreasCollectionOrigin;
				}
				else
				{
					return AreasCollectionOrigin.getReversed();
				}
			}
			set
			{ 
				if (AreasCollectionOrigin == value)
					return;
				AreasCollectionOrigin = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		#region CurrentLocation変更通知プロパティ
		private Location _CurrentLocation;

		[System.Xml.Serialization.XmlIgnoreAttribute]
		public Location CurrentLocation
		{
			get
			{ return _CurrentLocation; }
			set
			{ 
				if (_CurrentLocation == value)
					return;
				_CurrentLocation = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		public void Initialize()
		{
			wtc.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(wtc_PositionChanged);
			timer.Elapsed += OnElapsed_Timer;

			AreasCollectionOrigin = new AreasCollection();
			AreasCollection = new AreasCollection();
			CurrentLocation = new Location();
			
			AreasCollection.addUnMutingArea(new Area("駅のほう", new Location(34.824783, 137.737663), 1000));
			AreasCollection.addMutingArea(new Area("静岡大学浜松キャンパス", new Location(34.724783, 137.717663), 300));
			AreasCollection.addMutingArea(new Area("浜松学院大学", new Location(34.720673, 137.713224), 200));

			checkStart();
		}

		public void checkNow()
		{
			if (GpsSourceMode == 0)
			{
				checkAreas(wtc.Position.Location);
			}
			else if (GpsSourceMode == 1)
			{
				jsonPrevious = "";
				checkGeoLocation();
			}
		}

		void checkStart()
		{
			if (GpsSourceMode == 0)
			{
				wtc.Start();
				timer.Stop();
				jsonPrevious = "";
			}
			else if (GpsSourceMode == 1)
			{
				wtc.Stop();
				timer.Start();
				checkGeoLocation();
			}
		}

		void wtc_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
		{
			checkAreas(e.Position.Location);
		}

		void OnElapsed_Timer(object sender, ElapsedEventArgs e)
		{
			checkGeoLocation();
		}

		void checkGeoLocation()
		{
			Request request = new Request();
			WlanClient wran = new WlanClient();
			StringBuilder sb = new StringBuilder();

			foreach (WlanClient.WlanInterface wlanIface in wran.Interfaces)
			{
				Wlan.WlanBssEntry[] wlanBssEntries = wlanIface.GetNetworkBssList();

				foreach (Wlan.WlanBssEntry wlanBssEntry in wlanBssEntries)
				{
					string macAddress = BitConverter.ToString(wlanBssEntry.dot11Bssid).Replace("-", ":").ToLower();
					double signalStrength = wlanBssEntry.rssi;

					WifiAccessPoint ap = new WifiAccessPoint(macAddress, signalStrength);
					request.wifiAccessPoints.Add(ap);
				}
			}

			var jsonSerialiser = new JavaScriptSerializer();
			string json = jsonSerialiser.Serialize(request);

			if (json == jsonPrevious)
			{
				return;
			}
			jsonPrevious = json;

			string url = "https://www.googleapis.com/geolocation/v1/geolocate?key=AIzaSyDOaWcuebrinjUZJo1gq1fN0l5M4UcYK7k";

			HttpClient client = new HttpClient();
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			var result = client.PostAsync(url, content).Result;
			var contents = result.Content.ReadAsStringAsync().Result;

			Response response = jsonSerialiser.Deserialize<Response>(contents);
			ResponseLocation rl = response.location;

			checkAreas(new GeoCoordinate(rl.lat, rl.lng));
		}

		void checkAreas(GeoCoordinate g)
		{
			Location location = new Location(g.Latitude, g.Longitude);
			CurrentLocation = location;

			if (!firstMoved && Map != null)
			{
				Map.ZoomLevel = 14;
				Map.Center = location;

				firstMoved = true;
			}

			int result = GapMuteMode;
			foreach (Areas areas in AreasCollection)
			{
				foreach (Area area in areas)
				{
					GeoCoordinate pos = new GeoCoordinate(area.Location.Latitude, area.Location.Longitude);
					if (g.GetDistanceTo(pos) <= area.Radius)
					{
						result = (areas.Muting) ? 1 : 2;
					}
				}
			}

			if (result == 1)
			{
				AudioManager.SetMasterVolumeMute(true);
			}
			else if (result == 2)
			{
				AudioManager.SetMasterVolumeMute(false);
			}
		}

		void reloadMap()
		{
			Location l = Map.Center;
			Map.Center = new Location();
			Map.Center = l;
		}
	}	

	public class BrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return new SolidColorBrush((Color)value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class BoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (int)value == -1 ? false : true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
