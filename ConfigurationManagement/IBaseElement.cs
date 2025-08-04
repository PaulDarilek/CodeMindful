using System;

namespace ConfigurationManagement
{
    public interface IBaseElement
    {
        TimeSpan? AssetAge { get; }
        string AssetNumber { get; }
        AssetStatus AssetStatus { get; }
        string ClassName { get; }
        bool? Deleted { get;   }
        string Description { get; }
        string InstanceName { get; }
        bool? IsCloud { get; }
        string Location { get; }
        string ModelManufacturer { get; }
        string ModelName { get; }
        string Notes { get; }
        string PrimaryClient { get; }
        string Priority { get; }
        string SerialNumber { get; }
        string ShortDescription { get; }
        string Site { get; }
        Stage Stage { get; }
        string Supported { get; }
        string SupportedBy { get; }
        string TokenId { get; }
        string VersionNumber { get; }
        DateTime? WarrantyExpirationDate { get; set; }
    }
}