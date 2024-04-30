﻿$(document).ready(function () {
    $('#tab1-link').click(); // Simulate click on tab1-link when the page loads
});

$(document).on("click", ".admin-add-emptab", function () {
    var tabId = $(this).attr('href');
    $('.tab-pane').hide();
    $(tabId).show();
    $('#' + tabId.slice(1) + '-form').show().siblings().hide();
});

$(document).on("click", ".admin-add-emptab", function () {
    $('.admin-add-emptab').removeClass('active-border');
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
    var dobInput = document.getElementById("DOB");
    var ageInput = document.getElementById("Age");
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
$(document).ready(function () {
    // Add click event listener to the "Select All" checkbox
    $('#selectAll').on('click', function () {
        // Get the state of the "Select All" checkbox
        var isChecked = this.checked;

        // Apply highlighting to all rows based on the state of "Select All"
        $('#adminempmanagementtable tbody tr').each(function () {
            $(this).toggleClass('selected', isChecked);
        });
    });

    // Add click event listener to checkboxes in each row
    $('#adminempmanagementtable tbody').on('click', 'input[type="checkbox"]', function () {
        // Toggle highlighting for the row containing the clicked checkbox
        $(this).closest('tr').toggleClass('selected', this.checked);
    });
});


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

