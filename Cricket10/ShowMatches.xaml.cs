using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;

using System.Xml;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Net;
using System.Windows;
using System.Net.Http;
using System.Xml.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Windows.UI.ViewManagement;
using System.Threading;
using System;

using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Cricket10
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShowMatches : Page
    {


        string rssContent;

        //    InningsDetail currentDetail;
        
        public ShowMatches()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;


            Window.Current.SizeChanged += Current_SizeChanged;



        }

        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            // Get the new view state
            // Add: using Windows.UI.ViewManagement;
            string CurrentViewState = ApplicationView.GetForCurrentView().Orientation.ToString();

            // Trigger the Visual State Manager
            VisualStateManager.GoToState(this, CurrentViewState, true);

        }

        void App_BackRequested(Object sender,BackRequestedEventArgs e)
        {
            Frame rootFrame =   Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            // Navigate back if possible, and if the event has not
            // already been handled.
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested +=   App_BackRequested;
            Frame rootFrame = Window.Current.Content as Frame;

            string myPages = "";
            foreach (PageStackEntry page in rootFrame.BackStack)
            {
                myPages += page.SourcePageType.ToString() + "\n";
            }
          //  stackCount.Text = myPages;

            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }

            getScore();
           
            
        }

        private async void getScore()
        {
            int count_matches = 0;
            resultTextBlock.Text = "";
            HttpClient httpClient = new HttpClient();
            rssContent = await httpClient.GetStringAsync("http://static.cricinfo.com/rss/livescores.xml");

            IEnumerable<RSScontent> RssData = from rss in XElement.Parse(rssContent).Descendants("item")

                                              select new RSScontent
                                              {
                                                  Title = rss.Element("title").Value,
                                                  Description = rss.Element("description").Value,
                                                  Link = rss.Element("link").Value,
                                                  Guid = rss.Element("guid").Value
                                              };
            ObservableCollection<RSScontent> dataList = new ObservableCollection<RSScontent>();
            foreach (RSScontent r in RssData)
            {
                dataList.Add(r);
            }

            listView.ItemsSource = dataList;
           listView.IsItemClickEnabled=true;
        }
       

        public async void RSSCall()
        {
            HttpClient httpClient = new HttpClient();
            rssContent = await httpClient.GetStringAsync("http://static.cricinfo.com/rss/livescores.xml");


        }

       
        private void OnKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                getScore();
                this.Focus(FocusState.Programmatic);
            }
        }

        private void listView_ItemClick(object sender, ItemClickEventArgs e)
        {
            RSScontent a = e.ClickedItem as RSScontent;

            string uriToLaunch = a.Link;
            var uri = new Uri(uriToLaunch);
            //  Launch(a.Link);
            //     mywebview.Navigate(new Uri(a.Link, UriKind.Absolute));
            this.Frame.Navigate(typeof(Webview),uriToLaunch);
        }

        async void Launch(string uriToLaunch)
        {
              var uri = new Uri(uriToLaunch);

            var success = await Windows.System.Launcher.LaunchUriAsync(uri);

            if (success)
            {
                // URI launched
            }
            else
            {
                // URI launch failed
            }
        }

    }
}
