using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ARF.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public bool IsActivated { get; set; }
        public bool IsDeleted { get; set; }
        public int? CountryId { get; set; }
        public int? MajorId { get; set; }
        public int? UniversityId { get; set; }
        public int? CollegeId { get; set; }

        public string UniversityOther { get; set; }
        public string CollegeOther { get; set; }
        public string MajorOther { get; set; }
        public string NationalId { get; set; }

        public bool ForcePasswordChange { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string Address { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}