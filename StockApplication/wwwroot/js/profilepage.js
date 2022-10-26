$(function () {
    getCurrentUser();
});

function getCurrentUser() {
    $.get("Stock/getCurrentUser", function (user) {
        formatUser(user);
        formatBalance(user);
        formatStock(user);
        if (user.username === "admin") {
            $("#buttons").html("");
        }
    });
}

function formatUser(user) {
    let out = "" + user.username;
    $("#name").html(out);

}


function formatBalance(user) {
    let out = user.balance + "$";
    $("#balance").html(out);

}

function formatStock(user) {
    $.get("Stock/getStocksForUser?id=" + user.id, function (stockList) {
        let out = "<table class='table table-striped'>" +
            "<tr>" +
            "<th>Stock name</th><th>(Shares)</th>" +
            "</tr>";
        for (let stock of stockList) {
            out += "<tr>" +
                "<td>" + stock.name + "</td>" +
                "<td>" + stock.amount + "</td>" +
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

