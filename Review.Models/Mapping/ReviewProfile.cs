using AutoMapper;
using Review.Models.RequestModel;

namespace Review.Models.Mapping
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<ReviewRequestModel, Review.Models.Models.ReviewDB.Review>();
            CreateMap<Review.Models.Models.ReviewDB.Review, ReviewResponseModel>();
        }

    }
}
