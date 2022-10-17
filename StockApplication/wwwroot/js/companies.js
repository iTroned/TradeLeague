$(function () {
    getAllCompanies();
});

function getAllCompanies() {
    $.get("Stock/getAllCompanies", function (allCompanies) {
        formatCompanies(allCompanies);
    });
}

function formatCompanies(companies) {
    let out = "<table class='table table-striped'>" +
        "<tr>" +
        "<th>Name</th><th>ID</th><th>Value</th>" +
        "</tr>";
    for (let company of companies) {
        out += "<tr>" +
            "<td><a href='company.html?id="+company.id+"'>" + company.name + "</td>" +
            "<td>" + company.id + "</td>" +
            "<td>" + company.value + "</td>" +
            "</tr>";
    }
    out += "</table>";
    $("#companies").html(out);
}