$(document).on('change', '#addpurchase', function () {
    var selectedValue = $(this).val();
    if (selectedValue === "Addmanually") {
        window.location.href = '/purchase/addpurchaserequest';
    } else if (selectedValue === "importuser") {
        window.location.href = '/purchase/purchaseimport';
    }
});


//function savepurchaserequest() {
//    var purchaserequest = {
//        purchaseid: $('#purchaseid').val(),
//        assettype: $('#assettype').val(),
//        requiredon: $('#requiredon').val(),
//        requestedby: $('#requestedby').val(),
//        vendorname: $('#vendorname').val(),
//        quotationprice: $('#quotationprice').val(),
//        attachfile: $('#attachfile')[0].files[0],
//        createdby: $('#createdby').val(),
//        createddate: $('#createddate').val()
//    };

//    var formdata = new formdata();

//    for (var key in purchaserequest) {
//        formdata.append(key, purchaserequest[key]);
//    }

//    $.ajax({
//        url: '/purchase/addpurchaserequest',
//        type: 'post',
//        data: formdata,
//        contenttype: false,
//        processdata: false,
//        success: function (response) {
//            if (response.statuscode === 200) {
//                // set success message and show the success modal
//                $('.purchase-success-message').text('new purchase request created successfully!');
//                $('#purchaserequestsuccessmodal').modal('show');
//            } else {
//                // set error message and show the error modal
//                $('.purchase-error-message').text('an error occurred while creating the purchase request. please try again.');
//                $('#purchaserequesterrormodal').modal('show');
//            }
//        },
//        error: function (error) {
//            console.error(error);
//            alert('an error occurred while saving the purchase request. please try again.');
//        }
//    });
//}


// Utility function to format date
function PurchaseformatDate(dateString) {
    var date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        day: '2-digit',
        month: 'short',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
    });
}


$(document).on('click', '.itprequestno', function () {
    var purchaseRequestID = $(this).data('purchaseid');

    $.ajax({
        url: '/purchase/getpurchaserequestdetails',
        type: 'GET',
        data: { id: purchaseRequestID },
        success: function (response) {
            var data = $.parseJSON(response);

            $('#purchaseStatusModalLabel').text(data.PRNumber + ' Status');


            $('#itpurchaseStatusModal .purchaseStatus-list').eq(0).find('.sender-name').text(data.RequestedBy);
            $('#itpurchaseStatusModal .purchaseStatus-list').eq(0).find('.purchaseStatus-date').text(PurchaseformatDate(data.CreatedDate));

            if (data.FinalStatus == "Approved") {

                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.approved-by').text('Approved By');
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.sender-name').text(data.ApprovedBy);
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.purchaseStatus-date').text(PurchaseformatDate(data.ApprovedDate));
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('i').removeClass('fa-circle').addClass('fa-check-circle').css('color', 'green'); // Set tick to check-circle and color to green
            } else if (data.FinalStatus == "Rejected") {

                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.approved-by').text('Rejected By');
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.sender-name').text(data.RejectedBy);
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.purchaseStatus-date').text(PurchaseformatDate(data.RejectedDate));
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('i').removeClass('fa-circle').addClass('fa-check-circle').css('color', 'red'); // Set tick to check-circle and color to red
            } else {

                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.approved-by').text('Pending');
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.sender-name').text('');
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.purchaseStatus-date').text('');
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('i').removeClass('fa-check-circle').addClass('fa-circle').css('color', 'gray'); // Set tick to circle and color to gray
            }


            if (data.PO) {
                $('#itpurchaseStatusModal .purchaseStatus-list').eq(2).find('.fa').addClass('fa-check-circle').removeClass('fa-circle');
            } else {
                $('#itpurchaseStatusModal .purchaseStatus-list').eq(2).find('.fa').addClass('fa-circle').removeClass('fa-check-circle');
            }

            if (data.TaxInvoice) {
                $('#itpurchaseStatusModal .purchaseStatus-list').eq(3).find('.fa').addClass('fa-check-circle').removeClass('fa-circle');
            } else {
                $('#itpurchaseStatusModal .purchaseStatus-list').eq(3).find('.fa').addClass('fa-circle').removeClass('fa-check-circle');
            }

            $('#itpurchaseStatusModal').modal('show');
        },
        error: function (xhr, status, error) {
            console.error('Error fetching purchase request details:', error);
        }
    });
});



function AttchemnetValidation(fileInput, validationRequired, attachwrapper, filefieldid) {

    let isValid = true;
    if ($(filefieldid).attr("data-existingfile") != undefined && $(filefieldid).attr("data-existingfile") != "") {
        return isValid;
    }

    var file = fileInput[0].files[0];
    var parentDiv = fileInput.closest('.form-group');

    $('.' + attachwrapper).css('border', '');

    if (validationRequired) {
        if (!file) {
            parentDiv.addClass('is-invalid');
            fileInput.addClass('is-invalid');
            $('.' + attachwrapper).css('border', '1px solid red');
            parentDiv.find('.invalid-feedback').text('Attach File is required.').show();
            isValid = false;
        }
    }

    if (file) {
        var allowedTypes = ['application/pdf', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];
        var maxSize = 2 * 1024 * 1024;

        if ($.inArray(file.type, allowedTypes) === -1 || file.size > maxSize) {
            parentDiv.addClass('is-invalid');
            fileInput.addClass('is-invalid');
            parentDiv.find('.invalid-feedback').text('Invalid file type or size. Please upload a PDF or DOC file less than 2MB.').show();
            $('.' + attachwrapper).css('border', '1px solid red');
            isValid = false;
        } else {
            parentDiv.removeClass('is-invalid');
            fileInput.removeClass('is-invalid');
            parentDiv.find('.invalid-feedback').hide();
            $('.' + attachwrapper).css('border', '');
        }
    }
    return isValid;
}

$('#itpurchase-selectAll').click(function () {
    $('.purchaseitadmin-check').prop('checked', this.checked);
});

function exportPurchase() {
    var selectedPurchaseRequestIds = [];

    // Gather selected purchase request IDs
    $('#adminpurchasetable').find('.purchaseitadmin-check:checked').each(function () {
        var purchaseId = $(this).closest('tr').find('.itprequestno').data('purchaseid');
        selectedPurchaseRequestIds.push(purchaseId);
    });

    if (selectedPurchaseRequestIds.length === 0) {
        alert('Please select at least one purchase request to export.');
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/purchase/ExportSelectedPurchaseRequests',
        data: JSON.stringify({ selectedPurchaseRequestIds: selectedPurchaseRequestIds }),
        contentType: 'application/json; charset=utf-8',
        xhrFields: {
            responseType: 'blob' // Handle the response as a blob
        },
        success: function (data) {
            // Create a new Blob object with the response data
            var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = 'SelectedPurchaseRequests.xlsx'; // File name for download
            link.click(); // Trigger the download
        },
        error: function (xhr, status, error) {
            console.error('Error Details:', {
                status: status,
                error: error,
                responseText: xhr.responseText
            });
            alert('An error occurred while exporting the purchase requests. Please try again.');
        }
    });
}


$('#vendor-selectAll').click(function () {
    $('.vendor-check').prop('checked', this.checked);
});

//super admin export

function exportPurchaseSuperAdmin() {
    var selectedPurchaseRequestIds = [];

    // Gather selected purchase request IDs
    $('#adminpurchasesuperadmintable').find('.purchasesuperadmin-check:checked').each(function () {
        var purchaseId = $(this).closest('tr').find('.itprequestno').data('purchaseid');
        selectedPurchaseRequestIds.push(purchaseId);
    });

    if (selectedPurchaseRequestIds.length === 0) {
        alert('Please select at least one purchase request to export.');
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/purchase/ExportSelectedPurchaseRequests',
        data: JSON.stringify({ selectedPurchaseRequestIds: selectedPurchaseRequestIds }),
        contentType: 'application/json; charset=utf-8',
        xhrFields: {
            responseType: 'blob' // Handle the response as a blob
        },
        success: function (data) {
            // Create a new Blob object with the response data
            var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = 'SelectedPurchaseRequests.xlsx'; // File name for download
            link.click(); // Trigger the download
        },
        error: function (xhr, status, error) {
            console.error('Error Details:', {
                status: status,
                error: error,
                responseText: xhr.responseText
            });
            alert('An error occurred while exporting the purchase requests. Please try again.');
        }
    });
}


$(document).ready(function () {
    $('.AssetType, .Requestedby, .VendorName').select2();


    $('#AttachFile-1').on('change', function () {
        var isValid = AttchemnetValidation($(this), true, "attach1wrapper", "#AttachFile-1");
        $(this).attr("data-existingfile", "");
        if (isValid) {
            var fileInput1 = $('#AttachFile-1');
            var file = fileInput1[0].files[0];
            if (file) {
                var fileName = file.name;
                var fileUrl = URL.createObjectURL(file);

                $('.attchedfile-1').attr('href', fileUrl).text(fileName).show();
            } else {
                $('.attchedfile-1').hide();
            }
            console.log("Attach1 File validation passed.");
        } else {
            console.log("Attach1 validation failed.");
        }
    });

    $('#AttachFile-2').on('change', function () {
        var isValid = AttchemnetValidation($(this), true, "attach2wrapper", "#AttachFile-2");
        $(this).attr("data-existingfile", "");
        if (isValid) {
            var fileInput2 = $('#AttachFile-2');
            var file = fileInput2[0].files[0];
            if (file) {
                var fileName = file.name;
                var fileUrl = URL.createObjectURL(file);

                $('.attchedfile-2').attr('href', fileUrl).text(fileName).show();
            } else {
                $('.attchedfile-2').hide();
            }
            console.log("Attach2 File validation passed.");
        } else {
            console.log("Attach2 validation failed.");
        }
    });

    $('#AttachFile-3').on('change', function () {
        var isValid = AttchemnetValidation($(this), true, "attach3wrapper", "#AttachFile-3");
        $(this).attr("data-existingfile", "");
        if (isValid) {
            var fileInput3 = $('#AttachFile-3');
            var file = fileInput3[0].files[0];
            if (file) {
                var fileName = file.name;
                var fileUrl = URL.createObjectURL(file);

                $('.attchedfile-3').attr('href', fileUrl).text(fileName).show();
            } else {
                $('.attchedfile-3').hide();
            }
            console.log("Attach3 File validation passed.");
        } else {
            console.log("Attach3 validation failed.");
        }
    });


    $('.vendoradd-Submit').on('click', function (event) {
        event.preventDefault();
        let isValid = true;
        var formData = new FormData();

        if ($('#PurchaseID').val().trim() === '') {
            $('#PurchaseID').addClass('is-invalid');
            isValid = false;
        } else {
            $('#PurchaseID').removeClass('is-invalid');
        }

        if ($('#AssetType').val() === null || $('#AssetType').val() === "") {
            $('#AssetType').next('.select2-container').addClass('is-invalid');
            $('#AssetType').next('.select2-container').css('border', '1px solid red');

        } else {
            $('#AssetType').next('.select2-container').removeClass('is-invalid');
            $('#AssetType').next('.select2-container').css('border', '');
        }

        if ($('#RequiredOn').val().trim() === '') {
            $('#RequiredOn').addClass('is-invalid');
            isValid = false;
        } else {
            $('#RequiredOn').removeClass('is-invalid');
        }

        if ($('#Requestedby').val() === null || $('#Requestedby').val() === "") {
            $('#Requestedby').next('.select2-container').addClass('is-invalid');
            $('#Requestedby').next('.select2-container').css('border', '1px solid red');

        } else {
            $('#Requestedby').next('.select2-container').removeClass('is-invalid');
            $('#Requestedby').next('.select2-container').css('border', '');
        }

        if ($('#VendorName-1').val() === null || $('#VendorName-1').val() === "") {
            $('#VendorName-1').next('.select2-container').addClass('is-invalid');
            $('#VendorName-1').next('.select2-container').css('border', '1px solid red');

        } else {
            $('#VendorName-1').next('.select2-container').removeClass('is-invalid');
            $('#VendorName-1').next('.select2-container').css('border', '');
        }

        if ($('#QuotationPrice-1').val().trim() == '0') {
            $('#QuotationPrice-1').addClass('is-invalid');
            isValid = false;
        } else {
            $('#QuotationPrice-1').removeClass('is-invalid');
        }

        var fileInput1 = $('#AttachFile-1');
        var AttchmentValidated = AttchemnetValidation(fileInput1, true, "attach1wrapper", "#AttachFile-1");

        if (AttchmentValidated == false) {
            isValid = false;
        }

        // If the form is valid, submit the form via AJAX
        if (isValid) {
            formData.append('PRNumber', $('#PurchaseID').val().trim());
            formData.append('AssetType', $('#AssetType').val().trim());
            formData.append('RequiredOn', $('#RequiredOn').val().trim());
            formData.append('Requestedby', $('#Requestedby').val().trim());

            formData.append('Vendorname1', $('#select2-VendorName-1-container').text());
            formData.append('Vendor1quotation', $('#QuotationPrice-1').val().trim());
            var fileInput1 = $('#AttachFile-1');
            var file1 = fileInput1[0].files[0];
            formData.append('AttachFile-1', file1);

            formData.append('Vendorname2', $('#select2-VendorName-2-container').text());
            formData.append('Vendor2quotation', $('#QuotationPrice-2').val().trim());
            var fileInput2 = $('#AttachFile-2');
            var file2 = fileInput2[0].files[0];
            formData.append('AttachFile-2', file2);

            formData.append('Vendorname3', $('#select2-VendorName-3-container').text());
            formData.append('Vendor3quotation', $('#QuotationPrice-3').val().trim());
            var fileInput3 = $('#AttachFile-3');
            var file3 = fileInput3[0].files[0];
            formData.append('AttachFile-3', file3);

            $.ajax({
                url: '/purchase/addpurchaserequest',
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.StatusCode == 200) {                        
                        $('#successMessage').text('Purchase Request created successfully.');
                        $('#successModal').modal('show'); 

                    } else {
                        // Show error message
                        $('#errorMessage').text('An error occurred while creating a purchase request.');
                        $('#errorModal').modal('show'); // Show error modal
                    }
                },
                error: function (xhr, status, error) {
                    // Show error message
                    $('#errorMessage').text('An error occurred while creating a purchase request.');
                    $('#errorModal').modal('show'); // Show error modal
                }
            });

        }
    });

    // Reset form on cancel
    $('.vendoradd-Cancel').on('click', function () {
        $('#purchase-form')[0].reset();
        $('.form-control').removeClass('is-invalid');
    });
});


$('.btn-addpurcase-close').on('click', function (event) {
    window.location.href = "/purchase/index";
});

