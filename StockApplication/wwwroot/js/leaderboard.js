$(function () {
    getAllUsers();
});

function getAllUsers() {
    $.get("Stock/getAllUsers", function (allUsers) {
        formatUsers(allUsers);
    });
}

function formatUsers(users) {
    let out = "<table id='tableSort' class='table table-striped table-bordered' cellspacing='0' width='100 %'>" +
        "<tr>" +
        "<th>#</th><th>Username</th><th>Balance</th><th>Total Value</th>" +
        "</tr>";
    let i = 1;
    let value = 0;
    for (let user of users) {
        value = totalValue(user.id);
        console.log(value);
        
        out += "<tr>" +
            "<td>#" + i + "</td>" +
            "<td>" + user.username + "</td>" +
            "<td>" + user.balance + "</td>" +
            "<td>" + value + "</td>"
            "</tr>";
        i++;
    }
    out += "</table>"; 
    $("#users").html(out);
    
}

function totalValue(user) {
    value = 0;
    $.get("Stock/getUsersValue?id=" + user, function (data) {
        console.log(data);
        $("#totalValue").html(data);
        $("#totalValue").val();
    
    });
    
    return value;
}

