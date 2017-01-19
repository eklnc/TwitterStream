using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TwitterStream;

namespace TwitterStreamApi.Controllers
{
    public class TwitterStreamApiController : ApiController
    {
        [HttpGet]
        public TweetModel TwitterSteam(bool containsRetweet, bool containsLocationNotification)
        {
            var stream = new TwitterStream.TwitterStream();
            TweetModel tweet = stream.Start(containsRetweet, containsLocationNotification);

            return tweet;
        }
    }
}
