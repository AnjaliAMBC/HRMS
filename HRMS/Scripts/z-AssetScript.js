$(document).ready(function () {
    $('#addassetinfo').change(function () {
        if ($(this).val() === 'Addmanually') {
            window.location.href = '/Asset/AddAsset';
        }
    });
});