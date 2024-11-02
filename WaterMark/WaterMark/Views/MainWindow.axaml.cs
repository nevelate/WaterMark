using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using DynamicData;
using FluentAvalonia.UI.Windowing;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WaterMark.Views
{
    public partial class MainWindow : AppWindow
    {
        private TopLevel topLevel;
        private bool holding;

        private Point offset;
        private Control lastSelectedControl;

        public MainWindow()
        {
            InitializeComponent();

            TitleBar.ExtendsContentIntoTitleBar = true;
            TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;
            
            ChangeTransparency(3);

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

        private void ChangeBackdrop(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if(e.Source is MenuItem menuItem)
            {
                ChangeTransparency((sender as Control).GetLogicalChildren().IndexOf(menuItem) - 1);
            }
        }

        private void ChangeTheme(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (e.Source is MenuItem menuItem)
            {
                App.Current.RequestedThemeVariant = (sender as Control).GetLogicalChildren().IndexOf(menuItem) switch
                {
                    1 => ThemeVariant.Default,
                    2 => ThemeVariant.Light,
                    3 => ThemeVariant.Dark,
                };
            }
        }

        public void ChangeTransparency(int index)
        {
            switch (index)
            {
                case 0:
                    if (this.TryFindResource("ApplicationPageBackgroundThemeBrush", App.Current?.ActualThemeVariant, out var value))
                    {
                        var brush = value as SolidColorBrush;
                        Background = brush;
                    }
                    TransparencyLevelHint = new List<WindowTransparencyLevel>()
                    {
                        WindowTransparencyLevel.None,
                    };
                    AccentAcrylicBorder.IsVisible = false;
                    AcrylicBorder.IsVisible = false;
                    break;
                case 1:
                    Background = Brushes.Transparent;
                    TransparencyLevelHint = new List<WindowTransparencyLevel>()
                        {
                            WindowTransparencyLevel.Mica,
                        };
                    AccentAcrylicBorder.IsVisible = false;
                    AcrylicBorder.IsVisible = false;
                    break;
                case 2:
                    Background = Brushes.Transparent;
                    TransparencyLevelHint = new List<WindowTransparencyLevel>()
                        {
                            WindowTransparencyLevel.AcrylicBlur,
                        };
                    AccentAcrylicBorder.IsVisible = false;
                    AcrylicBorder.IsVisible = true;
                    break;
                case 3:
                    Background = Brushes.Transparent;
                    TransparencyLevelHint = new List<WindowTransparencyLevel>()
                        {
                            WindowTransparencyLevel.AcrylicBlur,
                        };
                    AccentAcrylicBorder.IsVisible = true;
                    AcrylicBorder.IsVisible = false;
                    break;
            }
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
                    await Task.Delay(50); // ������� ���� ��
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