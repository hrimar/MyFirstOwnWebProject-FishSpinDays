namespace FishSpinDays.Common.API.Models.Comments
{
    public class CommentInteractionResultModel
    {
        public string Message { get; set; }
        public int? NewLikesCount { get; set; }
        public int? NewUnlikesCount { get; set; }
    }
}