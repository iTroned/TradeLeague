$(function () {
    // hent kunden med kunde-id fra url og vis denne i skjemaet

    const id = window.location.search.substring(1);
    const url = "Stock/getUserByID?" + id;
        $.get(url, function (user) {
            $("#id").val(user.id); // må ha med id inn skjemaet, hidden i html
            $("#username").val(user.username);
            $("#balance").val(user.balance);
            $("#ownedStocks").val(user.ownedStocks);
        });
});

function editUser() {
    const user = {
        id: $("#id").val(), // må ha med denne som ikke har blitt endret for å vite hvilken kunde som skal endres
        username: $("#username").val(),
        balance: $("#balance").val(),
        ownedStocks: $("#ownedStocks").val(),
    };
    $.post("Stock/updateUser", user, function (OK) {
        if (OK) {
            window.location.href = 'home.html';
        }
        else {
            $("#feil").html("Feil i db - prøv igjen senere");
        }
    });
}

    