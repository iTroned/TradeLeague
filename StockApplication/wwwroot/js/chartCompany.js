$(function () {
    const id = window.location.search.substring(1);
    const url = "Stock/getCompanyByID?" + id;
    

    $.get(url, function (company) {
        $("#name").html(company.name);
        
        chart(company);
        formatCompany(company);
    });

});

function chart(company) {
    const array = JSON.parse(company.values);
    const ctx = document.getElementById('companyChart').getContext('2d');
    const labels = ["10 mins ago", "9 mins ago", "8 mins ago", "7 mins ago", "6 mins ago", "5 mins ago", "4 mins ago", "3 mins ago", "2 mins ago", "1 min ago"];
    const companyChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: company.name,
                data: array,
                fill: false,
                borderColor: 'rgb(75,192,192)',
                tension: 0.1
            }]
        }
    })
}

function formatCompany(company) {

}