using System;
using System.ComponentModel.DataAnnotations;

namespace ConfigurationManagement
{
    /// <summary>Common Attributes for Configuration Item or Asset class</summary>
    public abstract class BaseElement : IBaseElement
    {
        /// <summary></summary>
        /// <remarks>Unique and Case Sensitive</remarks>
        [MaxLength(254)]
        public string InstanceName { get; set; }

        /// <summary>Name of Class for CI or Asset</summary>
        public string ClassName { get; }

        /// <summary>Tag number of physical entiity represented by the CI or Asset</summary>
        [MaxLength(254)]
        public string AssetNumber { get; set; }

        /// <summary>Date asset starts being tracked<summary>
        DateTime? AssetBirthDate { get; set; }

        /// <summary>Age from <see cref="AssetBirthDate"/> to Today</summary>
        public TimeSpan? AssetAge =>
            AssetBirthDate == null ?
            (TimeSpan?)null :
            DateTime.Today.Subtract(AssetBirthDate.Value.Date);

        /// <summary>Date the warranty expires</summary>
        public DateTime? WarrantyExpirationDate { get; set; }

        /// <summary>Status of Asset</summary>
        public AssetStatus AssetStatus { get; set; }

        /// <summary>Indicates that a CI is hosted in the cloud which could be a public or a private cloud</summary>
        public bool? IsCloud { get; set; }

        /// <summary>Textual desciption of CI or asset</summary>
        public string Description { get; set; }

        /// <summary></summary>
        [MaxLength(254)]
        public string ShortDescription { get; set; }

        /// <summary>Location of CI or Asset. (Building,Floor,Room,Rack)</summary>
        public string Location { get; set; }

        /// <summary>Soft Deleted</summary>
        public bool? Deleted { get; set; }

        /// <summary>Manufacturer Name</summary>
        public string ModelManufacturer { get; set; }

        /// <summary>Model Name</summary>
        public string ModelName { get; set; }

        /// <summary>General textual notes about the CI or asset instance</summary>
        public string Notes { get; set; }

        /// <summary>User or Group linked to the CI or Asset</summary>
        public string PrimaryClient { get; set; }

        /// <summary>Customer Assigned Priority</summary>
        /// <remarks>PickList: 1-5</remarks>
        public string Priority { get; set; }

        /// <summary>Manugacturer Serial Number</summary>
        public string SerialNumber { get; set; }

        /// <summary>Site Information</summary>
        [MaxLength(254)]
        public string Site { get; set; }

        /// <summary>Specifies the stages of a CI or asset instance</summary>
        public Stage Stage { get; set; }

        /// <summary>Is Technical Support provided for this CI or Asset</summary>
        /// <remarks>PickList: Yes, No, N/A</remarks>
        public string Supported { get; set; }

        /// <summary>User/Group who is responsible for supporting the CI or Asset</summary>
        /// <remarks>Contact information for issues</remarks>
        public string SupportedBy { get; set; }

        /// <summary>Unique Identifier by Discovery Application</summary>
        /// <remarks>Could be Active Directory Identifier</remarks>
        [MaxLength(254)]
        public string TokenId { get; set; }

        /// <summary>Internal</summary>
        public string VersionNumber { get; set; }

        protected BaseElement()
        {
            ClassName = GetType().FullName ?? GetType().Name;
        }
    }
}
