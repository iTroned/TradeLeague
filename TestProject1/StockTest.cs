using Microsoft.Extensions.Logging;
using Moq;
using StockApplication.Code;
using StockApplication.Code.DAL;
using StockApplication.Controllers;
using System.Threading.Tasks;
using System;
using Xunit;

namespace TestProject1
{
    public class StockTest
    {
        private readonly Mock<IStockRepository> mockRep = new Mock<IStockRepository>();
        private readonly Mock<ILogger<StockController>> mockLog = new Mock<ILogger<StockController>>();

        [Fact]
        public async Task Lagre()
        {
            //Arrange
            var innKunde = new User
        }
        {

        }
    }
}
