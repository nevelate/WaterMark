using FluentAvalonia.UI.Controls;
using ReactiveUI;
using System.Diagnostics;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace WaterMark.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool isChained;
		private double watermarkWidth;
        private double watermarkHeight;

        public bool IsChained
		{
			get => isChained;
			set => this.RaiseAndSetIfChanged(ref isChained, value);
		}

		public double WatermarkWidth
		{
			get => watermarkWidth;
			set 
			{
				this.RaiseAndSetIfChanged(ref watermarkWidth, value);
			}
		}

        public double WatermarkHeight
        {
            get => watermarkHeight;
            set
            {
                this.RaiseAndSetIfChanged(ref watermarkHeight, value);
            }
        }

        public ReactiveCommand<string, Unit> OpenUrlCommand { get; init; }
        public ReactiveCommand<Unit, Unit> ShowAboutDialogCommand { get; init; }
        public ReactiveCommand<Unit, Unit> ShowCreditsDialogCommand { get; init; }

        public MainViewModel()
        {
            OpenUrlCommand = ReactiveCommand.Create<string>(OpenUrl);
            ShowAboutDialogCommand = ReactiveCommand.CreateFromTask(ShowAboutDialog);
            ShowCreditsDialogCommand = ReactiveCommand.CreateFromTask(ShowCreditsDialog);
        }

        public void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task ShowAboutDialog()
        {
            var aboutDialog = new ContentDialog()
            {
                Title = "About",
                Content = "Watermark 0.1.1 by nevelate",
                IsPrimaryButtonEnabled = false,
                IsSecondaryButtonEnabled = false,
                CloseButtonText = "Close",
            };
            await aboutDialog.ShowAsync();
        }

        public async Task ShowCreditsDialog()
        {
            var creditsDialog = new ContentDialog()
            {
                Title = "Credits",
                Content = "To be added",
                IsPrimaryButtonEnabled = false,
                IsSecondaryButtonEnabled = false,
                CloseButtonText = "Close",
            };
            await creditsDialog.ShowAsync();
        }
    }
}
