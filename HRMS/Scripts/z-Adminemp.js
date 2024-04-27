$(document).ready(function () {
    $('#tab1-link').click(); // Simulate click on tab1-link when the page loads
});

$(document).on("click", ".nav-link", function () {
    var tabId = $(this).attr('href');
    $('.tab-pane').hide();
    $(tabId).show();
    $('#' + tabId.slice(1) + '-form').show().siblings().hide();
});

$(document).on("click", ".nav-link", function () {
    $('.nav-link').removeClass('active-border');
    $(this).addClass('active-border').siblings().removeClass('active-border');
});

$(document).on("change", "#copyPresentAddress", function () {
    if (this.checked) {
        $('#permanentAddress').val($('#presentAddress').val());
    }
});


$('#copyPresentAddress').change(function () {
    if (this.checked) {
        $('#permanentAddress').val($('#presentAddress').val());
    }
});


function calculateAge() {
    var dobInput = document.getElementById("dateOfBirth");
    var ageInput = document.getElementById("age");
    var dob = new Date(dobInput.value);
    var today = new Date();

    // Calculate the age
    var age = today.getFullYear() - dob.getFullYear();
    if (today.getMonth() < dob.getMonth() || (today.getMonth() == dob.getMonth() && today.getDate() < dob.getDate())) {
        age--;
    }

    // Display the calculated age
    ageInput.value = age;
}


// Function to add a new client

//document.addEventListener('DOMContentLoaded', function () {
//    var clientSelect = document.getElementById('client');

//    // Function to add a new option to the dropdown
//    function addOption(value, text) {
//        var option = document.createElement('option');
//        option.value = value;
//        option.text = text;
//        clientSelect.appendChild(option);
//    }

//    // Example: Add a new option
//    addOption('client4', 'Client 4');

//    // Example: Add an event listener to show the added option
//    clientSelect.addEventListener('change', function () {
//        console.log('Selected option: ' + this.value);
//    });
//});


//document.getElementById('copyPresentAddress').addEventListener('change', function () {
//    if (this.checked) {
//        var presentAddress = document.getElementById('presentAddress').value;
//        document.getElementById('permanentAddress').value = presentAddress;
//    } else {
//        document.getElementById('permanentAddress').value = '';
//    }
//});

    //function filterDepartments() {
    //        var input, filter, select, option, i;
    //input = document.getElementById('departmentInput');
    //filter = input.value.toUpperCase();
    //select = document.getElementById('departmentSelect');
    //option = select.getElementsByTagName('option');

    //for (i = 0; i < option.length; i++) {
    //            if (option[i].innerHTML.toUpperCase().indexOf(filter) > -1) {
    //    option[i].style.display = '';
    //            } else {
    //    option[i].style.display = 'none';
    //            }
    //        }
    //    }

