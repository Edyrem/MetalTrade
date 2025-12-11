$(document).on("click", ".photo-delete-btn", function () {

    let id = $(this).data("id");
    let link = $(this).data("link");
    let $photoContainer = $("#photoContainer");

    Swal.fire({
        title: "Удалить фото?",
        text: "Это действие необратимо",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Удалить",
        cancelButtonText: "Отмена",
        reverseButtons: true
    }).then((result) => {

        if (!result.isConfirmed) return;

        $.ajax({
            url: '/Advertisement/DeleteAdvertisementPhotoAjax',
            type: "POST",
            data: {
                advertisementPhotoId: id,
                photoLink: link
            },

            success: function (response) {
                if (response.success) {

                    const card = $(`.photo-card[data-photo-id="${id}"]`);
                    card.animate({ opacity: 0, height: 0, margin: 0, padding: 0 }, 400, function () {
                        $(this).remove();
                    });

                    // Декремент существующих фото
                    let existingCount = parseInt($photoContainer.data("existing-photos")) || 0;
                    existingCount = Math.max(existingCount - 1, 0);
                    $photoContainer.data("existing-photos", existingCount);

                    Swal.fire({
                        toast: true,
                        icon: 'success',
                        title: 'Фото удалено',
                        position: 'top-end',
                        showConfirmButton: false,
                        timer: 2000,
                        timerProgressBar: true
                    });

                } else {
                    Swal.fire("Ошибка", "Не удалось удалить фото", "error");
                }
            },

            error: function () {
                Swal.fire("Ошибка", "Произошёл сбой соединения", "error");
            }
        });

    });
});