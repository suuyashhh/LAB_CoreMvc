using System;

namespace Models.Admin
{
    public class PortfolioProject
    {
        public int ProjectId { get; set; }
        public int SrNo { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public string Technologies { get; set; }
        public string CodeLink { get; set; }
        public string LiveDemoLink { get; set; }
        public string ApkFile { get; set; }
        public string DesktopFile { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
