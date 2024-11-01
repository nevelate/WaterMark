using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WaterMark.Views
{
    public partial class MainView : UserControl
    {
        private TopLevel topLevel;
        private bool holding;

        private Point offset;
        private Control lastSelectedControl;

        public MainView()
        {
            InitializeComponent();

            Loaded += MainView_Loaded;

            SelectFileButton.AddHandler(DragDrop.DropEvent, OnDrop);
            SelectWatermarkButton.AddHandler(DragDrop.DropEvent, OnDropWatermark);

            SelectFileButton.Click += OpenFile;
            SelectWatermarkButton.Click += OpenImageWatermark;

            MainCanvas.PointerPressed += MainCanvas_PointerPressed;
            MainCanvas.PointerMoved += MainCanvas_PointerMoved;
            MainCanvas.PointerReleased += MainCanvas_PointerReleased;

            ImageWatermark.PropertyChanged += WatermarkPropertyChanged;
            TextWatermark.PropertyChanged += WatermarkPropertyChanged;

            fontComboBox.ItemsSource = FontManager.Current
                .SystemFonts
                .OrderBy(x => x.Name);
            fontComboBox.SelectedIndex = 0;
        }

        private async void WatermarkPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (SelectionBorder.IsVisible)
            {
                if (e.Property == TextBlock.TextProperty ||
                e.Property == TextBlock.FontSizeProperty ||
                e.Property == TextBlock.FontFamilyProperty ||
                e.Property == TextBlock.LineHeightProperty ||
                e.Property == TextBlock.FontStyleProperty ||
                e.Property == TextBlock.FontWeightProperty ||
                e.Property == WidthProperty ||
                e.Property == HeightProperty)
                {
                    await Task.Delay(50); // Костыли наше всё
                    SelectionBorder.Width = lastSelectedControl.Bounds.Width;
                    SelectionBorder.Height = lastSelectedControl.Bounds.Height;
                }
            }
        }

        private void MainCanvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            holding = false;
        }

        private void MainCanvas_PointerMoved(object? sender, PointerEventArgs e)
        {
            if (holding && e.Source is Control control)
            {
                if (MainCanvas.Children.Contains(control) && control.Name != "Image")
                {
                    Canvas.SetLeft(control, e.GetPosition(MainCanvas).X - offset.X);
                    Canvas.SetTop(control, e.GetPosition(MainCanvas).Y - offset.Y);

                    Canvas.SetLeft(SelectionBorder, Canvas.GetLeft(control));
                    Canvas.SetTop(SelectionBorder, Canvas.GetTop(control));
                }
            }
        }

        private void MainCanvas_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.Source is Control control)
            {
                if (MainCanvas.Children.Contains(control) && control.Name != "Image")
                {
                    SelectionBorder.Width = control.Bounds.Width;
                    SelectionBorder.Height = control.Bounds.Height;

                    Canvas.SetLeft(SelectionBorder, Canvas.GetLeft(control));
                    Canvas.SetTop(SelectionBorder, Canvas.GetTop(control));

                    SelectionBorder.IsVisible = true;

                    holding = true;
                    offset = e.GetPosition(control);
                    lastSelectedControl = control;
                }
                else
                {
                    SelectionBorder.IsVisible = false;
                }
            }
            else
            {
                SelectionBorder.IsVisible = false;
            }
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

                SelectFileButton.IsVisible = false;
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

                SelectFileButton.IsVisible = false;
            }
        }

        public void OnDropWatermark(object? sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Files))
            {
                ImageWatermark.Source = WriteableBitmap.Decode(File.OpenRead(e.Data.GetFiles().First().Path.AbsolutePath));
                ImageWatermark.InvalidateVisual();
            }
        }
    }
}