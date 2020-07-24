using FlagData;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.DualScreen;

namespace FlagFacts
{
    public partial class AllFlagsPage : ContentPage
    {
        // is not spanned when first viewed...
        bool wasSpanned = false;

        public AllFlagsPage()
        {
            BindingContext = DependencyService.Get<FlagDetailsViewModel>();
            InitializeComponent();
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!DeviceIsSpanned)
            {   // use Navigation on single screen
                await this.Navigation.PushAsync(new FlagDetailsPage());
            }
        }

        bool DeviceIsSpanned => DualScreenInfo.Current.SpanMode != TwoPaneViewMode.SinglePane;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DualScreenInfo.Current.PropertyChanged += DualScreen_PropertyChanged;
            UpdateLayouts(); // for first page load
        }
        protected override void OnDisappearing()
        {
            DualScreenInfo.Current.PropertyChanged -= DualScreen_PropertyChanged;
            base.OnDisappearing();
        }

        void DualScreen_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateLayouts();
        }
        async void UpdateLayouts()
        {
            if (DeviceIsSpanned)
            {   // two screens: side by side
                twoPaneView.TallModeConfiguration = TwoPaneViewTallModeConfiguration.TopBottom;
                twoPaneView.WideModeConfiguration = TwoPaneViewWideModeConfiguration.LeftRight;
                wasSpanned = true;
            }
            else
            {   // single-screen: only list is shown
                twoPaneView.PanePriority = TwoPaneViewPriority.Pane1;
                twoPaneView.TallModeConfiguration = TwoPaneViewTallModeConfiguration.SinglePane;
                twoPaneView.WideModeConfiguration = TwoPaneViewWideModeConfiguration.SinglePane;
                // wasSpanned check is needed, or this will open on first-run or rotation
                // stack count is needed, or we might push multiple on rotation
                if (wasSpanned && Navigation.NavigationStack.Count == 1)
                {   // open the detail page
                    await Navigation.PushAsync(new FlagDetailsPage());
                }
                wasSpanned = false;
            }
        }
    }
}