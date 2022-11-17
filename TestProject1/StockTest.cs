using Microsoft.Extensions.Logging;
using Moq;
using StockApplication.Code;
using StockApplication.Code.DAL;
using StockApplication.Controllers;
using System.Threading.Tasks;
using System;
using Xunit;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Collections.Generic;
using System.Text;


namespace TestProject1
{
    public class KundeAppTest
    {
        private const string _loggedIn = "loggedIn";
        private const string _notLoggedIn = "";
        

        private readonly Mock<IStockRepository> mockRep = new Mock<IStockRepository>();
        private readonly Mock<ILogger<StockController>> mockLog = new Mock<ILogger<StockController>>();

        private readonly Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
        private readonly MockHttpSession mockSession = new MockHttpSession();
        
        

        [Fact]
        public async Task GetAllClientUsers() //Not dependent on log-in
        {
            // Arrange
            var user1 = new ClientUser("Luddebassen", 100);
            var user2 = new ClientUser("Luddebassen", 100);
            var user3 = new ClientUser("Luddebassen", 100);

            
            var userList = new List<ClientUser>();
            userList.Add(user1);
            userList.Add(user2);
            userList.Add(user3);

            mockRep.Setup(k => k.GetAllClientUsers()).ReturnsAsync(userList);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

 
            // Act
            var resultat = await stockController.GetAllUsers() as OkObjectResult;

            // Assert 
            Assert.Equal((int)HttpStatusCode.OK,resultat.StatusCode);
            Assert.Equal<List<ClientUser>>((List<ClientUser>)resultat.Value, userList);
        }

        [Fact]
        public async Task SaveUserNotLoggedIn()
        {
           //Arrange
            ServerResponse serverRespons = new ServerResponse(false, "User not logged in");
            mockRep.Setup(u => u.UpdateUser(It.IsAny<User>())).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[_loggedIn] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedObjectResult)await stockController.UpdateUser(It.IsAny<User>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal(serverRespons.Response, result.Value);


        }
       



    }
}