function HighlightAdminActiveLink(element) {
    $('#admindash-menu li').removeClass('sidebaractive');
    $('#admindash-menu li a.active').removeClass('active');
    $(element).addClass('active ');
    $(element).parent().addClass('sidebaractive');
}


$('.admin-dashboard').click(function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));

    $.ajax({
        url: '/admindash/index',
        type: 'GET',   
        dataType: 'html',
        success: function (response) {
            //Clear old date and bind again
            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");

            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-dashboard-view").html();
            $(".admin-dashboard-container").html(formContent);

            $('.admin-emppadd-container').hide();
            $('.admin-empmanagement-container').hide();
            $('.admin-dashboard-container').show();
            $('.admin-attendance-container').hide();
            $('.admin-leave-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");

        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
});

$(document).on('click', '.admin-empmanagement', function (event) {
    $('#adminempmanagementtable').DataTable().destroy();
    event.preventDefault();
    HighlightAdminActiveLink($(this));

    $.ajax({
        url: '/admindashboard/employeemanagement',
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
                    { name: data.EmployeeName, email: data.OfficalEmailid, image: data.imagepath },
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
                "pageLength": 8, // Set the default number of rows to display
                "lengthChange": false,
                "info": true,
                "order": [],
                "columnDefs": [
                    {
                        "targets": 0,
                        "orderable": false,
                        "render": function (data, type, full, meta) {
                            return '<input type="checkbox" class="empmgmt-check"/>'
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
                                var imageURl = "/assets/empImages/" + data.image;
                                imageHtml = '<img src="' + imageURl + '" alt="Profile Image" style="width: 45px; height: 45px; border-radius: 50%; margin-right: 10px;">';
                            } else {
                                var firstNameInitial = data.name.charAt(0);
                                var lastNameInitial = data.name.split(' ').slice(-1)[0].charAt(0);
                                var shortName = firstNameInitial + lastNameInitial;
                                imageHtml = '<div class="list-image" style="width: 45px; height: 45px; border-radius: 50%; margin-right: 10px; background-color: #C4C4C4; color: white; display: flex; justify-content: center; align-items: center;">' + shortName + '</div>';
                            }
                            return '<div style="display: flex; align-items: center;">' +
                                imageHtml +
                                '<span style="margin-top: 0px; margin-right: 10px;">' + data.name + '<br><span style="color: #3E78CF;">' + data.email + '</span></span>' +
                                '</div>';
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
                        "orderable": false,
                        "render": function (data, type, full, meta) {
                            return '<span class="edit-btn" title="Edit"><i class="fas fa-pencil-alt"></i></span><span class="delete-btn" data-toggle="modal" title="Delete"><i class="fas fa-trash-alt"></i></span>';
                        }
                    },
                    {
                        "targets": 9,
                        "orderable": false,
                    },

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
                $('.deleteempnameval').text(deleteEmpID.trim());
            });

            $(document).on('click', '.delete-employee', function (event) {
                event.preventDefault();

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
                            $('.delete-message').html("You have deleted employee <span style='color: red;'>" + deleteEmpID + " </span> successfully ");
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
                    }
                });
            });

            $(document).on('click', '.refresh-emptablist', function () {
                $('#deleteConfirmationModal').modal("hide");
                $('#importSuccessModal').modal("hide");
                $('#EmpsuccessModal').modal("hide");
                $('.admin-empmanagement').click();
                $('.modal-backdrop').remove();
            });

            $(document).off('click', '.edit-btn').on('click', '.edit-btn', function (event) {
                event.preventDefault();

                var $row = $(this).closest("tr");
                var editEmpID = $row.find(".tdempid").text();
                AddUpdateEmployee(editEmpID);
                $('.addempheadline').text("Edit Employee");
                return;
            });

        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
});

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
                var downloadLink = document.createElement("a");
                downloadLink.href = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," + response;
                downloadLink.download = "EmployeeInfo.xlsx";
                document.body.appendChild(downloadLink);
                downloadLink.click();
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
    HighlightAdminActiveLink($(this));

    $(".hiddenadmindashboard").html("");
    $('.admin-dashboard-container').html("");
    $(".admin-emppadd-container").html("");
    $('.admin-empmanagement-container').html("");
    $('.admin-attendance-container').html("");
    $('.admin-leave-container').html("");

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
            //Clear old date and bind again
            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");

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
                //Clear old date and bind again
                $(".hiddenadmindashboard").html("");
                $('.admin-dashboard-container').html("");
                $(".admin-emppadd-container").html("");
                $('.admin-empmanagement-container').html("");
                $('.admin-attendance-container').html("");
                $('.admin-leave-container').html("");

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
    var newDesignation = $('#newDesignation').val();
    if (newDesignation.trim() !== '') {
        $.ajax({
            url: '/admindashboard/designationadd',
            method: 'POST',
            data: { newDesignation: newDesignation },
            success: function (response) {
                if (response.StatusCode == 200) {
                    console.log("Added Designation to DB");
                    $('#Designation').append('<option value="' + newDesignation + '">' + newDesignation + '</option>');
                    $('#addDesignationModel').modal('hide');
                } else {
                    console.log("Error while adding Designation to DB");
                }
            },
            error: function (xhr, status, error) {
                console.log("Errow while adding Destination to DB")
            }
        });
    }
});

$(document).on('click', '#saveDepartment', function (event) {
    event.preventDefault();
    var departmentName = $('#departmentName').val();
    if (departmentName.trim() !== '') {
        $.ajax({
            url: '/admindashboard/departmentadd',
            method: 'POST',
            data: { newDepartment: departmentName },
            success: function (response) {
                if (response.StatusCode == 200) {
                    console.log("Added department to DB");
                    $('#Department').append('<option value="' + departmentName + '">' + departmentName + '</option>');
                    $('#addDepartmentModel').modal('hide');
                } else {
                    console.log("Error while adding department to DB");
                }
            },
            error: function (xhr, status, error) {
                console.log("Errow while adding Department to DB")
            }
        });
    }
});

$(document).on('click', '#saveClient', function (event) {
    event.preventDefault();
    var newClientName = $('#newClientName').val();
    if (newClientName.trim() !== '') {
        $.ajax({
            url: '/admindashboard/clientadd',
            method: 'POST',
            data: { newClient: newClientName },
            success: function (response) {
                if (response.StatusCode == 200) {
                    console.log("Added client to DB");
                    $('#Client').append('<option value="' + newClientName + '">' + newClientName + '</option>');
                    $('#addClientModal').modal('hide');
                } else {
                    console.log("Error while adding client to DB");
                }
            },
            error: function (xhr, status, error) {
                console.log("Errow while adding client to DB")
            }
        });
    }
});

$(document).on('click', '#saveReportingManagerBtn', function () {
    var newReportingManager = $('#newReportingManager').val().trim();
    if (newReportingManager.trim() !== '') {
        $.ajax({
            url: '/admindashboard/rmadd',
            method: 'POST',
            data: { newRM: newReportingManager },
            success: function (response) {
                if (response.StatusCode == 200) {
                    console.log("Added RM to DB");
                    $('#ReportingManager').append('<option value="' + newReportingManager + '">' + newReportingManager + '</option>');
                    $('#addReportingManagerModal').modal('hide');
                } else {
                    console.log("Error while adding RM to DB");
                    $('#addReportingManagerModal').modal('hide');
                }
            },
            error: function (xhr, status, error) {
                console.log("Errow while adding RM to DB")
            }
        });
    }
});

$(document).on('click', '#saveLeaveManagerBtn', function () {
    var newLeaveRM = $('#newLeaveManager').val().trim();

    if (newLeaveRM.trim() !== '') {
        $.ajax({
            url: '/admindashboard/lmadd',
            method: 'POST',
            data: { newLM: newLeaveRM },
            success: function (response) {
                if (response.StatusCode == 200) {
                    console.log("Added LM to DB");
                    $('#LeavereportingManager').append('<option value="' + newLeaveRM + '">' + newLeaveRM + '</option>');
                    $('#addLeaveManagerModal').modal('hide');
                } else {
                    console.log("Error while adding LM to DB");
                    $('#addLeaveManagerModal').modal('hide');
                }
            },
            error: function (xhr, status, error) {
                console.log("Errow while adding LM to DB")
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