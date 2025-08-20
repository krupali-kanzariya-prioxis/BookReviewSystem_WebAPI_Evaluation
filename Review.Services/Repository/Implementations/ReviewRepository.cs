using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Review.Common;
using Review.Models.CommonModel;
using Review.Models.Models.ReviewDB;
using Review.Models.RequestModel;
using Review.Models.SpDbContext;
using Review.Services.Repository.Interfaces;
using Review.Services.RepositoryFactory;
using Review.Services.UnitOfWork;
using System.Data;
using static Review.Common.Enums;
using ReviewDBVar = Review.Models.Models.ReviewDB.Review;

namespace Review.Services.Repository.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private ReviewDBContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewRepository> _logger;
        private readonly ReviewManagementSpContext _spContext;
        private readonly IUnitOfWork _unitOfWork;

        public ReviewRepository(ReviewDBContext context, IMapper mapper, ILogger<ReviewRepository> logger, ReviewManagementSpContext spContext, IUnitOfWork unitOfWork)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _spContext = spContext;
            _unitOfWork = unitOfWork;
        }
        public List<ReviewResponseModel> GetReviews()
        {
            var Reviews = _context.Reviews
                .Where(r => r.Status == (byte)Enums.StatusTypeDB.Active)
                .ToListAsync();

            _logger.LogInformation("Got all Reviewss");
            var result = _mapper.Map<List<ReviewResponseModel>>(Reviews);
            _logger.LogInformation("Mapped all Reviewss");
            return result;
        }
        public async Task<Page> GetReviewsAsync(Dictionary<string, object> parameters)
        {
            try
            {
                var xmlParam = CommonHelper.DictionaryToXml(parameters, "Search");
                string sqlQuery = "sp_DynamicGetAllReviews {0}";
                object[] param = { xmlParam };

                var result = await _spContext.ExecutreStoreProcedureResultList(sqlQuery, param);

                var jsonResult = JsonConvert.DeserializeObject<List<ReviewResponseModel>>(result.Result?.ToString() ?? "[]") ?? [];

                result.Result = jsonResult;

                return result;
            }
            catch (Exception e)
            {
                var errorMessage = e.InnerException?.Message ?? e.Message;
                _logger.LogError(e, "SQL execution failed: {Message}", errorMessage);
                throw new HttpStatusCodeException(500, errorMessage);
            }
        }

        public ReviewResponseModel GetReviewById(string ReviewSid)
        {
            var Review = _context.Reviews.Where(r => r.ReviewSid == ReviewSid && r.Status == (byte)Enums.StatusTypeDB.Active).FirstOrDefault();
            _logger.LogInformation($"Got Reviews with ReviewSid : {ReviewSid}");
            var result = _mapper.Map<ReviewResponseModel>(Review);
            _logger.LogInformation($"Mapped Reviews with ReviewSid : {ReviewSid}");
            return result;
        }
        public async Task<ReviewResponseModel?> GetReviewByIdAsync(string reviewSid)
        {
            try
            {
                var sqlQuery = "sp_getReviewBySid {0}";
                object[] param = { reviewSid };

                var result = await _spContext.ExecuteStoreProcedure(sqlQuery, param);
                ReviewResponseModel response = JsonConvert.DeserializeObject<ReviewResponseModel>(result);

                _logger.LogInformation("Got record by SID");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting event by SID: {reviewSid}");
                throw new HttpStatusCodeException(500, "Internal Server Error while fetching event by SID.");
            }
        }
        public bool DeleteReview(string reviewSid)
        {
            var Reviews = _context.Reviews.FirstOrDefault(r => r.ReviewSid == reviewSid);
            if (Reviews == null) return false;

            _logger.LogInformation($"Found Reviews with ReviewSid : {reviewSid}");
            Reviews.Status = (byte)Enums.StatusTypeDB.Delete;
            //_context.Reviews.Remove(Reviews);
            _context.SaveChanges();
            _logger.LogInformation($"Deleted Reviews with ReviewSid : {reviewSid}");
            return true;
        }
        public async Task<bool> DeleteReviewAsync(string reviewSid)
        {
            var Reviews = await _unitOfWork.GetRepository<ReviewDBVar>().GetAllAsync();
            var Review = Reviews
                .FirstOrDefault(r => r.ReviewSid == reviewSid && r.Status == (int)StatusTypeDB.Active);

            if (Review == null) return false;

            Review.Status = (int)StatusTypeDB.Delete;
            Review.LastModifiedAt = DateTime.UtcNow;

            _context.Reviews.Update(Review);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Deleted Reviews with ReviewSid : {reviewSid}");
            return true;
        }
        public bool AddReview(ReviewRequestModel Review, out string newSid)
        {
            var newReview = _mapper.Map<Models.Models.ReviewDB.Review>(Review);
            newSid = "REV" + Guid.NewGuid().ToString().ToUpper();
            newReview.ReviewSid = newSid;
            newReview.Status = (byte)Enums.StatusTypeDB.Active;
            newReview.CreatedAt = DateTime.Now;
            newReview.LastModifiedAt = DateTime.Now;

            _context.Reviews.Add(newReview);
            _context.SaveChanges();

            _logger.LogInformation("Inserted new Reviews with Sid: {ReviewSid}", newSid);
            return true;
        }
        public async Task<ReviewResponseModel> AddReviewAsync(ReviewRequestModel Reviews)
        {
            try
            {
                var newReview = _mapper.Map<Models.Models.ReviewDB.Review>(Reviews);
                newReview.ReviewSid = "REV" + Guid.NewGuid().ToString().ToUpper();
                newReview.Status = (byte)Enums.StatusTypeDB.Active;
                newReview.ReviewDate = DateTime.Now.Date;
                newReview.CreatedAt = DateTime.Now;
                newReview.LastModifiedAt = DateTime.Now;

                await _unitOfWork.GetRepository<ReviewDBVar>().InsertAsync(newReview);
                _context.SaveChanges();     

                _logger.LogInformation("Inserted new Reviews");
                return new ReviewResponseModel
                {
                    BookId = newReview.BookId,
                    ReviewSID = newReview.ReviewSid,
                    ReviewerName = newReview.ReviewerName,
                    Comment = newReview.Comment,
                    Status = (byte)Enums.StatusTypeDB.Active,
                    Rating = newReview.Rating,
                    ReviewDate = DateTime.Now.Date,
                    CreatedAt = DateTime.Now,
                    LastModifiedAt = DateTime.Now
                };
            }
            catch (Exception)
            {
                throw new HttpStatusCodeException(500);
            }
        }
        public bool UpdateReview(string reviewSid, ReviewRequestModel review)
        {
            var existingReviews = _context.Reviews.FirstOrDefault(r => r.ReviewSid == reviewSid);

            if (existingReviews == null)
                return false;

            var createdAt = existingReviews.CreatedAt;

            _mapper.Map(review, existingReviews);
            existingReviews.ReviewSid = reviewSid;
            existingReviews.CreatedAt = createdAt;
            existingReviews.LastModifiedAt = DateTime.Now;

            _context.SaveChanges();
            _logger.LogInformation("Updated Reviews with Sid: {ReviewSid}", reviewSid);

            return true;
        }
        public async Task<ReviewResponseModel> UpdateReviewAsync(string reviewSid, ReviewRequestModel model)
        {
            try
            {
                var Review = await _unitOfWork.GetRepository<ReviewDBVar>().SingleOrDefaultAsync(x =>
                    x.ReviewSid == reviewSid && x.Status == (int)StatusTypeDB.Active);

                if (Review == null) return null;

                Review.BookId = model.BookId;
                Review.ReviewerName = model.ReviewerName;
                Review.Comment = model.Comment;
                Review.Status = (byte)Enums.StatusTypeDB.Active;
                Review.Rating = model.Rating;
                Review.ReviewDate = DateTime.Now.Date;
                Review.LastModifiedAt = DateTime.Now;

                _unitOfWork.GetRepository<ReviewDBVar>().Update(Review);
                await _unitOfWork.CommitAsync();

                return new ReviewResponseModel
                {
                    BookId = Review.BookId,
                    ReviewSID = Review.ReviewSid,
                    ReviewerName = Review.ReviewerName,
                    Comment = Review.Comment,
                    Status = Review.Status,
                    Rating = Review.Rating,
                    ReviewDate = Review.ReviewDate,
                    CreatedAt = Review.CreatedAt,
                    LastModifiedAt = (DateTime)Review.LastModifiedAt
                };
            }
            catch (Exception)
            {
                throw new HttpStatusCodeException(500);
            }
        }
        public async Task<List<ReviewResponseModel>?> GetReviewsByRatingAsync(int rating)
        {
            try
            {
                var sqlQuery = "sp_getReviewsByRating {0}";
                object[] param = { rating };

                var result = await _spContext.ExecuteStoreProcedure(sqlQuery, param);
                List<ReviewResponseModel>? response = JsonConvert.DeserializeObject<List<ReviewResponseModel>>(result);

                _logger.LogInformation("Got record by SID");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting event by SID: {rating}");
                throw new HttpStatusCodeException(500, "Internal Server Error while fetching event by SID.");
            }
        }

    }
}
