using FluentValidation;
using Review.Models.RequestModel;
using System;

namespace Review.Models.ValidationClass
{
    public class ReviewRequestModelValidator : AbstractValidator<ReviewRequestModel>
    {
        public ReviewRequestModelValidator()
        {
            RuleFor(r => r.BookId).GreaterThanOrEqualTo(1).WithMessage("Book ID must not be empty.");

            RuleFor(r => r.ReviewerName)
                .NotEmpty().WithMessage("Reviewer name must not be empty.")
                .MaximumLength(100).WithMessage("Reviewer name must not exceed 100 characters.");

            RuleFor(r => r.Rating)
                .NotNull().WithMessage("Rating must not be empty.")
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(r => r.Comment)
                .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters.");

            //RuleFor(r => r.ReviewDate)
            //    .NotEmpty().WithMessage("Review date is required.")
            //    .LessThanOrEqualTo(DateTime.Now).WithMessage("Review date cannot be in the future.");

            //RuleFor(r => r.CreatedAt)
            //    .LessThanOrEqualTo(DateTime.Now).WithMessage("CreatedAt cannot be in the future.");

            //RuleFor(r => r.LastModifiedAt)
            //    .GreaterThanOrEqualTo(r => r.CreatedAt)
            //    .When(r => r.LastModifiedAt.HasValue)
            //    .WithMessage("LastModifiedAt must be greater than or equal to CreatedAt.");
        }
    }
}
