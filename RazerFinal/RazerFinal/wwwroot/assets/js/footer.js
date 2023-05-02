$('.fmo-shop').on('click',function(){
    
    $('.fmo-shop').addClass('d-none');
    $('.fmc-shop').removeClass('d-none');
    $('.footer-mobile-shop').toggleClass('d-none');
    console.log('opened');
})
$('.fmc-shop').on('click',function(){
    
    $('.fmc-shop').addClass('d-none');
    $('.fmo-shop').removeClass('d-none');
    $('.footer-mobile-shop').toggleClass('d-none');
    console.log('closed');
})

$('.fmo-explore').on('click',function(){
    
    $('.fmo-explore').addClass('d-none');
    $('.fmc-explore').removeClass('d-none');
    $('.footer-mobile-explore').toggleClass('d-none');
    console.log('opened');
})
$('.fmc-explore').on('click',function(){
    
    $('.fmc-explore').addClass('d-none');
    $('.fmo-explore').removeClass('d-none');
    $('.footer-mobile-explore').toggleClass('d-none');
    console.log('closed');
})

$('.fmo-support').on('click',function(){
    
    $('.fmo-support').addClass('d-none');
    $('.fmc-support').removeClass('d-none');
    $('.footer-mobile-support').toggleClass('d-none');
    console.log('opened');
})
$('.fmc-support').on('click',function(){
    
    $('.fmc-support').addClass('d-none');
    $('.fmo-support').removeClass('d-none');
    $('.footer-mobile-support').toggleClass('d-none');
    console.log('closed');
})

$('.fmo-company').on('click',function(){
    
    $('.fmo-company').addClass('d-none');
    $('.fmc-company').removeClass('d-none');
    $('.footer-mobile-company').toggleClass('d-none');
    console.log('opened');
})
$('.fmc-company').on('click',function(){
    
    $('.fmc-company').addClass('d-none');
    $('.fmo-company').removeClass('d-none');
    $('.footer-mobile-company').toggleClass('d-none');
    console.log('closed');
})


$(window).resize(function () {  
    let screen = $(window).width();
    if (screen > 767) {
    $('.fmo-shop').removeClass('d-none');
    $('.fmc-shop').addClass('d-none');
    $('.footer-mobile-shop').addClass('d-none');

    $('.fmo-explore').removeClass('d-none');
    $('.fmc-explore').addClass('d-none');
    $('.footer-mobile-explore').addClass('d-none');
        
    $('.fmo-support').removeClass('d-none');
    $('.fmc-support').addClass('d-none');
    $('.footer-mobile-support').addClass('d-none');

    $('.fmo-company').removeClass('d-none');
    $('.fmc-company').addClass('d-none');
    $('.footer-mobile-company').addClass('d-none');

    }
})