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
    public sealed partial class MainPage : Page
    {

        static int toastIndex = 1;
        string rssContent;
        string scorepage;
        //    InningsDetail currentDetail;
        public MainPage()
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
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.


            //System.Timers.Timer MyTimer = new System.Timers.Timer();
            //MyTimer.Elapsed += new System.Timers.ElapsedEventHandler(MyTimer_Elapsed);
            //MyTimer.Interval = 2000;
            //MyTimer.Enabled = true;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                 AppViewBackButtonVisibility.Collapsed;

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
            
            if (teamTextBlock.Text != "")
            {

                foreach (RSScontent r in RssData)
                {
                    if (r.Title.ToLower().Contains(teamTextBlock.Text.ToLower()))
                    {
                        int l = r.Title.IndexOf("*");
                        if (l > 0)
                        {

                            if ((bool)overcheckBox.IsChecked)
                            {
                                HttpClient httpClient1 = new HttpClient();
                                scorepage = await httpClient1.GetStringAsync(r.Guid);
                                Regex regex = new Regex(@"<title>\s*(.+?)\s*</title>",
                                RegexOptions.IgnoreCase);
                                Match match = regex.Match(scorepage);
                                string title = match.Groups[0].Value;

                                resultTextBlock.Text += title.Substring(7, title.IndexOf("|") - 7);


                            }
                            else

                                resultTextBlock.Text += r.Title;

                        }
                        else if (resultTextBlock.Text == "")
                            resultTextBlock.Text = "Match Not Started";

                        break;

                    }

                }

                if (resultTextBlock.Text == "")
                    resultTextBlock.Text = "Match not found!";

            }
            else
            {
                resultTextBlock.Text = "";

            }

        }
        private void clickMeButton_Click(object sender, RoutedEventArgs e)
        {

            getScore();
            // sendNotification(sender,e);

        }

        public async void RSSCall()
        {
            HttpClient httpClient = new HttpClient();
            rssContent = await httpClient.GetStringAsync("http://static.cricinfo.com/rss/livescores.xml");


        }

        /*
                private void sendNotification(object sender, RoutedEventArgs e)
                {
                    //string title;


                    //  title = doc.GetElementbyId("title").ToString();

                    // Use a helper method to check whether I can send a toast.
                    // Note: If this check wasn't here, the code would fail silently.
                    if (MySampleHelper.CanSendToasts())
                    {
                        // Use a helper method to create a new ToastNotification.
                        // Each time we run this code, we'll append the count (toastIndex in this example) to the message
                        // so that it can be seen as a unique message in the action center. This is not mandatory - we
                        // do it here for educational purposes.
                        ToastNotification toast = MySampleHelper.CreateTextOnlyToast("Scenario", String.Format("message {0}", toastIndex));

                        // Optional. Setting an expiration time on a toast notification defines the maximum
                        // time the notification will be displayed in action center before it expires and is removed. 
                        // If this property is not set, the notification expires after 7 days and is removed.
                        // Tapping on a toast in action center launches the app and removes it immediately from action center.
                        toast.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(3600);

                        // Display the toast.
                        ToastNotificationManager.CreateToastNotifier().Show(toast);
                        toastIndex++;

                        // Tell the user that the toast was sent successfully. 
                        // This can be ommitted in a real app.


                    }
                }
        */
        private void OnKeyDownHandler(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                getScore();
                this.Focus(FocusState.Programmatic);
            }
        }

        private void showscore_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
           
            this.Frame.Navigate(typeof(ShowMatches));
        }
    }
}
