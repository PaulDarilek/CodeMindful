using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CodeMindful.CodeTools.DotNet;

/// <remarks/>
[Serializable()]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
[XmlRoot(Namespace = "http://schemas.microsoft.com/developer/msbuild/2003", IsNullable = false)]
public partial class Project
{

    /// <remarks/>
    [XmlElement("Import", typeof(ProjectImport))]
    [XmlElement("ItemGroup", typeof(ProjectItemGroup))]
    [XmlElement("PropertyGroup", typeof(ProjectPropertyGroup))]
    public object[] Items { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    public decimal ToolsVersion { get; set; }
}

/// <remarks/>
[Serializable()]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
public partial class ProjectImport
{

    /// <remarks/>
    [XmlAttribute()]
    public string Project { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    public string Condition { get; set; }
    
}

/// <remarks/>
[Serializable()]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
public partial class ProjectItemGroup
{
            
    /// <remarks/>
    [XmlElement("None")]
    public ProjectItemGroupNone[] None { get; set; }

    /// <remarks/>
    [XmlElement("Compile")]
    public ProjectItemGroupCompile[] Compile { get; set; }

    /// <remarks/>
    [XmlElement("Reference")]
    public ProjectItemGroupReference[] Reference { get; set; }
}

/// <remarks/>
[Serializable()]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
public partial class ProjectItemGroupNone
{

    /// <remarks/>
    [XmlAttribute()]
    public string Include { get; set; }
}

/// <remarks/>
[Serializable()]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
public partial class ProjectItemGroupCompile
{

    /// <remarks/>
    [XmlAttribute()]
    public string Include { get; set; }
}

/// <remarks/>
[Serializable()]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
public partial class ProjectItemGroupReference
{

    public string HintPath { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    public string Include { get; set; }
    
}

/// <remarks/>
[Serializable()]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
public partial class ProjectPropertyGroup
{

    public string PlatformTarget { get; set; }

    public bool DebugSymbols { get; set; }
    [XmlIgnore()]
    public bool DebugSymbolsSpecified { get; set; }

    /// <remarks/>
    public string DebugType { get; set; }

    /// <remarks/>
    public bool Optimize { get; set; }

    /// <remarks/>
    [XmlIgnore()]
    public bool OptimizeSpecified { get; set; }

    public string OutputPath { get; set; }

    /// <remarks/>
    public string DefineConstants { get; set; }

    public string ErrorReport { get; set; }

    /// <remarks/>
    public byte WarningLevel { get; set; }

    /// <remarks/>
    [XmlIgnore()]
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
    [XmlIgnore()]
    public bool FileAlignmentSpecified { get; set; }

    public bool AutoGenerateBindingRedirects { get; set; }

    /// <remarks/>
    [XmlIgnore()]
    public bool AutoGenerateBindingRedirectsSpecified { get; set; }

    /// <remarks/>
    [XmlAttribute()]
    public string Condition { get; set; }

}

/// <remarks/>
[Serializable()]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
public partial class ProjectPropertyGroupConfiguration
{

    [XmlAttribute()]
    public string Condition { get; set; }

    [XmlText()]
    public string Value { get; set; }
}

/// <remarks/>
[Serializable()]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://schemas.microsoft.com/developer/msbuild/2003")]
public partial class ProjectPropertyGroupPlatform
{
    [XmlAttribute()]
    public string Condition { get; set; }

    [XmlText()]
    public string Value { get; set; }
}
