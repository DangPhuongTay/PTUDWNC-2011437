﻿
namespace TatBlog.WebApi.Models
{
    public class PostDetail
    {
        // Mã bài viết
        public int Id { get; set; }

        // Tiêu đề bài viết
        public string Title { get; set; }

        // Mô tả hay giới thiệu ngắn về nội dung
        public string ShortDescription { get; set; }

        // Nội dung chi tiết của bài viết
        public string Description { get; set; }
        public bool Published { get; set; }
        //  Metadata
        public string Meta { get; set; }

        // Tên định danh để tạo tập tin hình ảnh
        public string UrlSlug { get; set; }

        // Đường dẫn tập tin hình ảnh
        public string ImageUrl { get; set; }

        // Số lượt xem, đọc bài viết
        public int ViewCount { get; set; }

        // Ngày giờ đăng
        public DateTime PostedDate { get; set; }

        // Ngày giờ cập nhật lần cuối
        public DateTime? ModifiedDate { get; set; }

        // Chuyên mục của bài viết
        public CategoryDto Category { get; set; }

        // Tác giả của bài viết
        public AuthorDto Author { get; set; }

        // Danh sách các từ khoá của bài viết
        public IList<TagDto> Tags { get; set; }

    }
}
