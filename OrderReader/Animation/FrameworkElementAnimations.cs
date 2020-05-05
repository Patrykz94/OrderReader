using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace OrderReader
{
    /// <summary>
    /// Helpers to animate framework elements in specific ways
    /// </summary>
    public static class FrameworkElementAnimations
    {
        /// <summary>
        /// Slides an element in from the right
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        /// <returns></returns>
        public static async Task SlideAndFadeInFromRightAsync(this FrameworkElement element, float seconds = 0.2f, bool keepMargin = true)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add slide from right animation
            sb.AddSlideFromRight(seconds, element.ActualWidth, keepMargin: keepMargin);

            // Add fade in animation
            sb.AddFadeIn(seconds);

            // Start animating
            sb.Begin(element);

            // Make element visible
            element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)seconds * 1000);
        }

        /// <summary>
        /// Slides an element in from the right
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        /// <returns></returns>
        public static async Task SlideAndFadeInFromLeftAsync(this FrameworkElement element, float seconds = 0.2f, bool keepMargin = true)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add slide from left animation
            sb.AddSlideFromLeft(seconds, element.ActualWidth, keepMargin: keepMargin);

            // Add fade in animation
            sb.AddFadeIn(seconds);

            // Start animating
            sb.Begin(element);

            // Make element visible
            element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)seconds * 1000);
        }

        /// <summary>
        /// Slides an element in from the top
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        /// <returns></returns>
        public static async Task SlideAndFadeInFromTopAsync(this FrameworkElement element, float seconds = 0.2f, bool keepMargin = true)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add slide from top animation
            sb.AddSlideFromTop(seconds, element.ActualHeight, keepMargin: keepMargin);

            // Add fade in animation
            sb.AddFadeIn(seconds);

            // Start animating
            sb.Begin(element);

            // Make element visible
            element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)seconds * 1000);
        }

        /// <summary>
        /// Slides an element in from the bottom
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        /// <returns></returns>
        public static async Task SlideAndFadeInFromBottomAsync(this FrameworkElement element, float seconds = 0.2f, bool keepMargin = true)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add slide from bottom animation
            sb.AddSlideFromBottom(seconds, element.ActualHeight, keepMargin: keepMargin);

            // Add fade in animation
            sb.AddFadeIn(seconds);

            // Start animating
            sb.Begin(element);

            // Make element visible
            element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)seconds * 1000);
        }

        /// <summary>
        /// Slides an element out to the left
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        /// <returns></returns>
        public static async Task SlideAndFadeOutToLeftAsync(this FrameworkElement element, float seconds = 0.2f, bool keepMargin = true)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add slide to left animation
            sb.AddSlideToLeft(seconds, element.ActualWidth, keepMargin: keepMargin);

            // Add fade in animation
            sb.AddFadeOut(seconds);

            // Start animating
            sb.Begin(element);

            // Make element visible
            element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)seconds * 1000);
        }

        /// <summary>
        /// Slides an element out to the right
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        /// <returns></returns>
        public static async Task SlideAndFadeOutToRightAsync(this FrameworkElement element, float seconds = 0.2f, bool keepMargin = true)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add slide to right animation
            sb.AddSlideToRight(seconds, element.ActualWidth, keepMargin: keepMargin);

            // Add fade in animation
            sb.AddFadeOut(seconds);

            // Start animating
            sb.Begin(element);

            // Make element visible
            element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)seconds * 1000);
        }

        /// <summary>
        /// Slides an element out to the bottom
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        /// <returns></returns>
        public static async Task SlideAndFadeOutToBottomAsync(this FrameworkElement element, float seconds = 0.2f, bool keepMargin = true)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add slide to bottom animation
            sb.AddSlideToBottom(seconds, element.ActualHeight, keepMargin: keepMargin);

            // Add fade in animation
            sb.AddFadeOut(seconds);

            // Start animating
            sb.Begin(element);

            // Make element visible
            element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)seconds * 1000);
        }

        /// <summary>
        /// Slides an element out to the top
        /// </summary>
        /// <param name="element">The element to animate</param>
        /// <param name="seconds">The time the animation will take</param>
        /// <param name="keepMargin">Whether to keep the element at the same width during animation</param>
        /// <returns></returns>
        public static async Task SlideAndFadeOutToTopAsync(this FrameworkElement element, float seconds = 0.2f, bool keepMargin = true)
        {
            // Create the storyboard
            var sb = new Storyboard();

            // Add slide to top animation
            sb.AddSlideToTop(seconds, element.ActualHeight, keepMargin: keepMargin);

            // Add fade in animation
            sb.AddFadeOut(seconds);

            // Start animating
            sb.Begin(element);

            // Make element visible
            element.Visibility = Visibility.Visible;

            // Wait for it to finish
            await Task.Delay((int)seconds * 1000);
        }
    }
}
