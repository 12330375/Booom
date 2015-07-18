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
using Windows.Media.Capture;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MidTerm
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class main : Page
    {
        public main()
        {
            this.InitializeComponent();
            this.gridView.ItemsSource = App.Ps;
            imgname.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            save.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var camera = new CameraCaptureUI();
            var file = await camera.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (file!=null) App.Ps.AddIMG(file);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int ind = gridView.SelectedIndex;
            App.Ps.Remove((Photo) gridView.SelectedItem);
        }
        public class Photo
        {
            public StorageFile Img { get; set; }
            public BitmapImage ImageSource { get; set; }
        }
        public class Photos : System.Collections.ObjectModel.ObservableCollection<Photo>
        {
            public async void AddIMG(StorageFile img)
            {
                Photo t = new Photo();
                t.Img = img;
                using (IRandomAccessStream fileStream = await img.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    t.ImageSource = bitmapImage;
                } 
                this.Add(t);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (gridView.SelectedIndex == -1) return;
            App._photo= (Photo)gridView.SelectedItem;
            this.Frame.Navigate(typeof(BasicPage1));
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            BitmapImage BitmapImage = new BitmapImage();
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            openPicker.FileTypeFilter.Add(".bmp");
            StorageFile my_file = await openPicker.PickSingleFileAsync();
            if (my_file!=null) App.Ps.AddIMG(my_file);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            int k = gridView.SelectedIndex;
            if (k < 0) return;
            imgname.Visibility = Windows.UI.Xaml.Visibility.Visible;
            save.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
        private void return1_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private async void save_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder picLib = KnownFolders.PicturesLibrary; 
            StorageFile img = ((Photo)gridView.SelectedItem).Img;
            StorageFile file = await picLib.CreateFileAsync(imgname.Text.ToString() + img.FileType.ToLower(), CreationCollisionOption.ReplaceExisting);
           
            IBuffer x = await FileIO.ReadBufferAsync(img);
            await FileIO.WriteBufferAsync(file, x);
            imgname.Text = "输入文件名";
            imgname.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            save.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
        
    }
}
