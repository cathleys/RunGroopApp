

using NetworkUtility.Ping;

namespace NetworkUtility.Tests.PingTests;
public class NetworkServiceTests
{

    [Fact]
    public void NetworkService_SendPing_ReturnString()
    {

        //Arrange

        var pingService = new NetworkService();
        //Act
        var result = pingService.SendPing();
        //Assert

    }
}
