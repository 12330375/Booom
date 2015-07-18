using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MidTerm
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            
            DispatcherTimer dst = new DispatcherTimer();
            dst.Interval = new TimeSpan(0, 0, 1);
            dst.Tick += new EventHandler<object>(mytile1);
            dst.Start();


        }

         void mytile1(object o, object args)
        {
            Random radnum1 = new Random();
            int num = radnum1.Next(1,10);


            XmlDocument wideTitleData = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150ImageAndText01);
            XmlNodeList texts = wideTitleData.GetElementsByTagName("text");
            ((XmlElement)texts[0]).InnerText = "Take a photo";
            XmlElement img1 = (XmlElement)(wideTitleData.GetElementsByTagName("image")[0]);
            img1.SetAttribute("src", "ms-appx:///Assets/" + num.ToString() + ".png");
            img1.SetAttribute("alt", "colorful");
            TileNotification notification1 = new TileNotification(wideTitleData);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification1);




        }



        private void picture(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(main));
        }

        private void explanation(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(use1));
        }
    }
}
