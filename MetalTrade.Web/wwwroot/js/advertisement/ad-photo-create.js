$(function () {
    const maxPhotos = 5;
    const allowedTypes = ["image/jpeg", "image/png", "image/gif"];
    const maxSizeMB = 3;

    const $photoList = $("#photoList");
    const $photoInput = $("#photoInput");

    $("#addPhotoBtn").on("click", function () {

        const currentCount = $photoList.find(".photo-card").length;
        const slotsLeft = maxPhotos - currentCount;

        if (slotsLeft <= 0) {
            toastr.warning(`Можно загрузить не более ${maxPhotos} фото`);
            return;
        }

        toastr.info(`Можно выбрать до ${slotsLeft} фото`);
        $photoInput.trigger("click");
    });

    $photoInput.on("change", function () {

        if (!this.files || this.files.length === 0) return;

        const files = Array.from(this.files);
        const currentCount = $photoList.find(".photo-card").length;
        const slotsLeft = maxPhotos - currentCount;

        if (files.length > slotsLeft) {
            toastr.warning(`Вы выбрали слишком много файлов (доступно ${slotsLeft})`);
            $photoInput.val("");
            return;
        }

        const validFiles = [];

        files.forEach(file => {

            if (!allowedTypes.includes(file.type)) {
                toastr.warning(`Файл ${file.name} недопустимого формата`);
                return;
            }

            if (file.size > maxSizeMB * 1024 * 1024) {
                toastr.warning(`Файл ${file.name} превышает ${maxSizeMB} МБ`);
                return;
            }

            validFiles.push(file);
        });

        if (validFiles.length === 0) {
            toastr.warning("Нет файлов, подходящих для загрузки");
            $photoInput.val("");
            return;
        }

        const formData = new FormData();
        validFiles.forEach(f => formData.append("photos", f));

        const advertisementId = $photoList.data("advertisement-id");
        formData.append("advertisementId", advertisementId);

        $.ajax({
            url: "/Advertisement/CreateAdvertisementPhotoAjax",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,

            success: function (response) {

                if (!response.success) {
                    toastr.error(response.message || "Не удалось загрузить фото");
                    return;
                }

                const $noPhotosMessage = $("#noPhotosMessage");
                if ($noPhotosMessage.length) {
                    $noPhotosMessage.remove();
                }

                response.photos.forEach(photo => {

                    const card = `
                        <div class="photo-card" data-photo-id="${photo.id}">
                            <img src="${photo.photoLink}" class="photo-card-img" />
                            <button class="photo-delete-btn"
                                    title="Удалить"
                                    data-id="${photo.id}"
                                    data-link="${photo.photoLink}">
                                <i class="fas fa-trash"></i>
                                <span class="tooltip">Удалить фото</span>
                            </button>
                        </div>
                    `;

                    $photoList.append(card);
                });

                toastr.success("Фото успешно добавлены");
                $photoInput.val("");
            },

            error: function () {
                toastr.error("Ошибка загрузки фото");
            }
        });
    });

});