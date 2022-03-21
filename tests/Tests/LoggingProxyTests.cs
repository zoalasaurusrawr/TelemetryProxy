using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelemetryProxy;
using Tests.Fixtures;
using Xunit;

namespace Tests;

public class LoggingProxyTests : IClassFixture<ProxyTestFixture>
{
    public ProxyTestFixture Fixture { get; }

    public LoggingProxyTests(ProxyTestFixture fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public void Creation_Succeeds()
    {
        //Arrange
        //Act
        ITestService service = LoggingProxy<ITestService>.Create<TestService>();

        //Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Creation_With_Constructor_Args_Succeeds()
    {
        //Arrange
        //Act
        ITestService service = LoggingProxy<ITestService>.Create<TestService>(Fixture.LoggerFactory, new object());

        //Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Creation_And_Call_Succeeds()
    {
        //Arrange
        ITestService service = LoggingProxy<ITestService>.Create<TestService>();

        //Act
        //Assert
        service.DoWork();
    }

    [Fact]
    public void Creation_And_Call_With_Constructor_Args_Succeeds()
    {
        //Arrange
        ITestService service = LoggingProxy<ITestService>.Create<TestService>(Fixture.LoggerFactory, new object());

        //Act
        //Assert
        service.DoWork();
    }

    [Fact]
    public void Creation_And_Call_With_Function_Args_Succeeds()
    {
        //Arrange
        ITestService service = LoggingProxy<ITestService>.Create<TestService>();

        //Act
        //Assert
        service.DoWork(new object());
    }

    [Fact]
    public void Creation_And_Call_With_Constructor_And_Function_Args_Succeeds()
    {
        //Arrange
        ITestService service = LoggingProxy<ITestService>.Create<TestService>(Fixture.LoggerFactory, new object());

        //Act
        //Assert
        service.DoWork(new object());
    }

    [Fact]
    public void Creation_And_Call_With_ActivitySource_And_Function_Args_Succeeds()
    {
        //Arrange
        ITestService service = LoggingProxy<ITestService>.Create<TestService>(Fixture.ActivitySource, Fixture.LoggerFactory);

        //Act
        //Assert
        service.DoWork(new object());
    }

    [Fact]
    public void Creation_And_Call_With_ActivitySource_And_Constructor_And_Function_Args_Succeeds()
    {
        //Arrange
        ITestService service = LoggingProxy<ITestService>.Create<TestService>(Fixture.ActivitySource, Fixture.LoggerFactory, new object());

        //Act
        //Assert
        service.DoWork(new object());
    }

    [Fact]
    public void Creation_And_Call_With_Function_With_Args_Succeeds()
    {
        //Arrange
        ITestService service = LoggingProxy<ITestService>.Create<TestService>();

        //Act
        //Assert
        service.DoNullableWork(new object());
    }

    [Fact]
    public void Creation_And_Call_With_Constructor_And_Function_With_Args_Succeeds()
    {
        //Arrange
        ITestService service = LoggingProxy<ITestService>.Create<TestService>(Fixture.LoggerFactory, new object());

        //Act
        //Assert
        service.DoNullableWork(new object());
    }

    [Fact]
    public void Creation_And_Call_With_ActivitySource_And_Function_With_Args_Succeeds()
    {
        //Arrange
        ITestService service = LoggingProxy<ITestService>.Create<TestService>(Fixture.ActivitySource, Fixture.LoggerFactory);

        //Act
        //Assert
        service.DoNullableWork(new object());
    }

    [Fact]
    public void Creation_And_Call_With_ActivitySource_And_Constructor_And_Function_With_Args_Succeeds()
    {
        //Arrange
        ITestService service = LoggingProxy<ITestService>.Create<TestService>(Fixture.ActivitySource, Fixture.LoggerFactory, new object());

        //Act
        //Assert
        service.DoNullableWork(new object());
    }
}
