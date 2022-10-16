$(function () {
    const id = window.location.search.substring(1);
    const url = "Stock/getCompanyByID?" + id;
    

    $.get(url, function (company) {
        $("#name").html(company.name);
        
        chart(company.values);
        formatCompany(company);
    });

});

function chart(values) {
    
    $("#chart").html(values);
}

function formatCompany(company) {

}