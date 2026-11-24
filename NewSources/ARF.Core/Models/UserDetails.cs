using ARF.Data;
using ARF.Data.DbModels;
using System.Collections.Generic;

namespace ARF.Core.Models
{
    public class UserDetails
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool ForcePasswordChange { get; set; }
        public User DbUser { get; set; }
        public List<RoleDetails> Roles { get; set; }
        public UserSession CurrentSession { get; set; }
        public RoleDetails CurrentRole { get; set; }

        public long? CollegeId
        {
            get
            {
                if (CurrentRole == null) return null;

                return CurrentRole.CollegeId;
            }
        }
    }

    public class RoleDetails
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public ArfUserRole Role { get; set; }
        public long? CollegeId { get; set; }
        protected Privilages? _privilages { get; set; }
    }
}