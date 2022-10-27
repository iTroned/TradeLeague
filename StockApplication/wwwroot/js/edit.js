//inspired by lectures
$(function () {


    $.get("Stock/getCurrentUser", function (user) { //get current User-objekt
            $("#id").val(user.id); // need id in schema, to confirm the correct user-object is being edited
            $("#username").val(user.username);
            $("#balance").val(user.balance);
        });
});

function editUser() {
    const user = {
        id: $("#id").val(), // need id to confirm correct user is being edited
        username: $("#username").val(), //new username
        balance: $("#balance").val(), //new balance
    };
    $.post("Stock/updateUser", user, function (OK) { //tries to update, if ok redirect to html, else error message
        if (OK) {
            window.location.href = 'index.html';
        }
        else {
            $("#feil").html("Something went wrong");
        }
    });
}

    