$(function () {
    getCurrentUser(); //get current user on page load
});

function getCurrentUser() {
    $.get("Stock/getCurrentUser", function (user) { //calling controller for current user-object
        formatUser(user);
        formatBalance(user);
        formatStock(user);
        totalValue(user);
        if (user.username === "admin") {
            $("#buttons").html("");
        }
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
    $.get("Stock/getUsersValueByID?id=" + user.id, function (data) {
        $("#totalValue").html(data.value + "$");
    });
}

function formatStock(user) { //formatting table with owned stocks
    $.get("Stock/getStocksForUser?id=" + user.id, function (stockList) { //getting user's owned stocks
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

function deleteUser() {
    let text = "Are you sure you want to delete this user?"
    if (confirm(text) == true) {
        $.get("Stock/deleteUser", function (OK) {
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

