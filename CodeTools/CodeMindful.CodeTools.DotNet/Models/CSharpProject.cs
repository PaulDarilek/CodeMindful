using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Microsoft.CSharpProject
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.

    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    [XmlRoot(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", IsNullable = false)]
    public partial class Project
    {
        [XmlElement("Import", typeof(ProjectImport))]
        [XmlElement("ItemGroup", typeof(ProjectItemGroup))]
        [XmlElement("PropertyGroup", typeof(ProjectPropertyGroup))]
        public object[] Items { get; set; }

        [XmlAttribute()] public decimal ToolsVersion { get; set; }
        [XmlAttribute()] public string DefaultTargets { get; set; }
    }

    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    public partial class ProjectImport
    {
        [XmlAttribute()] public string Project { get; set; }
        [XmlAttribute()] public string Condition { get; set; }
    }

    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    public partial class ProjectItemGroup
    {
        [XmlElement("None")] public ProjectItem[] None { get; set; }
        [XmlElement("Compile")] public ProjectItem[] Compile { get; set; }
        [XmlElement("Reference")] public ProjectItem[] Reference { get; set; }
    }

    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    public partial class ProjectItem
    {
        [XmlAttribute()] public string Include { get; set; }
    }


    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    public partial class ProjectPropertyGroup
    {
        public string PlatformTarget { get; set; }
        public bool DebugSymbols { get; set; }
        [XmlIgnore()] public bool DebugSymbolsSpecified { get; set; }
        public string DebugType { get; set; }
        public bool Optimize { get; set; }
        [XmlIgnore()] public bool OptimizeSpecified { get; set; }
        public string OutputPath { get; set; }
        public string DefineConstants { get; set; }
        public string ErrorReport { get; set; }
        public byte WarningLevel { get; set; }
        [XmlIgnore()] public bool WarningLevelSpecified { get; set; }
        public ProjectPropertyGroupConfiguration Configuration { get; set; }
        public ProjectPropertyGroupPlatform Platform { get; set; }
        public string ProjectGuid { get; set; }
        public string OutputType { get; set; }
        public string AppDesignerFolder { get; set; }
        public string RootNamespace { get; set; }
        public string AssemblyName { get; set; }
        public string TargetFrameworkVersion { get; set; }
        public ushort FileAlignment { get; set; }
        [XmlIgnore()] public bool FileAlignmentSpecified { get; set; }
        public bool AutoGenerateBindingRedirects { get; set; }
        [XmlIgnore()] public bool AutoGenerateBindingRedirectsSpecified { get; set; }
        [XmlAttribute()] public string Condition { get; set; }
    }

    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    public partial class ProjectPropertyGroupConfiguration
    {
        [XmlAttribute()] public string Condition { get; set; }
        [XmlText()] public string Value { get; set; }
    }

    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
    public partial class ProjectPropertyGroupPlatform
    {
        [XmlAttribute()] public string Condition { get; set; }
        [XmlText()] public string Value { get; set; }
    }

}