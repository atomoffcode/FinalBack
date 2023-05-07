$(document).ready(function () {
    $(document).on('click', 'deleteImage', function (e) {
        e.preventDefault();

        let url = $(this).attr('href')

        fetch(url)
            .then(res => res.text())
            .then(data => {
                $('.itemContainer').html(data)
            })
    })


    


    



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