namespace TwitterSchedulerHangfire
{
    public class PostScheduledTweetListDto
    {
        public List<PostScheduledTweetRequestDto> Tweet { get; set; } = new List<PostScheduledTweetRequestDto>();
    }
}
