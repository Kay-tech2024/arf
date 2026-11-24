using ARF.Data;
using OfficeOpenXml;
using System;
using System.Collections.Generic;

namespace ARF.Core.Models
{
    public class GenericImportModel
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string ThirdName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string NationalId { get; set; }

        public Gender? Gender { get; set; }
        public DateTime? Birthdate { get; set; }
        public int? Age { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Major { get; set; }
        public string Department { get; set; }
        public string College { get; set; }
        public string University { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string BankName { get; set; }
        public string AccountHolderName { get; set; }
        public string IBAN { get; set; }

        public string SheetName { get; set; }
        public int Row { get; set; }
        public int ResultColumn { get; set; }

        public bool IsImported { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();

        public ExcelWorksheet Sheet { get; set; }

        public int ThreadIndex { get; set; }

        public ImportResult Results { get; set; }
        public ExcelWorksheet OriginalSheet { get; set; }
    }
}