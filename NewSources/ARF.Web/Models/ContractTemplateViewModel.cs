using ARF.Web.App_Start.Code;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ARF.Web.Models
{
    public class ContractTemplateViewModel
    {
        [AllowHtml]
        [Required(ErrorMessageResourceName = "contract_arabic_required", ErrorMessageResourceType = typeof(Resources.ArfStrings))]
        [LocalizedDisplay("ContractArabic")]
        public string ContractArabic { get; set; }        
        
        [Required(ErrorMessageResourceName = "contract_start_date_required", ErrorMessageResourceType = typeof(Resources.ArfStrings))]
        [LocalizedDisplay("ContractStartDate")]
        public string ContractStartDate { get; set; }

        public int DownloadPDF { get; set; }

    }
}