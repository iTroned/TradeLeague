$(function () {
    getCurrentUser(); //get current user on page load
});

function getCurrentUser() {
    $.get("Stock/GetCurrentUser", function (user) { //calling controller for current user-object
        formatUser(user);
        formatBalance(user);
        formatStock(user);
        totalValue(user);
    });
}

function formatUser(user) { //formatting welcome message
    let out = "" + user.username;
    $("#name").html(out);

}


function formatBalance(user) { //display user.balance in balance-div.
    let out = user.balance + "$";
    $("#balance").html(out);

}
function totalValue(user) { //getting TotalValue for user, displaying in totalValue-div
    $.get("Stock/GetUsersValueByID?id=" + user.id, function (data) {
        $("#totalValue").html(data.value + "$");
    });
}

function formatStock(user) { //formatting table with owned stocks
    $.get("Stock/GetStocksForUser?id=" + user.id, function (stockList) { //getting user's owned stocks
        let out = "<table class='table table-striped'>" +
            "<tr>" +
            "<th>Stock name</th><th>Shares</th><th>Value</th>" +
            "</tr>";
        for (let stock of stockList) {
            out += "<tr>" +
                "<td>" + stock.name + "</td>" +
                "<td>" + stock.amount + "</td>" +
                "<td>" + stock.value + "$</td>" +
                "</tr>";
        }
        
        $("#ownedstock").html(out);
    });
}

function deleteUser() { //deleteUser function
    let text = "Are you sure you want to delete this user?"
    if (confirm(text) == true) { //confirm box to make sure user dosen't delete on accident
        $.get("Stock/DeleteUser", function (OK) {
            if (OK) {
                window.location.href = "index.html";
            }
            else {
                window.location.href = "profilepage.html"
            }
        });

    }
    else { text = "You canceled" }
}
function editUser() {
    window.location.href = "edit.html";
}
function logOut() {
    $.get("Stock/LogOut", function (OK) {
        window.location.href = "index.html";
    });
}

