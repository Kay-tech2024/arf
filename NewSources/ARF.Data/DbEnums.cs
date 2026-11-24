using System;

namespace ARF.Data
{
    public enum RequestAction : byte
    {
        Unspecified = 0,
        Submit = 1,
        Approve = 2,
        Reject = 3,
        ApproveWithComments = 4,
        Forward = 5,
        Resubmit = 6,
        SubmitCoresearcherData = 7,
        Cancel = 8,
        ReturnToResearcher = 9,
        ForwardToInitialReferee = 10,
        Publish = 11,
        ReturnToResearchUnitHead = 12,
        ReturnToViceDeanOfAcademicResearch = 13,
        SelectReferees = 14,
        ForwardToPressOficer = 15,
        ForwardToProofreader = 16,
        ForwardToResearchUnitSecretary = 17,
        ForwardToDeanOfAcademicResearch = 18,
        ForwardToSpareReferee = 19,
        ApproveWithNoReferees = 20,
        ApproveAndContractPreparation = 21,
        //ApproveAndForwardToDeanshipResearchCentersAndDatabases=23,
        //RejectAndForwardToDeanshipResearchCentersAndDatabases=24,
        SendToSecretary_of_Deanship_Research_Centers_and_databases = 61,
        ApproveAndSendToDirectorOfResearchProjectAndTeachingStaffMembers = 62,
        ForwardToDeanOfScientificResearch = 63,
        ApproveAndForwardToAccountant = 64,
        PaidClaims = 65,
        MarkUnofficialAndForwardToDeanOfScientificResearch = 66,
        ForwardToViceDean_for_research_centers_and_databases = 67,
        ForwardToSecretary_of_Standing_Committee_for_Financing_Research_Projects = 68,

        ApproveContainer = 69,
        RejectContainer = 70,

        SendToViceDeanForHigherStudiesAndScientificResearch = 72,
        SendToDirector_of_Research_Projects_Financing_Program = 73,
        ApproveAndSendToDeanshOfScientificResearch = 74,
        ApproveAndSendToMembersOfGraduateStudies = 75,

        SendTo_ViceDean_for_research_centers_and_databases = 76,
        SendToDeanOfScientificResearch = 77,
        SendToAccountant_of_Deanship_of_Scientific_Research = 78,

        RejectAndReturnToDirectorOfFinancingProgram = 79,

        SendToDirectorOfProgramOfFinance = 81,
        SendToVicePresidentForHigherStudiesAndScientificResearch = 82,
        SendToUniversityRector = 83,

        ApproveAndSendToDirectorGeneralOfAdministrationAndFinance = 85,
        SendToViceDeanForHigherStudiesAndScientificResearch2 = 86,

        ApproveAndForwardToUniversityRector = 87,

        SendToDesigner = 88,
        ApproveAndSendToDesignerAndDirectorOfFinancing = 89,
        RejectAndReturnToDesigner = 90,
        RejectAndSendToDeanshOfScientificResearch = 91,
        RejectAndReturnToResearcher = 92,

        ApproveAndSendToDirectorOfFinancing = 93,
        RejectAndSendToDirectorOfFinancing = 115,

        ApproveAndSelectReferees = 94,

        ForwardToMainReferees = 95,
        ApproveAndReturnToResearcher = 96,
        ApproveAndReturnToResearcherToUploadQuotation = 119,
        RejectWithComments = 97,
        ForwardToResearcher = 98,

        CompleteTheRequest = 99,
        Canceled = 100,
        RejectAndReturnToViceDeanForResearchCentersAndDatabases = 101,
        SendToNewspaperDirector = 102,

        SendToFacultyMember = 103,
        ReturnToViceDeanForResearchCentersAndDatabases = 104,
        ReturnToDirectorOfFinancingProgram = 105,
        SendToViceDeanForResearchCentersAndDatabases = 106,
        ReturnToDesigner = 107,
        ApproveAndForwardToViceDeanOfResearchCentersAndDatabases = 108,
        RejectAndForwardToViceDeanOfResearchCentersAndDatabases = 109,
        SendToCommiteeMembers = 110,

        SendToDeanshOfScientificResearch = 111,

        Accept = 112,
        ResubmitAndReturnToCoresearchers = 113,
        SendToCoresearchers = 114,
        ReturnToResearcherForModifications = 115,
        ApproveAndForwardToUniversityDirector = 116,
        SendToResearcher = 117,
        ApproveAndSendToMainResearcher = 16,
        Confirm = 118,
        AcceptToReview = 151,
        RejectToReview = 152,
        RejectToReviewBySystem = 153,
        AcceptToReviewButNotResponded = 154,
    }

    public enum RequestType : byte
    {
        Unspecificed = 0,
        FundingRequest = 1,
        DesignRequest = 2,
        MidTermProgressReport = 3,
        FulltermProgressReport = 4,
        MaterialRequest = 5,
        AdvertisementLetterForCollegeDeans = 6,
        RefereeReward = 7,
        AnnouncementRequest = 8,
        AdvertisementLetterForIT = 9,
        AdvertisementLetterForNewspaper = 10,
        AdvertisementLetterForDesigner = 11,
        AdvertisementLetterForDARWebsite = 12,
        EditFundingRequest = 13,
    };

    public enum ReqFieldType : byte
    {
        Unspecified = 0,
        IslamicField = 1,
        ArabicLanguageField = 2,
        SocialField = 3,
        AppliedScienceField = 4
    }

    public enum PublicationType : byte
    {
        Unspecificed = 0,
        Book = 1,
        Handbook = 2,
        Thesis = 3,
        Dictionary = 4,
        Encyclopedia = 5,
        Probe = 6
    };

    public enum CodeType : byte
    {
        Unspecified = 0,
        SMS = 1,
        Email = 2,
    };

    [Flags]
    public enum RequestStatus : long
    {
        // Funding request
        FundingRequestRecievedByViceDeanOfGraduateStudies = 1,
        FundingRequestRecievedByViceDeanOfGraduateStudiesCollege = 900001, // These are just virtual Status
        FundingRequestRecievedByViceDeanOfGraduateStudiesInstitute = 900002,  // These are just virtual Status

        FundingRequestRecievedByMembersOfGraduateStudies = 2,
        FundingRequestRecievedByDeanOfScientificResearch = 3,
        FundingRequestRecivedByViceDeanOfResearchCenterAndDatabases = 4,
        FundingRequestRecievedBySecertaryOfDeanShipResearchCentersAndDatabases = 5,
        FundingRequestRecievedBySecretaryOfCommitteeResearchFinance = 7,
        FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjects = 8,
        FundingRequestPendingMainRefrees = 10,
        FundingRequestReceivedByDirectorOfFinancingAfterDeanApproval = 14,
        FundingRequestWaitingForContractPreparation = 16,
        FundingRequestContractIssuedWaitingForResearcher = 17,
        FundingRequestContractAcceptedByResearcher = 18,
        FundingRequestContractReceivedByDeanOfScientificResearch = 19,
        FundingRequestSignOfficalLetterAttachedWithContractByDeanOfScientificResearch = 20,
        FundingRequestContractReceivedByResearchCentersAndDatabases = 21,
        FundingRequestPendingSubmission = 803,
        FundingRequestReceivedByDirectorOfProgramOfFinancing = 804,
        FundingRequestReceivedByDirectorOfProgramOfFinancingAfterRefereeSelection = 805,
        FundingRequestReceivedByDirectorOfProgramOfFinancingAfterRefereeDecision = 806,
        FundingRequestReceivedBySpareReferee = 807,
        FundingRequestPendingModificationsByResearcher = 808,
        FundingRequestPendingModificationsByResearcherForUploadQuotations = 815,
        FundingRequestReceivedByDirectorOfProgramOfFinancingAfterResearcher = 809,
        FundingRequestReceivedByDirectorOfProgramOfFinancingAfterResearcherUploadQuotations = 822,
        FundingRequestContractRejectedByResearcher = 810,
        FundingRequestContractReceivedByDeanOfScientificResearchAfterResearcher = 811,
        FundingRequestContractReceivedByDeanOfScientificResearchAfterSecretary = 813,
        FundingRequestRecievedBySecretaryOfCommitteeResearchFinanceAfterDirectorOfProgramOfFinancing = 812,
        FundingRequestReceivedByDirectorOfProgramOfFinancingAfterSecretary = 820,
        FundingRequestPendingMembersOfTheStandingCommitteeForFinancingResearchProjectsAfterDirectorOfProgramOfFinancing = 821,
        FundingRequestReceivedByResearcher = 8090,
        FundingRequestReceivedByViceDeanOfResearchCentersAndDatabasesAfterRefereeSelection = 8091,
        FundingRequestPendingCouncillMember = 8092,
        FundingRequestPendingCollegeCommittee = 8093,
        FundingRequestReturnedToResearcher = 8094,
        FundingRequestPendingCoresearchers = 8095,
        FundingRequestRejectedByCoresearchers = 8096,

        FundingRequestPendingCoresearchersAfterRequestUpdateAfterRefereeDecision = 8101,

        // Final Report request,
        FinalReportPendingSubmission = 600,
        FinalReportRecievedByVice_of_Deanship_Research_Centers_and_databases = 602,
        FinalReportRecievedByDirector_of_Research_Projects_Financing_Program = 603,
        FinalReportRecievedByDeanOfScientificResearch = 604,
        FinalReportRecievedByAccountant_of_Deanship_of_Scientific_Research = 605,
        FinalReportRecievedByResearcher = 606,
        FinalReportRecievedByDeanOfScientificResearchContainer = 607,
        FinalReportRecievedBy_ViceDean_for_research_centers_and_databases = 608,
        FinalReportRecievedBySecretary_of_Standing_Committee_for_Financing_Research_Projects = 609,
        FinalReportCompleted = 610,

        // Mid-term report
        MidTermReportRecievedByResearcher = 701,
        MidTermReportReceivedByReferee=710,
        MidTermReportReceivedByViceDeanForHigherStudiesAndScientificResearchAfterReferee=711,
        MidTermReportReceivedByViceDeanForHigherStudiesAndScientificResearch = 702,
        MidTermReportRecievedByVice_of_Deanship_Research_Centers_and_databases = 703,
        MidTermReportRecievedByDirector_of_Research_Projects_Financing_Program = 704,
        MidTermReportRecievedByDeanOfScientificResearch = 705,
        MidTermReportRecievedByAccountant_of_Deanship_of_ScientificResearch = 706,
        MidTermReportPendingSubmission = 707,
        MidTermReportRecievedByAccountant_of_Deanship_of_Scientific_Research = 708,
        MidTermReportRecievedByDirector_of_Research_Projects_Financing_ProgramReturnedToResearcher = 709,

        // Referee reward
        RefereeRewardRecievedByDirector_of_Research_Projects_Financing_Program = 851,

        RefereeRewardRecievedBy_ViceDean_for_research_centers_and_databases = 852,
        RefereeRewardRecievedByDeanOfScientificResearch = 853,
        RefereeRewardRecievedByAccountant_of_Deanship_of_Scientific_Research = 854,

        // Material request
        MaterialRequestPendingSubmissionByResearcher = 901,

        MaterialRequestReceivedByViceDeanForHigherStudiesAndScientificResearch = 902,
        MaterialRequestReceivedByDirectorOfProgramOfFinance = 903,
        MaterialRequestReceivedByVicePresidentForHigherStudiesAndScientificResearch = 904,
        MaterialRequestReceivedByUniversityRector = 905,
        MaterialRequestReceivedByDeanOfScientificResearch = 906,
        MaterialRequestReceivedByDirectorGeneralOfAdministrationAndFinance = 907,
        MaterialRequestReceivedByViceDeanForHigherStudiesAndScientificResearch2 = 908,
        MaterialRequestReceivedByResearchCentersAndDatabases = 909,
        MaterialRequestReceivedByUniversityDirector = 910,
        MaterialRequestReceivedByResearcherAfterDirectorGeneralOfAdministrationAndFinance = 911,

        // Edit Funding Request
        EditFundingRequestPendingSubmissionByResearcher = 1001,
        EditFundingRequestReceivedByViceDeanForHigherStudiesAndScientificResearch = 1002,
        EditFundingRequestReceivedByDeanOfScientificResearch = 1003,
        EditFundingRequestReceivedByResearchCentersAndDatabases = 1004,
        EditFundingRequestReceivedByDirectorOfProgramOfFinance = 1005,
        EditFundingRequestReceivedBySecretaryOfStandingCommitteeForFinancingResearchProjects = 1006,
        EditFundingRequestReceivedByMemberOfStandingCommitteeForFinancingResearchProjects = 1007,
        EditFundingRequestReceivedByDirectorOfProgramOfFinanceAfterViceDeanForResearchCentersAndDatabasesDecision = 1008,
        EditFundingRequestPendingCoResearcherApproval = 1009,
        EditFundingRequestPendingCollegeCommittee = 1010,
        EditFundingRequestReceivedByViceDeanForResearchCentersAndDatabases = 1011,

        // Design request
        DesignRequestReceivedByDesigner = 801,

        DesignRequestReceivedByDeanOfScientificResearch = 802,
        DesignRequestReceivedByDirectorOfProgramOfFinance = 890,
        DesignRequestReceivedByDirectorOfProgramOfFinanceAfterDesign = 892,
        DesignRequestReceivedByDesignerAfterDesigner = 893,
        DesignRequestReceivedByViceDeanForResearchCentersAndDatabases = 894,

        DeansLetterRequestPendingSubmission = 870,
        DeansLetterRequestRecievedBy_ViceDean_for_research_centers_and_databases = 871,
        DeansLetterRecievedBy_DeanOfScientificResearch = 872,

        Pending = 291,
        Rejected = 301,
        Approved = 321,
        RejectedByResearcher = 331,
        Canceled = 341,
        Completed = 351,
        NotStarted = 1000,
        Started = 10001,
        NotSubmitted = 10002
    }

    public enum RequestCoresearcherResponseAgreementType : byte
    {
        RequestCreated = 1,
        RequestUpdateAfterRefereeDecision = 2,
    }

    public enum RequestCoresearcherResponseStatus : byte
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        RejectedBySystem = 4,
    }

    public enum TempUrlType : byte
    {
        Unspecified = 0,
        SmsActivation = 1,
        ResearcherEmailActivation = 2,
        CoresearcherApproval = 3,
        RefereeApproval = 4,
        ResetPassword = 5,
        RegisterReferee = 6,
        RefereeAccountActivation = 7,
        RefereeEmailActivation = 8,
        SmsActivationNoUser = 9,
        EmailActivationNoUser = 10,
        WelcomeMessageResearcher = 11,        
        CoresearcherNotification = 12,
        WelcomeMessageReferee = 13,
    }

    public enum RequestHistoryTypes
    {
        Unspecificed = 0,
        RequestCreated = 1,
        SubmittedByMainResearcher = 2,
        ApprovedByCoresearcher = 3,
        SendLinkToCoResearcher = 4,
        RejectByCoResearcher = 5,
        Approve = 14,
        Reject = 15,
        ApproveWithComments = 16,
        Forward = 17,
        Resubmit = 18,
        SubmitCoresearcherData = 19,
        Cancel = 20,
        ReturnToResearcher = 21,
        ForwardToInitialReferee = 22,
        Publish = 23,
        ReturnToResearchUnitHead = 24,
        ReturnToViceDeanOfAcademicResearch = 25,

        EmailSent = 26,
        ReturnToMainResearcher = 27
    }

    public enum LetterTypes : byte
    {
        /// <summary>
        /// Start funding program letter by dean of deanship to director of program
        /// </summary>
        FundingLetter,

        /// <summary>
        /// Request design from director of program  to desinger
        /// </summary>
        RequestDesign,

        /// <summary>
        ///  letter from director of program  to وكیل العمادة لمراكز البحوث وقواعد المعلومات
        /// </summary>
        FundingLetterToDeanshipResearch,
    }

    public enum RequestActions : byte
    {
        Unspecified = 0,
        Approve = 1,
        Reject = 2,
        Returned = 3,
        ConditionalApproval = 4,
        Cancel = 5,
        Submitted = 6,
        Resubmitted = 7
    }

    public enum RequestStep : byte
    {
        Unspecified = 0,
        New = 1,
        ScintificType = 2,
        Bank = 3,
        Participants = 4,
        Agreement = 5,
    }

    public enum RequestCommendation : int
    {
        ValidForFunding = 1,
        ValidForFundingAfterWorkingWithTheNotesReceived = 2,
        NotValidForFundingAtAll = 3
    }

    public enum SubmitTheProposedDesignStatus : int
    {
        Approve = 1,
        Rejected = 2,
    }

    public enum ArfUserRole
    {
        None = 0,

        Referee = 5,
        UniversityRector = 12,
        Researcher = 1,
        Accountant = 23,

        //TODO: REMOVE OLD ROLE new Roles for ARF
        /// <summary>
        /// مدير النظام
        /// </summary>
        SystemAdmin = 100,

        /// <summary>
        ///   عمید عمادة البحث العلمي
        /// </summary>
        DeanofDeanshipofScientificResearch = 101,

        /// <summary>
        /// وكیل العمادة لمراكز البحوث وقواعد المعلومات
        /// </summary>
        ViceDeanshipofResearchcCentersAndDatabases = 102,

        /// <summary>
        /// عمید عمادة تقنیة المعلومات
        /// </summary>
        DeanOfTheDeanshipOfInformationTechnology = 103,

        /// <summary>
        /// عضو ھیئة تدریس
        /// </summary>
        DarWebsiteManager = 104,

        /// <summary>
        /// المصمم
        /// </summary>
        Designer = 105,

        /// <summary>
        /// الباحث الرئیس
        /// </summary>
        PrincipalResearcher = 106,

        /// <summary>
        /// وكیل الكلیة للدراسات العلیا والبحث العلمي ومدراء المراكز العلمیة رئیس وحدة البحوث
        /// </summary>
        ViceDeanForHigherStudiesAndScientificResearch = 107,

        /// <summary>
        /// عميد الكلية
        /// </summary>
        CollegeDean = 201,

        /// <summary>
        /// أعضاء وحدة ومراكز البحوث
        /// </summary>
        UnitMembersAndResearchCenters = 108,

        /// <summary>
        /// وكیل الجامعة للدارسات العلیا والبحث العلمي
        /// </summary>
        VicePresidentForHigherStudiesAndScientificResearch = 109,

        /// <summary>
        /// مدیر الجامعة
        /// </summary>
        UniversityDirector = 110,

        /// <summary>
        /// سكرتیر وكالة العمادة لمراكز البحوث وقواعد المعلومات
        /// </summary>
        SecretaryOfTheDeanshipOfResearchCentersAndDatabases = 111,

        /// <summary>
        /// أمین اللجنة الدائمة لتمویل المشروعات البحثیة
        /// </summary>
        SecretaryOfTheStandingCommitteeForFinancingResearchProjects = 112,

        /// <summary>
        /// أعضاء اللجنة الدائمة لتمویل المشروعات البحثیة
        /// </summary>
        MembersOfTheStandingCommitteeForFinancingResearchProjects = 113,

        /// <summary>
        /// مدیر برنامج تمویل المشروعات البحثیة وأعضاء ھیئة التدریس العاملین
        /// </summary>
        DirectorOfTheProgramOfFinancingResearchProjectsAndTeachingStaffMembers = 114,

        /// <summary>
        /// مدیر عام الشؤون الإداریة والمالیة
        /// </summary>
        DirectorGeneralOfAdministrationAndFinance = 117,

        /// <summary>
        /// مدیر صحيفة مرآة الجامعة
        /// </summary>
        UniversityMirrorDirector = 120,

        /// <summary>
        /// رئيس اللجنة الدائمة لتمویل المشروعات البحثیة
        /// </summary>
        HeadOfTheStandingCommitteeForFinancingResearchProjects = 121,

        /// <summary>
        /// عضو وحدة البحوث
        /// </summary>
        ResearchUnitMember = 122,
    };

    public enum FileType : short
    {
        Unspecified = 0,
        BookRequest = 1,
        HandBookRequest = 2,
        ProbeRequest = 3,
        DictionaryRequest = 4,
        BankDetails = 5,
        ThesisRequest = 6,
        encyclopediaRequest = 7,
        RefereeOpinion = 8,
        AcademicCouncilMemberOpinion = 9,
        DeanshipOfAcademicResearchCouncilMemberOpinion = 10,
        ResearchUnitMemberOpinion = 11,
        MidTermAttachment = 12,
        Contract = 13,
        Resume = 14,
        ArabicDesign = 15,
        EnglishDesign = 16,
        NewspaperLetter = 17,
        CollegeDeansLetter = 18,
        ItLetter = 19,



        // Funding Request sections
        SectionProjectImportance = 20,
        SectionProblem = 21,
        SectionGoals = 22,
        SectionAssumptions = 23,
        SectionTerms = 24,
        SectionObjectiveLimitations = 25,
        SectionSpatialLimitations = 26,
        SectionTemporalLimitations = 27,
        SectionTheoriticalFramework = 28,
        SectionPreviousStudies = 29,
        SectionProcedures = 30,
        SectionSociety = 31,
        SectionTools = 32,
        SectionChapters = 33,
        SectionReferences = 34,

        IbanPhotocopy = 35,

        SectionPublishedPapers = 36,
        SectionInterests = 37,
        SectionCareerHistory = 38,
        SectionTrainingCourses = 39,
        SectionAwards = 40,
        SectionMemberships = 41,
        SectionIbanPhotocopy = 42,
        RequestMaterialsQuotation1 = 43,
        RequestMaterialsQuotation2 = 44,
        RequestMaterialsQuotation3 = 45,
        MaterialsRequest = 46,
        FinalTermAttachment = 47,
        MaterialsRequestCheque = 48,
        Other = 200
    };

    public enum RequestViewStatus : byte
    {
        Unspecified = 0,
        Actionable = 1,
        Returned = 2,
        Approved = 5,
        Rejected = 6,
        Canceled = 7,
        Inbox = 8,
        Outbox = 9,
        Overdue = 10,
    }

    public enum RequestTab : byte
    {
        Unspecified = 0,
        Pending = 1,
        Archive = 2
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public enum Gender : byte
    {
        All = 2,
        Male = 0,
        Female = 1
    }

    public enum LanguagePreference
    {
        DontCare,
        Arabic,
        English
    }

    public enum MenuPreference
    {
        DontCare,
        Collapsed,
        Expanded
    }

    public enum RequestFields
    {
        Id,
        Date,
        Status,
        Month,
        Year,
        Amount,
        JobSeekerName,
        JobSeekerNin,
        Updated,
        DepartmentName,
        JpcName,
        Type
    }

    public enum TemplateKey : byte
    {
        Unspecified = 0,
        CreateNewUser = 1,
        UniversityMirrorNewspaper = 2,
        CollegeDeans = 3,
        ITDeanship = 4,
        CollegeDeans_startCycle = 5,
        ITDeanship_StartCycle = 6,
        funding_request_to_researcher = 7,
        Secretary_of_Standing_Committee_for_Financing_Research_Projects_ApprovalNotificationForResearcher = 8,
        RecievedByDirector_of_Research_Projects_Financing_Program_Notification = 9,
        RefereeRewardRecievedByAccountant_of_Deanship_of_ScientificResearchToRefrees_notification = 10,
        RecievedByDirector_of_Research_Projects_Financing_ProgramForSpareRefree_Notification = 11,
        RecievedByDirector_of_Research_Projects_Financing_Program_Approval_To_researcher_Notificatio = 12,
        RecievedByResearcher_To_researcher_AfterRsearcher_modify_Notification = 13,

        RecievedByDirector_of_Research_Projects_Financing_Program_Approval_To_researcher_AfterRsearcher_modify_Notification = 14,
        RecievedByResearcher_To_researcher_Decide_on_the_contract_Notification = 15,
        RecievedByDirector_of_Research_Projects_Financing_Program_Adoption_copy_To_the_researcher_Notification = 16,
        RecievedFundingRequestByHeadOfResearchUnit_Notification = 17,
        RecievedByDirector_of_Research_Projects_Financing_Program_To_researcher_Notification = 18,

        RecievedByDirector_of_Research_Projects_Financing_Program_Approval_half_period_To_researcher_Notification = 19,
        RecievedByAccountant_of_Deanship_of_ScientificResearch_to_researcher_Notification = 20,
        FinalReportRecievedByVice_of_Deanship_Research_Centers_and_databases_to_researcher_Notification = 21,
        FinalReportRecievedByDirector_of_Research_Projects_Financing_Program_Accreditation_pay_researcher_Notification = 22,
        FinalReportRecievedByAccountant_of_Deanship_of_Scientific_Research_To_researcher_pay_full_Notification = 23,
        FinalReportRecievedBySecretary_of_Standing_Committee_for_Financing_Research_Projects_Making_decision_container_Notification = 24,

        RecievedFundingRequestByHeadOfResearchUnit_To_college_Vicedean_and_college_dean_Notification = 25,
        RecievedByDirector_of_Research_Projects_Financing_Program_To_Dean_of_it_Notification = 26,
        RecievedByDirector_of_Research_Projects_Financing_Program_To_Director_of_University_Mirror_newspaper_Notification = 27,
        RecievedByDirector_of_Research_Projects_Financing_Program_To_f_Notification = 28,

        //new
        RecievedByRecgarcher_complete_Fundingrequest_To_researcher_Notification = 29,

        RecievedByRecgarcher_Aproval_forContract = 30,

        FinalReportRecievedBySecretary_of_Standing_Committee_for_Financing_ApproveContener = 31,

        RecievedByResearcherModifyrequest_Notification = 32,

        NewRefereeEmailActivation = 33,
        NewUserSmsActivation = 34,
        UserActivated = 35,
        ResetPassword = 36,
        ThesisAgreeement = 37,
        BookAgreement = 38,
        ProbeAgreement = 39,
        HandbookAgreement = 40,
        EncyclopediaAgreement = 41,
        DicationaryAgreeement = 42,
        RegisterReferee = 43,
        InviteReferee = 44,
        RegisterRefereeSMS = 45,
        InviteRefereeSMS = 46,
        ContactUsMessage = 47,
        RefereeAccountActivation = 48,
        NewResearcherEmailActivation = 49,
        WelcomeMessageResearcher = 53,
        EditUserPassword = 50,
        SmsActivationNoUser = 51,
        EmailActivationNoUser = 52,
        CoresearcherNotification = 54,
        ExternalRefereeSMSNotification = 58,
        ExternalRefereeEmailNotification = 59,
        InternalRefereeSMSNotification = 60,
        InternalRefereeEmailNotification = 61,
        NewCouncilMemberWelcomeEmail = 62,
        NewCouncilMemberWelcomeSMS = 63,
        NewMembersOfTheStandingCommitteeForFinancingResearchProjectsWelcomeEmail = 64,
        NewMembersOfTheStandingCommitteeForFinancingResearchProjectsWelcomeSMS = 65,
        CoResearcherUpdateRequestEmailNotification = 66,
        CoResearcherUpdateRequestSMSNotification = 67,
        FundingRequestCompletedNotification = 68,
        MaterialRequestChequeReceivedByResearcherEmailNotification = 69,
        MaterialRequestChequeReceivedByResearcherSMSNotification = 70,
        CoResearcherEditRequestEmailNotification = 71,
        CoResearcherEditRequestSMSNotification = 72,
        EditRequestCompletedEmailNotification = 73,
        EditRequestCompletedSMSNotification = 74,
        MidTermExternalRefereeSMSNotification = 75,
        MidTermExternalRefereeEmailNotification =76,
        MidTermInternalRefereeSMSNotification = 77,
        MidTermInternalRefereeEmailNotification = 78,
    }

    public enum NotificationForUser : byte
    {
        AttachmentRequestApproval = 0,
        AttachmentRequestRejected = 1,
        TransferRequestApproval = 2,
        TransferRequestRejected = 3,
        PlacementRequestApproval = 4,
        PlacementRequestRejected = 5,
        ExitRequestApproval = 6,
        ExitRequestRejected = 7,
        ExtentionRequestApproval = 8,
        ExtentionRequestRejected = 9,
        SkipEligibilityCheckRequestApproval = 10,
        SkipEligibilityCheckRequestRejected = 11,
        SetupClaimRequestApproval = 12,
        SetupClaimRequestRejected = 13,
        FixedClaimRequestApproval = 14,
        FixedClaimRequestRejected = 15,
        AttachmentClaimRequestApproval = 16,
        AttachmentClaimRequestRejected = 17,
        PlacementClaimRequestApproval = 18,
        PlacementClaimRequestRejected = 19,
        SustainabilityClaimRequestApproval = 20,
        SustainabilityClaimRequestRejected = 21,
        CoachingClaimRequestAppoval = 22,
        CoachingClaimRequestRejected = 23,
        AttachmentNonJwDClaimRequestApproval = 24,
        AttachmentNonJwDClaimRequestRejected = 25,
        PlacementNonJwDClaimRequestApproval = 26,
        PlacementNonJwDClaimRequestRejected = 27,
        SustainabilityNonJwDRequestApproval = 28,
        SustainabilityNonJwDRequestRejected = 29,
        AttachmentJwDClaimRequestApproval = 30,
        AttachmentJwDClaimRequestRejected = 31,
        PlacementJwDClaimRequestApproval = 32,
        PlacementJwDClaimRequestRejected = 33,
        SustainabilityJwdClaimRequestApproval = 34,
        SustainabilityJwdClaimRequestRejected = 35,
        RefferalRequest = 36
    }

    public enum StatusType : byte
    {
        Basic = 0,
        Extended = 1
    }

    public enum ProviderType : byte
    {
        Unspecified = 0,
        JPC = 1,
        RO = 2,
        DR = 3,
        Branch = 4
    }

    public enum ExitingReason : byte
    {
        Unspecified = 0,
        JourneyEnded = 1,
        SustainabilityReached = 2,
        Penalization = 3,
        Transferred = 4,
        Other = 5
    }

    public enum NotificationTab : byte
    {
        Unspecified = 0,
        Requests = 1,
        Claims = 2,
        Contracts = 3
    }

    public enum CancelationReason : byte
    {
        Unspecified = 0,
        NewAttachmentRequest = 1,
        JobseekerAttached = 2,
        JobseekerExited = 3,
        RejectedByBM = 4,
        RejectedByPM = 5,
        RejectedByTaqat = 6,
        RejectedByJobseeker = 7
    }

    public enum JobseekerCitizenshipType : byte
    {
        Unspecified = 0,
        Saudi = 1,
        NonSaudiWithSaudiMother = 2,
        GccResidentWithSaudiMother = 3,
        MigrationTribe = 4
    }

    public enum FieldType : byte
    {
        Unspecified = 0,
        Numnber = 1,
        ShortText = 2,
        LongText = 3,
        Date = 4,
        DateRange = 5,
        DateTime = 6,
        DateTimeRange = 7,
        Email = 8,
        Phone = 9,
        File = 10,
        Array = 11,
        Name = 12,
        Country = 13
    }

    public enum PaymentMethod : byte
    {
        Unspecified = 0,
        Cash = 1,
        SalaryDeduction = 2
    }

    public enum VacationType : byte
    {
        Unspecified = 0,
        Ordinary = 1,
        Emergency = 2,
        Exceptional = 3,
        ChildCare = 4
    }

    public enum PublishTimeType : byte
    {
        NewPublish = 1,
        Republish = 2
    }

    public enum Degree : byte
    {
        Master = 1,
        Phd = 2
    }

    public enum PublishType : byte
    {
        Governmental = 1,
        Commercial = 2
    }

    public enum BankAccountType : byte
    {
        Local = 1,
        International = 2
    }

    public enum BookType : byte
    {
        PublicScience = 1,
        Systamatic = 2,
        Cultural = 3
    }

    public enum BookLanguage : byte
    {
        Arabic = 1,
        TranslatedTo = 2
    }

    public enum HandBookLanguage : byte
    {
        Arabic = 1,
        TranslatedTo = 2
    }

    public enum HandBookType : byte
    {
        Cultural = 1,
        ChildrenStories = 2,
        Other = 3
    }

    public enum ProbeType : byte
    {
        ManuScript = 1,
        RareBook = 2
    }

    public enum EncyclopediaLanguage : byte
    {
        Arabic = 1,
        TranslatedTo = 2
    }

    public enum DictionaryLanguage : byte
    {
        Arabic = 1,
        TranslatedTo = 2
    }

    public enum ScientificMaterialType : byte
    {
        Commercial = 1,
        Governmental = 2
    }

    public enum ReportNo1RequestStatus : byte
    {
        Submitted = 1,
        Approved = 2,
        PendingMidTermFunding = 3,
        PendingFullTermFunding = 4,
        Rejected = 5,
        MidTermFundingPaid = 6,
        FullTermFundingPaid = 7
    }

    public enum ReportUserRoles : byte
    {
        Referee = 1,
        ResearchTeam = 2,
        MainResearcher = 3,
        AssistantResearcher = 4,
        AssociateResearcher = 5,
        Consultant = 6
    }

    public enum ReportNo2RequestStatus : byte
    {
        Submitted = 1,
        Approved = 2,
        Rejected = 3
    }

    public enum FormType : byte
    {
        Unspecified = 0,
        IslamicFundingRequest = 1,
        ArabicFundingRequest = 2,
        ScientificFundingRequest = 3,
        SocialScienceFundingRequest = 4
    }

    public enum FormFieldType : byte
    {
        Unspecified = 0,
        TableHeading = 1,
        Integer = 2,
        NormalText = 3,
        Date = 4,
        Monetary = 5,
        checkbox = 6,
        Dropdown = 7,
        Email = 8,
        ShortText = 9,
        Heading = 10,
        Text = 11,
        LongText = 12,
        Phone = 13,
        AttachedFile = 14,
        SubHeading = 15,
        DateTime = 16,
        Table = 17,
        ColumnHeading = 18,
        TableEnd = 19,
        TabContainer = 20,
        Float = 21,
        Paragraph = 22,
        HTML = 23,
        IBAN = 24,
        Name = 25,
    }

    public enum DateType { Unspecified = 0, Hijri = 1, Gregorian = 2 }

    public enum ResearcherRole : byte
    {
        Unspecified = 0,
        MainResearcher = 1,
        AssociateResearcher = 2,
        AssistantResearcher = 3,
        Consultant = 4,
    }

    public enum ImportFormat : byte
    {
        Unspecified = 0,
        Referees = 1,
        Researchers = 2
    }



    public enum QualificationType : byte
    {
        Unspecified = 0,
        Doctorate = 1,
        Masters = 2,
        Bachelors = 3,
        Diploma = 4,
        UndergraduateStudent = 5,
        MasterStudent = 6,
        PhdStudent = 7,
        Professor = 8,
        AssociateProfessor = 9,
        AssistantProfessor = 10,
        Technician = 11,
        Vocational = 12,
    }




    public enum MaterialType : byte
    {
        Unspecified = 0,
        Equipment = 1,
        Consumables = 2,
        Chemicals = 3,
        LaboratoryAnalyses = 4,
    }

    public enum FundingStatus : byte
    {
        Unspecified = 0,
        Approved = 1,
        Declined = 2,
        Pending = 3,
    }

    public enum EditFundingRequestType : byte
    {
        WithdrawalFromProject = 1,
        UpdateDuration = 2,
        UpdateResearchersTeam = 3,
        UpdateMaterials = 4,
        UpdateDurationResearchersTeam = 5,
        UpdateDurationMaterials = 6,
        UpdateDurationResearchersTeamMaterials = 7,
        UpdateResearchersTeamMaterials = 8,

    }
}