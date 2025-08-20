using Review.Models.CommonModel;
using Review.Models.RequestModel;

namespace Review.Services.Repository.Interfaces
{
    public interface IReviewRepository
    {
        List<ReviewResponseModel> GetReviews();
        ReviewResponseModel GetReviewById(string reviewSid);
        bool AddReview(ReviewRequestModel review, out string newSid);
        bool UpdateReview(string reviewSid, ReviewRequestModel review);
        bool DeleteReview(string reviewSid);
        //List<ReviewResponseModel> FilterReviewsByPrice(int MinPrice, int MaxPrice, int pageNumber, int pageSize);
        //List<ReviewResponseModel> SearchReviews(string searchedText);

        Task<Page> GetReviewsAsync(Dictionary<string, object> parameters);
        Task<ReviewResponseModel?> GetReviewByIdAsync(string reviewSid);
        Task<ReviewResponseModel> AddReviewAsync(ReviewRequestModel model);
        Task<ReviewResponseModel> UpdateReviewAsync(string reviewSid, ReviewRequestModel model);
        Task<bool> DeleteReviewAsync(string reviewSid);
        Task<List<ReviewResponseModel>?> GetReviewsByRatingAsync(int rating);
    }
}
