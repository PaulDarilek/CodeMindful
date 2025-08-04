namespace ConfigurationManagement
{
    /// <summary>Stage of CI or Asset</summary>
    public enum Stage
    {
        Unknown = 0,
        Requirements,
        Defined,
        Analyzed,
        Approved,
        Chartered,
        Designed,
        Developed,
        Build,
        Tested,
        Released,
        Operational,
        Published,
        Unpublished,
        Retired,
    }
}
