var id = "";
var type = "";

$(function () {
    const string = window.location.search;
    const param = new URLSearchParams(string);
    id = param.get('id'); 
    type = param.get('type'); 
    
    showItem(id, type);
});
function showItem(id, type) {
    if (type === "user") {
        const url = "Stock/getUserByID?id=" + id;
        $.get(url, function (user) {
            formatUser(user);
        });
    }
    else if (type === "company") {
        $.get("Stock/getCompanyByID?" + id, function (company) {
            formatCompany(company);
        });
    }
    else {
        window.location.href = "index.html";
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
    if (type === "user") {
        $.get("Stock/deleteUser?id=" + id, function (OK) {
            if (OK) {
                window.location.href = "index.html";
            }
            else {

            }
        });
    }
    else if (type === "company") {
        $.get("Stock/deleteCompany?id=" + id, function (OK) {
            if (OK) {
                window.location.href = "index.html";
            }
            else{

            }
        });
    }
    
}