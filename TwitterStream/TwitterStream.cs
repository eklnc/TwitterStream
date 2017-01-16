using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace TwitterStream
{
    public class TwitterStream : OAuthBase
    {
        private readonly string _accessToken = ConfigurationManager.AppSettings["access_token"];
        private readonly string _accessTokenSecret = ConfigurationManager.AppSettings["access_token_secret"];
        private readonly string _consumerKey = ConfigurationManager.AppSettings["consumer_key"];
        private readonly string _consumerSecret = ConfigurationManager.AppSettings["consumer_secret"];

        public void StartTweetInvi()
        {
            //FilteredStream stream = new FilteredStream();
            //// Create a Token to access Twitter
            //IToken token = new Token("userKey", "userSecret", "consumerKey", "consumerSecret");
            //// Adding Tracks filters
            //stream.AddTrack("HelloMartha");
            //stream.AddTrack("TrackNumber2!");
            //// Write the Text of each Tweet in the Console
            //stream.StartStream(token, x => Console.WriteLine(x.Text));
        }

        public void Start()
        {
            //Twitter Streaming API
            string streamUrl = ConfigurationManager.AppSettings["stream_url"];

            HttpWebRequest webRequest = null;
            HttpWebResponse webResponse = null;
            StreamReader responseStream = null;
            string postparameters = (ConfigurationManager.AppSettings["track_keywords"].Length == 0 ? string.Empty : "&track=" + ConfigurationManager.AppSettings["track_keywords"]) +
                                    (ConfigurationManager.AppSettings["language"].Length == 0 ? string.Empty : "&language=" + ConfigurationManager.AppSettings["language"]);

            if (!string.IsNullOrEmpty(postparameters))
            {
                if (postparameters.IndexOf('&') == 0)
                    postparameters = postparameters.Remove(0, 1).Replace("#", "%23");
            }

            int wait = 250;

            try
            {
                while (true)
                {
                    try
                    {
                        //Connect
                        webRequest = (HttpWebRequest)WebRequest.Create(streamUrl);
                        webRequest.Timeout = -1;
                        webRequest.Headers.Add("Authorization", GetAuthHeader(streamUrl + "?" + postparameters));

                        Encoding encode = Encoding.GetEncoding("utf-8");
                        if (postparameters.Length > 0)
                        {
                            webRequest.Method = "POST";
                            webRequest.ContentType = "application/x-www-form-urlencoded";

                            byte[] twitterTrack = encode.GetBytes(postparameters);

                            webRequest.ContentLength = twitterTrack.Length;
                            Stream twitterPost = webRequest.GetRequestStream();
                            twitterPost.Write(twitterTrack, 0, twitterTrack.Length);
                            twitterPost.Close();
                        }

                        webResponse = (HttpWebResponse)webRequest.GetResponse();
                        Stream stream = webResponse.GetResponseStream();
                        responseStream = new StreamReader(stream, encode);

                        //Read the stream.
                        while (true)
                        {
                            try
                            {
                                var jsonText = responseStream.ReadLine();
                                if (jsonText == string.Empty)
                                {
                                    continue;
                                }

                                //Success
                                wait = 250;

                                TweetStreamObject tweetStreamObject = JsonConvert.DeserializeObject<TweetStreamObject>(jsonText);

                                if (tweetStreamObject == null)
                                {
                                    continue;
                                }

                                string tweetBody = tweetStreamObject.text;
                                bool isRetweeted = tweetStreamObject.retweeted;
                                string source = tweetStreamObject.source;

                                if (tweetBody.StartsWith("RT") || isRetweeted)
                                {
                                    continue;
                                }

                                if (tweetBody.Contains("I'm at") || source.Contains("foursquare"))
                                {
                                    continue;
                                }

                                string createdTime = ConvertToDateTime(tweetStreamObject.created_at);
                                float[] coordinateObject = tweetStreamObject.coordinates?.coordinates;
                                string name = tweetStreamObject.user.name;
                                string username = tweetStreamObject.user.screen_name;
                                string place = tweetStreamObject.place?.full_name;

                                var tweetObject = new TweetModel
                                {
                                    Tweet = tweetBody,
                                    Coordinates = coordinateObject == null ? string.Empty 
                                                                            : "(" + coordinateObject[0].ToString(CultureInfo.InvariantCulture).Replace(",", ".") + "," + 
                                                                                    coordinateObject[1].ToString(CultureInfo.InvariantCulture).Replace(",", ".") + ")",
                                    CreateTime = createdTime,
                                    FullName = name,
                                    UserName = username,
                                    Place = place
                                };

                                //Write Status
                                Console.Write("KÝMDEN: " + tweetObject.FullName + " - " + "@" + tweetObject.UserName + "\n" +
                                              "TWEET: " + tweetObject.Tweet + "\n" +
                                              "NE ZAMAN ATILDI?: " + tweetObject.CreateTime + "\n" +
                                              "NEREDEN ATILDI?: " + tweetObject.Place + "\n" +
                                              "KOORDÝNASYON:" + tweetObject.Coordinates + "\n" +
                                              "--------------------------------" +
                                              "\n\n");
                            }
                            catch (Exception ex)
                            {
                                Console.Write("\n\n" + "HATA:" + ex.Message + "\n\n");
                            }

                        }
                    }
                    catch (WebException ex)
                    {
                        Console.WriteLine(ex.Message);
                        if (ex.Status == WebExceptionStatus.ProtocolError)
                        {
                            //-- From Twitter Docs -- 
                            //When a HTTP error (> 200) is returned, back off exponentially. 
                            //Perhaps start with a 10 second wait, double on each subsequent failure, 
                            //and finally cap the wait at 240 seconds. 
                            //Exponential Backoff
                            if (wait < 10000)
                                wait = 10000;
                            else
                            {
                                if (wait < 240000)
                                    wait = wait * 2;
                            }
                        }
                        else
                        {
                            //-- From Twitter Docs -- 
                            //When a network error (TCP/IP level) is encountered, back off linearly. 
                            //Perhaps start at 250 milliseconds and cap at 16 seconds.
                            //Linear Backoff
                            if (wait < 16000)
                                wait += 250;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        if (webRequest != null)
                            webRequest.Abort();
                        if (responseStream != null)
                        {
                            responseStream.Close();
                            responseStream = null;
                        }

                        if (webResponse != null)
                        {
                            webResponse.Close();
                            webResponse = null;
                        }
                        Console.WriteLine("Waiting: " + wait);
                        Thread.Sleep(wait);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Waiting: " + wait);
                Thread.Sleep(wait);
            }
        }

        private static string ConvertToDateTime(string tweetDate)
        {
            List<string> splittedDate = tweetDate.Split(' ').ToList();

            string day = string.Empty;
            switch (splittedDate[0])
            {
                case "Mon": day = "Pazartesi"; break;
                case "Tue": day = "Salý"; break;
                case "Wed": day = "Çarþamba"; break;
                case "Thu": day = "Perþembe"; break;
                case "Fri": day = "Cuma"; break;
                case "Sat": day = "Cumartesi"; break;
                case "Sun": day = "Pazar"; break;
            }

            string month = string.Empty;
            switch (splittedDate[1])
            {
                case "Jan": month = "Ocak"; break;
                case "Feb": month = "Þubat"; break;
                case "Mar": month = "Mart"; break;
                case "Apr": month = "Nisan"; break;
                case "May": month = "Mayýs"; break;
                case "Jun": month = "Haziran"; break;
                case "Jul": month = "Temmuz"; break;
                case "Aug": month = "Aðustos"; break;
                case "Sep": month = "Eylül"; break;
                case "Oct": month = "Ekim"; break;
                case "Nov": month = "Kasým"; break;
                case "Dec": month = "Aralýk"; break;
            }

            string dayNumber = splittedDate[2];
            string time = splittedDate[3];
            string year = splittedDate[5];

            string resultDate = string.Join(" ", dayNumber, month, year, day, time);
            return resultDate;
        }

        private string GetAuthHeader(string url)
        {
            string normalizedString;
            string normalizeUrl;
            string timeStamp = GenerateTimeStamp();
            string nonce = GenerateNonce();


            string oauthSignature = GenerateSignature(new Uri(url), _consumerKey, _consumerSecret, _accessToken, _accessTokenSecret, "POST", timeStamp, nonce, out normalizeUrl, out normalizedString);


            // create the request header
            const string headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                                        "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                                        "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                                        "oauth_version=\"{6}\"";

            return string.Format(headerFormat,
                Uri.EscapeDataString(nonce),
                Uri.EscapeDataString(Hmacsha1SignatureType),
                Uri.EscapeDataString(timeStamp),
                Uri.EscapeDataString(_consumerKey),
                Uri.EscapeDataString(_accessToken),
                Uri.EscapeDataString(oauthSignature),
                Uri.EscapeDataString(OAuthVersion));
        }
    }
}