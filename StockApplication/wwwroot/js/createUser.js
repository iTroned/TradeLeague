function createUser() {
    const user = {
        username: $("#username").val()
    }
    const url = "Stock/createUser";
    $.post(url, user, function (OK) {
        if (OK) {
            window.location.href = 'home.html';
        }
        else {
            $("#error").html("Something went wrong");
        }
    });
};