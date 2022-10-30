# StockApplication





1. Create your user!

2. Go to Company list!

3. Buy your shares!

4. Check the leaderboard!



Locations for all files:<br/>

StockApplication/Program.cs <br/>
StockApplication/Startup.cs <br/>

**VIEWS**: <br/>
StockApplication/wwwroot/companies.html -> javascript: StockApplication/wwwroot/js/companies.js <br/>
StockApplication/wwwroot/company.html -> javascript: StockApplication/wwwroot/js/chartCompany.js <br/>
StockApplication/wwwroot/createCompany.html -> javascript: StockApplication/wwwroot/js/createCompany.js <br/> 
StockApplication/wwwroot/createUser.html -> javascript: StockApplication/wwwroot/js/createUser.js <br/>
StockApplication/wwwroot/edit.html -> javascript: StockApplication/wwwroot/js/edit.js <br/>
StockApplication/wwwroot/index.html ->  <br/>
StockApplication/wwwroot/leaderboard.html -> javascript: StockApplication/wwwroot/js/leaderboard.js <br/>
StockApplication/wwwroot/profilepage.html -> javascript: StockApplication/wwwroot/js/profilepage.js <br/> 
StockApplication/wwwroot/swappage.html -> javascript: StockApplication/wwwroot/js/swappage.js <br/> 


**MODEL**: <br/>
StockApplication/Code/Stock.cs <br/> 
StockApplication/Code/User.cs <br/> 
StockApplication/Code/Company.cs <br/>

StockApplication/Code/StockName.cs <br/> 
StockApplication/Code/Stock.cs <br/>

**BACKGROUND-SERVICE** (VALUE UPDATER): <br/>
StockApplication/Code/ValueUpdater.cs <br/>

 
**CONTROLLER**: <br/>
StockApplication/Code/Controllers/StockController.cs <br/>


**DATA ACCESS LAYER**: <br/>
StockApplication/Code/DAL/DBInit.cs <br/> 
StockApplication/Code/DAL/StockContext.cs <br/> 
StockApplication/Code/DAL/StockRepository.cs <br/>
StockApplication/Code/DAL/IStockRepository.cs <br/>

