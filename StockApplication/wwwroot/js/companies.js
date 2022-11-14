//code inspired from lectures
$(function () {
    getAllCompanies();
});

function getAllCompanies() { //getting list with allCompanies
    $.get("Stock/GetAllCompanies", function (allCompanies) {
        formatCompanies(allCompanies);
    });
}

function formatCompanies(companies) { //organizing table displaying all companies
    let out = "<table class='table table-striped' id='table'>" +
        "<tr>" +
        "<th>Name</th><th>Value</th>" +
        "</tr>";
    for (let company of companies) {
        out += "<tr>" +
            "<td>" + '<a onclick="goToCompany(\'' + company.id + '\')">' + company.name + "</a></td>" + //onclick redirect to html displaying info about chosen company
            "<td>" + company.value + "$</td>" +
            "</tr>";
    }
    out += "</table>";
    $("#companies").html(out);
}
    
function goToCompany(id) { //setting Company-session and being redirected to html displaying info about company
    $.get("Stock/SetCurrentCompany?id=" + id, function (OK) {
        if (OK) {
            window.location.href = "company.html";
        }
    });
}

//function from w3schools to search: https://www.w3schools.com/howto/howto_js_filter_table.asp
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