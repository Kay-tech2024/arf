using System;

namespace ARF.Web.Models
{
    public class CommentViewModel
    {
        public bool IsAction { get; set; }
        public string Action { get; set; }
        public string Comment { get; set; }
        public string Username { get; set; }

        public DateTime Date { get; set; }
        public string DateStr { get; set; }
        public long Status { get; set; }
        public bool Hide { get; set; } = false;
        public string Role { get; set; }
        public int RoleGroupCode { get; set; }
        public int CollegeResearchUnitCode { get; set; }
        public string RoleKey { get; set; }
    }
}