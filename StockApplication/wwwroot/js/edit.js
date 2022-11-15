//inspired by lectures
$(function () {


    $.get("Stock/GetCurrentUser", function (user) { //get current User-objekt
        $("#id").val(user.id); // need id in schema, to confirm the correct user-object is being edited
        $("#username").val(user.username);
        $("#balance").val(user.balance);
    })
    .fail(function () {
        $("#error").html("Something went wrong on server.")
    });
});

function editUser() {
    const user = {
        id: $("#id").val(), // need id to confirm correct user is being edited
        username: $("#username").val(), //new username
        balance: $("#balance").val(), //new balance
    };
    $.post("Stock/UpdateUser", user, function (response) { //tries to update, if ok redirect to html, else error message
        window.location.href = 'profilepage.html';
    })
    .fail(function () {
        $("#error").html("Something went wrong on server.")
    });
}

    