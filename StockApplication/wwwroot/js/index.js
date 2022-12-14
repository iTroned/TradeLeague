//inspired by lectures
$(function () {
    $.get("Stock/GetLoggedInStatus", function () {
        getAllUsers();
        getAllCompanies();
    })
    .fail(function () {
        window.location.href = 'login.html';
    });
});

function getAllUsers() {
    $.get("Stock/GetAllUsers", function (allUsers) { 
        formatUsers(allUsers);
    });
}
function getAllCompanies() {
    $.get("Stock/GetAllCompanies", function (allCompanies) {
        formatCompanies(allCompanies);
    });
}

function formatUsers(users) {
    let out = "<table class='table table-striped'>" +
        "<tr>" +
        "<th>Username</th><th>ID</th><th>Balance</th>" +
        "</tr>";
    for (let user of users) {
        out += "<tr>" +
            "<td>" + user.username + "</td>" +
            "<td>" + user.id + "</td>" +
            "<td>" + user.balance + "</td>" +
            "</tr>";
    }
    out += "</table>";
    $("#users").html(out);
}
function formatCompanies(companies) {
    let out = "<table class='table table-striped'>" +
        "<tr>" +
        "<th>Name</th><th>ID</th><th>Value</th>" +
        "</tr>";
    for (let company of companies) {
        out += "<tr>" +
            "<td>" + company.name + "</td>" +
            "<td>" + company.id + "</td>" +
            "<td>" + company.value + "</td>" +
            "</tr>";
    }
    out += "</table>";
    $("#companies").html(out);
}