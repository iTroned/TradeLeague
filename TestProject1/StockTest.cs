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
        private const string SessionKeyUser = "_currentUser";
        private const string SessionKeyCompany = "_currentCompany";
        

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
        public async Task UpdateUserLogInOk()
        {
            //Arrange
            ServerResponse serverRespons = new ServerResponse(true);
            mockRep.Setup(u => u.UpdateUser(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = SessionKeyUser;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.UpdateUser(It.IsAny<string>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("User updated", result.Value);
        }

        [Fact]
        public async Task UpdateUserLogInNotOk()
        {
            //Arrange
            string serverRespons = "You have to be logged in to perform this action!";
            
            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedObjectResult)await stockController.UpdateUser(It.IsAny<string>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }

        [Fact]
        public async Task UpdateUserLogInOk_DBerror()
        {
            //Arrange
            ServerResponse serverRespons = new ServerResponse(false, "Username is the same!");
            mockRep.Setup(u => u.UpdateUser(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = SessionKeyUser;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (BadRequestObjectResult)await stockController.UpdateUser(It.IsAny<string>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(serverRespons.Response, result.Value);
        }

        [Fact]
        public async Task GetUserByID_Found()
        {
            //Arrange
            var user = new User(Guid.NewGuid(), "Luddebassen", "Luddebassen", "Luddebassen", 100);
            var clientUser = new ClientUser(user.username, user.balance); //controller returns a new object clientUser (not all parameters are being sent back to the client)

            mockRep.Setup(u => u.GetUserByID(user.id)).ReturnsAsync(user);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (OkObjectResult)await stockController.GetUserByID(user.id.ToString());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(result.Value.ToString(), clientUser.ToString()); //casting both to String to ensure same type for both parameters.
        }

        [Fact]
        public async Task GetUserByID_NotFound()
        {
            //Arrange
            Guid id = Guid.NewGuid(); //generating a random id that does not exist for any users
            string serverRespons = "User could not be found!";


            mockRep.Setup(u => u.GetUserByID(id)).ReturnsAsync((User)null); //returns null for User-object if User.id is not found

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (NotFoundObjectResult)await stockController.GetUserByID(id.ToString());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }

        [Fact]
        public async Task GetUserByUsername_Found_LogInOK()
        {
            //Arrange
            var user = new User(Guid.NewGuid(), "Luddebassen", "Luddebassen", "Luddebassen", 100);
            var clientUser = new ClientUser(user.username, user.balance); //controller returns a new object clientUser (not all parameters are being sent back to the client)

            mockRep.Setup(u => u.GetUserByUsername(user.username)).ReturnsAsync(user);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = user.id.ToString(); //connect id to session
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.GetUserByUsername(user.username);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode); //OK = OK
            Assert.Equal(result.Value.ToString(), clientUser.ToString()); //casting to string so both parameters are the same object type
        }

        [Fact]
        public async Task GetUserByUsername_NotFound()
        {
            //Arrange
            string username = "not_found"; //generating a random id that does not exist for any users
            string serverRespons = "User could not be found!";
            mockRep.Setup(u => u.GetUserByUsername(username)).ReturnsAsync((User)null); //returns null for User-object if User.id is not found

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //do not need sessions because we check if object exists before checking log in

            //Act
            var result = (NotFoundObjectResult)await stockController.GetUserByUsername(username);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal(result.Value, serverRespons);
        }

        [Fact]
        public async Task GetUserByUsername_Found_LogInNotOK()
        {
            //Arrange
            var user = new User(Guid.NewGuid(), "Luddebassen", "Luddebassen", "Luddebassen", 100);
            string serverRespons = "You have to be logged in to perform this action!";

            mockRep.Setup(u => u.GetUserByUsername(user.username)).ReturnsAsync(user);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn; //not logged in
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedObjectResult)await stockController.GetUserByUsername(user.username);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal(result.Value, serverRespons);
        }

        [Fact]

        public async Task CheckUsername_Valid()
        {
            //Arrange
            string username = "Luddebassen";
            string serverRespons = "Username is valid";

            mockRep.Setup(u => u.CheckUsername(username)).ReturnsAsync(true);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //not dependent on login (dont need session)

            //Act
            var result = (OkObjectResult)await stockController.CheckUsername(username);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }
        [Fact]

        public async Task CheckUsername_Taken()
        {
            //Arrange
            string username = "Luddebassen";
            string serverRespons = "Username was invalid or already taken!";

            mockRep.Setup(u => u.CheckUsername(username)).ReturnsAsync(false);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //not dependent on login (dont need session)

            //Act
            var result = (BadRequestObjectResult)await stockController.CheckUsername(username);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }
        [Fact]
        public async Task CreateCompany_LogInNotOK()
        {
            //Arrange
            string serverRespons = "You have to be logged in to perform this action!";


            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedObjectResult)await stockController.CreateCompany(It.IsAny<string>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }

        [Fact]
        public async Task CreateCompany_LogInOk_DBerror()
        {
            //Arrange
            var serverRespons = new ServerResponse(false, "Server Exception!");
            string name = "Company";

            mockRep.Setup(u => u.CreateCompany(name)).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = SessionKeyUser;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (BadRequestObjectResult)await stockController.CreateCompany(name);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(serverRespons.Response, result.Value);
        }

        [Fact]
        public async Task CreateCompany_LogInOk_dbOK()
        {
            //Arrange
            var serverRespons = new ServerResponse(true);
            string name = "Company";


            mockRep.Setup(u => u.CreateCompany(name)).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = SessionKeyUser;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.CreateCompany(name);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Company created", result.Value);
        }

        [Fact]
        public async Task GetCompanyByID_Found()
        {
            //Arrange
            Company company = new Company(Guid.NewGuid(), "Company", 10, "[0,1,2,3,4,5,6,7,8,9]");
            ClientCompany clientCompany = new ClientCompany(company.name, company.value, company.values);
            

            mockRep.Setup(u => u.GetCompanyByID(company.id)).ReturnsAsync(company);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (OkObjectResult)await stockController.GetCompanyByID(company.id.ToString());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(clientCompany.ToString(), result.Value.ToString());
        }

        [Fact]
        public async Task GetCompanyByID_NotFound()
        {
            //Arrange 
            Guid id = Guid.NewGuid(); //random id
            string serverRespons = "Company could not be found!";

            mockRep.Setup(u => u.GetCompanyByID(id)).ReturnsAsync((Company)null); //returns null for User-object if User.id is not found

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act 
            var result = (NotFoundObjectResult)await stockController.GetCompanyByID(id.ToString());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }

        [Fact]
        public async Task GetAllCompanies_OK() //Not dependent on log-in
        {
            // Arrange
            //list with company
            var company1 = new Company(Guid.NewGuid(), "Company1", 100, "[22,33,100]");
            var company2 = new Company(Guid.NewGuid(), "Company2", 100, "[22,33,100]");
            var company3 = new Company(Guid.NewGuid(), "Company3", 100, "[22,33,100]");
            
            var companyList = new List<Company>();
            companyList.Add(company1);
            companyList.Add(company2);
            companyList.Add(company3);

            //list with companies that is being sent to the user
            var clientCompany1 = new ClientCompany(company1.name, company1.value, company1.values);
            var clientCompany2 = new ClientCompany(company2.name, company2.value, company2.values);
            var clientCompany3 = new ClientCompany(company3.name, company3.value, company3.values);

            var clientCompanylist = new List<ClientCompany>();
            clientCompanylist.Add(clientCompany1);
            clientCompanylist.Add(clientCompany2);
            clientCompanylist.Add(clientCompany3);


            mockRep.Setup(u => u.GetAllCompanies()).ReturnsAsync(companyList);

            var stockController = new StockController(mockRep.Object, mockLog.Object);


            // Act
            var result = await stockController.GetAllCompanies() as OkObjectResult;

            // Assert 
            
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(result.Value.ToString(), clientCompanylist.ToString());
        }

        [Fact]
        public async Task DeleteUser_LogInNotOK()
        {
            //Arrange
            string serverRespons = "You have to be logged in to perform this action!";


            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedObjectResult)await stockController.DeleteUser();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }

        [Fact]
        public async Task DeleteUser_LogInOK_DBError()
        {
            //Arrange
            ServerResponse serverRespons = new ServerResponse(false, "Server Exception!");

            mockRep.Setup(u => u.DeleteUser(It.IsAny<string>())).ReturnsAsync(serverRespons);
            
            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = SessionKeyUser;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (BadRequestObjectResult)await stockController.DeleteUser();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(serverRespons.Response, result.Value);
        }

        [Fact]
        public async Task DeleteUser_LogInOK()
        {
            //Arrange
            ServerResponse serverRespons = new ServerResponse(true);

            mockRep.Setup(u => u.DeleteUser(It.IsAny<string>())).ReturnsAsync(serverRespons);
            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = SessionKeyUser;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.DeleteUser();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("User deleted", result.Value);

        }
        [Fact]
        public async Task GetCurrentCompany_ActiveNotOK() //checking if session for company is active
        {
            //Arrange
            Guid id = Guid.NewGuid();
            string serverRespons = "No company is currently active!";

            //mockRep setup not needed because no function is called if company is not active

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyCompany] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedObjectResult)await stockController.GetCurrentCompany();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }

        [Fact]
        public async Task GetCurrentCompany_ActiveOK_NotFound()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            string serverRespons = "Company could not be found!";

            mockRep.Setup(u => u.GetCompanyByID(id)).ReturnsAsync((Company)null);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyCompany] = id.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (NotFoundObjectResult)await stockController.GetCurrentCompany();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }
        [Fact]
        public async Task GetCurrentCompany_ActiveOK_Found()
        {
            //Arrange
            var company1 = new Company(Guid.NewGuid(), "Company1", 100, "[22,33,100]");
            var clientCompany1 = new ClientCompany(company1.name, company1.value, company1.values);

            mockRep.Setup(u => u.GetCompanyByID(company1.id)).ReturnsAsync(company1);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyCompany] = company1.id.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.GetCurrentCompany();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(clientCompany1.ToString(), result.Value.ToString());

        }

        [Fact]
        public async Task GetCurrentUser_LogInNotOK()
        {
            //Arrange
            string serverRespons = "You have to be logged in to perform this action!";


            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedObjectResult)await stockController.GetCurrentUser();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }

        [Fact]
        public async Task GetCurrentUser_LogInOK_DBError()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            string serverRespons = "User could not be found!";

            mockRep.Setup(u => u.GetUserByID(id)).ReturnsAsync((User)null);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = id.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (NotFoundObjectResult)await stockController.GetCurrentUser();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }

        [Fact]
        public async Task GetCurrentUser_LogInOK_dbOK()
        {
            //Arrange
            User user = new User(Guid.NewGuid(), "Luddebassen", "Luddebassen", "Luddebassen", 100);
            ClientUser clientUser = new ClientUser(user.username, user.balance);

            mockRep.Setup(u => u.GetUserByID(user.id)).ReturnsAsync(user);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = user.id.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.GetCurrentUser();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(clientUser.ToString(), result.Value.ToString());
        }

        [Fact]
        public async Task GetListOfStocksForUserWithID_LogInNotOK()
        {
            //Arrange
            string serverRespons = "You have to be logged in to perform this action!";

            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedObjectResult)await stockController.GetStocksForUser(It.IsAny<string>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }

        [Fact]
        public async Task GetAllUsersValue()
        {
            //Arrange
            List<ClientStock> userList = new List<ClientStock>();
            mockRep.Setup(k => k.GetAllUsersTotalValue()).ReturnsAsync(userList);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (OkObjectResult)await stockController.GetUsersValue();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal<List<ClientStock>>(userList, (List<ClientStock>)result.Value);
        }

        [Fact]
        public async Task GetUserValueByID_LogInNotOK()
        {
            //Arrange
            string serverRespons = "You have to be logged in to perform this action!";

            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (UnauthorizedObjectResult)await stockController.GetUsersValueByID(It.IsAny<string>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }

        [Fact]
        public async Task GetUserValueByID_LogInOK_NotFound()
        {
            //Arrange
            Guid id = Guid.NewGuid(); //generating a random id that does not exist for any users
            string serverRespons = "User could not be found!";
            

            mockRep.Setup(u => u.GetUsersValueByID(id.ToString())).ReturnsAsync((ClientStock)null); //returns null for stockname-object for id is not found

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = id.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (NotFoundObjectResult)await stockController.GetUsersValueByID(id.ToString());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }

        [Fact]
        public async Task GetUserValueByID_LogInOK_Found()
        {
            //Arrange
            var user = new User(Guid.NewGuid(), "Luddebassen", "Luddebassen", "Luddebassen", 100);
            var userStock = new ClientStock("Luddebassen", 22, 100);
            
            mockRep.Setup(u => u.GetUsersValueByID(user.id.ToString())).ReturnsAsync(userStock); //returns null for stockname-object for id is not found

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = user.id.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.GetUsersValueByID(user.id.ToString());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(userStock, result.Value);
        }

        [Fact]
        public async Task BuyStock_LogInNotOK()
        {
            //Arrange
            string serverRespons = "You have to be logged in to perform this action or No company is currently active";

            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null
            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockSession[SessionKeyCompany] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            
            //Act
            var result = (UnauthorizedObjectResult)await stockController.BuyStock(It.IsAny<int>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }

        [Fact]
        public async Task BuyStock_LogInOK_dbError()
        {
            //Arrange
            Guid userid = Guid.NewGuid();
            Guid companyid = Guid.NewGuid();
            ServerResponse serverRespons = new ServerResponse(false, "Server Exception!");

            mockRep.Setup(u => u.TryToBuyStockForUser(userid.ToString(), companyid.ToString(), It.IsAny<int>())).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = userid.ToString();
            mockSession[SessionKeyCompany] = companyid.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (BadRequestObjectResult)await stockController.BuyStock(It.IsAny<int>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(serverRespons.Response, result.Value);

        }

        [Fact]
        public async Task BuyStock_LogInOK_dbOK()
        {
            //Arrange
            Guid userid = Guid.NewGuid();
            Guid companyid = Guid.NewGuid();
            ServerResponse serverRespons = new ServerResponse(true);

            mockRep.Setup(u => u.TryToBuyStockForUser(userid.ToString(), companyid.ToString(), It.IsAny<int>())).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = userid.ToString();
            mockSession[SessionKeyCompany] = companyid.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.BuyStock(It.IsAny<int>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Stock sucessfully bought", result.Value);

        }

        [Fact]
        public async Task SellStock_LogInNotOK()
        {
            //Arrange
            string serverRespons = "You have to be logged in to perform this action or No company is currently active";

            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null
            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockSession[SessionKeyCompany] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;


            //Act
            var result = (UnauthorizedObjectResult)await stockController.SellStock(It.IsAny<int>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }

        [Fact]
        public async Task SellStock_LogInOK_dbError()
        {
            //Arrange
            Guid userid = Guid.NewGuid();
            Guid companyid = Guid.NewGuid();
            ServerResponse serverRespons = new ServerResponse(false, "Server Exception!");

            mockRep.Setup(u => u.TryToSellStockForUser(userid.ToString(), companyid.ToString(), It.IsAny<int>())).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = userid.ToString();
            mockSession[SessionKeyCompany] = companyid.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (BadRequestObjectResult)await stockController.SellStock(It.IsAny<int>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(serverRespons.Response, result.Value);

        }

        [Fact]
        public async Task SellStock_LogInOK_dbOK()
        {
            //Arrange
            Guid userid = Guid.NewGuid();
            Guid companyid = Guid.NewGuid();
            ServerResponse serverRespons = new ServerResponse(true);

            mockRep.Setup(u => u.TryToSellStockForUser(userid.ToString(), companyid.ToString(), It.IsAny<int>())).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = userid.ToString();
            mockSession[SessionKeyCompany] = companyid.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.SellStock(It.IsAny<int>());

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Stock sucessfully sold", result.Value);
        }

        [Fact]
        public async Task GetCurrentStock_LogInNotOK()
        {
            //Arrange
            string serverRespons = "You have to be logged in to perform this action or No company is currently active";

            //no mockRep setup because if user is not logged in the updateUser-function in the repo is not called and will return null
            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = _notLoggedIn;
            mockSession[SessionKeyCompany] = _notLoggedIn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;


            //Act
            var result = (UnauthorizedObjectResult)await stockController.GetCurrentStock();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);
        }

        [Fact]
        public async Task GetCurrentStock_LogInOK_dbError()
        {
            //Arrange
            Guid userid = Guid.NewGuid();
            Guid companyid = Guid.NewGuid();
            
            string serverRespons = "Stock could not be found!";

            mockRep.Setup(u => u.GetStockWithUserAndCompany(userid.ToString(), companyid.ToString())).ReturnsAsync((Stock)null);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = userid.ToString();
            mockSession[SessionKeyCompany] = companyid.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (BadRequestObjectResult)await stockController.GetCurrentStock();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(serverRespons, result.Value);

        }

        [Fact]
        public async Task GetCurrentStock_LogInOK_dbOK()
        {
            //Arrange
            Guid userid = Guid.NewGuid();
            Guid companyid = Guid.NewGuid();
            Stock stock = new Stock(Guid.NewGuid(), 1, userid, companyid, "Company1");
            ClientStock clientStock = new ClientStock(stock.companyName, stock.amount);
            

            mockRep.Setup(u => u.GetStockWithUserAndCompany(userid.ToString(), companyid.ToString())).ReturnsAsync(stock);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = userid.ToString();
            mockSession[SessionKeyCompany] = companyid.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.GetCurrentStock();

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(clientStock.ToString(), result.Value.ToString());
        }

        [Fact]
        public async Task CreateUser_dbError()
        {
            //Arrange
            string username = "luddebassen";
            string passord = "luddebassen";
            ServerResponse serverRespons = new ServerResponse(false, "Server Exception!");

            mockRep.Setup(u => u.CreateUser(username, passord)).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (BadRequestObjectResult)await stockController.CreateUser(username, passord);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(serverRespons.Response, result.Value);
        }

        [Fact]
        public async Task CreateUser_dbOK_UserNotFound()
        {
            //Arrange
            string username = "luddebassen";
            string passord = "luddebassen";
            ServerResponse serverRespons = new ServerResponse(true);
            string serverRespons2 = "User could not be found!";


            mockRep.Setup(u => u.CreateUser(username, passord)).ReturnsAsync(serverRespons);
            mockRep.Setup(u => u.GetUserByUsername(username)).ReturnsAsync((User)null);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (NotFoundObjectResult)await stockController.CreateUser(username, passord);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal(serverRespons2, result.Value);
        }

        [Fact]
        public async Task CreateUser_dbOK_UserFound()
        {
            //Arrange
            string username = "luddebassen";
            string passord = "luddebassen";
            ServerResponse serverRespons = new ServerResponse(true);
            User user = new User(Guid.NewGuid(), username, passord, passord, 100);


            mockRep.Setup(u => u.CreateUser(username, passord)).ReturnsAsync(serverRespons);
            mockRep.Setup(u => u.GetUserByUsername(username)).ReturnsAsync(user);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = user.id.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.CreateUser(username, passord);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("User created", result.Value);
        }

        [Fact]
        public async Task LogIn_dbError()
        {
            //Arrange
            string username = "luddebassen";
            string passord = "luddebassen";
            ServerResponse serverRespons = new ServerResponse(false, "Server Exception!");

            mockRep.Setup(u => u.LogIn(username, passord)).ReturnsAsync(serverRespons);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (BadRequestObjectResult)await stockController.LogIn(username, passord);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Equal(serverRespons.Response, result.Value);
        }

        [Fact]
        public async Task LogIn_dbOK_UserNotFound()
        {
            //Arrange
            string username = "luddebassen";
            string passord = "luddebassen";
            ServerResponse serverRespons = new ServerResponse(true);
            string serverRespons2 = "User could not be found!";


            mockRep.Setup(u => u.LogIn(username, passord)).ReturnsAsync(serverRespons);
            mockRep.Setup(u => u.GetUserByUsername(username)).ReturnsAsync((User)null);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            //Act
            var result = (NotFoundObjectResult)await stockController.LogIn(username, passord);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
            Assert.Equal(serverRespons2, result.Value);
        }

        [Fact]
        public async Task LogIn_dbOK_UserFound()
        {
            //Arrange
            string username = "luddebassen";
            string passord = "luddebassen";
            ServerResponse serverRespons = new ServerResponse(true);
            User user = new User(Guid.NewGuid(), username, passord, passord, 100);


            mockRep.Setup(u => u.LogIn(username, passord)).ReturnsAsync(serverRespons);
            mockRep.Setup(u => u.GetUserByUsername(username)).ReturnsAsync(user);

            var stockController = new StockController(mockRep.Object, mockLog.Object);

            mockSession[SessionKeyUser] = user.id.ToString();
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            stockController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var result = (OkObjectResult)await stockController.LogIn(username, passord);

            //Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("Logged in", result.Value);
        }
    }
}