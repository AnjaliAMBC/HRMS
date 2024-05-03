$(document).ready(function () {
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
        $('#Permanent_Address').val($('#PresentAddress').val());
    }
});

$(document).on("change", ".emp-dob", function () {
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
});

//$(document).ready(function () {
//    $('#saveNewDesignation').click(function () {
//        var newDesignation = $('#newDesignation').val();
//        $.ajax({
//            url: 'AdminDashboard/DesignationAdd',
//            method: 'POST',
//            data: { designation: newDesignation },
//            success: function (response) {
//                // Handle success
//                $('#addDesignationModel').modal('hide');
//            },
//            error: function (xhr, status, error) {
//                // Handle error
//            }
//        });
//    });
//});
