$(document).ready(function () {
    $(window).on('beforeunload', function () {
        $.cookies.del('filter');
    });
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
                console.log(url)
                let url2 = "/" + url.split('/')[1] + "/mainbasket"
                console.log(url2)
                fetch(url2)
                    .then(res2 => {
                        return res2.text()
                    })
                    .then(data2 => {
                        $('.mainbasket').html(data2)
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
    $(document).on('click', '.filter-item-title', function () {
        console.log('opened');
        $(this).next().toggleClass('filter-opened');
        $(this).toggleClass('angle-twisted');
    })



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

















