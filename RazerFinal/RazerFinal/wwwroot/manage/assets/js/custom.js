﻿$(document).ready(function () {
    $(document).on('click', 'deleteImage', function (e) {
        e.preventDefault();

        let url = $(this).attr('href')

        fetch(url)
            .then(res => res.text())
            .then(data => {
                $('.itemContainer').html(data)
            })
    })


    


    $('#Spec_ProductId').change(function () {
        var productId = $(this).val();
        let url = window.location.href;
        let url2 = "/" + url.split('/')[1] + '/' + url.split('/')[2] + '/' + url.split('/')[3] + '/' + url.split('/')[4] + `/getcats?productId=${productId}`;
        console.log(url)
        console.log(url2)
        fetch(url2)
            .then(res => {
                return res.text();
            }).then(data => {
                $('#Spec_CategorySpecId').html(data);
                let catId = $('#Spec_CategorySpecId').val();
                let url3 = "/" + url.split('/')[1] + '/' + url.split('/')[2] + '/' + url.split('/')[3] + '/' + url.split('/')[4] + `/getspecs?catspecId=${catId}`;
                fetch(url3)
                    .then(res => {
                        return res.text();
                    }).then(data => {
                        $('#Spec_SpecificationId').html(data);
                    })
            })
    });
    $('#Spec_CategorySpecId').change(function () {
        var catspecId = $(this).val();
        let url = window.location.href;
        let url2 = "/" + url.split('/')[1] + '/' + url.split('/')[2] + '/' + url.split('/')[3] + '/' + url.split('/')[4] + `/getspecs?catspecId=${catspecId}`;
        console.log(url)
        console.log(url2)
        fetch(url2)
            .then(res => {
                return res.text();
            }).then(data => {
                $('#Spec_SpecificationId').html(data);
            })
    });



    $(document).on('click', '.deleteBtn', function (e) {
        e.preventDefault();
        let url = $(this).attr('href');
        console.log(url)
        Swal.fire({
            title: 'Silmek istediyine eminsen?',
            text: "Bu prosesden geri qayida bilmeyessiniz!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Beli, Sil!'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url)
                    .then(res => res.text())
                    .then(data => {
                        $('.indexContainer').html(data)
                    })


                Swal.fire(
                    'Deleted!',
                    'Your file has been deleted.',
                    'success'
                )
            }
        })
    })
})