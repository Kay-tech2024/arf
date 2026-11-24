using ARF.Data.DbModels;
using ARF.Web.App_Start.DWF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ARF.Web.Models
{
    public class SectionsViewModel
    {
        public long? FormId { get; set; }
        public long Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public bool IsActive { get; set; }
        public List<FormFieldModel> FormFields { get; set; } = new List<FormFieldModel>();


    }
    

}