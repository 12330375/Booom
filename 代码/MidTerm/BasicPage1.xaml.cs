using MidTerm.Common;
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
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel;
// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace MidTerm
{ /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class BasicPage1 : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        //申明图像变量
        public BitmapImage srcBitmapImage;
        public WriteableBitmap srcWriteAbleBitmap;
        public WriteableBitmap clone;
        private bool isReady;

        private WriteableBitmap cloneForSlider;
        private bool needClone;
        private Guid decoderId;

        /// <summary>
        /// 可将其更改为强类型视图模型。
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper 在每页上用于协助导航和
        /// 进程生命期管理
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        StorageFile file;

        public BasicPage1()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            DataTransferManager.GetForCurrentView().DataRequested += MainPage_DataRequested;
            IMG.Source = App._photo.ImageSource;
            file = App._photo.Img;
            isReady = false;
            needClone = true;
            preRead();

        }

        /// <summary>
        ///使用在导航过程中传递的内容填充页。 在从以前的会话
        /// 重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="sender">
        /// 事件的来源; 通常为 <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">事件数据，其中既提供在最初请求此页时传递给
        /// <see cref="Frame.Navigate(Type, Object)"/> 的导航参数，又提供
        /// 此页在以前会话期间保留的状态的
        /// 的字典。 首次访问页面时，该状态将为 null。</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。  值必须符合
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        ///<param name="sender">事件的来源；通常为 <see cref="NavigationHelper"/></param>
        ///<param name="e">提供要使用可序列化状态填充的空字典
        ///的事件数据。</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper 注册

        /// 此部分中提供的方法只是用于使
        /// NavigationHelper 可响应页面的导航方法。
        /// 
        /// 应将页面特有的逻辑放入用于
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// 和 <see cref="GridCS.Common.NavigationHelper.SaveState"/> 的事件处理程序中。
        /// 除了在会话期间保留的页面状态之外
        /// LoadState 方法中还提供导航参数。

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void preRead()
        {
            show.Text = "稍等一下，图片还没加载完呢。-V-";

            if (file != null)
            {
                srcBitmapImage = new BitmapImage();
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    srcBitmapImage.SetSource(stream);
                    switch (file.FileType.ToLower())
                    {
                        case ".jpg":
                            decoderId = BitmapDecoder.JpegDecoderId;
                            break;
                        case ".jpeg":
                            decoderId = BitmapDecoder.JpegDecoderId;
                            break;
                        case ".bmp":
                            decoderId = BitmapDecoder.BmpDecoderId;
                            break;
                        case ".png":
                            decoderId = BitmapDecoder.PngDecoderId;
                            break;
                        default:
                            return;
                    }
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(decoderId, stream);
                    int width = (int)decoder.PixelWidth;
                    int height = (int)decoder.PixelHeight;
                    PixelDataProvider dataProvider = await decoder.GetPixelDataAsync();
                    byte[] pixels = dataProvider.DetachPixelData();
                    srcWriteAbleBitmap = new WriteableBitmap(width, height);
                    Stream pixelStream = srcWriteAbleBitmap.PixelBuffer.AsStream();
                    pixelStream.Write(pixels, 0, pixels.Length);
                    pixelStream.Dispose();
                    clone = new WriteableBitmap(width, height);
                    Stream pixelStreamClone = clone.PixelBuffer.AsStream();
                    pixelStreamClone.Write(pixels, 0, pixels.Length);
                    pixelStreamClone.Dispose();
                    stream.Dispose();
                }
                IMG.Source = srcWriteAbleBitmap;
                show.Text = "图片加载完成 可以进行处理了=v= 效果是可以叠加的.如果不需叠加请在每次添加新的效果时先[[重置]]噢";
                isReady = true;
            }
        }


        private async void _save_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker save = new FileSavePicker();
            save.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            save.DefaultFileExtension = ".jpg";
            save.SuggestedFileName = "new_image";
            save.FileTypeChoices.Add(".bmp", new List<string>() { ".bmp" });
            save.FileTypeChoices.Add(".png", new List<string>() { ".png" });
            save.FileTypeChoices.Add(".jpg", new List<string>() { ".jpg", ".jpeg" });
            StorageFile saveItem = await save.PickSaveFileAsync();
            if (saveItem == null) return;
            try
            {
                Guid encoderId;
                switch (saveItem.FileType.ToLower())
                {
                    case ".jpg":
                        encoderId = BitmapEncoder.JpegEncoderId;
                        break;
                    case ".bmp":
                        encoderId = BitmapEncoder.BmpEncoderId;
                        break;
                    case ".png":
                        encoderId = BitmapEncoder.PngEncoderId;
                        break;
                    default:
                        encoderId = BitmapEncoder.PngEncoderId;
                        break;
                }
                IRandomAccessStream fileStream = await saveItem.OpenAsync(FileAccessMode.ReadWrite);
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encoderId, fileStream);
                Stream pixelStream = srcWriteAbleBitmap.PixelBuffer.AsStream();
                byte[] pixels = new byte[pixelStream.Length];
                pixelStream.Read(pixels, 0, pixels.Length);
                for (int i = 0; i < pixels.Length; i += 4)
                {
                    byte temp = pixels[i];
                    pixels[i] = pixels[i + 2];
                    pixels[i + 2] = temp;
                }
                encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight,
                    (uint)srcWriteAbleBitmap.PixelWidth, (uint)srcWriteAbleBitmap.PixelHeight, 96, 96, pixels);
                await encoder.FlushAsync();

            }
            catch (Exception etemp)
            {
                throw etemp;
            }
        }

        private void _fudiao_Click(object sender, RoutedEventArgs e)
        {
            int height = srcWriteAbleBitmap.PixelHeight;
            int width = srcWriteAbleBitmap.PixelWidth;
            try
            {
                WriteableBitmap desImage = new WriteableBitmap(width, height);
                byte[] temp = srcWriteAbleBitmap.PixelBuffer.ToArray();

                int b = 0, g = 0, r = 0;
                for (int j = 0; j < height - 1; j++)
                {
                    for (int i = 0; i < width * 4 - 4; i += 4)
                    {
                        b = Math.Abs(temp[i + j * width * 4] - temp[i + 4 + j * width * 4] + 128);
                        g = Math.Abs(temp[i + 1 + j * width * 4] - temp[i + 1 + 4 + j * width * 4] + 128);
                        r = Math.Abs(temp[i + 2 + j * width * 4] - temp[i + 2 + 4 + j * width * 4] + 128);
                        temp[i + j * width * 4] = (byte)(b > 0 ? (b < 255 ? b : 255) : 0);
                        temp[i + 1 + j * width * 4] = (byte)(g > 0 ? (g < 255 ? g : 255) : 0);
                        temp[i + 2 + j * width * 4] = (byte)(r > 0 ? (r < 255 ? r : 255) : 0);
                    }
                }
                for (int i = 0; i < width * 4; i += 4)
                {
                    temp[i + (height - 1) * width * 4] = temp[i + ((height - 1) - 1) * width * 4];
                    temp[i + 1 + (height - 1) * width * 4] = temp[i + 1 + ((height - 1) - 1) * width * 4];
                    temp[i + 2 + (height - 1) * width * 4] = temp[i + 2 + ((height - 1) - 1) * width * 4];
                }
                for (int j = 0; j < height; j++)
                {
                    temp[width * 4 - 4 + j * width * 4] = temp[width * 4 - 4 - 4 + j * width * 4];
                    temp[width * 4 - 4 + 1 + j * width * 4] = temp[width * 4 - 4 + 1 - 4 + j * width * 4];
                    temp[width * 4 - 4 + 2 + j * width * 4] = temp[width * 4 - 4 + 2 - 4 + j * width * 4];
                }
                srcWriteAbleBitmap = new WriteableBitmap(width, height);
                Stream sTemp = srcWriteAbleBitmap.PixelBuffer.AsStream();
                sTemp.Seek(0, SeekOrigin.Begin);
                sTemp.Write(temp, 0, width * 4 * height);
                IMG.Source = srcWriteAbleBitmap;
                needClone = true;
            }
            catch (Exception etemp)
            {
                throw etemp;
            }
        }

        private void resetFun()
        {
            int width = cloneForSlider.PixelWidth;
            int height = cloneForSlider.PixelHeight;
            try
            {

                byte[] temp = cloneForSlider.PixelBuffer.ToArray();
                srcWriteAbleBitmap = new WriteableBitmap(width, height);
                Stream sTemp = srcWriteAbleBitmap.PixelBuffer.AsStream();
                sTemp.Seek(0, SeekOrigin.Begin);
                sTemp.Write(temp, 0, width * 4 * height);
                IMG.Source = srcWriteAbleBitmap;
            }
            catch (Exception etemp)
            {
                throw etemp;
            }
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            int width = clone.PixelWidth;
            int height = clone.PixelHeight;
            try
            {

                byte[] temp = clone.PixelBuffer.ToArray();
                srcWriteAbleBitmap = new WriteableBitmap(width, height);
                Stream sTemp = srcWriteAbleBitmap.PixelBuffer.AsStream();
                sTemp.Seek(0, SeekOrigin.Begin);
                sTemp.Write(temp, 0, width * 4 * height);
                contrastChange.Value = 50;
                brightChange.Value = 50;
                needClone = true;
                IMG.Source = srcWriteAbleBitmap;

            }
            catch (Exception etemp)
            {
                throw etemp;
            }
        }

        private void gray_Click(object sender, RoutedEventArgs e)
        {
            int height = srcWriteAbleBitmap.PixelHeight;
            int width = srcWriteAbleBitmap.PixelWidth;
            try
            {
                WriteableBitmap desImage = new WriteableBitmap(width, height);
                byte[] temp = srcWriteAbleBitmap.PixelBuffer.ToArray();

                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width * 4; i += 4)
                    {
                        int avg = ((temp[i + j * width * 4] + temp[i + 1 + j * width * 4] + temp[i + 2 + j * width * 4]) / 3) > 100 ? 255 : 0;
                        temp[i + j * width * 4] = (byte)(avg);
                        temp[i + 1 + j * width * 4] = (byte)(avg);
                        temp[i + 2 + j * width * 4] = (byte)(avg);
                    }
                }
                srcWriteAbleBitmap = new WriteableBitmap(width, height);
                Stream sTemp = srcWriteAbleBitmap.PixelBuffer.AsStream();
                sTemp.Seek(0, SeekOrigin.Begin);
                sTemp.Write(temp, 0, width * 4 * height);
                IMG.Source = srcWriteAbleBitmap;
                needClone = true;
            }
            catch (Exception etemp)
            {
                throw etemp;
            }
        }

        private void _dipian_Click(object sender, RoutedEventArgs e)
        {
            int height = srcWriteAbleBitmap.PixelHeight;
            int width = srcWriteAbleBitmap.PixelWidth;
            try
            {
                WriteableBitmap desImage = new WriteableBitmap(width, height);
                byte[] temp = srcWriteAbleBitmap.PixelBuffer.ToArray();

                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width * 4; i += 4)
                    {
                        temp[i + j * width * 4] = (byte)(255 - temp[i + j * width * 4]);
                        temp[i + 1 + j * width * 4] = (byte)(255 - temp[i + 1 + j * width * 4]);
                        temp[i + 2 + j * width * 4] = (byte)(255 - temp[i + 2 + j * width * 4]);
                    }
                }
                srcWriteAbleBitmap = new WriteableBitmap(width, height);
                Stream sTemp = srcWriteAbleBitmap.PixelBuffer.AsStream();
                sTemp.Seek(0, SeekOrigin.Begin);
                sTemp.Write(temp, 0, width * 4 * height);
                IMG.Source = srcWriteAbleBitmap;
                needClone = true;
            }
            catch (Exception etemp)
            {
                throw etemp;
            }
        }

        private void meanFilter_Click(object sender, RoutedEventArgs e)
        {
            int height = srcWriteAbleBitmap.PixelHeight;
            int width = srcWriteAbleBitmap.PixelWidth;
            try
            {
                WriteableBitmap desImage = new WriteableBitmap(width, height);
                byte[] temp = srcWriteAbleBitmap.PixelBuffer.ToArray();

                byte[] tempMask = (byte[])temp.Clone();

                for (int j = 2; j < height - 2; j++)
                {
                    for (int i = 8; i < width * 4 - 8; i += 4)
                    {
                        int avgb = 0, avgg = 0, avgr = 0;
                        for (int m = -2; m <= 2; m++)
                        {
                            for (int n = -8; n <= 8; n += 4)
                            {
                                avgb += tempMask[i + n + (j + m) * width * 4];
                                avgg += tempMask[i + n + (j + m) * width * 4 + 1];
                                avgr += tempMask[i + n + (j + m) * width * 4 + 2];
                            }
                        }
                        temp[i + j * width * 4] = (byte)(avgb / 25);
                        temp[i + 1 + j * width * 4] = (byte)(avgg / 25);
                        temp[i + 2 + j * width * 4] = (byte)(avgr / 25);
                    }
                }
                srcWriteAbleBitmap = new WriteableBitmap(width, height);
                Stream sTemp = srcWriteAbleBitmap.PixelBuffer.AsStream();
                sTemp.Seek(0, SeekOrigin.Begin);
                sTemp.Write(temp, 0, width * 4 * height);
                IMG.Source = srcWriteAbleBitmap;
                needClone = true;
            }
            catch (Exception etemp)
            {
                throw etemp;
            }
        }

        private void gray2_Click(object sender, RoutedEventArgs e)
        {
            int height = srcWriteAbleBitmap.PixelHeight;
            int width = srcWriteAbleBitmap.PixelWidth;
            try
            {
                WriteableBitmap desImage = new WriteableBitmap(width, height);
                byte[] temp = srcWriteAbleBitmap.PixelBuffer.ToArray();

                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width * 4; i += 4)
                    {
                        int avg = ((temp[i + j * width * 4] + temp[i + 1 + j * width * 4] + temp[i + 2 + j * width * 4]) / 3);
                        temp[i + j * width * 4] = (byte)(avg);
                        temp[i + 1 + j * width * 4] = (byte)(avg);
                        temp[i + 2 + j * width * 4] = (byte)(avg);
                    }
                }
                srcWriteAbleBitmap = new WriteableBitmap(width, height);
                Stream sTemp = srcWriteAbleBitmap.PixelBuffer.AsStream();
                sTemp.Seek(0, SeekOrigin.Begin);
                sTemp.Write(temp, 0, width * 4 * height);
                IMG.Source = srcWriteAbleBitmap;
                needClone = true;
            }
            catch (Exception etemp)
            {
                throw etemp;
            }
        }

        private void neon_Click(object sender, RoutedEventArgs e)
        {
            int height = srcWriteAbleBitmap.PixelHeight;
            int width = srcWriteAbleBitmap.PixelWidth;
            try
            {
                byte[] temp = srcWriteAbleBitmap.PixelBuffer.ToArray();
                byte[] tempMask = (byte[])temp.Clone();
                int r = 0, g = 0, b = 0;
                for (int j = 0; j < height - 1; j++)
                {
                    for (int i = 0; i < width * 4 - 4; i += 4)
                    {
                        b = (int)Math.Sqrt((tempMask[i + j * width * 4] - tempMask[i + 4 + j * width * 4]) * (tempMask[i + j * width * 4]
                            - tempMask[i + 4 + j * width * 4]) + (tempMask[i + j * width * 4] - tempMask[i + (j + 1) * width * 4]) * (tempMask[i + j * width * 4]
                            - tempMask[i + (j + 1) * width * 4]));
                        g = (int)Math.Sqrt((tempMask[i + 1 + j * width * 4] - tempMask[i + 1 + 4 + j * width * 4]) * (tempMask[i + 1 + j * width * 4]
                             - tempMask[i + 1 + 4 + j * width * 4]) + (tempMask[i + 1 + j * width * 4] - tempMask[i + 1 + (j + 1) * width * 4]) * (tempMask[i + 1 + j * width * 4]
                             - tempMask[i + 1 + (j + 1) * width * 4]));
                        b = (int)Math.Sqrt((tempMask[i + 2 + j * width * 4] - tempMask[i + 2 + 4 + j * width * 4]) * (tempMask[i + 2 + j * width * 4]
                             - tempMask[i + 2 + 4 + j * width * 4]) + (tempMask[i + 2 + j * width * 4] - tempMask[i + 2 + (j + 1) * width * 4]) * (tempMask[i + 2 + j * width * 4]
                             - tempMask[i + 2 + (j + 1) * width * 4]));
                        temp[i + j * width * 4] = (byte)(b > 0 ? (b < 255 ? b : 255) : 0);
                        temp[i + 1 + j * width * 4] = (byte)(g > 0 ? (g < 255 ? g : 255) : 0);
                        temp[i + 2 + j * width * 4] = (byte)(r > 0 ? (r < 255 ? r : 255) : 0);
                    }
                }
                for (int i = 0; i < width * 4; i += 4)
                {
                    temp[i + (height - 1) * width * 4] = temp[i + ((height - 1) - 1) * width * 4];
                    temp[i + 1 + (height - 1) * width * 4] = temp[i + 1 + ((height - 1) - 1) * width * 4];
                    temp[i + 2 + (height - 1) * width * 4] = temp[i + 2 + ((height - 1) - 1) * width * 4];
                }
                for (int j = 0; j < height; j++)
                {
                    temp[width * 4 - 4 + j * width * 4] = temp[width * 4 - 4 - 4 + j * width * 4];
                    temp[width * 4 - 4 + 1 + j * width * 4] = temp[width * 4 - 4 + 1 - 4 + j * width * 4];
                    temp[width * 4 - 4 + 2 + j * width * 4] = temp[width * 4 - 4 + 2 - 4 + j * width * 4];
                }
                srcWriteAbleBitmap = new WriteableBitmap(width, height);
                Stream sTemp = srcWriteAbleBitmap.PixelBuffer.AsStream();
                sTemp.Seek(0, SeekOrigin.Begin);
                sTemp.Write(temp, 0, width * 4 * height);
                IMG.Source = srcWriteAbleBitmap;
                needClone = true;
            }
            catch (Exception etemp)
            {
                throw etemp;
            }
        }

        private void bright()
        {
            int bChange = (int)(brightChange.Value - 50) * 3;
            int height = srcWriteAbleBitmap.PixelHeight;
            int width = srcWriteAbleBitmap.PixelWidth;
            try
            {
                byte[] temp = srcWriteAbleBitmap.PixelBuffer.ToArray();

                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width * 4; i += 4)
                    {
                        temp[i + j * width * 4] = (byte)((temp[i + j * width * 4] + bChange) > 0 ? ((temp[i + j * width * 4] + bChange) < 255 ? (temp[i + j * width * 4] + bChange) : 255) : 0);
                        temp[i + 1 + j * width * 4] = (byte)((temp[i + 1 + j * width * 4] + bChange) > 0 ? ((temp[i + 1 + j * width * 4] + bChange) < 255 ? (temp[i + 1 + j * width * 4] + bChange) : 255) : 0);
                        temp[i + 2 + j * width * 4] = (byte)((temp[i + 2 + j * width * 4] + bChange) > 0 ? ((temp[i + 2 + j * width * 4] + bChange) < 255 ? (temp[i + 2 + j * width * 4] + bChange) : 255) : 0);
                    }
                }
                srcWriteAbleBitmap = new WriteableBitmap(width, height);
                Stream sTemp = srcWriteAbleBitmap.PixelBuffer.AsStream();
                sTemp.Seek(0, SeekOrigin.Begin);
                sTemp.Write(temp, 0, width * 4 * height);
                IMG.Source = srcWriteAbleBitmap;
            }
            catch (Exception etemp)
            {
                throw etemp;
            }
        }

        private void contrast()
        {
            double cChange = (contrastChange.Value) / 50;
            int height = srcWriteAbleBitmap.PixelHeight;
            int width = srcWriteAbleBitmap.PixelWidth;
            try
            {
                byte[] temp = srcWriteAbleBitmap.PixelBuffer.ToArray();
                byte[] tempMask = (byte[])temp.Clone();

                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width * 4; i += 4)
                    {
                        int tempRGB = (int)((tempMask[i + j * width * 4] - 127) * cChange + 127);
                        temp[i + j * width * 4] = (byte)(tempRGB > 0 ? (tempRGB < 255 ? tempRGB : 255) : 0);
                        tempRGB = (int)((tempMask[i + 1 + j * width * 4] - 127) * cChange + 127);
                        temp[i + 1 + j * width * 4] = (byte)(tempRGB > 0 ? (tempRGB < 255 ? tempRGB : 255) : 0);
                        tempRGB = (int)((tempMask[i + 2 + j * width * 4] - 127) * cChange + 127);
                        temp[i + 2 + j * width * 4] = (byte)(tempRGB > 0 ? (tempRGB < 255 ? tempRGB : 255) : 0);
                    }
                }
                srcWriteAbleBitmap = new WriteableBitmap(width, height);
                Stream sTemp = srcWriteAbleBitmap.PixelBuffer.AsStream();
                sTemp.Seek(0, SeekOrigin.Begin);
                sTemp.Write(temp, 0, width * 4 * height);
                IMG.Source = srcWriteAbleBitmap;
            }
            catch (Exception etemp)
            {
                throw etemp;
            }
        }


        private void brightChange_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!isReady) return;
            if (needClone)
            {
                cloneForSlider = new WriteableBitmap(srcWriteAbleBitmap.PixelWidth, srcWriteAbleBitmap.PixelHeight);
                byte[] temp = srcWriteAbleBitmap.PixelBuffer.ToArray();
                Stream sTemp = cloneForSlider.PixelBuffer.AsStream();
                sTemp.Seek(0, SeekOrigin.Begin);
                sTemp.Write(temp, 0, srcWriteAbleBitmap.PixelWidth * 4 * srcWriteAbleBitmap.PixelHeight);
                IMG.Source = srcWriteAbleBitmap;
                needClone = false;
            }
            resetFun();
            contrast();
            bright();
        }

        private void contrastChange_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (!isReady) return;
            if (needClone)
            {
                cloneForSlider = new WriteableBitmap(srcWriteAbleBitmap.PixelWidth, srcWriteAbleBitmap.PixelHeight);
                byte[] temp = srcWriteAbleBitmap.PixelBuffer.ToArray();
                Stream sTemp = cloneForSlider.PixelBuffer.AsStream();
                sTemp.Seek(0, SeekOrigin.Begin);
                sTemp.Write(temp, 0, srcWriteAbleBitmap.PixelWidth * 4 * srcWriteAbleBitmap.PixelHeight);
                IMG.Source = srcWriteAbleBitmap;
                needClone = false;
            }
            resetFun();
            contrast();
            bright();
        }

        private void woodCut_Click(object sender, RoutedEventArgs e)
        {
            int height = srcWriteAbleBitmap.PixelHeight;
            int width = srcWriteAbleBitmap.PixelWidth;
            try
            {
                byte[] temp = srcWriteAbleBitmap.PixelBuffer.ToArray();

                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width * 4; i += 4)
                    {
                        int tempRGB = (int)((temp[i + j * width * 4] + temp[i + 1 + j * width * 4] + temp[i + 2 + j * width * 4]) / 3);
                        temp[i + j * width * 4] = (byte)(tempRGB > 122.5 ? 0 : 255);
                        temp[i + 1 + j * width * 4] = (byte)(tempRGB > 122.5 ? 0 : 255);
                        temp[i + 2 + j * width * 4] = (byte)(tempRGB > 122.5 ? 0 : 255);
                    }
                }
                srcWriteAbleBitmap = new WriteableBitmap(width, height);
                Stream sTemp = srcWriteAbleBitmap.PixelBuffer.AsStream();
                sTemp.Seek(0, SeekOrigin.Begin);
                sTemp.Write(temp, 0, width * 4 * height);
                IMG.Source = srcWriteAbleBitmap;
            }
            catch (Exception etemp)
            {
                throw etemp;
            }
        }



        private async void MainPage_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var GetFiles = args.Request.GetDeferral();
            try
            {
                var name = String.Format("{0}_{1:yyyy-MM-dd_HH-mm-ss}.jpg", "MyApp", DateTime.Now);
                StorageFolder tempFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("MidTermTemp", CreationCollisionOption.ReplaceExisting);
                StorageFile saveItem = await tempFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
                byte[] pixels;
                using (Stream pixelStream = srcWriteAbleBitmap.PixelBuffer.AsStream())
                {
                    pixels = new byte[pixelStream.Length];
                    pixelStream.Read(pixels, 0, pixels.Length);
                    for (int i = 0; i < pixels.Length; i += 4)
                    {
                        byte temp = pixels[i];
                        pixels[i] = pixels[i + 2];
                        pixels[i + 2] = temp;
                    }
                }
                using (IRandomAccessStream fileStream = await saveItem.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, fileStream);
                    encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight,
                        (uint)srcWriteAbleBitmap.PixelWidth, (uint)srcWriteAbleBitmap.PixelHeight, 96, 96, pixels);
                    await encoder.FlushAsync();
                    using (var outputStream = fileStream.GetOutputStreamAt(0))
                    {
                        await outputStream.FlushAsync();
                    }
                }

                DataPackage data = new DataPackage();
                data.Properties.Title = "共享图片";
                data.Properties.Description = "分享一些内容。";
                List<IStorageItem> files = new List<IStorageItem>();
                files.Add(saveItem);
                data.SetStorageItems(files);
                args.Request.Data = data;
            }
            finally
            {
                GetFiles.Complete();
            }
        }

        private void share_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

  
       
    }
}
