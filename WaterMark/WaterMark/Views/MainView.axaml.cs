using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using System.IO;
using System.Linq;

namespace WaterMark.Views
{
    public partial class MainView : UserControl
    {
        private TopLevel topLevel;

        public MainView()
        {
            InitializeComponent();

            Loaded += MainView_Loaded;

            SelectFileButton.AddHandler(DragDrop.DropEvent, OnDrop);
            SelectWatermarkButton.AddHandler(DragDrop.DropEvent, OnDropWatermark);

            SelectFileButton.Click += OpenFile;
            SelectWatermarkButton.Click += OpenImageWatermark;
        }

        private void MainView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            topLevel = TopLevel.GetTopLevel(this);
        }

        private async void OpenFile(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new()
            {
                Title = "Select image",
                AllowMultiple = false,
                FileTypeFilter = [FilePickerFileTypes.ImageAll],
            });

            if (files.Count > 0) 
            {
                Image.Source = WriteableBitmap.Decode(await files[0].OpenReadAsync());
                Image.InvalidateVisual();
            }
        }

        private async void OpenImageWatermark(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new()
            {
                Title = "Select watermark",
                AllowMultiple = false,
                FileTypeFilter = [FilePickerFileTypes.ImageAll],
            });

            if (files.Count > 0)
            {
                WatermarkPreview.Source = WriteableBitmap.Decode(await files[0].OpenReadAsync());
                WatermarkPreview.InvalidateVisual();

                ImageWatermark.Source = WriteableBitmap.Decode(await files[0].OpenReadAsync());
                ImageWatermark.InvalidateVisual();
            }
        }

        public void OnDrop(object? sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Files))
            {
                ImageWatermark.Source = WriteableBitmap.Decode(File.OpenRead(e.Data.GetFiles().First().Path.AbsolutePath));
                ImageWatermark.InvalidateVisual();
            }
        }

        public void OnDropWatermark(object? sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Files))
            {
                WatermarkPreview.Source = WriteableBitmap.Decode(File.OpenRead(e.Data.GetFiles().First().Path.AbsolutePath));
                WatermarkPreview.InvalidateVisual();

                WatermarkPreview.Source = WriteableBitmap.Decode(File.OpenRead(e.Data.GetFiles().First().Path.AbsolutePath));
                WatermarkPreview.InvalidateVisual();
            }
        }
    }
}