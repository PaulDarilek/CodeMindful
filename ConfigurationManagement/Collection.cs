namespace ConfigurationManagement
{
    public interface ICollection : IBaseElement
    {
    }

    public abstract class Collection : BaseElement, ICollection
    {
    }

    public sealed class Organization : Collection
    {
        /// <summary></summary>
        string BusinessCategory { get; set; }

        /// <summary>Email Address of Organization</summary>
        string Email { get; set; }

        /// <summary>The uniform resource identifier (URI) (RFC 2079)</summary>
        string URI { get; set; }
    }

    public sealed class Role : Collection
    {
        /// <summary>Name that Role is commonly known in the context (AD or Organization)</summary>
        public string CommonName { get; set; }
    }

    public sealed class UserCommunity : Collection
    {
        /// <summary>User who is single point of Contact for the community</summary>
        public string Contact { get; set; }

        /// <summary>Number of Users in the community</summary>
        public int? Count { get; set; }
    }


}
