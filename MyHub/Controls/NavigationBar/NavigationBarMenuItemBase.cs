using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyHub.Controls.NavigationBar
{
    public abstract class NavigationBarMenuItemBase
    {
        /// <summary>
        /// Gets the arguments that can be passed optionally to the target page.
        /// </summary>
        public virtual object Arguments
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the image that is displayed in the navigation bar.
        /// </summary>
        public virtual ImageSource Image
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the positions of the current item in the navigation bar.
        /// </summary>
        public virtual NavigationBarItemPosition Position
        {
            get { return NavigationBarItemPosition.Top; }
        }

        /// <summary>
        /// Gets the symbol that is displayed in the navigation bar.
        /// </summary>
        public abstract Symbol Symbol { get; }

        /// <summary>
        /// Gets the symbol character that is displayed in the
        /// navigation bar.
        /// </summary>
        public virtual char SymbolAsChar
        {
            get { return (char)Symbol; }
        }
    }
}
