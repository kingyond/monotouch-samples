using System;

using Foundation;
using UIKit;

namespace UICatalog
{
	[Register ("SearchBarEmbeddedInNavigationBarViewController")]
	public class SearchBarEmbeddedInNavigationBarViewController : SearchControllerBaseViewController
	{
		private UISearchController _searchController;

		public SearchBarEmbeddedInNavigationBarViewController (IntPtr handle)
			: base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Create the search results view controller and use it for the UISearchController.
			var searchResultsController = (SearchResultsViewController)Storyboard.InstantiateViewController (SearchResultsViewController.StoryboardIdentifier);

			// Create the search controller and make it perform the results updating.
			_searchController = new UISearchController (searchResultsController);
			_searchController.SetSearchResultsUpdater (searchResultsController.UpdateSearchResultsForSearchController);
			_searchController.HidesNavigationBarDuringPresentation = false;

			// Configure the search controller's search bar. For more information on how to configure
			// search bars, see the "Search Bar" group under "Search".
			_searchController.SearchBar.SearchBarStyle = UISearchBarStyle.Minimal;
			_searchController.SearchBar.Placeholder = "Search";

			// Include the search bar within the navigation bar.
			NavigationItem.TitleView = _searchController.SearchBar;

			DefinesPresentationContext = true;
		}
	}
}
