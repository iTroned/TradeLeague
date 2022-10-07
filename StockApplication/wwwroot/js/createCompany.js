function createUser() {
    const company = {
        name: $("#name").val()
    }
    const url = "Stock/createCompany";
    $.post(url, company, function (OK) {
        if (OK) {
            window.location.href = 'index.html';
        }
        else {
            $("#error").html("Something went wrong");
        }
    });
};