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
        private string StreamUrl
        {
            get { return ConfigurationManager.AppSettings["stream_url"]; }
        }

        private string AccessToken
        {
            get { return ConfigurationManager.AppSettings["access_token"]; }
        }

        private string AccessTokenSecret
        {
            get { return ConfigurationManager.AppSettings["access_token_secret"]; }
        }

        private string ConsumerKey
        {
            get { return ConfigurationManager.AppSettings["consumer_key"]; }
        }

        private string ConsumerSecret
        {
            get { return ConfigurationManager.AppSettings["consumer_secret"]; }
        }

        public void Start()
        {
            HttpWebRequest webRequest = null;
            HttpWebResponse webResponse = null;
            StreamReader responseStream = null;

            string postparameters = GetPostParameters();

            int wait = 250;

            while (true)
            {
                try
                {
                    responseStream = GetStream(StreamUrl, postparameters);

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

                            TweetModel tweetObject = GetTweetObject(jsonText);

                            if (tweetObject == null)
                            {
                                continue;
                            }

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
                    Console.Write("\n\n" + "HATA:" + ex.Message + "\n\n");

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
                finally
                {
                    if (responseStream != null)
                    {
                        responseStream.Close();
                        responseStream = null;
                    }

                    Console.WriteLine("Waiting: " + wait);
                    Thread.Sleep(wait);
                }
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


            string oauthSignature = GenerateSignature(new Uri(url), ConsumerKey, ConsumerSecret, AccessToken, AccessTokenSecret, "POST", timeStamp, nonce, out normalizeUrl, out normalizedString);


            // create the request header
            const string headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                                        "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                                        "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                                        "oauth_version=\"{6}\"";

            return string.Format(headerFormat,
                Uri.EscapeDataString(nonce),
                Uri.EscapeDataString(Hmacsha1SignatureType),
                Uri.EscapeDataString(timeStamp),
                Uri.EscapeDataString(ConsumerKey),
                Uri.EscapeDataString(AccessToken),
                Uri.EscapeDataString(oauthSignature),
                Uri.EscapeDataString(OAuthVersion));
        }

        private StreamReader GetStream(string streamUrl, string postparameters)
        {
            HttpWebRequest webRequest = null;
            HttpWebResponse webResponse = null;
            StreamReader responseStream = null;

            webRequest = (HttpWebRequest)WebRequest.Create(streamUrl);
            webRequest.Timeout = -1;
            var authorizationString = GetAuthHeader(streamUrl + "?" + postparameters);
            webRequest.Headers.Add("Authorization", authorizationString);

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

            return responseStream;
        }

        private string GetPostParameters()
        {
            string postparameters = (ConfigurationManager.AppSettings["track_keywords"].Length == 0 ? string.Empty : "&track=" + ConfigurationManager.AppSettings["track_keywords"]) +
                                  (ConfigurationManager.AppSettings["language"].Length == 0 ? string.Empty : "&language=" + ConfigurationManager.AppSettings["language"]);

            if (!string.IsNullOrEmpty(postparameters))
            {
                if (postparameters.IndexOf('&') == 0)
                {
                    postparameters = postparameters.Remove(0, 1).Replace("#", "%23");
                }

                postparameters = postparameters.Replace(",", "%2C");
            }

            return postparameters;
        }

        private TweetModel GetTweetObject(string jsonText)
        {
            TweetStreamObject tweetStreamObject = JsonConvert.DeserializeObject<TweetStreamObject>(jsonText);

            if (tweetStreamObject == null)
            {
                return null;
            }

            string tweetBody = tweetStreamObject.text;
            bool isRetweeted = tweetStreamObject.retweeted;
            string source = tweetStreamObject.source;

            if (tweetBody.StartsWith("RT") || isRetweeted)
            {
                return null;
            }

            if (tweetBody.Contains("I'm at") || source.Contains("foursquare"))
            {
                return null;
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

            return tweetObject;
        }
    }
}