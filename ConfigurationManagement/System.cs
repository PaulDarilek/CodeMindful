using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigurationManagement
{
    public class System : BaseElement
    {   
        public bool? IsVirtual { get; set; }
    }

    public class ApplicationSystem : System
    {
        public string BuildNumber { get; set; }
        public string PatchNumber { get; set; }
        public string ServicePack { get; set; }
    }


    public class SystemComponent : BaseElement
    {
        /// <summary>Whether system is virtual or physical</summary>
        public bool IsVirtual { get; set; }

        /// <summary>Class of the System</summary>
        public string ClassId { get; set; }

        /// <summary>System that component belongs</summary>
        public string SystemName { get; set; }

    }

    public class SystemService : BaseElement
    {
        /// <summary></summary>
        public string ClassId { get; set; }

        /// <summary>Name of System hosting this service</summary>
        public string SystemName { get; set; }
    }

    public class ApplicationService : SystemService
    {
        /// <summary>Primary Function of Application Service</summary>
        /// <remarks>PickList: Website, Batch, WindowService</remarks>
        public string ApplicationServiceType { get; set; }

        /// <summary>Qualified Path to binary file that implements the service</summary>
        public string PathName { get; set; }

    }
}
