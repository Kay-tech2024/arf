using ARF.Data;
using System;

namespace ARF.Core
{
    public static class AccessPrivilagesMatrix
    {
        public static Privilages GetByUserRole(ArfUserRole role)
        {
            switch (role)
            {
                case ArfUserRole.SystemAdmin:
                    return SystemAdmin;

                case ArfUserRole.DeanofDeanshipofScientificResearch:
                    return DeanofDeanshipofScientificResearch;

                case ArfUserRole.ViceDeanshipofResearchcCentersAndDatabases:
                    return DeanshipofResearchcCentersAndDatabases;

                case ArfUserRole.DeanOfTheDeanshipOfInformationTechnology:
                    return DeanOfTheDeanshipOfInformationTechnology;

                case ArfUserRole.DarWebsiteManager:
                    return DarWebsiteManager;

                case ArfUserRole.Designer:
                    return Designer;

                case ArfUserRole.PrincipalResearcher:
                    return PrincipalResearcher;

                case ArfUserRole.ViceDeanForHigherStudiesAndScientificResearch:
                    return ViceDeanForGraduateStudies;

                case ArfUserRole.UnitMembersAndResearchCenters:
                    return UnitMembersAndResearchCenters;

                case ArfUserRole.VicePresidentForHigherStudiesAndScientificResearch:
                    return VicePresidentForHigherStudiesAndScientificResearch;

                case ArfUserRole.UniversityDirector:
                    return UniversityDirector;

                case ArfUserRole.SecretaryOfTheDeanshipOfResearchCentersAndDatabases:
                    return SecretaryOfTheDeanshipOfResearchCentersAndDatabases;

                case ArfUserRole.SecretaryOfTheStandingCommitteeForFinancingResearchProjects:
                    return SecretaryOfTheStandingCommitteeForFinancingResearchProjects;

                case ArfUserRole.MembersOfTheStandingCommitteeForFinancingResearchProjects:
                    return MembersOfTheStandingCommitteeForFinancingResearchProjects;

                case ArfUserRole.DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers:
                    return DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers;

                case ArfUserRole.Referee:
                    return Referee;

                case ArfUserRole.DirectorGeneralOfAdministrationAndFinance:
                    return DirectorGeneralOfAdministrationAndFinance;

                case ArfUserRole.Accountant:
                    return Accountant;

                default:
                    return User;
            }
        }

        // public const Privilages PrincipalResearcher = Privilages.account_login | Privilages.account_profile | Privilages.requests_create;

        public const Privilages SystemAdmin =

            Privilages.requests_view_requests |
            Privilages.colleges_view_list |
            Privilages.majors_view_list |
            Privilages.college_profile |
            Privilages.colleges_create_college |

            Privilages.users_create_user |
            Privilages.users_edit_user |
            Privilages.users_view_users_list |
            Privilages.users_delete_user |
            Privilages.reports |
            Privilages.settings_email |
            Privilages.settings_sms |
            Privilages.settings_jobs |
            Privilages.settings_templates |
            Privilages.account_login |
            Privilages.account_profile;

        public const Privilages DeanofDeanshipofScientificResearch =
    Privilages.requests_view_requests |

    Privilages.account_login |
    Privilages.account_profile;

        public const Privilages DeanshipofResearchcCentersAndDatabases =
  Privilages.requests_view_requests |

  Privilages.account_login |
  Privilages.account_profile;

        public const Privilages DeanOfTheDeanshipOfInformationTechnology =
Privilages.requests_view_requests |

Privilages.account_login |
Privilages.account_profile;

        public const Privilages DarWebsiteManager =
Privilages.requests_view_requests |

Privilages.account_login |
Privilages.account_profile;

        public const Privilages Designer =
     Privilages.requests_view_requests |

     Privilages.account_login |
     Privilages.account_profile;

        public const Privilages PrincipalResearcher =
           Privilages.requests_view_requests |
           Privilages.requests_create |
           Privilages.account_login |
           Privilages.account_profile;

        public const Privilages ViceDeanForGraduateStudies =
         Privilages.requests_view_requests |

         Privilages.account_login |
         Privilages.account_profile;

        public const Privilages UnitMembersAndResearchCenters =
       Privilages.requests_view_requests |

       Privilages.account_login |
       Privilages.account_profile;

        public const Privilages VicePresidentForHigherStudiesAndScientificResearch =
     Privilages.requests_view_requests |

     Privilages.account_login |
     Privilages.account_profile;

        public const Privilages UniversityDirector =
     Privilages.requests_view_requests |

     Privilages.account_login |
     Privilages.account_profile;

        public const Privilages SecretaryOfTheDeanshipOfResearchCentersAndDatabases =
   Privilages.requests_view_requests |

   Privilages.account_login |
   Privilages.account_profile;

        public const Privilages SecretaryOfTheStandingCommitteeForFinancingResearchProjects =
Privilages.requests_view_requests |

Privilages.account_login |
Privilages.account_profile;

        public const Privilages MembersOfTheStandingCommitteeForFinancingResearchProjects =
Privilages.requests_view_requests |

Privilages.account_login |
Privilages.account_profile;

        public const Privilages DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers =
Privilages.requests_view_requests |

Privilages.account_login |
Privilages.account_profile;

        public const Privilages ThetwoMainArbitrators =
Privilages.requests_view_requests |

Privilages.account_login |
Privilages.account_profile;

        public const Privilages Referee =
Privilages.requests_view_requests |

Privilages.account_login |
Privilages.account_profile;

        public const Privilages DirectorGeneralOfAdministrationAndFinance =
Privilages.requests_view_requests |

Privilages.account_login |
Privilages.account_profile;

        public const Privilages Accountant =
Privilages.requests_view_requests |

Privilages.account_login |
Privilages.account_profile;

        //public const Privilages Researcher =
        //    //Privilages.requests_view_requests |
        //
        //    Privilages.account_login |
        //    Privilages.account_profile;

        public const Privilages User =
            Privilages.requests_view_requests |
            Privilages.account_login |
            Privilages.account_profile;

        public static Privilages[] ModuleRequests =
        {
            Privilages.requests_view_requests,
            Privilages.requests_create ,
            Privilages.requests_change_js_status ,
            Privilages.requests_approve_transfer ,
            Privilages.requests_approve_status_change_level1 ,
            Privilages.requests_approve_status_change
        };

        public static readonly Privilages[] ModuleClaims =
        {
            Privilages.claims_submit_financial_claim ,
            Privilages.claims_edit_claim_before_approval ,
            Privilages.claims_edit_claim ,
            Privilages.claims_delete_claim_before_approval ,
            Privilages.claims_delete_claim ,
            Privilages.claims_view_claims ,
            Privilages.claims_approve_claims ,
            Privilages.claims_preapprove_claims,
            Privilages.claims_submit_financial_penalty ,
            Privilages.claims_create_financial_penalty ,
            Privilages.claims_approve_financial_penalty
        };

        public static readonly Privilages[] ModuleDepartments =
        {
            Privilages.colleges_view_list,
            Privilages.colleges_create_college,
            Privilages.departments_edit_department,
            Privilages.college_profile,
            Privilages.departments_approve_contract
        };

        public static readonly Privilages[] ModuleUsers =
        {
            Privilages.users_create_user ,
            Privilages.users_edit_user ,
            Privilages.users_delete_user
        };

        public static readonly Privilages[] ModuleReports =
        {
            Privilages.reports
        };

        public static readonly Privilages[] ModuleSettings =
        {
            Privilages.settings_email ,
            Privilages.settings_sms ,
            Privilages.settings_jobs ,
            Privilages.settings_tawafuq ,
            Privilages.settings_webservices ,
            Privilages.settings_change_rag,
            Privilages.settings_templates
        };

        public static readonly Privilages[] ModuleAccount =
        {
            Privilages.account_login ,
            Privilages.account_profile
        };
    }

    [Flags]
    public enum Privilages : ulong
    {
        majors_view_list = ((ulong)1) << 38,
        jobseekers_create_js = ((ulong)1) << 0,
        jobseekers_refer_js = ((ulong)1) << 43,
        jobseekers_edit_js_walkin = ((ulong)1) << 1,
        jobseekers_refer_js_to_jpc = ((ulong)1) << 44,
        jobseekers_edit_js_all = ((ulong)1) << 2,
        jobseekers_delete_js = ((ulong)1) << 3,
        jobseekers_attach_js = ((ulong)1) << 4,
        jobseekers_attach_jpc_approve = ((ulong)1) << 41,
        jobseekers_import_momora_moe = ((ulong)1) << 42,
        requests_create = ((ulong)1) << 5,
        requests_change_js_status = ((ulong)1) << 6,
        requests_approve_transfer = ((ulong)1) << 7,
        requests_approve_status_change_level1 = ((ulong)1) << 8,
        requests_approve_status_change = ((ulong)1) << 9,
        requests_view_requests = ((ulong)1) << 39,
        jobseekers_import = ((ulong)1) << 10,
        claims_submit_financial_claim = ((ulong)1) << 11,
        claims_edit_claim_before_approval = ((ulong)1) << 12,
        claims_edit_claim = ((ulong)1) << 13,
        claims_delete_claim_before_approval = ((ulong)1) << 14,
        claims_delete_claim = ((ulong)1) << 15,
        claims_view_claims = ((ulong)1) << 16,
        claims_approve_claims = ((ulong)1) << 17,
        claims_preapprove_claims = ((ulong)1) << 40,
        claims_submit_financial_penalty = ((ulong)1) << 18,
        claims_create_financial_penalty = ((ulong)1) << 19,
        claims_approve_financial_penalty = ((ulong)1) << 20,
        colleges_view_list = ((ulong)1) << 21,
        colleges_create_college = ((ulong)1) << 22,
        departments_edit_department = ((ulong)1) << 23,
        departments_approve_contract = ((ulong)1) << 45,
        users_view_users_list = ((ulong)1) << 24,
        users_create_user = ((ulong)1) << 25,
        users_edit_user = ((ulong)1) << 26,
        users_delete_user = ((ulong)1) << 27,
        reports = ((ulong)1) << 28,
        settings_email = ((ulong)1) << 29,
        settings_sms = ((ulong)1) << 30,
        settings_jobs = ((ulong)1) << 31,
        settings_tawafuq = ((ulong)1) << 32,
        settings_webservices = ((ulong)1) << 33,
        settings_change_rag = ((ulong)1) << 34,
        settings_templates = ((ulong)1) << 46,
        account_login = ((ulong)1) << 35,
        account_profile = ((ulong)1) << 36,
        college_profile = ((ulong)1) << 37,
    }
}