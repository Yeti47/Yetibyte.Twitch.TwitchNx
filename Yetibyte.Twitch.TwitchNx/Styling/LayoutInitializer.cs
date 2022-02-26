using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yetibyte.Twitch.TwitchNx.Styling
{
	public class LayoutInitializer : ILayoutUpdateStrategy
	{
		public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
		{
            //AD wants to add the anchorable into destinationContainer
            //just for test provide a new anchorablepane 
            //if the pane is floating let the manager go ahead
            LayoutAnchorablePane? destPane = destinationContainer as LayoutAnchorablePane;
            if (destinationContainer != null &&
                destinationContainer.FindParent<LayoutFloatingWindow>() != null)
                return false;

            DefaultPaneAttribute? defaultPaneAttribute = null;

            if (anchorableToShow.Content != null)
            {
                defaultPaneAttribute = anchorableToShow.Content.GetType().GetCustomAttributes(typeof(DefaultPaneAttribute), true)
                    .FirstOrDefault() as DefaultPaneAttribute;
            }

            string targetPaneName = !string.IsNullOrWhiteSpace(defaultPaneAttribute?.DefaultPaneName) ? defaultPaneAttribute.DefaultPaneName : "MainLeft";

            var toolsPane = layout.Descendents()
                .OfType<LayoutAnchorablePane>().Concat(
                    layout.Descendents()
                    .OfType<LayoutAnchorable>()
                    .Select(a => a.Content)
                    .OfType<LayoutAnchorablePane>()
                ).FirstOrDefault(d => d.Name == targetPaneName);


            if (toolsPane != null)
            {
                toolsPane.Children.Add(anchorableToShow);
                return true;
            }

            return false;

		}


		public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
		{
		}


		public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
		{
			return false;
		}

		public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
		{

		}
	}
}
