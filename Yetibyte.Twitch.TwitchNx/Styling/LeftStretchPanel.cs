using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Yetibyte.Twitch.TwitchNx.Styling
{
    // Source: https://stackoverflow.com/questions/13039144/textbox-with-horizontalalignment-set-to-both-stretch-and-left/13040678#13040678
    public class LeftStretchPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement element in InternalChildren)
            {
                element.Measure(availableSize);
            }

            return new Size();
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            foreach (UIElement element in InternalChildren)
            {
                double width = arrangeBounds.Width;
                FrameworkElement? fwElement = element as FrameworkElement;

                if (fwElement != null && width > fwElement.MaxWidth)
                {
                    width = fwElement.MaxWidth;
                }

                element.Arrange(new Rect(0, 0, width, arrangeBounds.Height));
            }

            return arrangeBounds;
        }
    }
}
