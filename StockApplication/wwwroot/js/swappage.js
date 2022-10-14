$(function () {
    getAllUsers();
});

function getAllUsers() {
    $.get("Stock/getAllUsers", function (allUsers) {
        formatUsers(allUsers);
    });
}

function formatUsers(users) {
    let out = "<table class='table table-striped'>" +
        "<tr>" +
        "<th>Username</th><th>ID</th><th></th>" +
        "</tr>";
    for (let user of users) {
        out += "<tr>" +
            "<td>" + user.username + "</td>" +
            "<td>" + user.id + "</td>" +
            "<td> <button class='btn btn-primary'>Swap</button></td>" +
            "</tr>";
    }
    out += "</table>";
    $("#users").html(out);
}