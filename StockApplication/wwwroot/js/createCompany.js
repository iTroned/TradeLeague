//code inspired from lectures
function createCompany() {
    const company = {
        name: $("#name").val() //get name from input
    }
    const url = "Stock/CreateCompany"; //create company
    $.post(url, company, function (response) {
        if (response.Status) {
            window.location.href = 'index.html'; //redirecting to index after creation complete
        }
        else {
            $("#error").html(response.Response);
        }
    });
};