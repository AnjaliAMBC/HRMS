$(document).on('change', '#addpurchase', function () {
    var selectedValue = $(this).val();
    if (selectedValue === "Addmanually") {
        window.location.href = '/purchase/addpurchaserequest';
    } else if (selectedValue === "importuser") {
        window.location.href = '/purchase/purchaseimport';
    }
});


function savePurchaseRequest() {
    var purchaseRequest = {
        PurchaseID: $('#PurchaseID').val(),
        AssetType: $('#AssetType').val(),
        RequiredOn: $('#RequiredOn').val(),
        RequestedBy: $('#Requestedby').val(),
        VendorName: $('#VendorName').val(),
        QuotationPrice: $('#QuotationPrice').val(),
        AttachFile: $('#AttachFile')[0].files[0],
        CreatedBy: $('#CreatedBy').val(),
        CreatedDate: $('#CreatedDate').val()
    };

    var formData = new FormData();

    for (var key in purchaseRequest) {
        formData.append(key, purchaseRequest[key]);
    }

    $.ajax({
        url: '/purchase/AddPurchaseRequest',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.StatusCode == 200) {
                console.log(response.data);
                $('.purchase-success-message').text(response.Message);
                $('#purchaseSuccessModal').modal('show');
            } else {
                $('.purchase-success-message').text(response.Message);
                $('#purchaseSuccessModal').modal('show');
            }
        },
        error: function (error) {
            console.error(error);
            alert('An error occurred while saving the purchase request. Please try again.');
        }
    });
}


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

