function HighlightAdminActiveLink(element) {
    $('#admindash-menu li').removeClass('sidebaractive');
    $('#admindash-menu li a.active').removeClass('active');
    $(element).addClass('active ');
    $(element).parent().addClass('sidebaractive');
}


//$('.admin-dashboard').click(function (event) {
//    event.preventDefault();
//    HighlightAdminActiveLink($(this));

//    $.ajax({
//        url: '/admindash/index',
//        type: 'GET',   
//        dataType: 'html',
//        success: function (response) {
//            //Clear old date and bind again
//            $(".hiddenadmindashboard").html("");
//            $('.admin-dashboard-container').html("");
//            $(".admin-emppadd-container").html("");
//            $('.admin-empmanagement-container').html("");
//            $('.admin-attendance-container').html("");
//            $('.admin-leave-container').html("");

//            $(".hiddenadmindashboard").html(response);
//            var formContent = $(".hiddenadmindashboard").find(".admin-dashboard-view").html();
//            $(".admin-dashboard-container").html(formContent);

//            $('.admin-emppadd-container').hide();
//            $('.admin-empmanagement-container').hide();
//            $('.admin-dashboard-container').show();
//            $('.admin-attendance-container').hide();
//            $('.admin-leave-container').hide();
//            $('.admin-ticketing-container').hide();
//            $(".hiddenadmindashboard").html("");

//        },
//        error: function (xhr, status, error) {
//            var err = eval("(" + xhr.responseText + ")");
//        }
//    });
//});

//$(document).on('click', '.admin-empmanagement', function (event) {
//    $('#adminempmanagementtable').DataTable().destroy();
//    event.preventDefault();
//    HighlightAdminActiveLink($(this));

//    $.ajax({
//        url: '/admindashboard/employeemanagement',
//        type: 'GET',
//        dataType: 'html',
//        success: function (response) {
//            history.pushState({ url: '/admindashboard/employeemanagement' }, '', '/admindashboard/employeemanagement');
//            $(".hiddenadmindashboard").html("");
//            $('.admin-dashboard-container').html("");
//            $(".admin-emppadd-container").html("");
//            $('.admin-empmanagement-container').html("");
//            $('.admin-attendance-container').html("");
//            $('.admin-leave-container').html("");

//            $(".hiddenadmindashboard").html(response);
//            var formContent = $(".hiddenadmindashboard").find(".admin-empmanagement-view").html();
//            $(".admin-empmanagement-container").html(formContent);

//            $('.admin-empmanagement-container').show();
//            $('.admin-dashboard-container').hide();
//            $('.admin-emppadd-container').hide();
//            $('.admin-attendance-container').hide();
//            $('.admin-leave-container').hide();
//            $('.admin-ticketing-container').hide();

//            var emData = $(".hiddenadmindashboard").find(".emplsthidden").html();
//            $(".emplsthidden").html("");

//            var json = $.parseJSON(emData);
//            var data = json.map(data => {
//                console.log(data);
//                return ["",
//                    data.EmployeeID,
//                    { name: data.EmployeeName, email: data.OfficalEmailid, image: data.imagepath },
//                    data.Designation,
//                    data.Department,
//                    data.EmployeeType,
//                    data.Location,
//                    data.EmployeeStatus,
//                    "",
//                    ""];
//            });

//            $(".hiddenadmindashboard").html("");
//            var table = $('#adminempmanagementtable').DataTable({
//                data: data,
//                "paging": true,
//                "searching": true,
//                "pageLength": 8, // Set the default number of rows to display
//                "lengthChange": false,
//                "info": true,
//                "order": [],
//                "columnDefs": [
//                    {
//                        "targets": 0,
//                        "orderable": false,
//                        "render": function (data, type, full, meta) {
//                            return '<input type="checkbox" class="empmgmt-check"/>'
//                        }
//                    },
//                    {
//                        "targets": 1,
//                        "class": "tdempid"
//                    },
//                    {
//                        "targets": 2,
//                        "class": "tdempname",
//                        "render": function (data, type, full, meta) {
//                            var imageHtml = '';
//                            if (data.image) {
//                                var imageURl = "/assets/empImages/" + data.image;
//                                imageHtml = '<img src="' + imageURl + '" alt="Profile Image" style="width: 45px; height: 45px; border-radius: 50%; margin-right: 10px;">';
//                            } else {
//                                var firstNameInitial = data.name.charAt(0);
//                                var lastNameInitial = data.name.split(' ').slice(-1)[0].charAt(0);
//                                var shortName = firstNameInitial + lastNameInitial;
//                                imageHtml = '<div class="list-image" style="width: 45px; height: 45px; border-radius: 50%; margin-right: 10px; background-color: #C4C4C4; color: white; display: flex; justify-content: center; align-items: center;">' + shortName + '</div>';
//                            }
//                            return '<div style="display: flex; align-items: center;">' +
//                                imageHtml +
//                                '<span style="margin-top: 0px; margin-right: 10px;">' + data.name + '<br><span style="color: #3E78CF;">' + data.email + '</span></span>' +
//                                '</div>';
//                        }


//                    },

//                    {
//                        "targets": 3,
//                    },
//                    {
//                        "targets": 4,
//                    },
//                    {
//                        "targets": 5,
//                    },
//                    {
//                        "targets": 6,
//                    },
//                    {
//                        "targets": 7,
//                    },
//                    {
//                        "targets": 8,
//                        "orderable": false,
//                        "render": function (data, type, full, meta) {
//                            return '<span class="edit-btn" title="Edit"><i class="fas fa-pencil-alt"></i></span><span class="delete-btn" data-toggle="modal" title="Delete"><i class="fas fa-trash-alt"></i></span>';
//                        }
//                    },
//                    {
//                        "targets": 9,
//                        "orderable": false,
//                    },

//                ],

//            });

//            $('#adminempmanagementtable thead th:first-child').html('<input type="checkbox" id="selectAll">');

//            $('#selectAll').on('change', function () {
//                var isChecked = $(this).prop('checked');
//                if (isChecked) {
//                    $('td input[type="checkbox"]').prop('checked', isChecked);
//                    $('#action').show();
//                }
//                else {
//                    $('td input[type="checkbox"]').prop('checked', false);
//                    $('#action').hide();
//                }
//            });

//            $('#menuIcon').on('click', function () {
//                // Your menu icon click handler code here
//            });

//            $('#dropdownMenu').on('click', function () {
//                // Your dropdown menu click handler code here
//            });

//            $('#dropdownMenuContent input[type="checkbox"]').change(function () {
//                var columnIdx = $(this).closest('a').index() + 1;
//                var isChecked = $(this).prop('checked');
//                table.column(columnIdx).visible(isChecked);
//            });

//            $('#dropdownMenuContent input[type="checkbox"]').prop('checked', true);

//            $(document).on('change', '.empmgmt-check', function () {
//                var isChecked = $(this).prop('checked');
//                if (isChecked) {
//                    $('#action').show();
//                }
//                else {
//                    $(this).prop('checked', false);
//                    if ($(".empmgmt-check:checked").length <= 0) {
//                        $('#action').hide();
//                        $('#selectAll').prop('checked', false);
//                    }
//                }
//            });

//            $(document).on('change', '#department-dropdown', function () {
//                var value = $(this).val();
//                var columnIndex = 4;
//                if (value === 'All') {
//                    table.column(columnIndex).search('').draw(); // Clear search and redraw table
//                } else {
//                    table.column(columnIndex).search(value).draw();
//                }
//            });

//            $(document).on('change', '#Location-dropdown', function () {
//                var value = $(this).val();
//                var columnIndex = 6;
//                if (value === 'All') {
//                    table.column(columnIndex).search('').draw(); // Clear search and redraw table
//                } else {
//                    table.column(columnIndex).search(value).draw();
//                }
//            });

//            $(document).on('change', '#status-dropdown', function () {
//                var value = $(this).val();
//                var columnIndex = 7;
//                if (value === 'All') {
//                    table.column(columnIndex).search('').draw(); // Clear search and redraw table
//                } else {
//                    table.column(columnIndex).search(value).draw();
//                }
//            });

//            $(document).on('click', '.clearfilter', function () {
//                $('#department-dropdown').val($('#department-dropdown option:first').val());
//                $('#Location-dropdown').val($('#Location-dropdown option:first').val());
//                $('#status-dropdown').val($('#status-dropdown option:first').val());
//                table.search('').columns().search('').draw();
//            });

//            $(document).on('keyup', '.advancesearch', function () {
//                table.search(this.value).draw(); // Search value across all columns and redraw table
//            });

//            $(document).on('click', '.delete-btn', function () {
//                $('.delete-employee').removeAttr('disabled');
//                $('.delete-message').html("");
//                $('#deleteConfirmationModal').modal("show");
//                var $row = $(this).closest("tr");
//                var deleteEmpID = $row.find(".tdempid").text();
//                var deleteEmpName = $row.find(".tdempname").text();
//                $('.delete-message').css("color", "black");
//                $('.delete-message').html('Are you sure you want to delete <span class="deleteempnameval" style="font-weight: bold; font-style: italic; color:red"></span> ?');
//                $('.deleteempid').html(deleteEmpID.trim());
//                $('.deleteempnameval').text(deleteEmpID.trim());
//            });

//            $(document).on('click', '.delete-employee', function (event) {
//                event.preventDefault();

//                $('.delete-employee').prop('disabled', true);
//                var deleteEmpID = $('.deleteempid').html();
//                var deleteEmpName = $('.deleteempnameval').text();

//                var deletEmpInfo = {
//                    empID: deleteEmpID,
//                    empName: deleteEmpName
//                }

//                $.ajax({
//                    url: '/admindashboard/deleteemp',
//                    type: 'POST',
//                    data: deletEmpInfo,
//                    dataType: "json",
//                    success: function (result) {
//                        if (result.JsonResponse.StatusCode == 200) {
//                            $('.delete-message').html("You have deleted employee <span style='color: red;'>" + deleteEmpID + " </span> successfully ");
//                            $('.delete-message').css("color", "blue");
//                            $('.delete-employee').prop('disabled', true);
//                        } else {
//                            $('.delete-message').html(result.JsonResponse.Message);
//                            $('.delete-message').css("color", "red");
//                            $('.delete-employee').prop('disabled', false);
//                        }
//                    },
//                    error: function (xhr, status, error) {
//                        console.error("Error deleting employee:", error);
//                    }
//                });
//            });

//            $(document).on('click', '.refresh-emptablist', function () {
//                $('#deleteConfirmationModal').modal("hide");
//                $('#importSuccessModal').modal("hide");
//                $('#EmpsuccessModal').modal("hide");
//                $('.admin-empmanagement').click();
//                $('.modal-backdrop').remove();
//            });

//            $(document).off('click', '.edit-btn').on('click', '.edit-btn', function (event) {
//                event.preventDefault();

//                var $row = $(this).closest("tr");
//                var editEmpID = $row.find(".tdempid").text();
//                AddUpdateEmployee(editEmpID);
//                $('.addempheadline').text("Edit Employee");
//                return;
//            });

//        },
//        error: function (xhr, status, error) {
//            var err = eval("(" + xhr.responseText + ")");
//        }
//    });
//});

$(document).on('change', '#action', function () {
    if ($(this).val() == "export") {
        var selectedEmployeeIds = [];
        $(".empmgmt-check:checked").each(function () {
            var trElement = $(this).closest("tr");
            var empId = trElement.find(".tdempid").text();
            selectedEmployeeIds.push(empId);
        });

        if (selectedEmployeeIds.length === 0) {
            alert("Please select at least one employee.");
            return;
        }

        $.ajax({
            type: "POST",
            url: '/admindashboard/ExportToExcel',
            data: JSON.stringify({ selectedEmployeeIds: selectedEmployeeIds }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var byteCharacters = atob(response);
                var byteNumbers = new Array(byteCharacters.length);
                for (var i = 0; i < byteCharacters.length; i++) {
                    byteNumbers[i] = byteCharacters.charCodeAt(i);
                }
                var byteArray = new Uint8Array(byteNumbers);
                var blob = new Blob([byteArray], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });

                var downloadLink = document.createElement("a");
                var url = window.URL.createObjectURL(blob);
                downloadLink.href = url;
                downloadLink.download = "EmployeeInfo.xlsx";
                document.body.appendChild(downloadLink);
                downloadLink.click();
                window.URL.revokeObjectURL(url);
                document.body.removeChild(downloadLink);
            },
            error: function (xhr, status, error) {
                alert("Error occurred while exporting data: " + error);
            }
        });
    }
});





//Leave Tracker
$('.admin-leave').click(function (event) {
    event.preventDefault();
    //HighlightAdminActiveLink($(this));
    $.ajax({
        url: '/adminleave/index',
        type: 'GET',
        dataType: 'html',
        success: function (response) {
            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");
            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-leavecalender-view").html();
            $(".admin-leave-container").html(formContent);
            $('.admin-leave-container').show();
            $('.admin-attendance-container').hide();
            $('.admin-empmanagement-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");
        },
        error: function (xhr, status, error) {
            console.error("Error deleting employee:", error);
        }
    });
});

//My Request link
$('.admin-ticketing').click(function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));

    $(".hiddenadmindashboard").html("");
    $('.admin-dashboard-container').html("");
    $(".admin-emppadd-container").html("");
    $('.admin-empmanagement-container').html("");
    $('.admin-attendance-container').html("");
    $('.admin-leave-container').html("");

    $('.admin-ticketing-container').show();
    $('.admin-leave-container').hide();
    $('.admin-attendance-container').hide();
    $('.admin-empmanagement-container').hide();
    $('.admin-emppadd-container').hide();
    $('.admin-dashboard-container').hide();

    $(".hiddenadmindashboard").html("");
});

function AddUpdateEmployee(empID) {
    $.ajax({
        url: '/admindashboard/addEmployee',
        type: 'GET',
        data: { empid: empID },
        dataType: 'html',
        success: function (response) {
            $('.admin-empmanagement-view').html("");
            $('.admin-empmanagement-view').html(response);

        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
}

$(document).on('change', '#addemployee', function () {
    event.preventDefault();

    if ($(this).val() == "Addmanually") {
        AddUpdateEmployee("");
        $('.addempheadline').text("Add Employee");
    }
    else {
        $.ajax({
            url: '/admindashboard/importuser',
            type: 'GET',
            dataType: 'html',
            success: function (response) {
                $('.admin-empmanagement-view').html("");
                $('.admin-empmanagement-view').html(response);
            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")");
            }
        });
    }

});

var formData = new FormData();

$(document).off('change', '#file-upload-input').on('change', '#file-upload-input', function (event) {
    event.preventDefault();
    var file = this.files[0];
    formData = new FormData();
    formData.append('file', file);
});

$(document).on('click', '.btn-importuser-submit', function () {
    event.preventDefault();
    $.ajax({
        url: '/admindashboard/importusers',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        dataType: 'json',
        beforeSend: function () {
            $('.show-progress').show();
        },
        success: function (response) {
            $('#importSuccessModal').modal('show');
            $('.success-message').text(response.JsonResponse.Message);
            $('.show-progress').hide();
        },
        error: function (xhr, status, error) {
            $('.show-progress').hide();
            console.error('Error uploading file:', error);
        },
        complete: function () {
            isSubmitting = false;
            $('.show-progress').hide();
        }
    });
});

$(document).on('click', '.save-and-next', function () {
    event.preventDefault();
    var currentTab = $('.nav-link.active');
    var targetFormId = currentTab.attr('href');
    var currentForm = $(targetFormId + '-form');
    console.log("Current Form:", currentForm);

    if (currentForm[0].checkValidity() === false) {
        event.stopPropagation();
        currentForm.addClass('was-validated');
        console.log("Form is invalid");
    } else {
        var nextTab = currentTab.parent().next().find('.nav-link');
        if (nextTab.length > 0) {
            nextTab.tab('show');
            console.log("Moving to next tab");
        }
    }
});

$(document).on('click', '.addemp-submit-btn', function () {
    event.preventDefault();

    var firstFrom = $('#tab1-form');
    console.log("Current Form:", firstFrom);

    var formValidated = true;

    if (firstFrom[0].checkValidity() === false) {
        event.stopPropagation();
        firstFrom.addClass('was-validated');
        console.log("Form is invalid");
        formValidated = false;
        $("#tab1-link").addClass("active");
        // Add 'show active' classes to the tab pane
        $("#tab1").addClass("show active");

        $("#tab7-link").removeClass("active");
        // Add 'show active' classes to the tab pane
        $("#tab7").removeClass("show");
        $("#tab7").removeClass("show");
    }

    if (formValidated) {
        var formData = {};
        $('.tabs-view').each(function () {
            $(this).find('input, select, textarea').each(function () {
                var fieldName = $(this).attr('id');
                var fieldValue = $(this).val();
                formData[fieldName] = fieldValue;
            });
        });

        if ($('.isaddaction').html() == "True") {
            $.ajax({
                url: '/admindashboard/addemployee',
                type: 'POST',
                data: formData,
                success: function (response) {
                    console.log('Data saved successfully:', response);

                    if (response.JsonResponse.StatusCode == 200) {
                        $(".addemp-message").css("color", "green");
                        $(".addemp-message").text("The employee has been added successfully.");

                        var forms = $('.tabs-view');
                        clearFormDataAndSelectFirstIndex(forms);
                    }
                    else {
                        $(".addemp-message").css("color", "red");
                        $(".addemp-message").text(response.JsonResponse.Message);
                    }
                    $('#EmpsuccessModal').modal('show');
                },
                error: function (xhr, status, error) {
                    console.error('Error occurred:', error);
                }
            });
        }
        else {
            $.ajax({
                url: '/admindashboard/updateemployee',
                type: 'POST',
                data: formData,
                success: function (response) {
                    console.log('Data saved successfully:', response);

                    if (response.JsonResponse.StatusCode == 200) {
                        $(".addemp-message").css("color", "green");
                        $(".addemp-message").text("The employee has been updated successfully.");

                        var forms = $('.tabs-view');
                        clearFormDataAndSelectFirstIndex(forms);
                    }
                    else {
                        $(".addemp-message").css("color", "red");
                        $(".addemp-message").text(response.JsonResponse.Message);
                    }
                    $('#EmpsuccessModal').modal('show');
                },
                error: function (xhr, status, error) {
                    console.error('Error occurred:', error);
                }
            });
        }
    }
});

$(document).on('click', '#saveNewDesignation', function (event) {
    event.preventDefault();
    var newDesignation = $('#newDesignation').val().trim();
    var isDuplicate = false;

    // Check for duplicate designation name
    $('#Designation option').each(function () {
        if ($(this).val().toLowerCase() === newDesignation.toLowerCase()) {
            isDuplicate = true;
            return false; // break the loop
        }
    });

    if (newDesignation === '') {
        $('#designationError').text('Designation name cannot be empty.').show();
        $('#successMessage').hide();
    } else if (isDuplicate) {
        $('#designationError').text('Designation name already exists.').show();
        $('#successMessage').hide();
    } else {
        $('#designationError').hide();
        $.ajax({
            url: '/admindashboard/designationadd',
            method: 'POST',
            data: { newDesignation: newDesignation },
            success: function (response) {
                if (response.StatusCode == 200) {
                    console.log("Added Designation to DB");
                    $('#Designation').append('<option value="' + newDesignation + '">' + newDesignation + '</option>');

                    $('#newDesignation').val('');
                    $('#addOptionForm').hide();
                    $('#successMessage').text('Designation added successfully.').show();

                    $('#saveNewDesignation').hide();
                    $('.modal-footer .btn-secondary').text('Close');

                    setTimeout(function () {
                        $('#successMessage').hide();
                        $('#addOptionForm').show();
                        $('#addDesignationModel').modal('hide');
                        $('#saveNewDesignation').show();
                        $('.modal-footer .btn-secondary').text('Close');
                    }, 3000); // Hide the message after 3 seconds
                } else {
                    console.log("Error while adding Designation to DB", response);
                    $('#designationError').text('Error while adding designation to DB').show();
                    $('#successMessage').hide();
                }
            },
            error: function (xhr, status, error) {
                console.log("Error while adding Designation to DB", error);
                $('#designationError').text('Error while adding designation to DB').show();
                $('#successMessage').hide();
            }
        });
    }
});





$(document).on('click', '#saveDepartment', function (event) {
    event.preventDefault();
    var departmentName = $('#departmentName').val().trim();
    var isDuplicate = false;

        // Check for duplicate department name
    $('#Department option').each(function () {
        if ($(this).val().toLowerCase() === departmentName.toLowerCase()) {
            isDuplicate = true;
            return false; // break the loop
        }
    });

    if (departmentName === '') {
        $('#departmentError').text('Department name cannot be empty.').show();
        $('#departmentSuccessMessage').hide(); // hide success message if present
    } else if (isDuplicate) {
        $('#departmentError').text('Department name already exists.').show();
        $('#departmentSuccessMessage').hide(); // hide success message if present
    } else {
        $('#departmentError').hide();
        $.ajax({
            url: '/admindashboard/departmentadd',
            method: 'POST',
            data: { newDepartment: departmentName },
            success: function (response) {               

                if (response.StatusCode == 200) {
                    console.log("Added department to DB");
                    $('#Department').append('<option value="' + departmentName + '">' + departmentName + '</option>');

                    // Clear and hide the input field, show success message
                    $('#departmentName').val('');
                    $('#addDepartmentForm').hide();
                    $('#departmentSuccessMessage').text('Department added successfully.').show();

                    // Hide Save button and change Close button to 'Close'
                    $('#saveDepartment').hide();
                    $('.modal-footer .btn-secondary').text('Close');

                    setTimeout(function () {
                        $('#departmentSuccessMessage').hide();
                        $('#addDepartmentForm').show();
                        $('#addDepartmentModel').modal('hide');
                        $('#saveDepartment').show();
                        $('.modal-footer .btn-secondary').text('Close');
                    }, 3000); // Hide the message after 3 seconds
                } else {
                    console.log("Error while adding department to DB");
                    $('#departmentError').text('Error while adding department to DB').show();
                    $('#departmentSuccessMessage').hide(); // hide success message if present
                }
            },
            error: function (xhr, status, error) {
                console.log("Error while adding Department to DB");
                $('#departmentError').text('Error while adding department to DB').show();
                $('#departmentSuccessMessage').hide(); // hide success message if present
            }
        });
    }
});


$(document).on('click', '#saveClient', function (event) {
    event.preventDefault();
    var newClientName = $('#newClientName').val().trim();
    var isDuplicate = false;

    // Check for duplicate client name
    $('#Client option').each(function () {
        if ($(this).val().toLowerCase() === newClientName.toLowerCase()) {
            isDuplicate = true;
            return false; // break the loop
        }
    });

    if (newClientName === '') {
        $('#clientError').text('Client name cannot be empty.').show();
        $('#clientSuccessMessage').hide();
    } else if (isDuplicate) {
        $('#clientError').text('Client name already exists.').show();
        $('#clientSuccessMessage').hide();
    } else {
        $('#clientError').hide();
        $.ajax({
            url: '/admindashboard/clientadd',
            method: 'POST',
            data: { newClient: newClientName },
            success: function (response) {
                if (response.StatusCode == 200) {
                    console.log("Added client to DB");
                    $('#Client').append('<option value="' + newClientName + '">' + newClientName + '</option>');

                    // Clear and hide the input field, show success message
                    $('#newClientName').val('');
                    $('#addClientForm').hide();
                    $('#clientSuccessMessage').text('Client added successfully.').show();

                    // Hide Save button and change Close button to 'Close'
                    $('#saveClient').hide();
                    $('.modal-footer .btn-secondary').text('Close');

                    setTimeout(function () {
                        $('#clientSuccessMessage').hide();
                        $('#addClientForm').show();
                        $('#addClientModal').modal('hide');
                        $('#saveClient').show();
                        $('.modal-footer .btn-secondary').text('Close');
                    }, 3000); // Hide the message after 3 seconds
                } else {
                    console.log("Error while adding client to DB");
                    $('#clientError').text('Error while adding client to DB').show();
                    $('#clientSuccessMessage').hide();
                }
            },
            error: function (xhr, status, error) {
                console.log("Error while adding client to DB");
                $('#clientError').text('Error while adding client to DB').show();
                $('#clientSuccessMessage').hide();
            }
        });
    }
});


$(document).on('click', '#saveReportingManagerBtn', function () {
    var newReportingManager = $('#newReportingManager').val().trim();
    var isDuplicate = false;

    // Check for duplicate reporting manager name
    $('#ReportingManager option').each(function () {
        if ($(this).val().toLowerCase() === newReportingManager.toLowerCase()) {
            isDuplicate = true;
            return false; // break the loop
        }
    });

    if (newReportingManager === '') {
        $('#rmError').text('Name cannot be empty.').show();
        $('#rmSuccessMessage').hide();
    } else if (isDuplicate) {
        $('#rmError').text('Name already exists.').show();
        $('#rmSuccessMessage').hide();
    } else {
        $('#rmError').hide();
        $.ajax({
            url: '/admindashboard/rmadd',
            method: 'POST',
            data: { newRM: newReportingManager },
            success: function (response) {
                if (response.StatusCode == 200) {
                    console.log("Added RM to DB");
                    $('#ReportingManager').append('<option value="' + newReportingManager + '">' + newReportingManager + '</option>');

                    // Clear and hide the input field, show success message
                    $('#newReportingManager').val('');
                    $('#addReportingManagerForm').hide();
                    $('#rmSuccessMessage').text('Reporting Manager added successfully.').show();

                    // Hide Save button and change Close button to 'Close'
                    $('#saveReportingManagerBtn').hide();
                    $('.modal-footer .btn-secondary').text('Close');

                    setTimeout(function () {
                        $('#rmSuccessMessage').hide();
                        $('#addReportingManagerForm').show();
                        $('#addReportingManagerModal').modal('hide');
                        $('#saveReportingManagerBtn').show();
                        $('.modal-footer .btn-secondary').text('Close');
                    }, 3000); // Hide the message after 3 seconds
                } else {
                    console.log("Error while adding RM to DB");
                    $('#rmError').text('Error while adding Reporting Manager to DB').show();
                    $('#rmSuccessMessage').hide();
                }
            },
            error: function (xhr, status, error) {
                console.log("Error while adding RM to DB");
                $('#rmError').text('Error while adding Reporting Manager to DB').show();
                $('#rmSuccessMessage').hide();
            }
        });
    }
});


$(document).on('click', '#saveLeaveManagerBtn', function () {
    var newLeaveRM = $('#newLeaveManager').val().trim();
    var isDuplicate = false;

    // Check for duplicate leave manager name
    $('#LeavereportingManager option').each(function () {
        if ($(this).val().toLowerCase() === newLeaveRM.toLowerCase()) {
            isDuplicate = true;
            return false; // break the loop
        }
    });

    // Validate input and display messages
    if (newLeaveRM === '') {
        $('#lmError').text('Name cannot be empty.').show();
        $('#lmSuccessMessage').hide();
        console.log("Name is empty");
    } else if (isDuplicate) {
        $('#lmError').text('Name already exists.').show();
        $('#lmSuccessMessage').hide();
        console.log("Name already exists");
    } else {
        $('#lmError').hide();
        $.ajax({
            url: '/admindashboard/lmadd',
            method: 'POST',
            data: { newLM: newLeaveRM },
            success: function (response) {
                if (response.StatusCode == 200) {
                    console.log("Added LM to DB");
                    $('#LeavereportingManager').append('<option value="' + newLeaveRM + '">' + newLeaveRM + '</option>');

                    // Clear and hide the input field, show success message
                    $('#newLeaveManager').val('');
                    $('#addLeaveManagerForm').hide();
                    $('#lmSuccessMessage').text('Leave Manager added successfully.').show();
                    console.log("Success message set");

                    // Hide Save button and change Close button to 'Close'
                    $('#saveLeaveManagerBtn').hide();
                    $('.modal-footer .btn-secondary').text('Close');

                    setTimeout(function () {
                        $('#lmSuccessMessage').hide();
                        $('#addLeaveManagerForm').show();
                        $('#addLeaveManagerModal').modal('hide');
                        $('#saveLeaveManagerBtn').show();
                        $('.modal-footer .btn-secondary').text('Close');
                    }, 3000); // Hide the message after 3 seconds
                } else {
                    console.log("Error while adding LM to DB");
                    $('#lmError').text('Error while adding Leave Manager to DB').show();
                    $('#lmSuccessMessage').hide();
                }
            },
            error: function (xhr, status, error) {
                console.log("Error while adding LM to DB");
                $('#lmError').text('Error while adding Leave Manager to DB').show();
                $('#lmSuccessMessage').hide();
            }
        });
    }
});




function toggleDropdown() {
    document.getElementById("myDropdown").classList.toggle("show");
}

function downloadTemplate() {
    var link = document.createElement('a');
    link.href = '/assets/templates/importusertemplate.xlsx';
    link.download = 'ImportUserTemplate.xlsx';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

$(document).ready(function () {
    $(document).on('click', '#dropdownMenu', function () {
        $('#dropdownMenuContent').toggle();
    });

    $(document).on('click', function (event) {
        if (!$(event.target).closest('.dropdown-columns').length && !$(event.target).is('#dropdownMenu')) {
            $('#dropdownMenuContent').hide();
        }
    });
});

$(document).on('click', '.compOff-History-Page', function () {
    window.location.href = "/adminleave/index?fromcompoff=true";
});

//$(document).on('click', '.leave-request-Page', function () {
//    window.location.href = "/adminleave/index?fromcompoff=true";
//});
