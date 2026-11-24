using ARF.Data;
using ARF.Data.DbModels;
using ARF.TimeMachine;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ARF.Core.Managers
{
    public class TemplatesManager : AbstractManager
    {
        private Dictionary<TemplateKey, Template> _templatesDict;

        private static List<Template> _defaults = new List<Template>()
        {
                new Template
                {
                    TemplateKey = (short)TemplateKey.UniversityMirrorNewspaper,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "<h2>خطاب الإعلان</h2>",
                    BodyArabic = "<p>السلام عليكم ورحمة الله وبركاته وبعد:    "+
                    "فأسال الله لكم الاعانة والتوفيق. ثم أشير إلى أن عمادة البحث العلمي تعمل حاليا على"+
                    "إطلاق الدورة لبرنامج تمويل المشروعات البحثية للعام الجامعي "+
                    " آمل التكرم بتوجيه من يلزم بوضع الإعلان المرفق على صفحات صحيفة مرآة الجامع"+
                    "والسلام عليكم ورحمة الله وبركاته. "+
                     "عميد عمادة البحث العلمي  "+
                     "د. عبد الرحمن بن عبد العزيز "+
                    "</b>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.CollegeDeans_startCycle,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "<h2>إشعار بتاريخ بداية دورة التمويل</h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "سعادة العميد {name}  <br>"+
                     " تجدون في المرفقات الخطاب المتضمن لتاريخ بداية دورة التمويل.<br>"+

                    "</b>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.ITDeanship_StartCycle,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "<h2>خطاب الإعلان</h2>",
                    BodyArabic = "<p>سعادة عميد عمادة تقنية المعلومات                                        حفظه الله   <br>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:"+
                    "فأشير إلى رغبة عمادة البحث العلمي إلى توجيه رسائل جوال قصيرة لكل أعضاء هيئة التدريس بالجامعة، تتضمن النص التالي: <br>"+
                    "يسر عمادة البحث العلمي ان تعلن عن انطلاق الدورة العاشرة لبرنامج تمويل المشروعات  "+
                    "البحثية في الجامعة في الفترة الممتدة وذلك خلال الفترة الممتدة من {start date-h} "+
                     "الاولى ١٤٣٨ه الموافق {start date-g} وحتى نهاية {enddate-h}  "+
                     "{enddate-g} . ولمزيد من المعلومات يمكن الرجوع الى موقع عمادة البحث العلمي على بوابة الجامعة: "+
                    "آمل من سعادتكم توجيه من ترون لاتخاذ اللازم. شاكراً لكم حسن تعاونكم وجميل تعاملكم."+
                    "والسلام عليكم ورحمة الله وبركاته،،،"+
                    "عميد البحث العلمي                                                  "+
                    " د. عبد الرحمن بن عبد العزيز المقبل                                                                    "+

                    "</b>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.funding_request_to_researcher,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "<h2>إشعار بطلب التمويل</h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "الباحث {researcher_name},نفيدكم بإنه تم إرسال طلب التمويل الخاص بكم "+

                    "</b>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.Secretary_of_Standing_Committee_for_Financing_Research_Projects_ApprovalNotificationForResearcher,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "<h2>إشعار بطلب التمويل</h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "سعادة امين اللجنة الدائمة لتمويل المشروعات البحثية <br>"+
                     " نفيدكم بأنه تم إرسال اليكم طلب لإتخاذ القرار بخصوص طلب التمويل <br>"+
                     " رقم الطلب{request_id} "+

                    "</b>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByDirector_of_Research_Projects_Financing_Program_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "<h2>إشعار طلب التحكيم للمحكمين الاساسيان </h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "سعادة مدير برنامج تمويل المشروعات البحثية   <br>"+
                     " نفيدكم بأنه تم إرسال اليكم طلب لإرسال طلب التحكيم للمحكمين الأساسيان <br>"+
                     " رقم الطلب{request_id} "+

                    "</b>",
                },

                 new Template
                {
                    TemplateKey = (short)TemplateKey.EditUserPassword,
                    SubjectEnglish = "Your new password on ARF",
                    BodyEnglish = "<p>Dear <b>{Name}</b>,</p><p>Your password for the account {Username} has been updated as follows:</p><ul><li>Password: <b>{Password}</b></li></ul><p>Best regards,</p><p><em>ARF Team</em></p>",
                    SubjectArabic = "معلومات حساب في نظام تمويل المشروعات البحثية",
                    BodyArabic = "<p>مرحبًأ يا <b>{Name}</b>,</p><p>لقد تم تغيير كلمة المرور لحسابك {Username} وهي كالتالي:</p><ul<li>كلمة المرور: <b>{Password}</b></li></ul><p>تحياتنا,</p><p><em>فريق ARF</em></p>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.RefereeRewardRecievedByAccountant_of_Deanship_of_ScientificResearchToRefrees_notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "<h2>إشار بصرف مكافأة محكم  </h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "سعادة المحكم{refree_name}   <br>"+
                     " نفيدكم بأنه تم صرف مكافأة لكم  <br>"+

                    "</b>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByDirector_of_Research_Projects_Financing_ProgramForSpareRefree_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "<h2>إشعار طلب التحكيم للمحكم المرجح </h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "سعادة المحكم المرجح{SpareRefree_name}   <br>"+
                     " نفيدكم بأنه تم إرسال اليكم طلب التحكيم <br>"+
                     " رقم الطلب{request_id} "+

                    "</b>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByDirector_of_Research_Projects_Financing_Program_Approval_To_researcher_Notificatio,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "<h2>إشعار بحالة الطلب  </h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "الباحت {researcher_name}   <br>"+
                     " نفيدكم بأن حالة طلبكم {request_status} <br>"+
                     " رقم الطلب{request_id} "+

                    "</b>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByResearcher_To_researcher_AfterRsearcher_modify_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "<h2>إشعار بحالة تعديل الطلب   </h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "الباحت {researcher_name}   <br>"+
                     " نفيدكم بأن حالة طلبكم {request_status} <br>"+
                     " رقم الطلب{request_id} "+

                    "</b>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByDirector_of_Research_Projects_Financing_Program_Approval_To_researcher_AfterRsearcher_modify_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "<h2>إشعار بحالة تعديل الطلب   </h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "الباحت {researcher_name}   <br>"+
                     " نفيدكم بأن حالة طلبكم {request_status} <br>"+
                     " رقم الطلب{request_id} "+

                    "</b>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByResearcher_To_researcher_Decide_on_the_contract_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "<h2>إشعار بإتخاذ قرار بخصوص العقد   </h2>",
                    BodyArabic = "<b>"+
                     "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                     "الباحت {researcher_name}   <br>"+
                     " يرجى أخذ قرار بخصوص العقد  <br>"+
                     " رقم العقد{contract_id} "+

                    "</b>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByDirector_of_Research_Projects_Financing_Program_Adoption_copy_To_the_researcher_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "<h2>إشعار بحالةإعتماد الطلب   </h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "الباحت {researcher_name}   <br>"+
                     " نفيدكم بأن حالة طلبكم {request_status} <br>"+
                     " رقم الطلب{request_id} "+

                    "</b>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedFundingRequestByHeadOfResearchUnit_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعار إستلام خطاب التمويل ونسخه من العقد",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "المكرم <b>{name}</b>:    <br>"+
                     "مرفق لكم نسخة من العقد"+
                     "  "+

                    "</b>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.FundingRequestCompletedNotification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعار إستلام خطاب التمويل ونسخه من العقد",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "المكرم <b>{name}</b>:    <br>"+
                     "مرفق نسخة من العقد"+
                     "  "+

                    "</b>",
                },

        //Funding request Notification End Here

        // Mid-Term_Notification
                new Template
                {
                    TemplateKey = (short)TemplateKey.MidTermExternalRefereeSMSNotification,
                    SubjectEnglish = "You are invited to review MidTerm request.",
                    BodyEnglish = "سعادة الدكتور {FullName} نفيدكم بأنه تم ترشيحكم للانضمام لبرنامج تمويل المشروعات البحثية بجامعة الإمام محمد بن سعود الإسلامية؛ لتحكيم طلب تمويل مشروعي بحثي، لطفاً اضغط على هذا الرابط للدخول على صفحة التحكيم. عمادة البحث العلمي.",
                    SubjectArabic = "You are invited to review MidTerm request",
                    BodyArabic = "سعادة الدكتور {FullName} نفيدكم بأنه تم ترشيحكم للانضمام لبرنامج تمويل المشروعات البحثية بجامعة الإمام محمد بن سعود الإسلامية؛ لتحكيم طلب تمويل مشروعي بحثي لطفاً  قم بزيارة بريدكم الإلكتروني للإطلاع على التفاصيل. عمادة البحث العلمي.",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.MidTermInternalRefereeSMSNotification,
                    SubjectEnglish = "You are invited to review MidTerm request.",
                    BodyEnglish = "سعادة عضو/عضوة هيئة التدريس نفيدكم بأنه تم ترشيحكم لتحكيم طلب تمويل مشروعي بحثي وبانتظار موافقتكم الكريمة على التحكيم. ويسعدنا في عمادة البحث العلمي التعاون معكم.",
                    SubjectArabic = "You are invited to review MidTerm request",
                    BodyArabic = "سعادة عضو/عضوة هيئة التدريس نفيدكم بأنه تم ترشيحكم لتحكيم طلب تمويل مشروعي بحثي وبانتظار موافقتكم الكريمة على التحكيم. ويسعدنا في عمادة البحث العلمي التعاون معكم.",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.MidTermExternalRefereeEmailNotification,
                    SubjectEnglish = "سعادة الدكتور عضو هيئة التدريس بجامعة 'الإمام محمد بن سعود الإسلامية'",
                    BodyEnglish = "سعادة الدكتورسعادة الدكتور {FullName} وفقه الله<br/>السلام عليكم ورحمة الله وبركاته،،<br/>ترحب بكم عمادة البحث العلمي في برنامجها تمويل المشروعات البحثية وتتمنى لكم مزيداً من النجاح والتميز.<br/>وتدعوكم لتحكيم طلب 	تمويل مشروعي بحثي،  لطفاً.. تفضل بالدخول على <a href='{REQUEST_DETAIL_PUBLIC_LINK}'>(هذا الرابط) </a>  للاطلاع على المادة المطلوب تحكيمها.<br/><br/>والله ولي التوفيق",
                    SubjectArabic = "سعادة الدكتور عضو هيئة التدريس بجامعة 'الإمام محمد بن سعود الإسلامية'",
                    BodyArabic = "سعادة الدكتورسعادة الدكتور {FullName} وفقه الله<br/>السلام عليكم ورحمة الله وبركاته،،<br/>ترحب بكم عمادة البحث العلمي في برنامجها تمويل المشروعات البحثية وتتمنى لكم مزيداً من النجاح والتميز.<br/>وتدعوكم لتحكيم طلب 	تمويل مشروعي بحثي،  لطفاً.. تفضل بالدخول على <a href='{REQUEST_DETAIL_PUBLIC_LINK}'>(هذا الرابط) </a>  للاطلاع على المادة المطلوب تحكيمها.<br/><br/>والله ولي التوفيق",
                },
                new Template
                {
                    TemplateKey = (short)TemplateKey.MidTermInternalRefereeEmailNotification,
                    SubjectEnglish = "سعادة الدكتور عضو هيئة التدريس بجامعة 'الإمام محمد بن سعود الإسلامية'",
                    BodyEnglish = "سعادة الدكتور عضو هيئة التدريس بجامعة 'الإمام محمد بن سعود الإسلامية'السلام عليكم ورحمة الله وبركاته،،<br/>ترحب بكم عمادة البحث العلمي في برنامجها تمويل المشروعات البحثية وتتمنى لكم مزيداً من النجاح والتميز..<br/>وتدعوكم لتحكيم طلب تمويل مشروعي بحثي،  لطفاً.. تفضل بالدخول على صفحتكم على <a href='{REQUEST_DETAIL_INTERNAL_LINK}'>نظام تمويل المشروعات البحثية</a> ومن ثم اختيار صفة ( محكم )؛ للاطلاع على المادة المطلوب تحكيمها.<br/>ملاحظة: إن كنت غير مسجلاً بـ  نظام تمويل المشروعات البحثية، لطفاً.. يرجى تسجيل مستخدم جديد.<br/><br/>والله ولي التوفيق.",
                    SubjectArabic = "سعادة الدكتور عضو هيئة التدريس بجامعة 'الإمام محمد بن سعود الإسلامية'",
                    BodyArabic = "سعادة الدكتور عضو هيئة التدريس بجامعة 'الإمام محمد بن سعود الإسلامية'السلام عليكم ورحمة الله وبركاته،،<br/>ترحب بكم عمادة البحث العلمي في برنامجها تمويل المشروعات البحثية وتتمنى لكم مزيداً من النجاح والتميز..<br/>وتدعوكم لتحكيم طلب تمويل مشروعي بحثي،  لطفاً.. تفضل بالدخول على صفحتكم على <a href='{REQUEST_DETAIL_INTERNAL_LINK}'>نظام تمويل المشروعات البحثية</a> ومن ثم اختيار صفة ( محكم )؛ للاطلاع على المادة المطلوب تحكيمها.<br/>ملاحظة: إن كنت غير مسجلاً بـ  نظام تمويل المشروعات البحثية، لطفاً.. يرجى تسجيل مستخدم جديد.<br/><br/>والله ولي التوفيق.",
                },
                new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByDirector_of_Research_Projects_Financing_Program_To_researcher_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعار بإستلام تقرير نصف المدة    </h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "الباحث{researcher_name}<br>"+
                     " نفيدكم بتقرير نصف المدة <br>"+
                     " رقم التقرير{report_id} "+

                    "</b>"
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByDirector_of_Research_Projects_Financing_Program_Approval_half_period_To_researcher_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعاربقرار مدير برنامج تمويل المشروعات البحثية بخصوص نصف المدة     </h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "الباحث{researcher_name}<br>"+
                     " نفيدكم بقرار مدير برنامج تمويل المشروعات البحثية  <br>"+
                     " حالة التقرير{half_report_status} "+

                    "</b>"
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByAccountant_of_Deanship_of_ScientificResearch_to_researcher_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعاربصرف مكافأة نصف المدة      </h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "الباحث{researcher_name}<br>"+
                     " نفيدكم بأنه تم صرف مكافأة نصف المدة  <br>"+

                    "</b>"
                },

        //Full-period report

                new Template
                {
                    TemplateKey = (short)TemplateKey.FinalReportRecievedByVice_of_Deanship_Research_Centers_and_databases_to_researcher_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعاربإعادة الطلب للباحث لإستكماله      </h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "الباحث{researcher_name}<br>"+
                     " نفيدكم بأنه تم إرجاع الطلب لإستكمال النواقص  <br>"+
                     "رقم الطلب{request_id}"+

                    "</b>"
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.FinalReportRecievedByDirector_of_Research_Projects_Financing_Program_Accreditation_pay_researcher_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعارإعتماد صرف المبالغ المالية لنهاية المدة للمشروع البحثي       </h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "الباحث{researcher_name}<br>"+
                     " نفيدكم بأنه تم إعتماد صرف المبالغ المالية لنهاية المدة للمشروع البحثي  <br>"+

                    "</b>"
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.FinalReportRecievedByAccountant_of_Deanship_of_Scientific_Research_To_researcher_pay_full_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعار صرف المبالغ المالية لنهاية المدة للمشروع البحثي       </h2>",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "الباحث{researcher_name}<br>"+
                     " نفيدكم بأنه تم  صرف المبالغ المالية لنهاية المدة للمشروع البحثي  <br>"+

                    "</b>"
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.FinalReportRecievedBySecretary_of_Standing_Committee_for_Financing_Research_Projects_Making_decision_container_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعار بقرار امين اللجنة الدائمة لتمويل المشروعات البحثية",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "الباحث{researcher_name}<br>"+
                    " نفيدكم بإشعار بقرار امين اللجنة الدائمة لتمويل المشروعات البحثية  <br>"+
                    "حالة الطلب {request_status}"+

                    "</b>"
                },

        //   Announcement request

                new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedFundingRequestByHeadOfResearchUnit_To_college_Vicedean_and_college_dean_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعار باستلام خطاب بداية دورة التمويل ",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته {name} وبعد:<br>"+
                    ""+
                    "مرفق خطاب إعلان بداية دورة تمويل المشروعات البحثية."+

                    "</b>"
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByDirector_of_Research_Projects_Financing_Program_To_Dean_of_it_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعار بخطاب لطلب الاعلان على الموقع الالكتروني لجامعة الامام محمد بن سعود الإسلامية",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                   "سعادة عميد تقنية المعلومات {name}"+
                    " نفيدكم بإشعار طلب الاعلان على الموقع الالكتروني لجامعة الامام محمد بن سعود الإسلامية  <br>"+

                    "</b>"
                },

   new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByDirector_of_Research_Projects_Financing_Program_To_Director_of_University_Mirror_newspaper_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعار بخطاب لطلب الاعلان على الموقع الالكتروني لجامعة الامام محمد بن سعود الإسلامية",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                   "مدير صحيفة مرأة الجامعة{}"+
                    " نفيدكم بإشعار طلب الاعلان على الموقع الالكتروني لجامعة الامام محمد بن سعود الإسلامية  <br>"+

                    "</b>"
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByDirector_of_Research_Projects_Financing_Program_To_f_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعار  بتصميم الاعلان الخاص ببداية دورة التمويل ",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                     "عضو هيئة التدريس  {name}"+
                    " نفيدكم بالتصميم الخاص ببداية الدورة  <br>"+

                    "</b>"
                },

                       new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByRecgarcher_complete_Fundingrequest_To_researcher_Notification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعار باكتمال طلب التمويل     </h2>",
                    BodyArabic = "<b>"+
                    "عزيزي الباحث نفيدكم بأنه تم قبول طلب التمويل الخاص بكم :<br>"+
                    "الباحث{researcher_name}<br>"+

                    "</b>"
                },

                 new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByRecgarcher_Aproval_forContract,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشعار بالموافقة على العقد      </h2>",
                    BodyArabic = "<b>"+
                    "عزيزي الباحث {researcher_name} نفيدكم بأنه تمت الموافقة على الغقد الخاص بكم  :<br>"+

                    "</b>"
                },

                       new Template
                {
                    TemplateKey = (short)TemplateKey.FinalReportRecievedBySecretary_of_Standing_Committee_for_Financing_ApproveContener
,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "الموافقة على الوعاء    </h2>",
                    BodyArabic = "<b>"+
                    "عزيزي الباحث {researcher_name}  نفيدكم بأنه تم الموافقه على الوعلء البحثي   :<br>"+

                    "</b>"
                },

                   new Template
                {
                    TemplateKey = (short)TemplateKey.RecievedByResearcherModifyrequest_Notification
,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "الموافقة على الوعاء    </h2>",
                    BodyArabic = "<b>"+
                    "عزيزي الباحث {researcher_name}  نفيدكم بأنه تم الموافقه على الوعلء البحثي   :<br>"+

                    "</b>"
                },

                 new Template
                {
                    TemplateKey = (short)TemplateKey.RegisterReferee,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic ="دعوة لتحكيم مادة علمية",
                     BodyArabic = "<p>الأستاذ الفاضل <b>{name}</b>، السلام عليكم ورحمة الله وبركاته وبعد:    "+
                    "يسرنا إبلاغكم بأنه قد تم دعوتكم لتحكيم مادة علمية لجامعة الإمام محمد بن سعود الإسلامية، وهي بعنوان <b>\"{title}\"</b> ونوعها <b>{type}</b>. للتسحيل يرجى الضغط <a href=\"{link}\">هنا</a>."+
                    "</p>",
                },

                   new Template
                {
                    TemplateKey = (short)TemplateKey.RegisterRefereeSMS,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic ="دعوة لتحكيم مادة علمية",
                     BodyArabic = "<p>الأستاذ الفاضل <b>{name}</b>، السلام عليكم ورحمة الله وبركاته وبعد:    "+
                    "يسرنا إبلاغكم بأنه قد تم دعوتكم لتحكيم مادة علمية لجامعة الإمام محمد بن سعود الإسلامية، وهي بعنوان <b>\"{title}\"</b> ونوعها <b>{type}</b>. للتسحيل يرجى الضغط {link}."+
                    "</p>",
                },

                 new Template
                {
                    TemplateKey = (short)TemplateKey.InviteReferee,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic ="دعوة لتحكيم مادة علمية",
                     BodyArabic = "<p>الأستاذ الفاضل <b>{name}</b>، السلام عليكم ورحمة الله وبركاته وبعد:    "+
                    "يسرنا إبلاغكم بأنه قد تم دعوتكم لتحكيم مادة علمية لجامعة الإمام محمد بن سعود الإسلامية، وهي بعنوان <b>\"{title}\"</b> ونوعها <b>{type}</b>. لقبول الدعوة يرجى الدخول للموقع من <a href=\"{link}\">هنا</a>."+
                    "</p>",
                },

                      new Template
                {
                    TemplateKey = (short)TemplateKey.InviteRefereeSMS,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic ="دعوة لتحكيم مادة علمية",
                     BodyArabic = "الأستاذ الفاضل <b>{name}</b>، السلام عليكم ورحمة الله وبركاته وبعد:    "+
                    "يسرنا إبلاغكم بأنه قد تم دعوتكم لتحكيم مادة علمية لجامعة الإمام محمد بن سعود الإسلامية، وهي بعنوان <b>\"{title}\"</b> ونوعها <b>{type}</b>. لقبول الدعوة يرجى الدخول للموقع من {link}."+
                    "</p>",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.NewUserSmsActivation,
                    SubjectEnglish = "",
                    BodyEnglish = "Your activation code in ARF is {code}",
                    SubjectArabic = "",
                    BodyArabic = "رمز التحقق الخاص بك في نظام تمويل المشروعات البحثية هو {code}",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.SmsActivationNoUser,
                    SubjectEnglish = "",
                    BodyEnglish = "Your activation code in ARF is {code}",
                    SubjectArabic = "",
                    BodyArabic = "رمز التحقق الخاص بك في نظام تمويل المشروعات البحثية هو {code}",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.EmailActivationNoUser,
                    SubjectEnglish = "Your activation code in ARF",
                    BodyEnglish = "Your activation code in ARF is {code}",
                    SubjectArabic = "رمز التفعيل",
                    BodyArabic = "رمز التحقق الخاص بك في نظام تمويل المشروعات البحثية هو {code}",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.NewResearcherEmailActivation,
                    SubjectEnglish = "",
                    BodyEnglish = "",

                    SubjectArabic = "تفعيل مستخدم جديد في نظام تمويل المشروعات البحثية",
                    BodyArabic = "<div style=\"direction: rtl\"><p>سعادة الدكتور <b>{name}</b> عضو هيئة التدريس بجامعة \"الإمام محمد بن سعود الإسلامية\"</p><p>السلام عليكم ورحمة الله وبركاته،،</p><p>ترحب بكم \"عمادة البحث العلمي\" في برنامجها \"تمويل المشروعات البحثية\" وتتمنى لكم مزيداً من النجاح والتميز.. </p><p>لتفعيل حسابكم <b>{email}</b> لطفاً <a href=\"{link}\">اضغط هنا</a>.</p> <p>والله ولي التوفيق. </p></div>",
                },

                  new Template
                {
                    TemplateKey = (short)TemplateKey.CoresearcherNotification,
                    SubjectEnglish = "",
                    BodyEnglish = "",

                    SubjectArabic = "دعوة جديدة للمشاركة في مشروع بحثي",
                    BodyArabic = "<div style=\"direction: rtl\"><p>سعادة الأستاذ <b>{name}</b> \"</p><p>السلام عليكم ورحمة الله وبركاته،،</p><p>تم دعوتكم للمشاركة في مشروع بحثي بعنوان {title}. للموافقة أو الرفض لطفًا.. <a href=\"{link}\">اضغط هنا</a>.</p> <p>والله ولي التوفيق. </p></div>",
                },

                  new Template
                {
                    TemplateKey = (short)TemplateKey.WelcomeMessageResearcher,
                    SubjectEnglish = "",
                    BodyEnglish = "",

                    SubjectArabic = "مرحبًا بكم في نظام تمويل المشروعات البحثية",
                    BodyArabic = "<div style=\"direction: rtl\"><p>سعادة الدكتور <b>{name}</b> عضو هيئة التدريس بجامعة \"الإمام محمد بن سعود الإسلامية\"</p><p>السلام عليكم ورحمة الله وبركاته،،</p><p>ترحب بكم \"عمادة البحث العلمي\" في برنامجها \"تمويل المشروعات البحثية\" وتتمنى لكم مزيداً من النجاح والتميز..<p>والله ولي التوفيق. </p></div>",
                },
                new Template
                {
                    TemplateKey = (short)TemplateKey.NewCouncilMemberWelcomeEmail,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "بيانات عضو المجلس بنظام تمويل المشروعات البحثية",
                    BodyArabic = "<div style=\"direction: rtl\"><p>{FullName} </p> <p>السلام عليكم ورحمة الله وبركاته،، </p> <p>ترحب بكم 'عمادة البحث العلمي' في برنامجها 'تمويل المشروعات البحثية' وتتمنى لكم مزيداً من النجاح والتميز.. </p><p>نفيدكم بأنه  تم إضافتكم إلى مجلس مركز/وحدة البحوث في كلية /معهد الكلية او المعهد.</p><p>والبيانات الخاصة بكم للدخول على النظام هي:</p><p>اسم المستخدم : {Email}</p><p>كلمة المرور: {Password}</p><p>للتسجيل الدخول في نظام تمويل المشروعات البحثية <a href='{Link}'> إضغط هنا</a></p><p>والله ولي التوفيق. </p></div>",
                },
                new Template
                {
                    TemplateKey = (short)TemplateKey.NewCouncilMemberWelcomeSMS,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "بيانات عضو المجلس بنظام تمويل المشروعات البحثية",
                    BodyArabic = "سعادة الأستاذ الدكتور {FullName} عضو المجلس لقد تم إنشاء حساب لكم على نظام تمويل المشروعات البحثية الإلكتروني. نرجو الاطلاع على بريدكم الإلكتروني الجامعي الرسمي.",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.NewMembersOfTheStandingCommitteeForFinancingResearchProjectsWelcomeEmail,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "بيانات عضومجلس اللجنة الدائمة لتمويل المشروعات البحثية",
                    BodyArabic = "<div style=\"direction: rtl\"><p>{FullName} </p> <p>السلام عليكم ورحمة الله وبركاته،، </p> <p>ترحب بكم 'عمادة البحث العلمي' في برنامجها 'تمويل المشروعات البحثية' وتتمنى لكم مزيداً من النجاح والتميز.. </p><p>نفيدكم بأنه  تم إضافتكم إلى مجلس اللجنة الدائمة لتمويل المشروعات البحثية.</p><p>والبيانات الخاصة بكم للدخول على النظام هي:</p><p>اسم المستخدم : {Email}</p><p>كلمة المرور: {Password}</p><p>للتسجيل الدخول في نظام تمويل المشروعات البحثية <a href='{Link}'> إضغط هنا</a></p><p>والله ولي التوفيق. </p></div>",
                },
                new Template
                {
                    TemplateKey = (short)TemplateKey.NewMembersOfTheStandingCommitteeForFinancingResearchProjectsWelcomeSMS,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "بيانات عضواللجنة الدائمة لتمويل المشروعات البحثية",
                    BodyArabic = "سعادة الأستاذ الدكتور {FullName} عضو المجلس لقد تم إنشاء حساب لكم على نظام تمويل المشروعات البحثية الإلكتروني. نرجو الاطلاع على بريدكم الإلكتروني الجامعي الرسمي.",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.NewRefereeEmailActivation,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "تفعيل مستخدم جديد في نظام تمويل المشروعات البحثية",
                    BodyArabic = "<div style=\"direction: rtl\"><p>سعادة الأستاذ الدكتور <b>{name}</b> </p><p>السلام عليكم ورحمة الله وبركاته،،</p><p>ترحب بكم \"عمادة البحث العلمي\" في برنامجها \"تمويل المشروعات البحثية\" وتتمنى لكم مزيداً من النجاح والتميز.. </p><p>لتفعيل حسابكم (<b>{email}</b>) لطفاً <a href=\"{link}\">اضغط هنا</a>. </p><p>والله ولي التوفيق. </p></div>",
                },
                new Template
                {
                    TemplateKey = (short)TemplateKey.RefereeAccountActivation,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "تم تفعيل حسابك في نظام تمويل المشروعات البحثية",
                    BodyArabic = "<div style=\"direction: rtl\"><p>سعادة الأستاذ الدكتور<b>{name}</b> </p><p>السلام عليكم ورحمة الله وبركاته،،</p><p>ترحب بكم \"عمادة البحث العلمي\" في برنامجها \"تمويل المشروعات البحثية\" وتتمنى لكم مزيداً من النجاح والتميز.. </p><p>هذا وقد تم تفعيل حسابكم كـ \"محكم\" وللدخول للموقع، لطفاً <a href=\"{link}\">اضغط هنا</a>. </p><p>والله ولي التوفيق. </p></div>",
                },
                   new Template
                {
                    TemplateKey = (short)TemplateKey.ResetPassword,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إعادة تفعيل كلمة المرور",
                    BodyArabic = "<p>السلام عليكم ورحمة الله وبركاته وبعد:    "+
                    "بناء على طلبكم؛ يرجى الضغط <a href=\"{link}\">هنا</a> لإعادة تعيين كلمة المرور للحساب {username}."+
                    "</p>",
                },
                     new Template
                {
                    TemplateKey = (short)TemplateKey.ContactUsMessage,
                    SubjectEnglish = "Attension: {fullname} wants to contact us for something.",
                    BodyEnglish = "Dear System Admin,"+
                                    "<br />This is to inform you that a contact with name {fullname} and email Id {emailId} wants to contact us.<br/>Please look into his/her message and take action.<br />His/Her message is as below.<br /><br />Subject: {subject}<br />{message}<br />Thank You."+
                    "</p>",
                    SubjectArabic ="أتنسيون: {fullname} يريد الاتصال بنا للحصول على شيء ما.",
                     BodyArabic = "عزيزي مسؤول النظام، "+
                                    "<br /> هذا لإعلامك بأن جهة اتصال تحمل الاسم {fullname} ورقم البريد الإلكتروني {emailId} يريد الاتصال بنا. <br/> يرجى الاطلاع على رسالته واتخاذ إجراء. <br /> هيس / هر الرسالة على النحو التالي. <br /> <br /> الموضوع: {subject} <br /> {message} <br /> شكرا."+
                    "</p>",
                },
                     //modify template
                new Template
                {
                    TemplateKey = (short)TemplateKey.ExternalRefereeSMSNotification,
                    SubjectEnglish = "You are invited to review funding request.",
                    BodyEnglish = "سعادة الدكتور {FullName} نفيدكم بأنه تم ترشيحكم للانضمام لبرنامج تمويل المشروعات البحثية بجامعة الإمام محمد بن سعود الإسلامية؛ لتحكيم طلب تمويل مشروعي بحثي، لطفاً اضغط على هذا الرابط للدخول على صفحة التحكيم. عمادة البحث العلمي.",
                    SubjectArabic = "You are invited to review funding request",
                    BodyArabic = "سعادة الدكتور {FullName} نفيدكم بأنه تم ترشيحكم للانضمام لبرنامج تمويل المشروعات البحثية بجامعة الإمام محمد بن سعود الإسلامية؛ لتحكيم طلب تمويل مشروعي بحثي لطفاً  قم بزيارة بريدكم الإلكتروني للإطلاع على التفاصيل. عمادة البحث العلمي.",
                },
              
                new Template
                {
                    TemplateKey = (short)TemplateKey.InternalRefereeSMSNotification,
                    SubjectEnglish = "You are invited to review funding request.",
                    BodyEnglish = "سعادة عضو/عضوة هيئة التدريس نفيدكم بأنه تم ترشيحكم لتحكيم طلب تمويل مشروعي بحثي وبانتظار موافقتكم الكريمة على التحكيم. ويسعدنا في عمادة البحث العلمي التعاون معكم.",
                    SubjectArabic = "You are invited to review funding request",
                    BodyArabic = "سعادة عضو/عضوة هيئة التدريس نفيدكم بأنه تم ترشيحكم لتحكيم طلب تمويل مشروعي بحثي وبانتظار موافقتكم الكريمة على التحكيم. ويسعدنا في عمادة البحث العلمي التعاون معكم.",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.ExternalRefereeEmailNotification,
                    SubjectEnglish = "سعادة الدكتور عضو هيئة التدريس بجامعة 'الإمام محمد بن سعود الإسلامية'",
                    BodyEnglish = "سعادة الدكتورسعادة الدكتور {FullName} وفقه الله<br/>السلام عليكم ورحمة الله وبركاته،،<br/>ترحب بكم عمادة البحث العلمي في برنامجها تمويل المشروعات البحثية وتتمنى لكم مزيداً من النجاح والتميز.<br/>وتدعوكم لتحكيم طلب 	تمويل مشروعي بحثي،  لطفاً.. تفضل بالدخول على <a href='{REQUEST_DETAIL_PUBLIC_LINK}'>(هذا الرابط) </a>  للاطلاع على المادة المطلوب تحكيمها.<br/><br/>والله ولي التوفيق",
                    SubjectArabic = "سعادة الدكتور عضو هيئة التدريس بجامعة 'الإمام محمد بن سعود الإسلامية'",
                    BodyArabic = "سعادة الدكتورسعادة الدكتور {FullName} وفقه الله<br/>السلام عليكم ورحمة الله وبركاته،،<br/>ترحب بكم عمادة البحث العلمي في برنامجها تمويل المشروعات البحثية وتتمنى لكم مزيداً من النجاح والتميز.<br/>وتدعوكم لتحكيم طلب 	تمويل مشروعي بحثي،  لطفاً.. تفضل بالدخول على <a href='{REQUEST_DETAIL_PUBLIC_LINK}'>(هذا الرابط) </a>  للاطلاع على المادة المطلوب تحكيمها.<br/><br/>والله ولي التوفيق",
                },
                new Template
                {
                    TemplateKey = (short)TemplateKey.InternalRefereeEmailNotification,
                    SubjectEnglish = "سعادة الدكتور عضو هيئة التدريس بجامعة 'الإمام محمد بن سعود الإسلامية'",
                    BodyEnglish = "سعادة الدكتور عضو هيئة التدريس بجامعة 'الإمام محمد بن سعود الإسلامية'السلام عليكم ورحمة الله وبركاته،،<br/>ترحب بكم عمادة البحث العلمي في برنامجها تمويل المشروعات البحثية وتتمنى لكم مزيداً من النجاح والتميز..<br/>وتدعوكم لتحكيم طلب تمويل مشروعي بحثي،  لطفاً.. تفضل بالدخول على صفحتكم على <a href='{REQUEST_DETAIL_INTERNAL_LINK}'>نظام تمويل المشروعات البحثية</a> ومن ثم اختيار صفة ( محكم )؛ للاطلاع على المادة المطلوب تحكيمها.<br/>ملاحظة: إن كنت غير مسجلاً بـ  نظام تمويل المشروعات البحثية، لطفاً.. يرجى تسجيل مستخدم جديد.<br/><br/>والله ولي التوفيق.",
                    SubjectArabic = "سعادة الدكتور عضو هيئة التدريس بجامعة 'الإمام محمد بن سعود الإسلامية'",
                    BodyArabic = "سعادة الدكتور عضو هيئة التدريس بجامعة 'الإمام محمد بن سعود الإسلامية'السلام عليكم ورحمة الله وبركاته،،<br/>ترحب بكم عمادة البحث العلمي في برنامجها تمويل المشروعات البحثية وتتمنى لكم مزيداً من النجاح والتميز..<br/>وتدعوكم لتحكيم طلب تمويل مشروعي بحثي،  لطفاً.. تفضل بالدخول على صفحتكم على <a href='{REQUEST_DETAIL_INTERNAL_LINK}'>نظام تمويل المشروعات البحثية</a> ومن ثم اختيار صفة ( محكم )؛ للاطلاع على المادة المطلوب تحكيمها.<br/>ملاحظة: إن كنت غير مسجلاً بـ  نظام تمويل المشروعات البحثية، لطفاً.. يرجى تسجيل مستخدم جديد.<br/><br/>والله ولي التوفيق.",
                },
                new Template
                {
                    TemplateKey = (short)TemplateKey.CoResearcherUpdateRequestSMSNotification,
                    SubjectEnglish = "",
                    BodyEnglish = "سعادة الأستاذ الدكتور {FullName} نفيدكم بأنه تم إضافتكم كمساعد باحث في المشروع البحثي {RequestTitle} وبانتظار موافقتكم الكريمة على ذلك، وتم إرسال رابط المشروع البحثي عبر بريديكم الإلكتروني. ولكم جزيل الشكر.",
                    SubjectArabic = "",
                    BodyArabic = "سعادة الأستاذ الدكتور {FullName} نفيدكم بأنه تم إضافتكم كمساعد باحث في المشروع البحثي {RequestTitle} وبانتظار موافقتكم الكريمة على ذلك، وتم إرسال رابط المشروع البحثي عبر بريديكم الإلكتروني. ولكم جزيل الشكر.",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.CoResearcherUpdateRequestEmailNotification,
                   SubjectEnglish = "سعادةالعضو المشارك في البحث بجامعة 'الإمام محمد بن سعود الإسلامية",
                    BodyEnglish = "سعادة الدكتور {FullName} <br/> السلام عليكم ورحمة الله وبركاته، <br/> ترحب بكم عمادة البحث العلمي في برنامجها تمويل المشروعات البحثية بجامعة الإمام محمد بن سعود الإسلامية وتتمنى لكم مزيداً من النجاح والتميز.. <br/> وتفيدكم بأنه تم إضافتكم كـ مساعد باحث في المشروع البحثي {RequestTitle} الخاص بالباحث:<br/> {MainResearcherFullName}، لطفاً.. تفضل بالدخول على هذا <a href='{REQUEST_DETAIL_PUBLIC_LINK}'>(هذا الرابط) </a> للاطلاع على المشروع البحثي.<br/> والله ولي التوفيق. ",
                    SubjectArabic = "سعادةالعضو المشارك في البحث بجامعة 'الإمام محمد بن سعود الإسلامية",
                    BodyArabic =  "سعادة الدكتور {FullName} <br/> السلام عليكم ورحمة الله وبركاته، <br/> ترحب بكم عمادة البحث العلمي في برنامجها تمويل المشروعات البحثية بجامعة الإمام محمد بن سعود الإسلامية وتتمنى لكم مزيداً من النجاح والتميز.. <br/> وتفيدكم بأنه تم إضافتكم كـ مساعد باحث في المشروع البحثي {RequestTitle} الخاص بالباحث:<br/> {MainResearcherFullName}، لطفاً.. تفضل بالدخول على هذا <a href='{REQUEST_DETAIL_PUBLIC_LINK}'>(هذا الرابط) </a> للاطلاع على المشروع البحثي.<br/> والله ولي التوفيق. ",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.CoResearcherEditRequestEmailNotification,
                    SubjectEnglish = "",
                    BodyEnglish = "سعادة الأستاذ الدكتور {FullName} نفيدكم بأنه تم إضافتكم كمساعد باحث في المشروع البحثي {RequestTitle} وبانتظار موافقتكم الكريمة على ذلك، وتم إرسال رابط المشروع البحثي عبر بريديكم الإلكتروني. ولكم جزيل الشكر.",
                    SubjectArabic = "",
                    BodyArabic = "سعادة الأستاذ الدكتور {FullName} نفيدكم بأنه تم إضافتكم كمساعد باحث في المشروع البحثي {RequestTitle} وبانتظار موافقتكم الكريمة على ذلك، وتم إرسال رابط المشروع البحثي عبر بريديكم الإلكتروني. ولكم جزيل الشكر.",
                },

                new Template
                {
                    TemplateKey = (short)TemplateKey.CoResearcherEditRequestSMSNotification,
                   SubjectEnglish = "سعادةالعضو المشارك في البحث بجامعة 'الإمام محمد بن سعود الإسلامية",
                    BodyEnglish = "سعادة الدكتور {FullName} <br/> السلام عليكم ورحمة الله وبركاته، <br/> ترحب بكم عمادة البحث العلمي في برنامجها تمويل المشروعات البحثية بجامعة الإمام محمد بن سعود الإسلامية وتتمنى لكم مزيداً من النجاح والتميز.. <br/> ونفيدكم بأنه تم التعديل على الفريق البحثي {RequestTitle} الخاص بالباحث:<br/> {MainResearcherFullName}، لطفاً.. تفضل بالدخول على هذا <a href='{REQUEST_DETAIL_PUBLIC_LINK}'>(هذا الرابط) </a> للاطلاع على المشروع البحثي.<br/> والله ولي التوفيق. ",
                    SubjectArabic = "سعادةالعضو المشارك في البحث بجامعة 'الإمام محمد بن سعود الإسلامية",
                    BodyArabic =  "سعادة الدكتور {FullName} <br/> السلام عليكم ورحمة الله وبركاته، <br/> ترحب بكم عمادة البحث العلمي في برنامجها تمويل المشروعات البحثية بجامعة الإمام محمد بن سعود الإسلامية وتتمنى لكم مزيداً من النجاح والتميز.. <br/> ونفيدكم بأنه تم التعديل على الفريق البحثي {RequestTitle} الخاص بالباحث:<br/> {MainResearcherFullName}، لطفاً.. تفضل بالدخول على هذا <a href='{REQUEST_DETAIL_PUBLIC_LINK}'>(هذا الرابط) </a> للاطلاع على المشروع البحثي.<br/> والله ولي التوفيق. ",
                },

               new Template
                {
                    TemplateKey = (short)TemplateKey.MaterialRequestChequeReceivedByResearcherEmailNotification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشار بصرف مكافأة محكم",
                    BodyArabic = "<b>"+
                    "السلام عليكم ورحمة الله وبركاته وبعد:<br>"+
                    "سعادة الباحث {FullName}   <br>"+
                     " نفيدكم بأنه تم صرف مكافأة لكم  <br>"+

                    "</b>",
                },

               new Template
                {
                    TemplateKey = (short)TemplateKey.MaterialRequestChequeReceivedByResearcherSMSNotification,
                    SubjectEnglish = "",
                    BodyEnglish = "",
                    SubjectArabic = "إشار بصرف مكافأة محكم",
                    BodyArabic = "السلام عليكم ورحمة الله وبركاته وبعد: "+ "سعادة الباحث {FullName} نفيدكم بأنه تم صرف مكافأة لكم ",                    
                },


        };

        private Dictionary<TemplateKey, Template> TemplatesDict
        {
            get
            {
                if (_templatesDict != null) return _templatesDict;

                if (HasOwinContext)
                {
                    _templatesDict = HttpContextManager.Current.GetOwinContext().Get<Dictionary<TemplateKey, Template>>(typeof(Dictionary<TemplateKey, Template>).FullName);
                }

                if (_templatesDict == null)
                {
                    var max = (byte)Enum.GetValues(typeof(TemplateKey)).Cast<TemplateKey>().Last();

                    _templatesDict = new Dictionary<TemplateKey, Template>();

                    var templates = DbContext.Templates.ToList();

                    foreach (var tmpl in templates.Union(_defaults))
                    {
                        if (tmpl.TemplateKey > max || _templatesDict.ContainsKey((TemplateKey)tmpl.TemplateKey)) continue;

                        tmpl.Created = TM.Now;
                        tmpl.CreatedBy = CurrentUser == null ? Utils.SystemAdminUserId : CurrentUser.Id;
                        tmpl.Updated = TM.Now;
                        tmpl.UpdatedBy = CurrentUser == null ? Utils.SystemAdminUserId : CurrentUser.Id;

                        _templatesDict.Add((TemplateKey)tmpl.TemplateKey, tmpl);

                        if (DbContext.Entry(tmpl).State == EntityState.Detached)
                        {
                            DbContext.Templates.Add(tmpl);
                        }
                    }

                    if (HasOwinContext)
                    {
                        HttpContextManager.Current.GetOwinContext().Set<Dictionary<TemplateKey, Template>>(typeof(Dictionary<TemplateKey, Template>).FullName, _templatesDict);
                    }

                    DbContext.SaveChanges();
                }

                return _templatesDict;
            }
        }

        public List<Template> Templates
        {
            get
            {
                return TemplatesDict.Select(d => d.Value).OrderBy(k => k.TemplateKey).ToList();
            }
        }

        public Template this[TemplateKey key]
        {
            get
            {
                if (TemplatesDict.ContainsKey(key))
                {
                    return TemplatesDict[key];
                }
                else
                {
                    var template = new Template
                    {
                        TemplateKey = (short)key,
                        SubjectEnglish = "New notification",
                        SubjectArabic = "إشعار جديد",
                        BodyEnglish = "<ul><li>Type: " + key.ToString() + "</li><li>Name: {Name}</li><li>Message: {Message}</li><li>Link: <a href=\"{Link}\">{Link}</a></li></ul>",
                        BodyArabic = "<ul><li>النوع: " + key.ToString() + "</li><li>الاسم: {Name}</li><li>الرسالة: {Message}</li><li>الرابط: <a href=\"{Link}\">{Link}</a></li></ul>"
                    };

                    this[key] = template;

                    return template;
                }
            }
            set
            {
                if (value == null || value.TemplateKey != (byte)key) return;

                if (TemplatesDict.ContainsKey(key))
                {
                    TemplatesDict[key] = value;
                }
                else
                {
                    TemplatesDict.Add(key, value);
                    DbContext.Templates.Add(value);
                }

                DbContext.SaveChanges();
            }
        }

        public void SendEmailAsync(Request request, TemplateKey key)
        {
            if (request == null) return;
            try
            {
                Hangfire.BackgroundJob.Enqueue(() => SendEmail(request.Id, key));
            }
            catch (Exception ex)
            {
                Logger.LogExceptionAsync(ex, request);
            }
        }

        public static void SendEmail(long requestId, TemplateKey key)
        {
            if (requestId <= 0 || key == TemplateKey.Unspecified) return;

            var db = new ArfDbEntities();

            var request = db.Requests.FirstOrDefault(r => r.Id == requestId);
            if (request == null) return;
        }

        public static string UpdateRequestTemplate(Request request, User user, TemplateKey key, out string subject)
        {
            subject = null;
            if (request == null || key == TemplateKey.Unspecified || request.Id <= 0 || request.Type == 0) return "Empty or invalid request";
            if (user == null || user.Id == null || user.Email == null || !user.Email.Contains("@") || user.Email.EndsWith("@")) return "Empty or invalid user";

            var lang = user.UserPreferences.OrderByDescending(u => u.Id).FirstOrDefault(u => u.Key == "LANG" && u.Value == "AR") != null ? LanguagePreference.Arabic : LanguagePreference.English;
            var template = TemplateUtils[key];
            var type = (Data.RequestType)request.Type;

            if (template == null) return "Invalid template";

            subject = (lang == LanguagePreference.Arabic ? template.SubjectArabic : template.SubjectEnglish).UpdateUserTemplateVariables(user).UpdateRequestTemplateVariables(request, lang);
            var body = (lang == LanguagePreference.Arabic ? template.BodyArabic : template.BodyEnglish).UpdateUserTemplateVariables(user).UpdateRequestTemplateVariables(request, lang);

            return body;
        }
    }
}