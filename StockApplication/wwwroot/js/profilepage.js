$(function () {
    getCurrentUser();
});

function getCurrentUser() {
    $.get("Stock/getCurrentUser", function (user) {
        formatUser(user);
        formatBalance(user);
        formatStock(user);
    });
}

function formatUser(user) {
    let out = "Welcome to your profile, " + user.username;
    $("#name").html(out);

}

function formatBalance(user) {
    let out = "" + user.balance + "$";
    $("#balance").html(out);

}

function formatStock(user) {
    let out = "<table class='table table-striped'>" +
        "<tr>" +
        "<th>Stock name</th><th>(Shares)</th>" +
        "</tr>";
        "<tr>" +
            "<td>" + user.ownedStock + "</td>" +
            //"<td>" + user.stockShares + "</td>" +
            "</tr>";
    
    $("#ownedstock").html(out);

}

