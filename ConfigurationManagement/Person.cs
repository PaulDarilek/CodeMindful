using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigurationManagement
{
    public class Person : BaseElement
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MilddleName { get; set; }

        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string OfficePhone {  get; set; }

        /// <summary>Additional Details such as roles or responsibilities</summary>
        public string AdditionalDetails { get; set; }

        public string Title { get; set; }
        public string WebPage { get; set; }

    }
}
