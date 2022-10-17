let uid = "";
$(function () {
    getCurrentUser();
});

function getAllUsers() {
    $.get("Stock/getAllUsers", function (allUsers) {
        formatUsers(allUsers);
    });
}
function getCurrentUser() {
    $.get("Stock/getCurrentUserID", function (id) {
        uid = id;
        getAllUsers();
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
            "<td>" + user.id + "</td>";
        if (user.id != uid) {
            //out += "<td>" + '<input type="button" class="btn btn-primary" value="Swap" "onClick="swapUser(\'' + user.id + '\');" />' + "</td>";
            out += "<td>" + '<button class="btn btn-primary" onclick="swapUser(\'' + user.id + '\')">Swap</button>' + "</td>";
        }
        else {
            out += "<td></td>";
        }
        out += "</tr>";
    }
    out += "</table>";
    $("#users").html(out);
}
function swapUser(id) {
    $.get("Stock/setCurrentUser?id=" + id, function (user) {
        uid = id;
        getAllUsers();
    });
}