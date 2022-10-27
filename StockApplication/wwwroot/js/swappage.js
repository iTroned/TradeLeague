let uid = "";
$(function () {
    getCurrentUser(); //get current User
});

function getAllUsers() {
    $.get("Stock/getAllUsers", function (allUsers) { //get all users
        formatUsers(allUsers);
    });
}
function getCurrentUser() {
    $.get("Stock/getCurrentUserID", function (id) { //get user ID from current session
        uid = id;
        getAllUsers();
    });
}

function formatUsers(users) { //display all users
    let out = "<table class='table table-striped'>" +
        "<tr>" +
        "<th>Username</th><th>ID</th><th></th>" +
        "</tr>";
    for (let user of users) {
        out += "<tr>" +
            "<td>" + user.username + "</td>" +
            "<td>" + user.id + "</td>";
        if (user.id != uid) { //display SWAP-button for every user EXCEPT the Current user-session
            
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
function swapUser(id) { //if SWAP-button is clicked, swap current session to new User and run functions to reload the table
    $.get("Stock/setCurrentUser?id=" + id, function (OK) {
        uid = id;
        getAllUsers();
    });
}