using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace ImageTest
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public MainPage()
        {
            this.InitializeComponent();

            Loaded += async (s, e) =>
            {
                PathText = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
                var files = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFilesAsync();
                files.ToList().ForEach(item => Items.Add(item.Path));
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _PathText;

        public string PathText
        {
            get { return _PathText; }
            set { _PathText = value; OnPropertyChanged(); }
        }

        private int _SelectedIndex = -1;

        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set { _SelectedIndex = value; OnPropertyChanged(); DisplayImage(value); }
        }

        private Visibility _ImageVisibility = Visibility.Collapsed;

        public Visibility ImageVisibility
        {
            get { return _ImageVisibility; }
            set { _ImageVisibility = value; OnPropertyChanged(); }
        }

        private ImageSource _ImageSource;

        public ImageSource ImageSource
        {
            get { return _ImageSource; }
            set { _ImageSource = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> Items { get; set; } = new ObservableCollection<string>();

        private void DisplayImage(int index)
        {
            if (index != -1)
            {
                myCanvas.Children.Clear();

                System.Diagnostics.Debug.WriteLine($"---- {Items[index]}");
                ImageSource = new BitmapImage(new Uri(Items[index]));
                for (int i = 0; i < 6; i++)
                {
                    var fileName = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Jpegs", $"{i}.jpeg");
                    System.Diagnostics.Debug.WriteLine($"---- {fileName}");
                    var image = new Image();
                    image.Loaded += (s, e) =>
                    {
                        System.Diagnostics.Debug.WriteLine($"image.Loaded. {fileName}");
                    };
                    image.SizeChanged += (s, e) =>
                    {
                        System.Diagnostics.Debug.WriteLine($"image.SizeChanged. {fileName}");
                    };
                    var bitmap = new BitmapImage(new Uri(fileName));
                    bitmap.ImageOpened += (s, e) =>
                    {
                        var ctl = s as BitmapImage;
                        System.Diagnostics.Debug.WriteLine($"bitmap.ImageOpened. {fileName}");
                        image.Width = ctl.PixelWidth * 70 / 100;
                        image.Height = ctl.PixelHeight * 70 / 100;
                    };
                    image.Source = bitmap;
                    Canvas.SetLeft(image, 100 * i);
                    Canvas.SetTop(image, 10 * i);
                    myCanvas.Children.Add(image);
                }

                System.Diagnostics.Debug.WriteLine($"Children.Count={myCanvas.Children.Count}");

                ImageVisibility = Visibility.Visible;
            }
        }

        public void ButtonClear()
        {
            ImageVisibility = Visibility.Collapsed;
            SelectedIndex = -1;
            // これは重要
            ImageSource = null;
        }

    }
}
