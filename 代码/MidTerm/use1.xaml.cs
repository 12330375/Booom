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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MidTerm
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class use1 : Page
    {
        public use1()
        {
            this.InitializeComponent();
            
           
        }



        private void button1(object sender, RoutedEventArgs e)
        {
            text2.Visibility = Visibility.Collapsed;
            text1.Visibility = Visibility.Visible;
            second.Visibility = Visibility.Collapsed;
            one.Visibility = Visibility.Visible;
            right3.Visibility = Visibility.Collapsed;
            right1.Visibility = Visibility.Visible;
            right4.Visibility = Visibility.Collapsed;
            right2.Visibility = Visibility.Visible;
        }

        private void button2(object sender, RoutedEventArgs e)
        {
            text1.Visibility = Visibility.Collapsed;
            text2.Visibility = Visibility.Visible;
            one.Visibility = Visibility.Collapsed;
            second.Visibility = Visibility.Visible;
            right1.Visibility = Visibility.Collapsed;
            right3.Visibility = Visibility.Visible;
            right2.Visibility = Visibility.Collapsed;
            right4.Visibility = Visibility.Visible;
        }

        private void button3(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }


    }
}
