$(document).on('click','.pc-navigation-dropdown-btn',function(){
    $('.pndb-down').toggleClass('d-none');
    $('.pndb-up').toggleClass('d-none');
    $('.pc-navigation-dropdown').toggleClass('d-none');
})
$(document).on('click','.filter-item-title',function () {
    console.log('opened');
    $(this).next().toggleClass('filter-opened');
    $(this).toggleClass('angle-twisted');
})



$(document).on('click','.shop-main-filter-open-btn',function(){
    $('.shop-main-filter').css('padding','20px');
    $('.shop-main-filter').css('width','300px');
})
$(document).on('click','.shop-main-filter-close-btn',function(){
    $('.shop-main-filter').css('padding','0');
    $('.shop-main-filter').css('width','0');
})






$(window).resize(function () { 
    let screen = $(window).width();
    $('.pndb-down').removeClass('d-none');
    $('.pndb-up').addClass('d-none');
    $('.pc-navigation-dropdown').addClass('d-none');
    if (screen > 767) {
        console.log('resized');
        $('.shop-main-filter').css('padding','0');
        $('.shop-main-filter').css('width','');
    }
    
})








