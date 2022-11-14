$(function () { //calling functions at page load
    getCompany();
    getStock();
});

function getCompany() {
    $.get("Stock/GetCurrentCompany", function (company) { //getting current session company
        $("#name").html(company.name);
        company.values = JSON.parse(company.values); //parsing json to get values as an array

        chart(company); //calling functions to display chart and values
        formatCompany(company);
    });
}

function chart(company) { //creating chart
    const ctx = document.getElementById('companyChart').getContext('2d'); //getting context from canvas in company.html
    const labels = ["10 mins ago", "9 mins ago", "8 mins ago", "7 mins ago", "6 mins ago", "5 mins ago", "4 mins ago", "3 mins ago", "2 mins ago", "1 min ago"]; //creating set labels
    companyChart = new Chart(ctx, { //new chart with context(ctx) and assigning data + designing, companyChart is global variable because we need to access it in our ajax call
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: company.name,
                data: company.values,
                fill: false,
                borderColor: 'rgb(75,192,192)',
                tension: 0,
                pointHitRadius: 100,
                pointRadius: 5
            }]
        }
    })
}

function getPercentage(numA, numB) { //function to get percentage increase/decrease in value
    let number = ((numA - numB) / numB) * 100;
    return (Math.round(number * 100) / 100).toFixed(2);
}

function formatCompany(company) { //format display of values
    let array = company.values; 
    let change = getPercentage(array[array.length - 1], array[array.length - 2]); //increase/decrease percentage from last two values
    let tenMin = getPercentage(array[array.length - 1], array[0]); //increase/decrease percentage from last ten minutes. (first value and last value compared)

    if (change > 0) { //assigning addition symbol to value if positive, (negative symbol is added automatically)
        change = "+" + change;
    }
    if (tenMin > 0) {
        tenMin = "+" + tenMin;
    }

    const out = "<br><br><h3>Current value: " + "<b>" + company.value + "$</b></h3>" + //formatting
        "<h4>Last change: " + "<b>" + change + "%</b></h4>" +
        "<h4>Last 10 minutes: " + "<b>" + tenMin + "%</b></h4>";

    $("#information").html(out); //updating information div with new formatted values
}


function getStock() {
    $.get("Stock/GetCurrentStockAmount", function (amount) { //returns amount of shares owned for current user at current company
        $("#current").html("Current stocks: " + amount);
    });
    $.get("Stock/GetBalance", function (bal) { //returns balance for current user
        $("#balance").html("Current balance: " + bal + "$");
    });
}
function buyStock() { //buy stock function onclick
    const amount = $("#amount").val(); 
    $.get("Stock/BuyStock?amount=" + amount, function (OK) { //current user tries to buy X amount of shares from current company
        if (OK) {
            $("#message").html("Successfully bought " + amount + " stocks!");
            getStock(); //if buy is successful, current amount of shares owned is updated
        }
        else {
            $("#message").html("Something went wrong while buying!");
        }
    });
}
function sellStock() { //sell stock function onclick
    const amount = $("#amount").val(); 
    $.get("Stock/SellStock?amount=" + amount, function (OK) { //current user tries to sell X amount of shares from current company
        if (OK) {
            $("#message").html("Successfully sold " + amount + " stocks!");
            getStock(); //if sell is successful, current amount of shares owned is updated
        }
        else {
            $("#message").html("Something went wrong while selling!");
        }
    });
}

function updateData() {
    $.ajax({ //ajax call to request updates from dal
        url: 'Stock/GetCurrentCompany', //get current session company
        type: 'get',
        success: function (data) { //call on functions to update site dynamically
            data.values = JSON.parse(data.values);
            formatCompany(data); //updating values
            companyChart.data.datasets.forEach((dataset) => { //adding new data to charts dataset
                dataset.data = data.values;
            });
            companyChart.update(); //updating chart
            

        }

    });
}

setInterval(updateData, 30000) //intervall every 30 seconds to check for updates