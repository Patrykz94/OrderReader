using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using OrderReader.Core;

namespace OrderReader
{
    /// <summary>
    /// A base page for all pages to gain base functionality
    /// </summary>
    public class BasePage<VM> : Page
        where VM : BaseViewModel, new()
    {
        #region Private Members

        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        private VM mViewModel;

        #endregion

        #region Public Properties

        /// <summary>
        /// The animation to play when the page is first loaded
        /// </summary>
        public PageAnimation PageLoadAnimation { get; set; } = PageAnimation.None;
        
        /// <summary>
        /// The animation to play when the page is unloaded
        /// </summary>
        public PageAnimation PageUnloadAnimation { get; set; } = PageAnimation.None;

        /// <summary>
        /// The time any slide animation takes to complete
        /// </summary>
        public float SlideSeconds { get; set; } = 0.2f;

        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        public VM ViewModel
        {
            get { return mViewModel; }
            set
            {
                // If nothing has changed, return
                if (mViewModel == value) return;

                // Update the value
                mViewModel = value;

                // Set the data context for this page
                DataContext = mViewModel;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BasePage()
        {
            // If we are animating in, hide to begin with
            if (PageLoadAnimation != PageAnimation.None) Visibility = Visibility.Collapsed;

            // Listen out for the page loading
            Loaded += BasePage_LoadedAsync;

            // Create a default view model
            ViewModel = new VM();
        }

        #endregion

        #region Animation Load / Unload

        /// <summary>
        /// Once the page is loaded, perform any required animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BasePage_LoadedAsync(object sender, RoutedEventArgs e)
        {
            // Animate the page in
            await AnimateInAsync();
        }

        /// <summary>
        /// Animate the page in
        /// </summary>
        /// <returns></returns>
        public async Task AnimateInAsync()
        {
            // Make sure we have something to do
            if (PageLoadAnimation == PageAnimation.None) return;

            switch (PageLoadAnimation)
            {
                case PageAnimation.SlideAndFadeInFromRight:

                    // Start the animation
                    await this.SlideAndFadeInFromRightAsync(SlideSeconds);

                    break;

                case PageAnimation.SlideAndFadeInFromTop:

                    // Start the animation
                    await this.SlideAndFadeInFromTopAsync(SlideSeconds);

                    break;
            }
        }

        /// <summary>
        /// Animate the page out
        /// </summary>
        /// <returns></returns>
        public async Task AnimateOutAsync()
        {
            // Make sure we have something to do
            if (PageUnloadAnimation == PageAnimation.None) return;

            switch (PageUnloadAnimation)
            {
                case PageAnimation.SlideAndFadeOutToLeft:

                    // Start the animation
                    await this.SlideAndFadeOutToLeftAsync(SlideSeconds);

                    break;

                case PageAnimation.SlideAndFadeOutToBottom:

                    // Start the animation
                    await this.SlideAndFadeOutToBottomAsync(SlideSeconds);

                    break;
            }
        }

        #endregion
    }
}
