using Smx.Winter;

namespace Smx.Winter.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    private SchemaDefinitions.AsmV3.AssemblyIdentity Given_AssemblyIdentity()
    {
        return new SchemaDefinitions.AsmV3.AssemblyIdentity
        {
            Name = "Microsoft-Windows-Dedup-Service",
            Version = "10.0.20348.2849",
            BuildType = "release",
            Language = "neutral",
            ProcessorArchitecture = "amd64",
            PublicKeyToken = "31bf3856ad364e35",
            VersionScope = "nonSxS"
        };
    }

    private SchemaDefinitions.AsmV3.AssemblyIdentity Given_DepoymentIdentity()
    {
        return new SchemaDefinitions.AsmV3.AssemblyIdentity
        {
            Name = "Microsoft-Windows-Dedup-Deployment",
            Version = "10.0.20348.2849",
            ProcessorArchitecture = "amd64",
            Language = "neutral",
            BuildType = "release",
            PublicKeyToken = "31bf3856ad364e35",
            VersionScope = "nonSxS"
        };
    }

    [Test]
    public void TestAssemblyNames()
    {
        var asmId = Given_AssemblyIdentity();
        var componentId = ComponentAppId.FromAssemblyIdentity(asmId);

        Assert.That(
            componentId.GetName(ComponentAppIdNameFormat.CbsLong),
            Is.EqualTo("amd64_microsoft-windows-dedup-service_31bf3856ad364e35_10.0.20348.2849_none_773c336e9bd2bbfb"));

        Assert.That(
            componentId.GetName(ComponentAppIdNameFormat.AppId),
            Is.EqualTo("Microsoft-Windows-Dedup-Service, Culture=neutral, Version=10.0.20348.2849, PublicKeyToken=31bf3856ad364e35, ProcessorArchitecture=amd64, versionScope=NonSxS"));
    }

    [Test]
    public void TestDeploymentNames()
    {
        var deploymentAsmId = Given_DepoymentIdentity();
        var componentId = ComponentAppId.FromAssemblyIdentity(deploymentAsmId);

        Assert.That(
            componentId.GetName(ComponentAppIdNameFormat.CbsShort),
            Is.EqualTo("microsoft-w..-deployment_31bf3856ad364e35_10.0.20348.2849_f8f9ef63a3376fad"));

        Assert.That(
            componentId.GetName(ComponentAppIdNameFormat.AppId),
            Is.EqualTo("Microsoft-Windows-Dedup-Deployment, Culture=neutral, Version=10.0.20348.2849, PublicKeyToken=31bf3856ad364e35, ProcessorArchitecture=amd64, versionScope=NonSxS"));

        Assert.That(
            componentId.GetName(ComponentAppIdNameFormat.Package),
            Is.EqualTo("Microsoft-Windows-Dedup-Deployment~31bf3856ad364e35~amd64~~10.0.20348.2849"));
    }
}
