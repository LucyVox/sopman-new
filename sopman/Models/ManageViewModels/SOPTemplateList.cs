using System;
namespace sopman.Models.ManageViewModels
{
    public class SOPTemplateList
    {
        public string SOPTemplateID { get; set; }
        public string TempName { get; set; }
        public string SOPCode { get; set; }

        public string ExpireDate { get; set; }
        public string InstanceId { get; set; }
        public string InstanceRef { get; set; }
        public string InstanceExpire { get; set; }
        public string ProjectId { get; set; }
        public string ExecuteSopID { get; set; }
        public string SectionId { get; set; }
        public int CompId { get; set; }
        public string ProjectName { get; set; }
        public string UserId { get; set; }

        public int countnum { get; set; }
        public DateTime creationdate { get; set; }
    }
}
