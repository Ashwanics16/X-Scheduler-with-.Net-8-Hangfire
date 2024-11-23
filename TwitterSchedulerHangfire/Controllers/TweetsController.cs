using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Tweetinvi;
using Tweetinvi.Models;

namespace TwitterSchedulerHangfire.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetsController : ControllerBase
    {

        [HttpPost("bulk")]
        public IActionResult ScheduleTweets(PostScheduledTweetListDto request)
        {
            var invalidTweets = new List<PostScheduledTweetRequestDto>();
            int scheduledcount = 0;
            foreach (var tweet in request.Tweet)
            {
                TimeSpan delay = tweet.Schedulefor - DateTime.UtcNow;

                if (delay <= TimeSpan.Zero)
                {
                    invalidTweets.Add(tweet);
                    continue;
                }
                BackgroundJob.Schedule(() => PostTweet(tweet.Adapt<PostTweetRequestDto>()), delay);
                scheduledcount++;
            }
            string message;
            if (invalidTweets.Any())
            {
                message = $"{scheduledcount} tweets scheduled successfully" +
                    $"{invalidTweets.Count} tweets had invalid dates and were not scheduled";
            }
            else
            {
                message = $"All {scheduledcount} Tweets scheduled successfully";
            }
            return Ok(message);
        }

        [HttpPost("Schedule")]
        public IActionResult ScheduleTweet(PostScheduledTweetRequestDto newTweet)
        {

            var delay = newTweet.Schedulefor - DateTime.UtcNow;
            if (delay > TimeSpan.Zero)
            {
                BackgroundJob.Schedule(() => PostTweet(newTweet.Adapt<PostTweetRequestDto>()), delay);
                return Ok("Tweet Scheduled Successfully !");

            }
            else
            {
                return BadRequest("Please Enter a valid date & time");
            }
        }


        [HttpPost]

        [AutomaticRetry(Attempts = 0)]
        public async Task<IActionResult> PostTweet(PostTweetRequestDto postTweetRequestDto)
        {
            var client = new TwitterClient(
                "Your Key",// API key
                "Your Key",//API secret Key
                "Your Key ", //Access Token
                "Your Key");//Access Token secret key
            var result = await client.Execute.AdvanceRequestAsync(BuildTwitterRequest(postTweetRequestDto, client));
            return Ok(result.Content);
        }

        private static Action<ITwitterRequest> BuildTwitterRequest(PostTweetRequestDto newpostTweetRequestDto, TwitterClient client)
        {
            return (ITwitterRequest request) =>
            {
                var jsonbody = client.Json.Serialize(newpostTweetRequestDto);
                var content = new StringContent(jsonbody, Encoding.UTF8, "application/json");
                request.Query.Url = "https://api.x.com/2/tweets";
                request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
                request.Query.HttpContent = content;
            };

        }
    }
}
