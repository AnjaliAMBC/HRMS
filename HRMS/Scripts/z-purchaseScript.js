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
    var purchaseRequestID = $(this).data('purchaseid'); // Get the PurchaseRequestID

    // AJAX request to fetch details from the server
    $.ajax({
        url: '/purchase/getpurchaserequestdetails', // Adjust the URL to your controller and action
        type: 'GET',
        data: { id: purchaseRequestID },
        success: function (response) {
            // Populate the modal with the fetched data
            var data = $.parseJSON(response);
            // Set the modal title with the PR number
            $('#purchaseStatusModalLabel').text(data.PRNumber + ' Status');

            // Display the requested by name and created date
            $('#itpurchaseStatusModal .purchaseStatus-list').eq(0).find('.sender-name').text(data.RequestedBy);
            $('#itpurchaseStatusModal .purchaseStatus-list').eq(0).find('.purchaseStatus-date').text(PurchaseformatDate(data.CreatedDate));

            if (data.ApprovedBy) {
                // Display approved status
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.approved-by').text('Approved By');
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.sender-name').text(data.ApprovedBy);
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.purchaseStatus-date').text(PurchaseformatDate(data.ApprovedDate));
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('i').removeClass('fa-circle').addClass('fa-check-circle').css('color', 'green'); // Set tick to check-circle and color to green
            } else if (data.RejectedBy) {
                // Display rejected status
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.approved-by').text('Rejected By');
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.sender-name').text(data.RejectedBy);
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.purchaseStatus-date').text(PurchaseformatDate(data.RejectedDate));
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('i').removeClass('fa-circle').addClass('fa-check-circle').css('color', 'red'); // Set tick to check-circle and color to red
            } else {
                // Display default status
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.approved-by').text('Pending');
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.sender-name').text('');
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('.purchaseStatus-date').text('');
                $('#itpurchaseStatusModal .purchaseStatus-list.approveorreject').find('i').removeClass('fa-check-circle').addClass('fa-circle').css('color', 'gray'); // Set tick to circle and color to gray
            }



            // Handle purchase order (PO) status
            if (data.PO) {
                $('#itpurchaseStatusModal .purchaseStatus-list').eq(2).find('.fa').addClass('fa-check-circle').removeClass('fa-circle');
            } else {
                $('#itpurchaseStatusModal .purchaseStatus-list').eq(2).find('.fa').addClass('fa-circle').removeClass('fa-check-circle');
            }

            // Handle tax invoice status
            if (data.TaxInvoice) {
                $('#itpurchaseStatusModal .purchaseStatus-list').eq(3).find('.fa').addClass('fa-check-circle').removeClass('fa-circle');
            } else {
                $('#itpurchaseStatusModal .purchaseStatus-list').eq(3).find('.fa').addClass('fa-circle').removeClass('fa-check-circle');
            }

            // Open the modal
            $('#itpurchaseStatusModal').modal('show');
        },
        error: function (xhr, status, error) {
            console.error('Error fetching purchase request details:', error);
        }
    });
});
