$(function () {
    getCurrentUser();
});

function getCurrentUser() {
    $.get("Stock/getCurrentUser", function (user) {
        formatUser(user);
    });
}

function getCurrentUSer() {
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
    $("#user").html(out);
}

$(function () {
    getAllUsers();
    getAllCompanies();
});

function getAllUsers() {
    $.get("Stock/getAllUsers", function (allUsers) {
        formatUsers(allUsers);
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