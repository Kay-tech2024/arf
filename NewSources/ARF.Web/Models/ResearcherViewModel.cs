using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARF.Web.Models
{
    public class ResearcherViewModel
    {
        public string NationalId { get; set; } // IBAN (length: 128)     
        public string FirstName { get; set; } // FirstName (length: 150)
        public string SecondName { get; set; } // SecondName (length: 150)
        public string ThirdName { get; set; } // ThirdName (length: 150)
        public string LastName { get; set; } // LastName (length: 150)
        public int? NationalityId { get; set; } // NationalityId      
        public int? UniversityId { get; set; } // UniversityId
        public int? CollegeId { get; set; } // CollegeId
        public int? DepartmentId { get; set; } // DepartmentId
        public string Address { get; set; } // Address (length: 350)
        public string Email { get; set; } // Email (length: 50)
        public string MobileNo { get; set; } // MobileNo (length: 50)
        public string Comments { get; set; } // Comments (length: 250)      
        public int? DegreeId { get; set; } // DegreeId

    }
}