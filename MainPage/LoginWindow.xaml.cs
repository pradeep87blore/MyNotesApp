using Hammock;
using Hammock.Authentication.OAuth;
using Hammock.Web;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Navigation;
using TweetSharp;

// Info about logging in with Twitter
// https://developer.twitter.com/en/docs/twitter-for-websites/log-in-with-twitter/guides/implementing-sign-in-with-twitter

namespace MainPage
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        string consumerKey = null;
        string consumerSecret = null;
        string awsIdentityPoolId = null;
        OAuthRequestToken requestToken;
        TwitterService service;

        // This contains the user's Twitter info once they successfully login
        TwitterUserInfo twitterUserInfo { get; set; } = null;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (!LoadSecrets())
            {
                string msg = "Failed to read the Twitter secrets. Ensure that the file secrets.txt is correctly located with the exe and its contents are properly filled\n";
                msg += "Sample contents of the file: \n";
                msg += "ConsumerKey=rDt8YmVT7gNYqIdHjPSbgrR5f\nConsumerSecret=CHq16rf72dRVtQfhPMPUbOhhgfgjFMncBZDhJbfmhrykaDV0j5\nIdentityPool=ap-southeast-2:39763976-9bda-4195-4195-53b439bda407";

                MessageBox.Show(msg);
                return;
            }            
            Login();
        }

        /// <summary>
        /// Initiate the login flow
        /// </summary>
        private void Login()
        {
            // Use the Consumer Key and Consumer Secret Key to fetch a valid Twitter Service handle
            service = new TwitterService(consumerKey, consumerSecret);

            // Retrieve an OAuth Request Token
            requestToken = service.GetRequestToken();

            // Obtain the URI that shall be used for the login
            Uri authenticationUrl = service.GetAuthenticationUrl(requestToken);
            StartTwitterLoginDialog(authenticationUrl.ToString());

        }

        /// <summary>
        /// Navigate to the Twitter login URL
        /// </summary>
        /// <param name="url"></param>
        private void StartTwitterLoginDialog(string twitterLoginUrl)
        {
            webBrowser.Navigated += WebBrowser_Navigated; // Subscribe to the Navigated event
            webBrowser.Navigate(twitterLoginUrl);
        }

        /// <summary>
        /// This is to set the parameters up for querying for the access token
        /// </summary>
        private readonly Func<FunctionArguments, RestRequest> _accessTokenQuery
                = args =>
                {
                    var request = new RestRequest
                    {
                        Credentials = new OAuthCredentials
                        {
                            ConsumerKey = args.ConsumerKey,
                            ConsumerSecret = args.ConsumerSecret,
                            Token = args.Token,
                            TokenSecret = args.TokenSecret,
                            Verifier = args.Verifier,
                            ParameterHandling = OAuthParameterHandling.HttpAuthorizationHeader,
                            SignatureMethod = OAuthSignatureMethod.HmacSha1,
                            Type = OAuthType.AccessToken
                        },
                        Method = WebMethod.Post,
                        Path = "https://api.twitter.com/oauth/access_token"
                    };
                    return request;
                };


        /// <summary>
        /// The event triggered when the web browser control navigates to a specified URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WebBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            try
            {
                if ((e.Uri != null) && (e.Uri.ToString().Contains("oauth_token")) && (e.Uri.ToString().Contains("oauth_verifier")))
                {
                    Console.WriteLine(e.Uri.PathAndQuery);

                    string[] query = e.Uri.Query.Split("oauth_verifier=");
                    var verifier = query[1];

                    var args = new FunctionArguments();

                    args.ConsumerKey = consumerKey;
                    args.ConsumerSecret = consumerSecret;
                    args.Token = requestToken.Token;
                    args.TokenSecret = requestToken.TokenSecret;
                    args.Verifier = verifier;


                    RestClient _oauth = new RestClient();

                    var request = _accessTokenQuery.Invoke(args);
                    var response = _oauth.Request(request);

                    var queryParams = HttpUtility.ParseQueryString(response.Content);
                    var accessToken = new OAuthAccessToken
                    {
                        Token = queryParams["oauth_token"] ?? "?",
                        TokenSecret = queryParams["oauth_token_secret"] ?? "?",
                        UserId = (int)Convert.ToInt64(queryParams["user_id"] ?? "0"),
                        ScreenName = queryParams["screen_name"] ?? "?"
                    };

                    Console.WriteLine(accessToken);

                    webBrowser.Visibility = Visibility.Collapsed;
                    GetUserInfo(accessToken);

                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


        }

        /// <summary>
        /// Fetch some user info and update this on to the UI
        /// </summary>
        /// <param name="accessToken"></param>
        private void GetUserInfo(OAuthAccessToken accessToken)
        {
            try
            {
                service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);

                GetUserProfileOptions userInfo = new GetUserProfileOptions()
                {
                    IncludeEntities = true,
                    SkipStatus = false
                };

                var result = service.BeginGetUserProfile(userInfo);
                var user = service.EndGetUserProfile(result);

                var imgUrlString = user.ProfileImageUrlHttps;
                if (imgUrlString.Contains("normal", StringComparison.OrdinalIgnoreCase))
                {
                    // To get a bigger image. Refer to https://developer.twitter.com/en/docs/accounts-and-users/user-profile-images-and-banners
                    imgUrlString = imgUrlString.Replace("normal", "bigger", StringComparison.OrdinalIgnoreCase);
                }

                twitterUserInfo = new TwitterUserInfo();
                twitterUserInfo.Id = user.Id.ToString();
                twitterUserInfo.Screen_name = user.ScreenName;
                twitterUserInfo.profileImage = imgUrlString;

                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        internal TwitterUserInfo GetTwitterUserInfo()
        {
            return twitterUserInfo;
        }


        /// <summary>
        /// Read the Secrets file
        /// </summary>
        /// <returns></returns>
        private bool LoadSecrets()
        {
            try
            {
                string line;

                /* The file contents are as follows:
                ConsumerKey=rDt8YmVT7gNYqIdHjPSbgrR5f
                ConsumerSecret=CHq16rf72dRVtQfhPMPUbOhhgfgjFMncBZDhJbfmhrykaDV0j5
                IdentityPool=ap-southeast-2:39763976-9bda-4195-4195-53b439bda407
                */

                System.IO.StreamReader secretsFile = new System.IO.StreamReader("secrets.keys");
                while ((line = secretsFile.ReadLine()) != null)
                {
                    if (line.Contains("ConsumerKey"))
                    {
                        var split = line.Split('=');
                        consumerKey = split[1];
                    }
                    else if (line.Contains("ConsumerSecret"))
                    {
                        var split = line.Split('=');
                        consumerSecret = split[1];
                    }
                    else if (line.Contains("IdentityPool"))
                    {
                        var split = line.Split('=');
                        awsIdentityPoolId = split[1];
                    }
                    System.Console.WriteLine(line);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            return true;
        }
    }
}
