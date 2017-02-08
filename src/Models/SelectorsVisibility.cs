namespace SpitOut.Models
{
    public enum SelectorsVisibility
    {
        /// <summary>
        /// Selectors always visible.
        /// </summary>
        Visible,

        /// <summary>
        /// Selectors always hidden.
        /// </summary>
        Hidden,

        /// <summary>
        /// Selectors enclosed into expander, which is initially collapsed.
        /// </summary>
        Collapsed,

        /// <summary>
        /// Selectors enclosed into expander, which is initially expanded.
        /// </summary>
        Expanded
    }
}