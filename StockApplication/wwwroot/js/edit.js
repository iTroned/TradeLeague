$(function () {


    $.get("Stock/getCurrentUser", function (user) {
            $("#id").val(user.id); // må ha med id inn skjemaet, hidden i html
            $("#username").val(user.username);
            $("#balance").val(user.balance);
        });
});

function editUser() {
    const user = {
        id: $("#id").val(), // må ha med denne som ikke har blitt endret for å vite hvilken kunde som skal endres
        username: $("#username").val(),
        balance: $("#balance").val(),
    };
    $.post("Stock/updateUser", user, function (OK) {
        if (OK) {
            window.location.href = 'index.html';
        }
        else {
            $("#feil").html("Something went wrong");
        }
    });
}

    