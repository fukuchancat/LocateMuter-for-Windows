using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using Livet;
using LocateMuter_for_Windows.Views;
using LocateMuter_for_Windows.ViewModels;
using System.IO;
using System.Xml.Serialization;

namespace LocateMuter_for_Windows
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application
	{
		MainWindow window;

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			MainWindowViewModel vm = new MainWindowViewModel();

			try
			{
				FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\" + "settings.xml", FileMode.Open);
				XmlSerializer serializer = new XmlSerializer(typeof(MainWindowViewModel));
				vm = (MainWindowViewModel)serializer.Deserialize(fs);
				fs.Close();
			}
			catch (FileNotFoundException)
			{
			}

			window = new MainWindow(vm);
			window.Show();
			//AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(MainWindowViewModel));
			FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\" + "settings.xml", FileMode.Create);
			serializer.Serialize(fs, window.DataContext);
			fs.Close();
		}

		//集約エラーハンドラ
		//private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		//{
		//    //TODO:ロギング処理など
		//    MessageBox.Show(
		//        "不明なエラーが発生しました。アプリケーションを終了します。",
		//        "エラー",
		//        MessageBoxButton.OK,
		//        MessageBoxImage.Error);
		//
		//    Environment.Exit(1);
		//}
	}
}
