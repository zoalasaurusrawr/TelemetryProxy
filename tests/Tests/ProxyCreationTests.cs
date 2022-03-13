using System.Diagnostics;
using TelemetryProxy;
using Xunit;

namespace Tests;

public class ProxyCreationTests
{
    [Fact]
    public void Creation_Succeeds()
    {
        //Arrange
        //Act
        ITestService service = ActivityProxy<ITestService>.Create<TestService>();

        //Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Creation_With_Constructor_Args_Succeeds()
    {
        //Arrange
        //Act
        ITestService service = ActivityProxy<ITestService>.Create<TestService>(new object());

        //Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Creation_And_Call_Succeeds()
    {
        //Arrange
        ITestService service = ActivityProxy<ITestService>.Create<TestService>();

        //Act
        //Assert
        service.DoWork();
    }

    [Fact]
    public void Creation_And_Call_With_Constructor_Args_Succeeds()
    {
        //Arrange
        ITestService service = ActivityProxy<ITestService>.Create<TestService>(new object());

        //Act
        //Assert
        service.DoWork();
    }

    [Fact]
    public void Creation_And_Call_With_Function_Args_Succeeds()
    {
        //Arrange
        ITestService service = ActivityProxy<ITestService>.Create<TestService>();

        //Act
        //Assert
        service.DoWork(new object());
    }

    [Fact]
    public void Creation_And_Call_With_Constructor_And_Function_Args_Succeeds()
    {
        //Arrange
        ITestService service = ActivityProxy<ITestService>.Create<TestService>(new object());

        //Act
        //Assert
        service.DoWork(new object());
    }

    [Fact]
    public void Creation_And_Call_With_ActivitySource_And_Function_Args_Succeeds()
    {
        //Arrange
        var activitySource = new ActivitySource(nameof(ProxyCreationTests));
        ITestService service = ActivityProxy<ITestService>.Create<TestService>(activitySource);

        //Act
        //Assert
        service.DoWork(new object());
    }

    [Fact]
    public void Creation_And_Call_With_ActivitySource_And_Constructor_And_Function_Args_Succeeds()
    {
        //Arrange
        var activitySource = new ActivitySource(nameof(ProxyCreationTests));
        ITestService service = ActivityProxy<ITestService>.Create<TestService>(activitySource, new object());

        //Act
        //Assert
        service.DoWork(new object());
    }
}