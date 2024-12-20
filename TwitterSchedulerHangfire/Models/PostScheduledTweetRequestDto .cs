﻿using Newtonsoft.Json;

namespace TwitterSchedulerHangfire
{
    public class PostScheduledTweetRequestDto
    {
        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;
        public DateTime Schedulefor { get; set; }
    }
}
