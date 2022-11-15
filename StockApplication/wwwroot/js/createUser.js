//code inspired from lectures
function createUser() {
    const user = {
        username: $("#username").val(), //username from input
        password: $("#password").val() //password from unput
    }
    const url = "Stock/CreateUser"; //create user
    $.post(url, user, function (response) {
        if (response.Status) {
            $("#error").html(response.Status);
            //window.location.href = 'index.html'; //redirecting to index after creation complete
        }
        else {
            $("#error").html("fuc");
        }
    });
};