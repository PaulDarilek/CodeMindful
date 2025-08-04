using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigurationManagement
{
    public abstract class LogicalEntity : BaseElement
    {
    }

    public class Account : LogicalEntity
    {
    }

    public class Activity : LogicalEntity
    {
        /// <summary></summary>
        /// <remarks>None, Manual, Interaction, Business Process, Start, End, Decision, Basic</remarks>
        public string ActivityType { get; set; }
    }

    /// <summary>Business Servcie is provided by one business to another or one group to another</summary>
    /// <remarks>
    /// Example: Customer Support, Payroll, Call Center
    /// Technical Service to support business services or other IT operations (Document Management, Backup/Recovery, Self-Service help desk)
    /// </remarks>
    public class BusinessService : LogicalEntity
    {
        /// <summary>Type of Service (Unknown, Business,Technical,Offering)</summary>
        public string ServiceType { get; set; }

        /// <summary></summary>
        public decimal? Cost { get; set; }

        /// <summary></summary>
        public string CostDescription { get; set; }

        /// <summary>Price of the service to the customer</summary>
        public decimal? Price { get; set; }

        /// <summary></summary>
        public DateTime? StartDate { get; set; }

        /// <summary></summary>
        public DateTime? EndDate { get; set; };

        /// <summary></summary>
        public bool? IsActive { get; set; }

        /// <summary>Effective Hours of the service</summary>
        /// <remarks>Lookup to BusinessHours</remarks>
        public string ServiceHours { get; set; }

        /// <summary></summary>
        /// <remarks>Lookup (User)</remarks>
        public string ServiceOwner { get; set; }

        /// <summary>Account Providing the Service</summary>
        /// <remarks>Lookup (Account)</remarks>
        public string ServiceProvider { get; set; }

        /// <summary></summary>
        public string ServiceType { get; set; }

        /// <summary></summary>
        public string ServiceOwner { get; set; }

        /// <summary></summary>
        /// <remarks>Lookup (BaseElement)</remarks>
        public string SubServiceOf { get; set; }

        /// <summary>Account that is a vendor for the service</summary>
        /// <remarks>Lookup (Account)</remarks>
        public string Vendor { get; set; }

    }

    public class Database : LogicalEntity
    {

    }

    public class Offering : LogicalEntity
    {
        /// <summary>Type of Offering: None, Service, Requestable</summary>
        public string OfferingType { get; set; }
    }

    public class PhyssicalLocation : LogicalEntity
    {
        public string Address { get; set; }
        public string AddressAdditionalInfo { get; set; }
        public string City { get; set; }
        public string CoordianteFormat { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Altitude { get; set; }
        public string Latitude { get; set; }
        public string Lonittude { get; set; }
        public string MailStop { get; set; }
        public string PostalCode { get; set; }
        public string StateOrProvince { get; set; }
        public string WebPage { get;set; }

    }

    public class Tag : LogicalEntity 
    {
        /// <summary>Name of the category for the tag. Assosicates Tag with Tag category</summary>
        public string CategoryName { get; set; }
        
        /// <summary>Whether Tag is a Tag Category or a Tag within the Category</summary>
        public bool? IsCategory { get; set; }
    }
}
