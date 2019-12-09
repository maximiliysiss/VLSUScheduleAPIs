namespace VLSUScheduleAPIs.Models
{
    public enum FilterType
    {
        Default,
        Teacher,
        Group,
        Lesson
    }

    public class FilterResult
    {
        public int ID { get; set; }
        public string Value { get; set; }
        public FilterType FilterType { get; set; }
    }
}
