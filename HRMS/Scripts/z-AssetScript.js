
$(document).on('change', '#addassetinfo', function () {
    var selectedValue = $(this).val();
    if (selectedValue === "Addmanually") {
        window.location.href = '/Asset/AddAsset';
    } else if (selectedValue === "BulkAsset") {
        window.location.href = '/Asset/AssetBulkImport';
    }
});
