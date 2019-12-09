using CommonLibrary.Models;

namespace Commonlibrary.Models
{
    public class Group
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int InstituteId { get; set; }
        public virtual Institute Institute { get; set; }
    }
}