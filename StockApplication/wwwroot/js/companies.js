$(function () {
    getAllCompanies();
});

function getAllCompanies() {
    $.get("Stock/getAllCompanies", function (allCompanies) {
        formatCompanies(allCompanies);
    });
}

function formatCompanies(companies) {
    let out = "<table class='table table-striped' id='table'>" +
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

//function from w3schools: https://www.w3schools.com/howto/howto_js_filter_table.asp
function search() {
    var input = document.getElementById("searchbar");
    var filter = input.value.toUpperCase();
    var table = document.getElementById("table");
    var tr = table.getElementsByTagName("tr");

    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td")[0];
        if (td) {
            txtValue = td.textContent || td.innerText;
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                tr[i].style.display = "";
            }
            else {
                tr[i].style.display = "none";
            }
        }
    }
}