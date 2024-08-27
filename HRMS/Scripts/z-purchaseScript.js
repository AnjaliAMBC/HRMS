$(document).on('change', '#addpurchase', function () {
    var selectedValue = $(this).val();
    if (selectedValue === "Addmanually") {
        window.location.href = '/purchase/purchaseadd';
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
