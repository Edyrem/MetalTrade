$(function () {
    let photoIndex = 0;
    const maxPhotos = 5;
    const allowedTypes = ["image/jpeg", "image/png", "image/gif"];
    const maxSizeMB = 4;

    // Контейнер для превью изображений
    const $photoContainer = $("#photoContainer");

    // Кнопка "Добавить фото"
    $("#addPhotoBtn").on("click", function () {
        if ($photoContainer.children(".photo-preview").length >= maxPhotos) {
            Swal.fire({
                toast: true,
                icon: 'warning',
                title: `Можно загрузить не более ${maxPhotos} фото`,
                position: 'top-end',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true
            });
            return;
        }

        // Создаём скрытый input
        const $input = $('<input type="file" name="PhotoFiles" accept="image/*" style="display:none;">');

        // Обработчик выбора файла
        $input.on("change", function () {
            if (!this.files || this.files.length === 0) return;

            const file = this.files[0];
            const reader = new FileReader();

            // Проверка типа
            if (!allowedTypes.includes(file.type)) {
                Swal.fire({
                    toast: true,
                    icon: 'warning',
                    title: `Недопустимый формат файла`,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true
                });
                $input.remove();
                return;
            }

            // Проверка размера
            if (file.size > maxSizeMB * 1024 * 1024) {
                Swal.fire({
                    toast: true,
                    icon: 'warning',
                    title: `Файл не должен превышать ${maxSizeMB} МБ`,
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true
                });
                $input.remove();
                return;
            }

            $input.attr("data-index", photoIndex++);
            $("#hiddenPhotoFileInputs").append($input);

            

            reader.onload = function (e) {
                // Создаём preview

                const $preview = $(`
                    <div class="photo-preview" data-index="${$input.data('index')}">
                        <button type="button" class="remove-photo btn btn-sm btn-danger mt-1">&times;</button>
                        <img src="${e.target.result}">
                    </div>
                `);

                $photoContainer.append($preview);

                Swal.fire({
                    toast: true,
                    icon: 'success',
                    title: 'Фото добавлено',
                    position: 'top-end',
                    showConfirmButton: false,
                    timer: 2000,
                    timerProgressBar: true
                });
            };

            reader.readAsDataURL(file);
        });

        // Автоматически открываем диалог выбора файла
        $input[0].click();
    });

    // Обработка удаления фото
    $photoContainer.on("click", ".remove-photo", function () {
        const index = $(this).parent().data("index");


        $(this).parent().fadeOut(250, function () {
            $(this).remove(); // удаляем preview
        });

        $(`input[data-index='${index}']`).remove(); // удаляем скрытый input
    });
});