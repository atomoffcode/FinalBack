$(document).on('click','.address-item-adder',function () {
    $('.address-all').addClass('d-none');
    $('.address-add').removeClass('d-none');
})
$(document).on('click','.address-add-close-btn',function () {
    $('.address-add').addClass('d-none');
    $('.address-all').removeClass('d-none');
})
$(document).on('click','.order-items-btn',function () {
    $(this).parents('tbody').parents('table').siblings(".order-detail").toggleClass('d-none');
})