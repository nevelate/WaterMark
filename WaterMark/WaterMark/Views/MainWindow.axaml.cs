using Avalonia.Controls;
using Avalonia.Media;
using FluentAvalonia.UI.Windowing;
using System.Collections.Generic;
using System.ComponentModel;

namespace WaterMark.Views
{
    public partial class MainWindow : AppWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            TitleBar.ExtendsContentIntoTitleBar = true;
            TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

            ChangeTransparency(4);
        }

        public void ChangeTransparency(int index)
        {
            switch (index)
            {
                case 0:
                    if (this.TryFindResource("ApplicationPageBackgroundThemeBrush", App.Current.ActualThemeVariant, out var value))
                    {
                        var brush = value as SolidColorBrush;
                        Background = brush;
                    }
                    TransparencyLevelHint = new List<WindowTransparencyLevel>()
                    {
                        WindowTransparencyLevel.None,
                    };
                    BackgroundBorder.IsVisible = false;
                    AccentAcrylicBorder.IsVisible = false;
                    AcrylicBorder.IsVisible = false;
                    break;
                case 1:
                    if (this.TryFindResource("ApplicationPageBackgroundThemeBrush", App.Current.ActualThemeVariant, out var color))
                    {
                        var brush = color as SolidColorBrush;
                        Background = brush;
                    }
                    TransparencyLevelHint = new List<WindowTransparencyLevel>()
                    {
                        WindowTransparencyLevel.None,
                    };
                    BackgroundBorder.IsVisible = true;
                    AccentAcrylicBorder.IsVisible = false;
                    AcrylicBorder.IsVisible = false;
                    break;
                case 2:
                    Background = Brushes.Transparent;
                    TransparencyLevelHint = new List<WindowTransparencyLevel>()
                        {
                            WindowTransparencyLevel.Mica,
                        };
                    BackgroundBorder.IsVisible = false;
                    AccentAcrylicBorder.IsVisible = false;
                    AcrylicBorder.IsVisible = false;
                    break;
                case 3:
                    Background = Brushes.Transparent;
                    TransparencyLevelHint = new List<WindowTransparencyLevel>()
                        {
                            WindowTransparencyLevel.AcrylicBlur,
                        };
                    BackgroundBorder.IsVisible = false;
                    AccentAcrylicBorder.IsVisible = false;
                    AcrylicBorder.IsVisible = true;
                    break;
                case 4:
                    Background = Brushes.Transparent;
                    TransparencyLevelHint = new List<WindowTransparencyLevel>()
                        {
                            WindowTransparencyLevel.AcrylicBlur,
                        };
                    BackgroundBorder.IsVisible = false;
                    AccentAcrylicBorder.IsVisible = true;
                    AcrylicBorder.IsVisible = false;
                    break;
            }
        }
    }
}