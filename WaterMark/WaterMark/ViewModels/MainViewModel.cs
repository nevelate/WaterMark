using ReactiveUI;
using System.Diagnostics;
using System.Reactive;
using System.Runtime.InteropServices;

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

        public MainViewModel()
        {
            OpenUrlCommand = ReactiveCommand.Create<string>(OpenUrl);
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
    }
}
