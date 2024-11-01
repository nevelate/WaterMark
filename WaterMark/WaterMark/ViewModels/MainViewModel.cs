using ReactiveUI;

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
    }
}
