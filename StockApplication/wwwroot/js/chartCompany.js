﻿$(function () {
    const id = window.location.search.substring(1);
    const url = "Stock/getCompanyByID?" + id;
    

    $.get(url, function (company) {
        $("#name").html(company.name);
        company.values = JSON.parse(company.values);
        chart(company);
        formatCompany(company);
    });

});

function chart(company) {
    const ctx = document.getElementById('companyChart').getContext('2d');
    const labels = ["10 mins ago", "9 mins ago", "8 mins ago", "7 mins ago", "6 mins ago", "5 mins ago", "4 mins ago", "3 mins ago", "2 mins ago", "1 min ago"];
    const companyChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: company.name,
                data: company.values,
                fill: false,
                borderColor: 'rgb(75,192,192)',
                tension: 0.1
            }]
        }
    })
}

function getPercentage(numA, numB) {
    let number = ((numA - numB) / numB) * 100;
    return (Math.round(number * 100) / 100).toFixed(2);
}

function formatCompany(company) {
    let array = company.values;
    let change = getPercentage(array[array.length - 1], array[array.length - 2]);
    let tenMin = getPercentage(array[array.length - 1], array[0]);
    

    const out = "<br><br><h3>Current value: " + "<b>" + company.value + "$</b></h3>" +
        "<h4>Last change: " + "<b>" + change + "%</b></h4>" +
        "<h4>Last 10 minutes: " + "<b>" + tenMin + "%</b></h4>";

    $("#information").html(out);
}
