//inspired by lectures
$(function () {
    getAllUsers();
});

function getAllUsers() {
    $.get("Stock/getUsersValue", function (allUsers) { //getting list of AllUsers with TotalValues
        formatUsers(allUsers);
    });
}

function formatUsers(users) { //formatting list of users in a table
    let out = "<table id='tableSort' class='table table-striped table-bordered'>" +
        "<tr>" +
        "<th>#</th><th>Username</th><th>Amount of Shares</th><th>Total Value</th>" +
        "</tr>";
    let i = 1;
    for (let user of users) {
        out += "<tr>" +
            "<td>#" + i + "</td>" +
            "<td>" + user.name + "</td>" +
            "<td>" + user.amount + "</td>" +
            "<td>" + user.value + "</td>"
            "</tr>";
        i++;
    }
    out += "</table>"; 
    $("#users").html(out);
    
}


//function from w3schools to search: https://www.w3schools.com/howto/howto_js_filter_table.asp
function search() {
    var input = document.getElementById("searchbar");
    var filter = input.value.toUpperCase();
    var table = document.getElementById("tableSort");
    var tr = table.getElementsByTagName("tr");

    for (i = 0; i < tr.length; i++) {
        td = tr[i].getElementsByTagName("td")[1];
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



