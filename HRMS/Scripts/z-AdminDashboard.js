function HighlightAdminActiveLink(element) {
    $('#admindash-menu li a.active').removeClass('active');
    $(element).addClass('active');
}

//Self service link js
$('.admin-dashboard').click(function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));

    $(".hiddenadmindashboard").html("");
    //$(".admin-dashboard-container").html("");
    $('.admin-dashboard-container').show();
    $('.admin-empmanagement-container').hide();
    $('.admin-emppadd-container').hide();
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
            $(".hiddenadmindashboard").html("");
            $(".admin-empmanagement-container").html("");
            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-empmanagement-view").html();
            $(".admin-empmanagement-container").html(formContent);

            $('.admin-empmanagement-container').show();
            $('.admin-dashboard-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-attendance-container').hide();
            $('.admin-leave-container').hide();
            $('.admin-ticketing-container').hide();

            var emData = $(".hiddenadmindashboard").find(".emplsthidden").html();
            $(".emplsthidden").html("");

            var json = $.parseJSON(emData);
            var data = json.map(data => {
                console.log(data);
                return ["",
                    data.EmployeeID,
                    { name: data.EmployeeName, email: data.OfficalEmailid, image: data.Reason },
                    data.Designation,
                    data.Department,
                    data.EmployeeType,
                    data.Location,
                    data.EmployeeStatus,
                    "",
                    ""];
            });

            $(".hiddenadmindashboard").html("");
            var table = $('#adminempmanagementtable').DataTable({
                data: data,
                "paging": true,
                "searching": true,
                "info": true,
                "order": [],
                "lengthMenu": [[10, 25, 50, -1], [10, 25, 50, "All"]],
                "columnDefs": [
                    {
                        "targets": 0,
                        "render": function (data, type, full, meta) {
                            return '<input type="checkbox" class="empmgmt-check" />'
                        }
                    },
                    {
                        "targets": 1,
                        "class": "tdempid"
                    },
                    {
                        "targets": 2,
                        "class": "tdempname",
                        "render": function (data, type, full, meta) {
                            var imageHtml = '';
                            if (data.image) {
                                var imageURl = "/Assets/EmpImages/" + data.image;
                                imageHtml = '<img src="' + imageURl + '" alt="Profile Image" style="width: 50px; height: 50px; border-radius: 50%; margin-right: 10px;">';
                            } else {
                                var firstNameInitial = data.name.charAt(0);
                                var lastNameInitial = data.name.split(' ').slice(-1)[0].charAt(0);
                                var shortName = firstNameInitial + lastNameInitial;
                                return imageHtml = '<div class="default-image" style="width: 50px; height: 50px; border-radius: 50%; background-color: black; color: #fff; text-align: center; line-height: 50px; margin-right: 10px;">' + shortName + '</div><span style="display: inline-block; vertical-align: top;">' + data.name + '<br>' + data.email + '</span>'
                            }
                            return imageHtml +
                                '<span style="display: inline-block; vertical-align: top;">' + data.name + '<br>' + data.email + '</span>';
                        }
                    },

                    {
                        "targets": 3,
                    },
                    {
                        "targets": 4,
                    },
                    {
                        "targets": 5,
                    },
                    {
                        "targets": 6,
                    },
                    {
                        "targets": 7,
                    },
                    {
                        "targets": 8,
                        "render": function (data, type, full, meta) {
                            return '<span class="edit-btn" title="Edit"><i class="fas fa-pencil-alt"></i></span><span class="delete-btn" data-toggle="modal" title="Delete"><i class="fas fa-trash-alt"></i></span>';
                        }
                    }

                ],

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

            $(document).on('change', '#department-dropdown', function () {
                var value = $(this).val();
                var columnIndex = 4;
                if (value === 'All') {
                    table.column(columnIndex).search('').draw(); // Clear search and redraw table
                } else {
                    table.column(columnIndex).search(value).draw();
                }
            });

            $(document).on('change', '#Location-dropdown', function () {
                var value = $(this).val();
                var columnIndex = 6;
                if (value === 'All') {
                    table.column(columnIndex).search('').draw(); // Clear search and redraw table
                } else {
                    table.column(columnIndex).search(value).draw();
                }
            });

            $(document).on('change', '#status-dropdown', function () {
                var value = $(this).val();
                var columnIndex = 7;
                if (value === 'All') {
                    table.column(columnIndex).search('').draw(); // Clear search and redraw table
                } else {
                    table.column(columnIndex).search(value).draw();
                }
            });

            $(document).on('click', '.clearfilter', function () {
                $('#department-dropdown').val($('#department-dropdown option:first').val());
                $('#Location-dropdown').val($('#Location-dropdown option:first').val());
                $('#status-dropdown').val($('#status-dropdown option:first').val());
                table.search('').columns().search('').draw();
            });

            $(document).on('keyup', '.advancesearch', function () {
                table.search(this.value).draw(); // Search value across all columns and redraw table
            });

            $(document).on('click', '.delete-btn', function () {
                $('.delete-employee').removeAttr('disabled');
                $('.delete-message').html("");
                $('#deleteConfirmationModal').modal("show");
                var $row = $(this).closest("tr");
                var deleteEmpID = $row.find(".tdempid").text();
                var deleteEmpName = $row.find(".tdempname").text();
                $('.delete-message').css("color", "black");
                $('.delete-message').html('Are you sure you want to delete <span class="deleteempnameval" style="font-weight: bold; font-style: italic; color:red"></span> ?');
                $('.deleteempid').html(deleteEmpID.trim());
                $('.deleteempnameval').text(deleteEmpName.trim());
            });

            $(document).on('click', '.delete-employee', function (event) {
                event.preventDefault();
                event.stopPropagation();

                $('.delete-employee').prop('disabled', true);
                var deleteEmpID = $('.deleteempid').html();
                var deleteEmpName = $('.deleteempnameval').text();

                var deletEmpInfo = {
                    empID: deleteEmpID,
                    empName: deleteEmpName
                }

                $.ajax({
                    url: '/admindashboard/deleteemp',
                    type: 'POST',
                    data: deletEmpInfo,
                    dataType: "json",
                    success: function (result) {
                        if (result.JsonResponse.StatusCode == 200) {
                            $('.delete-message').html("You have deleted employee " + deleteEmpName + " successfully ");
                            $('.delete-message').css("color", "blue");
                            $('.delete-employee').prop('disabled', true);
                        } else {
                            $('.delete-message').html(result.JsonResponse.Message);
                            $('.delete-message').css("color", "red");
                            $('.delete-employee').prop('disabled', false);
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error("Error deleting employee:", error);
                    },
                    complete: function () {
                        $('.delete-employee').prop('disabled', true);
                    }
                });
            });

            $(document).on('click', '.refresh-emptablist', function () {
                $('.admin-empmanagement').click();
                $('#deleteConfirmationModal').modal("hide");
                $('.modal-backdrop').remove();
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
            //$(".admin-attendance-container").html("");

            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-attendancemgmt-view").html();
            $(".admin-attendance-container").html(formContent);

            $('.admin-attendance-container').show();
            $('.admin-empmanagement-container').hide();
            $('.admin-emppadd-container').hide();
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
    $('.admin-emppadd-container').hide();
    $('.admin-dashboard-container').hide();
    $('.admin-ticketing-container').hide();

    $(".hiddenadmindashboard").html("");
});

//My Request link
$('.admin-ticketing').click(function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));

    $('.admin-ticketing-container').show();
    $('.admin-leave-container').hide();
    $('.admin-attendance-container').hide();
    $('.admin-empmanagement-container').hide();
    $('.admin-emppadd-container').hide();
    $('.admin-dashboard-container').hide();

    $(".hiddenadmindashboard").html("");
});

$(document).on('change', '#addemployee', function () {
    event.preventDefault();

    if ($(this).val() == "Addmanually") {

        $.ajax({
            url: '/admindashboard/addEmployee',
            type: 'GET',
            dataType: 'html',
            success: function (response) {
                //Clear old date and bind again
                $(".hiddenadmindashboard").html("");
                $(".admin-emppadd-container").html("");
                $('.admin-empmanagement-container').html("");

                $(".hiddenadmindashboard").html(response);
                var formContent = $(".hiddenadmindashboard").find(".admin-empadd-view").html();
                $(".admin-emppadd-container").html(formContent);

                $('.admin-emppadd-container').show();
                $('.admin-empmanagement-container').hide();
                $('.admin-dashboard-container').hide();
                $('.admin-attendance-container').hide();
                $('.admin-leave-container').hide();
                $('.admin-ticketing-container').hide();
                $(".hiddenadmindashboard").html("");

            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")");
            }
        });
    }
    else {
        $.ajax({
            url: '/admindashboard/importuser',
            type: 'GET',
            dataType: 'html',
            success: function (response) {
                //Clear old date and bind again
                $(".hiddenadmindashboard").html("");
                $(".admin-emppadd-container").html("");
                $('.admin-empmanagement-container').html("");

                $(".hiddenadmindashboard").html(response);
                var formContent = $(".hiddenadmindashboard").find(".admin-empadd-view").html();
                $(".admin-emppadd-container").html(formContent);

                $('.admin-emppadd-container').show();
                $('.admin-empmanagement-container').hide();
                $('.admin-dashboard-container').hide();
                $('.admin-attendance-container').hide();
                $('.admin-leave-container').hide();
                $('.admin-ticketing-container').hide();
                $(".hiddenadmindashboard").html("");

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
        success: function (response) {
            $('#importSuccessModal').modal('show');
            $('.success-message').text(response.JsonResponse.Message);
        },
        error: function (xhr, status, error) {
            console.error('Error uploading file:', error);
        },
        complete: function () {
            isSubmitting = false;
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
    var formData = {};
    $('.tabs-view').each(function () {
        $(this).find('input, select, textarea').each(function () {
            var fieldName = $(this).attr('id');
            var fieldValue = $(this).val();
            formData[fieldName] = fieldValue;
        });
    });

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
});

$(document).on('click', '#saveNewDesignation', function () {
    var newDesignation = $('#newDesignation').val();
    if (newDesignation.trim() !== '') {
        $('#Designation').append('<option value="' + newDesignation + '">' + newDesignation + '</option>');
        $('#addDesignationModel').modal('hide');
    }
});

$(document).on('click', '#saveDepartment', function () {
    var departmentName = $('#departmentName').val();
    $('#Department').append('<option value="' + departmentName + '">' + departmentName + '</option>');
    $('#addDepartmentModel').modal('hide');
});

$(document).on('click', '#saveClient', function () {
    var newClientName = $('#newClientName').val();
    $('#Client').append('<option value="' + newClientName + '">' + newClientName + '</option>');
    $('#addClientModal').modal('hide');
});

$(document).on('click', '#saveReportingManagerBtn', function () {
    var newReportingManager = $('#newReportingManager').val();
    $('#ReportingManager').append('<option value="' + newReportingManager + '">' + newReportingManager + '</option>');
    $('#addReportingManagerModal').modal('hide');
});

$(document).on('click', '#saveLeaveRMBtn', function () {
    var newLeaveRM = $('#newLeaveRM').val();
    $('#LeavereportingManager').append('<option value="' + newLeaveRM + '">' + newLeaveRM + '</option>');
    $('#addLeaveRMModal').modal('hide');
});

function toggleDropdown() {
    document.getElementById("myDropdown").classList.toggle("show");
}

function downloadTemplate() {
    // Replace this URL with the actual URL of your sample CSV file
    var sampleFileUrl = 'C:\inetpub\wwwroot\HRMS\SampleTemplate.xlsx';
    document.getElementById('download-link').setAttribute('href', sampleFileUrl);
}

function downloadTemplate() {
    var link = document.createElement('a');
    link.href = '/assets/templates/importusertemplate.csv'; // Replace this URL with the actual URL of your sample CSV file
    link.download = 'ImportUserTemplate.csv';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}


