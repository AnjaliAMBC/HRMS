$(document).on('change', '#addpurchase', function () {
    var selectedValue = $(this).val();
    if (selectedValue === "Addmanually") {
        window.location.href = '/purchase/purchaseadd';
    } else if (selectedValue === "importuser") {
        window.location.href = '/purchase/purchaseimport';
    }
});