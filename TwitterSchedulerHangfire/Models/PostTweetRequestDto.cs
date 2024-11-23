using Newtonsoft.Json;

namespace TwitterSchedulerHangfire
{
    public class PostTweetRequestDto
    {
        [JsonProperty("text")]
        public string Text { get; set; }=string.Empty;
    }
}
