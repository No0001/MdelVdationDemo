using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MdelVdationDemo.Models
{
    public class HomeViewModel
    {

        [CustomValidation(typeof(CustomValidationHelper), "IsValid")]
        public string TestCustomValidation { get; set; }

        [OtherRequired(nameof(TestCustomValidation), nameof(CheckedField.CheckDate), typeof(CheckedField))]
        public string TestOther { get; set; }

        [InsideModel]
        public List<Account> Accounts { get; set; }
    }

    public class Account
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}