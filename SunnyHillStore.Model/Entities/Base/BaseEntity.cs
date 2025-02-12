namespace SunnyHillStore.Model.Entities.Base
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
