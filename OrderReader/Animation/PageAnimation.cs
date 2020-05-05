namespace OrderReader
{
    /// <summary>
    /// Styles of page animations for appearing/disappearing
    /// </summary>
    public enum PageAnimation
    {
        /// <summary>
        /// No animation takes place
        /// </summary>
        None = 0,

        /// <summary>
        /// The page slides in and fades in from the right
        /// </summary>
        SlideAndFadeInFromRight = 1,

        /// <summary>
        /// The page slides out and fades out to the left
        /// </summary>
        SlideAndFadeOutToLeft = 2,

        /// <summary>
        /// The page slides in and fades in from the top
        /// </summary>
        SlideAndFadeInFromTop = 3,

        /// <summary>
        /// The page slides out and fades out to the bottom
        /// </summary>
        SlideAndFadeOutToBottom = 4,
    }
}
