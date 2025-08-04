using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigurationManagement
{
    public class BaseRelationship
    {
        /// <summary>Uniquely identifies the relationship record in the system. Use this field when you are importing relationships from external source.</summary>
        public string UniqueRelationshipID { get; set; }

        /// <summary></summary>
        /// <remarks>Unique Case Insensitive</remarks>
        public string InstanceId { get; set; }


        /// <summary>
        /// Unique identifier of the source CI. 
        /// When you are importing relationships, the value of this field must be same as value of the 
        /// Unique CI Source ID field of the source CI in the Base Element object. 
        /// The relationship record is imported only when the value of this field matches with the 
        /// Unique CI Source ID field value of a CI in the Base Element object.
        /// </summary>
        public string UniqueSourceID { get; set; }

        /// <summary>
        /// Unique identifier of the destination CI. 
        /// When you are importing relationships, the value of this field must be same as value of the 
        /// Unique CI Source ID field of the destination CI in the Base Element object. 
        /// The relationship record is imported only when the value of this field matches with the 
        /// Unique CI Source ID field value of a CI in the Base Element object.
        /// </summary>
        public string UniqueDestinationID { get; set; }

        /// <summary>Type fullname of the relationship</summary>
        public virtual string ClassName { get; set; }

        /// <summary>Relationship Type</summary>
        /// <remarks>PickList: Other, All Related, Component, Dependency, Has Impact, 
        /// Member of Collection, Service-Offering, Source-Destination, Service-Subservice, 
        /// Unknown, Account On System, Application System Services, Base Relationship, 
        /// Contract Component, Element Location, Genealogy, Hosted Access Point, 
        /// Hosted Service, Hosted System Components, In IP Subnet, In Segment, 
        /// IP Subnets In Collection, LNs In Collection, Offering Measured By,
        /// Segments In Collection, Setting Of
        /// </remarks>
        public string RelationType { get; set; }


        /// <summary>Unique Id of the dataset that the instance belongs</summary>
        /// <remarks>Can be identifier of a discovery application dataaset</remarks>
        public string DataSetId { get; set; }

        /// <summary>Text Description of the relationship</summary>
        public string Description { get; set; }
        
        /// <summary>Id of the Asset or CI 2nd part of the relationship</summary>
        /// <remarks>Destination: ClassName, InstanceId, InstanceName</remarks>
        public string Destination { get; set; }

        /// <summary>Whether the relationship has impact characteristics that are defined and active.</summary>
        public bool? HasImpact { get; set; }

        /// <summary>Model used for the impact relationship</summary>
        public string ImpactPropagationModel { get; set; }

        /// <summary>Weight of the Impact</summary>
        public byte? ImpactWeight { get; set; }

        /// <summary>User who last modified this instance</summary>
        public string LastModifiedBy { get; set; }

        /// <summary>Soft Delete</summary>
        public bool? MarkAsDeleted { get; set; }

        /// <summary>
        /// Identifier assigned either manually or by an identification activity. 
        /// It is unique to all instances in any dataset that represent the same real-life CI or relationship. 
        /// The value for this attribute stays the same when the instance is copied or moved to other datasets. 
        /// </summary>
        public string ReconcilliationId { get; set; }
        
        /// <summary>Name of the relationship</summary>
        public string RelationshipName { get; set; }

        /// <summary>Id of the Asset or CI 1st part of the relationship</summary>
        /// <remarks>Source: ClassName, InstanceId, InstanceName</remarks>
        public string Source { get; set; }

        /// <summary>Who created the instance</summary>
        public string Submitter { get; set; }

        public BaseRelationship()
        {
            ClassName = ClassName ?? GetType().FullName ?? GetType().Name;
        }
    }
}
