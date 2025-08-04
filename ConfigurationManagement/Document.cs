using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigurationManagement
{
    public class Document : BaseElement
    {
        /// <summary>The name of the person who wrote the document</summary>
        public string Author { get; set; }

        /// <summary>The date and time when the document was created</summary>
        public DateTime? DateCreated { get; set; }

        /// <summary>The date and time when the document was last updated</summary>
        public DateTime? DateUpdated { get; set; }

        /// <summary>Start of contract between providere and customer</summary>
        public DateTime? StartDate { get; set; }

        /// <summary>End of contract between providere and customer</summary>
        public DateTime? EndDate { get; set; }

        /// <summary></summary>
        public int? Size { get; set; }

        /// <summary>Status of Document</summary>
        /// <remarks>PickList: None, Draft, Pre-Released, Released, Obsolete</remarks>
        public string Status { get; set; }

        /// <summary>Format of Document file (Word, PDF or Text)</summary>
        public string DocumentType { get; set; }

        /// <summary>Name of File (if Attached) or Full path to file</summary>
        public string FiileName { get; set; }

        /// <summary>Set of words that convey subject of document and allow others to search for it</summary>
        public string Keywords { get; set; }

        /// <summary>Geographical area document is localized (en_US)</summary>
        public string Locale { get; set; }
    }

    public class Contract : Document
    {
        /// <summary>Type of Contract</summary>
        /// <remarks>PickList: SLA, OLA, UPC, Other, Unknown</remarks>
        public string ContractType { get; set; }
    }

}
