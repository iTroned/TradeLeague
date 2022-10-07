var id = "";
var type = "";

$(function () {
    id = alert($.session.get("tempID")); 
    type = alert($.session.get("tempType"));
    $.session.remove("tempID");
    $.session.remove("tempType");
    showItem();
});
function showItem() {
    if (type === "user") {
        $.get("Stock/getUserByID?" + id, function (user) {
            formatUser(user);
        });
    }
    else if (type === "company") {
        $.get("Stock/getCompanyByID?" + id, function (company) {
            formatCompany(company);
        });
    }
    else {
        window.location.href("index.html");
    }
}
function formatUser(user) {
    let out = "<table class='table table-striped'>" +
        "<tr>" +
        "<th>Username</th><th>ID</th><th>Balance</th>" +
        "</tr>";
    out += "<tr>" +
        "<td>" + user.username + "</td>" +
        "<td>" + user.id + "</td>" +
        "<td>" + user.balance + "</td>" +
        "</tr>";
    $("#deleting").html(out);
}
function formatCompany(company) {
    let out = "<table class='table table-striped'>" +
        "<tr>" +
        "<th>Name</th><th>ID</th><th>Value</th>" +
        "</tr>";
    out += "<tr>" +
        "<td>" + company.name + "</td>" +
        "<td>" + company.id + "</td>" +
        "<td>" + company.value + "</td>" +
        "</tr>";
    $("#deleting").html(out);
}
function deleteItem() {

}