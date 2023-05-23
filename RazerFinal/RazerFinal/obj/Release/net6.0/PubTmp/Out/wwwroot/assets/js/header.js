$('.menu-open-btn').on('click',function(){
    $('.header-mobile-bottom').css('height','calc(100vh - 49px)');
    $('.menu-open-btn').addClass('d-none');
    $('.menu-close-btn').removeClass('d-none');
    console.log('opened');
})
$('.menu-close-btn').on('click',function(){
    $('.header-mobile-bottom').css('height','0');
    $('.menu-close-btn').addClass('d-none');
    $('.menu-open-btn').removeClass('d-none');

    console.log('closed');
})

$('.header-cart-btn').on('click',function () {
    $('.header-cart-dropdown').toggleClass('d-none');
})


$('.header-search-open-btn').on('click',function(){
    $('.header-navs').toggleClass('d-none');
    $('.header-search').toggleClass('d-none');
})
$('.header-search-close-btn').on('click',function(){
    $('.header-navs').toggleClass('d-none');
    $('.header-search').toggleClass('d-none');
})


$(window).resize(function () {  
    let screen = $(window).width();
    if (screen > 767) {
        $('.header-mobile-bottom').css('height','0');
        $('.menu-close-btn').addClass('d-none');
        $('.menu-open-btn').removeClass('d-none');
        $('.header-cart-dropdown').addClass('d-none');
        
    }
    $('.header-navs').removeClass('d-none');
    $('.header-search').addClass('d-none');
})

