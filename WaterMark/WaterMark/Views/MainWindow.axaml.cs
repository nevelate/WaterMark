using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;
using DynamicData;
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
            
            ChangeTransparency(3);
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
    }
}