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
        "<th>Username</th><th>ID</th><th>Balance</th>" +
        "</tr>";
    for (let user of users) {
        out += "<tr>" +
            "<td>" + user.username + "</td>" +
            "<td>" + user.id + "</td>" +
            "<td>" + user.balance + "</td>" +
            //"<td> <a class='btn btn-primary' href='endre.html?id=" + kunde.id + "'>Endre</a></td>" +
            //"<td> <button class='btn btn-danger' onclick='slettKunde(" + kunde.id + ")'>Slett</button></td>" +
            "</tr>";
    }
    out += "</table>";
    $("#users").html(out);
}