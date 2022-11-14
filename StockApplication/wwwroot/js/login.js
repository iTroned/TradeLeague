//code inspired from lectures
function createUser() {
    const user = {
        username: $("#username").val(), //username from input
        password: $("#password").val() //password from unput
    }
    const url = "Stock/LogIn"; //create user
    $.post(url, user, function (OK) {
        if (OK) {
            window.location.href = 'index.html'; //redirecting to index after creation complete
        }
        else {
            $("#error").html("Something went wrong");
        }
    });
};