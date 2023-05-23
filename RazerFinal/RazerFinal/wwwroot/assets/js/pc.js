$(document).ready(function () {
    $(document).on('keyup', '#prsearch', function () {
        let url = window.location.href
        console.log()
        let val = $(this).val();
        if (val.trim().length > 3) {
            fetch(`${url.split('/')[1]}/store/search?name=${val}`)
                .then(res => {
                    return res.text()
                }).then(data => {

                    $('.prdsearch').html(data)
                })
        } else {
            fetch(`${url.split('/')[1]}/store/search?name=`)
                .then(res => {
                    return res.text()
                }).then(data => {

                    $('.prdsearch').html(data)
                })
        }
    })
    $(document).on('blur', '.innerprsearch', function () {
        let url = window.location.href
        $('.prdsearch').html('');
        //console.log()
        //fetch(`${url.split('/')[1]}/product/search?name=`)
        //    .then(res => {
        //        return res.text()
        //    }).then(data => {

        //        $('.prdsearch').html(data)
        //    })

    })
    $(window).on('beforeunload', function () {
        $.removeCookie('filter');
        $.removeCookie('cat');
    });
    $(document).on('click', '.removecompare', function (e) {

        e.preventDefault();
        e.stopPropagation();
        console.log(e.currentTarget)
        let url = $(e.currentTarget).attr('href');
        fetch(url)
            .then(res => {
                return res.text();
            }).then(data => {
                $('.compareindex').html(data)
            })

    })
    $(document).on('change', '.sortingId', function (e) {
        e.preventDefault();
        e.stopPropagation();
        let val = $(this).val();
        let url2 = `/store/shop?sortId=${val}`
        console.log(url2);
        fetch(url2)
            .then(res => {
            return res.text();
            }).then(data => {
                $('html').html(data)
            })
    })
    $(document).on('change', '.addcompare', function (e) {
        e.preventDefault();
        e.stopPropagation();
        let url = window.location.href
        let val = $(e.currentTarget).attr('data-value');
        let url2 =`/compare/checkcompare?id=${val}`
        console.log(url2);
        fetch(url2)
            .then(res => {
                return res.text();
            }).then(data => {
                $('.comparecart').html(data)
            })
    })
    $(document).on('change', '.customadd', function () {
        if ($('.customadd').is(":checked")) {
            console.log("checked")
            $('.existaddresses').addClass('d-none');
            $('.customaddress').removeClass('d-none');
        } else {
            $('.existaddresses').removeClass('d-none');
            $('.customaddress').addClass('d-none');
        }
    })
    //$(document).on('click', '.aremove', function (e) {
    //    e.preventDefault();
    //    e.stopPropagation();
    //    console.log(e.currentTarget)
    //    let url = $(e.currentTarget).attr('href');
    //    fetch(url)
    //        .then(res => {
    //            return res.text();
    //        }).then(data => {
    //            $('.address-list').html(data)
    //        })

    //})
    //$('#pfpchanger').submit(function (e) {
    //    e.preventDefault(); // Prevent the default form submission

    //    var formAction = $(this).attr('action');
    //    console.log("Form Action: " + formAction);

    //    // Perform any other desired actions

    //    // You can use fetch() here to make an AJAX request if needed
    //    var formData = new FormData(this);
    //    fetch(formAction, {
    //        method: 'POST',
    //        body: formData
    //    })
    //        .then(res => {
    //            // Handle the response from the server
    //            return res.text();

    //        }).then(data => {
    //            $('.pfpchanger').html(data)
    //        }).catch(error => {
    //            // Handle any errors that occur during the fetch request
    //        });
    //});
    $(document).on('click', '.addbasket', function (e) {
        e.preventDefault();
        e.stopPropagation();
        console.log(e.currentTarget)
        let url = $(e.currentTarget).attr('href');
        fetch(url)
            .then(res => {
                return res.text();
            }).then(data => {
                
                $('.headercart').html(data)
                $('.header-cart-basket').html(data)
                console.log(url)
                let url2 = "/" + url.split('/')[1] + "/mainbasket"
                let url3 = "/" + url.split('/')[1] + "/mainbasket2"
                console.log(url2)
                console.log(url3)
                fetch(url2)
                    .then(res2 => {
                        return res2.text()
                    })
                    .then(data2 => {
                        $('.mainbasket').html(data2)
                        fetch(url3)
                            .then(res2 => {
                                return res2.text()
                            })
                            .then(data3 => {
                                $('.header-cart-count').html(data3)
                                $('.header-cart-count2').html(data3)
                            })
                    })
            })            

    })
    $(document).on('change', '.form-select', function () {
        let val = $(this).val()
        fetch(`categorysorting?catId=${val}`)
            .then(res => {
                return res.text()
            }).then(data => {
                $('#shop-main').html(data)
            })
    })
    $(document).on('change', '.form-check-input', function () {
        console.log('ch');
        let val = $(this).val();
        fetch(`filtering?specId=${val}`)
            .then(res => {
                return res.text()
            }).then(data => {
                $('.shop-list').html(data)
            })
    })

    $(document).on('click', '.pc-navigation-dropdown-btn', function () {
        $('.pndb-down').toggleClass('d-none');
        $('.pndb-up').toggleClass('d-none');
        $('.pc-navigation-dropdown').toggleClass('d-none');
    })
    //$(document).on('click', '.filter-item-title', function () {
    //    console.log('opened');
    //    $(this).next().toggleClass('filter-opened');
    //    $(this).toggleClass('angle-twisted');
    //})



    $(document).on('click', '.shop-main-filter-open-btn', function () {
        $('.shop-main-filter').css('padding', '20px');
        $('.shop-main-filter').css('width', '300px');
    })
    $(document).on('click', '.shop-main-filter-close-btn', function () {
        $('.shop-main-filter').css('padding', '0');
        $('.shop-main-filter').css('width', '0');
    })






    $(window).resize(function () {
        let screen = $(window).width();
        $('.pndb-down').removeClass('d-none');
        $('.pndb-up').addClass('d-none');
        $('.pc-navigation-dropdown').addClass('d-none');
        if (screen > 767) {
            console.log('resized');
            $('.shop-main-filter').css('padding', '0');
            $('.shop-main-filter').css('width', '');
        }

    })
})

















