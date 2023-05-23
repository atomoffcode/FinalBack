document.addEventListener( 'DOMContentLoaded', function () {
    var main = new Splide( '#main-carousel', {
      type      : 'fade',
      rewind    : true,
      pagination: false,
      arrows    : false,
    } );
  
  
    var thumbnails = new Splide( '#thumbnail-carousel', {
      fixedWidth  : "80px",
      fixedHeight: "80px",
      gap         : 10,
      rewind      : true,
      pagination  : false,
      isNavigation: true,
    } );
  
  
    main.sync( thumbnails );
    main.mount();
    thumbnails.mount();
  } );


  var splideg = new Splide( '.glide-product-detail', {
    // type   : 'loop',
    arrows: true,
  } );
  splideg.mount();