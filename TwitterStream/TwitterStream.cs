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
            string postParameters = GetPostParameters();

            int wait = 250;

            HttpWebRequest webRequest = GetRequest(postParameters);

            while (true)
            {
                try
                {
                    StreamReader responseStream = GetStream(webRequest);

                    if (responseStream == null)
                    {
                        continue;
                    }

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
                        if (wait < 16000)
                            wait += 250;
                    }
                }
                finally
                {
                    Console.WriteLine("Waiting: " + wait);
                    Thread.Sleep(wait);
                }
            }
        }

        private HttpWebRequest GetRequest(string postParameters)
        {
            var authorizationString = GetAuthHeader(StreamUrl + "?" + postParameters);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(StreamUrl);
            webRequest.Timeout = -1;
            webRequest.Headers.Add("Authorization", authorizationString);

            var encode = Encoding.GetEncoding("utf-8");

            if (postParameters.Length > 0)
            {
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                byte[] twitterTrack = encode.GetBytes(postParameters);

                webRequest.ContentLength = twitterTrack.Length;
                Stream twitterPost = webRequest.GetRequestStream();
                twitterPost.Write(twitterTrack, 0, twitterTrack.Length);
                twitterPost.Close();
            }

            return webRequest;
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

        private StreamReader GetStream(HttpWebRequest webRequest)
        {
            Encoding encode = Encoding.GetEncoding("utf-8");

            var webResponse = (HttpWebResponse)webRequest.GetResponse();
            Stream stream = webResponse.GetResponseStream();

            if (stream == null)
            {
                return null;
            }

            var responseStream = new StreamReader(stream, encode);

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
            var tweetStreamObject = JsonConvert.DeserializeObject<TweetStreamObject>(jsonText);

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