function HighlightAdminActiveLink(element) {
    $('#admindash-menu li a.active').removeClass('active');
    $(element).addClass('active');
}

//Self service link js
$('.admin-dashboard').click(function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));

    $(".hiddenadmindashboard").html("");
    $(".admin-dashboard-container").html("");


    $('.admin-dashboard-container').show();
    $('.admin-empmanagement-container').hide();
    $('.admin-attendance-container').hide();
    $('.admin-leave-container').hide();
    $('.admin-ticketing-container').hide();

    $(".hiddenadmindashboard").html("");


});

//Dashboard link js
$('.admin-empmanagement').click(function (event) {
    $('#adminempmanagementtable').DataTable().destroy();

  

    event.preventDefault();
    HighlightAdminActiveLink($(this));

    $.ajax({
        url: '/admindashboard/employeemanagement',
        type: 'GET',
        dataType: 'html',
        success: function (response) {
            //Clear old date and bind again
            $(".hiddenadmindashboard").html("");
            $(".admin-empmanagement-container").html("");

            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-empmanagement-view").html();
            $(".admin-empmanagement-container").html(formContent);

            $('.admin-empmanagement-container').show();
            $('.admin-dashboard-container').hide();
            $('.admin-attendance-container').hide();
            $('.admin-leave-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");


            var table = $('#adminempmanagementtable').DataTable({
                "paging": true,
                "searching": false,
                "info": true,
                "order": [],
                /*   "lengthChange": false,*/
                "lengthMenu": [[1, 2, 5, -1], [1, 2, 5, "All"]],
                "columnDefs": [
                    { "orderable": false, "targets": [0, -1] }
                ]
            });

            $('#adminempmanagementtable thead th:first-child').html('<input type="checkbox" id="selectAll">');
            $('#selectAll').on('change', function () {
                var isChecked = $(this).prop('checked');
                if (isChecked) {
                    $('td input[type="checkbox"]').prop('checked', isChecked);
                    $('#action').show();
                }
                else {
                    $('td input[type="checkbox"]').prop('checked', false);
                    $('#action').hide();                  
                }
                
            });

            $('#menuIcon').on('click', function () {
                // Your menu icon click handler code here
            });

            $('#dropdownMenu').on('click', function () {
                // Your dropdown menu click handler code here
            });

            $('#dropdownMenuContent input[type="checkbox"]').change(function () {
                var columnIdx = $(this).closest('a').index() + 1;
                var isChecked = $(this).prop('checked');
                table.column(columnIdx).visible(isChecked);
            });

            $('#dropdownMenuContent input[type="checkbox"]').prop('checked', true);


            $(document).on('change', '.empmgmt-check', function () {
                var isChecked = $(this).prop('checked');
                if (isChecked) {
                    $('#action').show();
                }
                else {
                    $(this).prop('checked', false);
                    if ($(".empmgmt-check:checked").length <= 0) {
                        $('#action').hide();
                        $('#selectAll').prop('checked', false);
                    }                    
                }
            });


        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
});




//Attedence link js
$('.admin-attendance').click(function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));
    
    $.ajax({
        url: '/admindashboard/attendancemanagement',
        type: 'GET',
        dataType: 'html',
        success: function (response) {
            //clear old data and bind again
            $(".hiddenadmindashboard").html("");
            $(".admin-attendance-container").html("");

            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-attendancemgmt-view").html();
            $(".admin-attendance-container").html(formContent);

            $('.admin-attendance-container').show();
            $('.admin-empmanagement-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-leave-container').hide();
            $('.admin-ticketing-container').hide();

            $(".hiddenadmindashboard").html("");
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });



});

//Leave Tracker
$('.admin-leave').click(function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));

    $('.admin-leave-container').show();
    $('.admin-attendance-container').hide();
    $('.admin-empmanagement-container').hide();
    $('.admin-dashboard-container').hide();
    $('.admin-ticketing-container').hide();

    $(".hiddenadmindashboard").html("");
});


//My Request link
$('.admin-ticketing').click(function (event) {
    event.preventDefault();
    HighlightActiveLink($(this));

    $('.admin-ticketing-container').show();
    $('.admin-leave-container').hide();
    $('.admin-attendance-container').hide();
    $('.admin-empmanagement-container').hide();
    $('.admin-dashboard-container').hide();

    $(".hiddenadmindashboard").html("");
});


////EMp add tabs logic
//function addOption() {
//    var select = document.getElementById("status");
//    var optionText = prompt("Enter the new option text:");
//    if (optionText) {
//        var option = document.createElement("option");
//        option.text = optionText;
//        option.value = optionText.toLowerCase().replace(/\s/g, '');
//        select.add(option);
//    }
//}
//function validateForm1() {
//    var preview = document.getElementById('id-preview');
//    var empid = document.getElementById('employeeID').value;
//    var empname = document.getElementById('employeeName').value;
//    var fatherName = document.getElementById('fatherName').value;
//    var bloodGroup = document.getElementById('bgroup').value;
//    var status = document.getElementById('status').value;
//    var gendar = document.getElementById('gendar').value;
//    var maritalstatus = document.getElementById('marital').value;
//    var designation = document.getElementById('designation').value;
//    var department = document.getElementById('department').value;
//    var client = document.getElementById('client').value;
//    var employeetype = document.getElementById('employeetype').value;
//    var location = document.getElementById('location').value;
//    var manager = document.getElementById('manager').value;
//    var leave = document.getElementById('leave').value;
//    var number = document.getElementById('number').value;
//    var email = document.getElementById('email').value;
//    var offemail = document.getElementById('offemail').value;
//    var present = document.getElementById('present').value;
//    var dependentname = document.getElementById('dependentname').value;
//    var dependentrelation = document.getElementById('dependentrelation').value;
//    var dependentnumber = document.getElementById('dependentnumber').value;

//    var isValid = true;

//    document.getElementById('empid-error').innerText = '';
//    document.getElementById('emp-name').innerText = '';
//    document.getElementById('fathername-error').innerText = '';
//    document.getElementById('bloodgroup-error').innerText = '';
//    document.getElementById('status-error').innerText = '';
//    document.getElementById('gendar-error').innerText = '';
//    document.getElementById('marital-error').innerText = '';
//    document.getElementById('designation-error').innerText = '';
//    document.getElementById('department-error').innerText = '';
//    document.getElementById('client-error').innerText = '';
//    document.getElementById('employeetype-error').innerText = '';
//    document.getElementById('location-error').innerText = '';
//    document.getElementById('manager-error').innerText = '';
//    document.getElementById('leave-error').innerText = '';
//    document.getElementById('number-error').innerText = '';
//    document.getElementById('email-error').innerText = '';
//    document.getElementById('offemail-error').innerText = '';
//    document.getElementById('present-error').innerText = '';
//    document.getElementById('dependentname-error').innerText = '';
//    document.getElementById('dependentrelation-error').innerText = '';
//    document.getElementById('dependentnumber-error').innerText = '';

//    preview.classList.add('hide');

//    if (empid === '') {
//        document.getElementById('empid-error').innerText = 'Employee Id is required';
//        isValid = false;
//    }
//    if (empname == '') {
//        document.getElementById('emp-name').innerText = 'Employee Name is required';
//        isValid = false;
//    }
//    if (fatherName == '') {
//        document.getElementById('fathername-error').innerText = 'Father Name is required';
//        isValid = false;
//    }
//    if (bloodGroup == '') {
//        document.getElementById('bloodgroup-error').innerText = 'Blood group is required';
//        isValid = false;
//    }
//    if (status === '') {
//        document.getElementById('status-error').innerText = 'Select your status';
//        isValid = false;
//    }
//    if (gendar === '') {
//        document.getElementById('gendar-error').innerText = 'Gendar is Required';
//        isValid = false;
//    }
//    if (maritalstatus === '') {
//        document.getElementById('marital-error').innerText = 'Marital status is required';
//        isValid = false;
//    }
//    if (designation === '') {
//        document.getElementById('designation-error').innerText = 'Designation is required';
//        isValid = false;
//    }
//    if (department === '') {
//        document.getElementById('department-error').innerText = 'department is required';
//        isVaild = false;
//    }
//    if (client === '') {
//        document.getElementById('client-error').innerText = 'Client Name  is required';
//        isVaild = false;
//    }
//    if (employeetype === '') {
//        document.getElementById('employeetype-error').innerText = 'employee type is required';
//        isVaild = false;
//    }
//    if (location === '') {
//        document.getElementById('location-error').innerText = 'employee location is required';
//        isVaild = false;
//    }
//    if (manager === '') {
//        document.getElementById('manager-error').innerText = 'Manager name is required';
//        isVaild = false;
//    }
//    if (leave === '') {
//        document.getElementById('leave-error').innerText = 'Leave RM is required';
//        isVaild = false;
//    }
//    if (number === '') {
//        document.getElementById('number-error').innerText = 'Phone Number is required';
//        isValid = false;
//    } else if (!number.match(/^[1-9]\d{9}$/)) {
//        document.getElementById('number-error').innerText = 'Phone Number is invalid';
//        isValid = false;
//    }
//    if (email === '') {
//        document.getElementById('email-error').innerText = 'Email is required';
//        isValid = false;
//    } else if (!email.match('[a-z0-9._%+\-]+@@[a-z0-9.\-]+\.[a-z]{2,}$')) {
//        document.getElementById('email-error').innerText = 'Invalid email format';
//        isValid = false;
//    }
//    if (offemail === '') {
//        document.getElementById('offemail-error').innerText = 'Email is required';
//        isValid = false;
//    } else if (!offemail.match('[a-z0-9._%+\-]+@@[a-z0-9.\-]+\.[a-z]{2,}$)')) {
//        document.getElementById('offemail-error').innerText = 'Invalid email format';
//        isValid = false;
//    }
//    if (present === '') {
//        document.getElementById('present-error').innerText = 'present is required';
//        isVaild = false;
//    }
//    if (dependentname === '') {
//        document.getElementById('dependentname-error').innerText = 'Dependent Name is required';
//        isVaild = false;
//    }
//    if (dependentrelation === '') {
//        document.getElementById('dependentrelation-error').innerText = 'Dependent relation is required';
//        isVaild = false;
//    }
//    if (dependentnumber === '') {
//        document.getElementById('dependentnumber-error').innerText = 'Dependent number is required';
//        isVaild = false;
//    }

//    return isValid;
//}
//function switchToNextTab() {
//    // Find the radio input for the next tab and set it to checked
//    document.querySelector('.mytabs input[name="mytabs"][id="Bank"]').checked = true;
//}
//function validateForm2() {
//    var bankname = document.getElementById('bankname').value;
//    var accnum = document.getElementById('accnum').value;
//    var branch = document.getElementById('branch').value;
//    var typeofacc = document.getElementById('typeofacc').value;
//    var ifsc = document.getElementById('ifsc').value;

//    var isValid = true;

//    document.getElementById('bankname-error').innerText = '';
//    document.getElementById('accnum-error').innerText = '';
//    document.getElementById('branch-error').innerText = '';
//    document.getElementById('typeofacc-error').innerText = '';
//    document.getElementById('ifsc-error').innerText = '';

//    if (bankname === '') {
//        document.getElementById('bankname-error').innerText = 'Bank name is required';
//        isValid = false;
//    }
//    if (accnum === '') {
//        document.getElementById('accnum-error').innerText = 'Account number is required';
//        isValid = false;
//    }
//    if (branch === '') {
//        document.getElementById('branch-error').innerText = 'Branch name is required';
//        isValid = false;
//    }
//    if (typeofacc === '') {
//        document.getElementById('typeofacc-error').innerText = 'Type of account is required';
//        isValid = false;
//    }
//    if (ifsc === '') {
//        document.getElementById('ifsc-error').innerText = 'IFSC code is required';
//        isValid = false;
//    }
//    return isValid;
//}
//function validateForm3() {
//    var pan = document.getElementById('pan').value;
//    var aadhar = document.getElementById('aadhar').value;

//    var isValid = true;

//    document.getElementById('pan-error').innerText = '';
//    document.getElementById('aadhar-error').innerText = '';

//    if (pan === '') {
//        document.getElementById('pan-error').innerText = 'PAN number is required';
//        isValid = false;
//    }
//    if (aadhar === '') {
//        document.getElementById('aadhar-error').innerText = 'Aadhar number is required';
//        isValid = false;
//    }
//    return isValid;
//}
//function validateForm4() {
//    var uan = document.getElementById('uan').value;
//    var pf = document.getElementById('pf').value;
//    var esic = document.getElementById('esic').value;

//    var isValid = true;

//    document.getElementById('uan-error').innerText = "";
//    document.getElementById('pf-error').innerText = "";
//    document.getElementById('esic-error').innerText = "";

//    if (uan == '') {
//        document.getElementById('uan-error').innerText = 'UAN number is required';
//        isValid = false;
//    }
//    if (pf == '') {
//        document.getElementById('pf-error').innerText = 'PF number is required';
//        isValid = false;
//    }
//    if (esic == '') {
//        document.getElementById('esic-error').innerText = 'ESIC number is required';
//        isValid = false;
//    }

//    return isValid;
//}
//function validateForm5() {
//    var degree = document.getElementById('degree').value;
//    var college = document.getElementById('college').value;
//    var specialization = document.getElementById('specialization').value;
//    var yearofcompletion = document.getElementById('yearofcompletion').value;

//    var isValid = true;

//    document.getElementById('degree-error').innerText = '';
//    document.getElementById('college-error').innerText = '';
//    document.getElementById('specialization-error').innerText = '';
//    document.getElementById('yearofcompletion-error').innerText = '';

//    if (degree === '') {
//        document.getElementById('degree-error').innerText = 'Degree is required';
//        isValid = false;
//    }
//    if (college === '') {
//        document.getElementById('college-error').innerText = 'College name is required';
//        isValid = false;
//    }
//    if (specialization === '') {
//        document.getElementById('specialization-error').innerText = 'Specialization is required';
//        isValid = false;
//    }
//    if (yearofcompletion === '') {
//        document.getElementById('yearofcompletion-error').innerText = 'Year of completion is required';
//        isValid = false;
//    }
//    return isValid;
//}
//function validateForm6() {
//    var prename = document.getElementById('prename').value;
//    var title = document.getElementById('title').value;
//    var reliving = document.getElementById('reliving').value;

//    var isValid = true;

//    document.getElementById('prename-error').innerText = '';
//    document.getElementById('title-error').innerText = '';
//    document.getElementById('reliving-error').innerText = '';

//    if (prename === '') {
//        document.getElementById('prename-error').innerText = 'Previous company name is required';
//        isValid = false;
//    }

//    if (title === '') {
//        document.getElementById('title-error').innerText = 'Title is required';
//        isValid = false;
//    }

//    if (reliving === '') {
//        document.getElementById('reliving-error').innerText = 'Reason for relieving is required';
//        isValid = false;
//    }

//    return isValid;
//}
//function validateForm7() {
//}


//Emp Admin view table list

function toggleDropdown() {
    document.getElementById("myDropdown").classList.toggle("show");
}



