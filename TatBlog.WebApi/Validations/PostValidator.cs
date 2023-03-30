using FluentValidation;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Validations
{
    public class PostValidator : AbstractValidator<PostEditModel>
    {
        public PostValidator() {
            RuleFor(p => p.Title)
     .NotEmpty()
     .WithMessage("Tiêu đề của bài viết không được để trống")
     .MaximumLength(500)
     .WithMessage("Tiêu đề dài tối đa '{MaxLength}'");

            RuleFor(p => p.ShortDescription)
            .NotEmpty()
            .WithMessage("Giới thiệu về bài viết không được để trống");

            RuleFor(p => p.Description)
            .NotEmpty()
            .WithMessage("Mô tả về bài viết không được để trống");

            RuleFor(p => p.Meta)
            .NotEmpty()
            .WithMessage("Meta của bài viết không được để trống")
            .MaximumLength(1000)
            .WithMessage("Meta dài tối đa '{MaxLength}'");

            RuleFor(p => p.UrlSlug)
            .NotEmpty()
            .WithMessage("Slug của bài viết không được để trống")
            .MaximumLength(1000)
            .WithMessage("Slug dài tối đa '{MaxLength}'");

            RuleFor(p => p.UrlSlug)
            .NotEmpty()
            .WithMessage("Slug của bài viết không được để trống")
            .MaximumLength(1000)
            .WithMessage("Slug dài bài viết '{MaxLength}' kí tự");

            RuleFor(p => p.CategoryId)
            .NotEmpty()
            .WithMessage("Bạn phải chọn chủ đề cho bài viết");

            RuleFor(p => p.AuthorId)
            .NotEmpty()
            .WithMessage("Bạn phải chọn tác giả của bài viết");

            RuleFor(p => p.SelectedTags)
            .Must(HasAtLeastOneTag)
            .WithMessage("bạn phải nhập ít nhất một thẻ");
        }

        // Kiểm tra xem người dùng đã nhập ít nhất 1 thẻ (tag)
        private bool HasAtLeastOneTag(PostEditModel postModel, string selectedTags)
        {
            return postModel.GetSelectedTags().Any();
        }

    }
    
}
