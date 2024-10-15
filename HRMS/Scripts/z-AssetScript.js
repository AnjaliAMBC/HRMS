
$(document).on('change', '#addassetinfo', function () {
    var selectedValue = $(this).val();
    if (selectedValue === "Addmanually") {
        window.location.href = '/Asset/AddAsset';
    } else if (selectedValue === "BulkAsset") {
        window.location.href = '/Asset/AssetBulkImport';
    }
});



$(document).on('click', '.asset-info-view', function () {
    var sno = $(this).attr("data-assetid");
    window.location.href = '/asset/assetview?sno=' + sno;
    return;
});


$(document).on('click', '.Asset-id-select', function () {
    var sno = $(this).attr("data-assetid");
    window.location.href = '/asset/assetview?sno=' + sno;
    return;
});

$(document).on('click', '.asset-info-edit', function (event) {
    event.preventDefault();
    var sno = $(this).attr("data-assetid");
    window.location.href = '/asset/addasset?sno=' + sno;
});

function FormattedTransferDate(dateString) {
    var date = new Date(dateString);
    var year = date.getFullYear();
    var month = ('0' + (date.getMonth() + 1)).slice(-2);
    var day = ('0' + date.getDate()).slice(-2);
    var formattedDate = year + '-' + month + '-' + day;
    return formattedDate;
}


$(document).on('click', '.asset-info-transfer', function (event) {
    event.preventDefault();


    // Clear the previous values in the modal fields
    $('.isassettransferred').text("");
    $('#AssetTransfer-Location').val('').prop('selected', true);
    $('#AssetTransfer-Employee').val('').prop('selected', true);
    $('#AssetTransfer-AssignedBy').val('').prop('selected', true);
    $('#AssetTransfer-TransferDate').val('');

    var sno = $(this).attr("data-assetid");
    $('#assettransfer-popup').modal('show');
    $.ajax({
        url: '/asset/assettransfer',
        type: 'GET',
        data: { sno: sno },
        success: function (response) {
            data = $.parseJSON(response);

            $('.asset-transer-asseid').text(data.EditAssets.AssetID);
            if (data.AllocatedEmpInfo != null) {
                $('.asseettrasfer-assign-name').text(data.AllocatedEmpInfo.EmployeeName);
                $('.asseettrasfer-assign-designation').text(data.AllocatedEmpInfo.Designation);
                $('.asseet-transfer-image').attr("src", "/assets/empimages/" + data.AllocatedEmpInfo.EmployeeID + ".jpeg")
            }
            else {
                $('.asseettrasfer-assign-name').text("NA");
                $('.asseettrasfer-assign-designation').text("NA");
                $('.asseet-transfer-image').attr("src", "/assets/asset-icon-3.png")
            }
            $('.asseettrasfer-assign-location').text(data.EditAssets.Location);
            $('.asseettrasfer-assign-olddate').text(FormattedTransferDate(data.EditAssets.AssignedDate));
            $('.asseettrasfer-assign-oldassignby').text(data.EditAssets.AssignedBy);
            $('.asset-transer-assesno').text(data.EditAssets.SNo);

          
        },
        error: function (xhr, status, error) {
            alert("An error occurred: " + error);
        }
    });
});


$(document).on('click', '.assettransfer-purchaseinfo-Cancel', function (event) {
    event.preventDefault();
    if ($('.isassettransferred').text() == "yes") {
        window.location.href = "/asset/assetinfo";
        return;
    }

    $('#assettransfer-popup').modal('hide');
});


$(document).on('click', '.assettransfer-Transfer', function (event) {
    $('.isassettransferred').text("");
    event.preventDefault();
    var recordsno = $('.asset-transer-assesno').text();

    $('.form-control').removeClass('is-invalid');
    var isValid = true;

    var locationName = $('#AssetTransfer-Location').val();
    if (!locationName) {
        $('#AssetTransfer-Location').addClass('is-invalid');
        isValid = false;
    }
    else {
        $('#AssetTransfer-Location').removeClass('is-invalid');
    }

    var employeeId = $('#AssetTransfer-Employee').val();
    var employeeName = $('#AssetTransfer-Employee option:selected').text();
    if (!employeeId) {
        $('#AssetTransfer-Employee').addClass('is-invalid');
        isValid = false;
    }
    else {
        $('#AssetTransfer-Employee').removeClass('is-invalid');
    }

    var assignedById = $('#AssetTransfer-AssignedBy').val();
    var assignedByName = $('#AssetTransfer-AssignedBy option:selected').text();
    if (!assignedById) {
        $('#AssetTransfer-AssignedBy').addClass('is-invalid');
        isValid = false;
    }
    else {
        $('#AssetTransfer-AssignedBy').removeClass('is-invalid');
    }

    var transferDate = $('#AssetTransfer-TransferDate').val();
    if (!transferDate) {
        $('#AssetTransfer-TransferDate').addClass('is-invalid');
        isValid = false;
    } else {
        $('#AssetTransfer-TransferDate').removeClass('is-invalid');
    }

    if (isValid) {
        var assetTransferPostModel = {
            allocatedempid: employeeId,
            allocatedempname: employeeName,
            assignedbyid: assignedById,
            assignedbyname: assignedByName,
            transferdate: transferDate,
            sno: recordsno,
            location: $('#AssetTransfer-Location').val()
        };

        $.ajax({
            url: '/asset/assettransfersubmit',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(assetTransferPostModel),
            success: function (response) {
                if (response.StatusCode == 200) {
                    $('.isassettransferred').text("yes");
                    $('#assettransfer-message').html(`<div class="alert alert-success" style="color: green;">Asset transfer successful!</div>`);
                } else {
                    $('.isassettransferred').text("");
                    $('#assettransfer-message').html(`<div class="alert alert-danger" style="color: red;">Asset transfer failed! Please try again.</div>`);
                }
            },
            error: function (xhr, status, error) {
                $('.isassettransferred').text("");
                $('#assettransfer-message').html(`<div class="alert alert-danger" style="color: red;">An error occurred: ${error}</div>`);
            }
        });
    }
    else {
        $('#assettransfer-message').html(`<div class="alert alert-danger" style="color: red;">Please fill all required fields.</div>`);
    }
});


$('#AssetTransfer-Location').on('change', function () {
    var selectedLocation = $(this).val();

    if (selectedLocation) {
        $.ajax({
            url: '/asset/getemployeesbylocation', 
            type: 'GET',
            data: { Location: selectedLocation },
            success: function (employees) {
                var employeeDropdown = $('#AssetTransfer-Employee');
                employeeDropdown.empty(); // Clear existing options
                employeeDropdown.append('<option value="">Select Employee</option>'); 

                // Populate dropdown with filtered employees
                employees.forEach(function (employee) {
                    employeeDropdown.append(`<option value="${employee.EmployeeID}">${employee.EmployeeName}</option>`);
                });
            },
            error: function (xhr, status, error) {
                $('#assettransfer-message').html(`<div class="alert alert-danger" style="color: red;">An error occurred while loading employees: ${error}</div>`);
            }
        });
    } else {
        $('#AssetTransfer-Employee').empty().append('<option value="">Select Employee</option>'); 
    }
});


$(document).on('click', '.asset-info-delete', function () {
    var assetId = $(this).attr('data-assetid');
    var assetName = $(this).attr('data-assetname');
    $('#isAssetDeleted').text("");

    $('#assetNameToDelete').text(assetName);
    $('#deleteAssetModal').modal('show');

    $('#confirmDeleteAsset').off('click').on('click', function () {
        $.ajax({
            url: '/asset/deleteasset',
            type: 'POST',
            data: { sno: assetId },
            success: function (response) {
                if (response.StatusCode === 200) {
                    $('#deleteMessage').removeClass('alert-danger').addClass('alert-success');
                    $('#deleteMessage').html('Asset deleted successfully!').show();
                    $('#isAssetDeleted').text("yes");
                } else {
                    $('#deleteMessage').removeClass('alert-success').addClass('alert-danger');
                    $('#deleteMessage').html('Failed to delete asset! Please try again.').show();
                    $('#isAssetDeleted').text("");
                }
            },
            error: function (xhr, status, error) {
                $('#deleteMessage').removeClass('alert-success').addClass('alert-danger');
                $('#deleteMessage').html('An error occurred: ' + error).show();
                $('#isAssetDeleted').text("");
            }
        });
    });
});

$(document).on('click', '.asset-delete-refresh', function (event) {
    event.preventDefault();
    if ($('#isAssetDeleted').text() == "yes") {
        window.location.href = "/asset/assetinfo";
        return;
    }

    $('#deleteAssetModal').modal('hide');
});



$('.btn-asset-import-submit').click(function () {
    const fileInput = document.getElementById('asset-file-upload-input');
    const file = fileInput.files[0];

    if (!file) {
        alert("Please select a file before submitting.");
        return;
    }

    const formData = new FormData();
    formData.append("file", file);

    $.ajax({
        url: '/asset/importassets',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.StatusCode == 200) {
                $('#assetImportSuccessModal').modal('show');
                $('.asset-success-message').text(response.Message);
                $('.asset-success-message').css('color', 'green');
            } else {
                $('#assetImportSuccessModal').modal('show');
                $('.asset-success-message').text(response.Message);
                $('.asset-success-message').css('color', 'red');
            }
        },
        error: function (xhr, status, error) {
            $('#assetImportSuccessModal').modal('show');
            $('.asset-success-message').text(response.Message);
            $('.asset-success-message').css('color', 'red');
        }
    });
});


$(document).on("click", ".btn-asset-refreshpage", function (event) {
    event.preventDefault();
    window.location.href = "/asset/assetinfo";
    return;
});


function handleAssetFileUpload(input) {
    const file = input.files[0];
    if (file) {
        document.getElementById('asset-uploaded-file-info').style.display = 'block';
        document.getElementById('asset-uploaded-file-text').innerText = 'Selected file: ' + file.name;
    }
}



function downloadAssetTemplate() {
    var link = document.createElement('a');
    link.href = '/assets/templates/Sample Assets Import.xlsx';
    link.download = 'AssetImportTemplate.xlsx';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}


