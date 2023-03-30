using FluentValidation;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Validations
{
    public class CategoryValidator : AbstractValidator<CategoryEditModel>
    {
        public CategoryValidator() {
            RuleFor(a => a.Name)
            .NotEmpty()
            .WithMessage("Tên tác giả không được để trống")
            .MaximumLength(100)
            .WithMessage("Tên tác giả dài tối đa '{MaxLength}' kí tự");

            RuleFor(a => a.UrlSlug)
            .NotEmpty()
            .WithMessage("Slug của tác giả không được để trống")
            .MaximumLength(1000)
            .WithMessage("Slug dài tối đa '{MaxLength}' kí tự");
        }
    }
}
