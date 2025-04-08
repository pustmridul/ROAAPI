namespace MemApp.Application.Mem.Colleges.Models
{
    public class CollegeSearchModel
    {
        public string? SearchKey { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
    public class CollegeDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }

        public int Id { get; set; }
        public string? OldId { get; set; }

    }


}
