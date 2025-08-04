using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CodeHaptic.CodeTools.DotNet
{
    /// <remarks/>
    [SerializableAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    [XmlRootAttribute(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", IsNullable = false)]
    public partial class Project
    {

        /// <remarks/>
        [XmlElementAttribute("Import", typeof(ProjectImport))]
        [XmlElementAttribute("ItemGroup", typeof(ProjectItemGroup))]
        [XmlElementAttribute("PropertyGroup", typeof(ProjectPropertyGroup))]
        public object[] Items { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public decimal ToolsVersion { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    public partial class ProjectImport
    {

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string Project { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string Condition { get; set; }
        
    }

    /// <remarks/>
    [SerializableAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    public partial class ProjectItemGroup
    {
                
        /// <remarks/>
        [XmlElementAttribute("None")]
        public ProjectItemGroupNone[] None { get; set; }

        /// <remarks/>
        [XmlElementAttribute("Compile")]
        public ProjectItemGroupCompile[] Compile { get; set; }

        /// <remarks/>
        [XmlElementAttribute("Reference")]
        public ProjectItemGroupReference[] Reference { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    public partial class ProjectItemGroupNone
    {

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string Include { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    public partial class ProjectItemGroupCompile
    {

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string Include { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    public partial class ProjectItemGroupReference
    {

        public string HintPath { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string Include { get; set; }
        
    }

    /// <remarks/>
    [SerializableAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    public partial class ProjectPropertyGroup
    {

        public string PlatformTarget { get; set; }

        public bool DebugSymbols { get; set; }
        [XmlIgnoreAttribute()]
        public bool DebugSymbolsSpecified { get; set; }

        /// <remarks/>
        public string DebugType { get; set; }

        /// <remarks/>
        public bool Optimize { get; set; }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool OptimizeSpecified { get; set; }

        public string OutputPath { get; set; }

        /// <remarks/>
        public string DefineConstants { get; set; }

        public string ErrorReport { get; set; }

        /// <remarks/>
        public byte WarningLevel { get; set; }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool WarningLevelSpecified { get; set; }

        public ProjectPropertyGroupConfiguration Configuration { get; set; }

        public ProjectPropertyGroupPlatform Platform { get; set; }

        public string ProjectGuid { get; set; }

        public string OutputType { get; set; }

        public string RootNamespace { get; set; }

        public string AssemblyName { get; set; }

        public string TargetFrameworkVersion { get; set; }

        public ushort FileAlignment { get; set; }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool FileAlignmentSpecified { get; set; }

        public bool AutoGenerateBindingRedirects { get; set; }

        /// <remarks/>
        [XmlIgnoreAttribute()]
        public bool AutoGenerateBindingRedirectsSpecified { get; set; }

        /// <remarks/>
        [XmlAttributeAttribute()]
        public string Condition { get; set; }

    }

    /// <remarks/>
    [SerializableAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    public partial class ProjectPropertyGroupConfiguration
    {

        [XmlAttributeAttribute()]
        public string Condition { get; set; }

        [XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [SerializableAttribute()]
    [DesignerCategoryAttribute("code")]
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    public partial class ProjectPropertyGroupPlatform
    {
        [XmlAttributeAttribute()]
        public string Condition { get; set; }

        [XmlTextAttribute()]
        public string Value { get; set; }
    }


}
