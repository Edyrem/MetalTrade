$(function () {
    let photoIndex = 0;
    const maxPhotos = 5;
    const allowedTypes = ["image/jpeg", "image/png", "image/gif"];
    const maxSizeMB = 3;

    // Контейнер для превью изображений
    const $photoContainer = $("#photoContainer");
    // div для скрытых фото-файлов
    const $hiddenInputs = $("#hiddenPhotoFileInputs");


    

    // Кнопка "Добавить фото"
    $("#addPhotoBtn").on("click", function () {
        let alreadyAddedPhotosCount = $hiddenInputs.find("input").length;
        let slotsLeft = maxPhotos - alreadyAddedPhotosCount;

        if (slotsLeft <= 0) {
            toastr.warning(`Можно загрузить не более ${maxPhotos} фото`);
            return;
        }

        // Подсказка, сколько файлов можно выбрать
        toastr.info(`Можно выбрать до ${slotsLeft} фото`);

        // Создаём скрытый input
        const $multyInput = $('<input type="file" accept="image/*" multiple style="display:none;">');

        // Обработчик выбора файла
        $multyInput.on("change", function () {
            if (!this.files || this.files.length === 0) return;

            const files = Array.from(this.files);

            if ((alreadyAddedPhotosCount + files.length) > maxPhotos) {

                toastr.warning(`Вы выбрали слишком много файлов (макс. ${maxPhotos})`);
                return;
            }

            // Обрабатываем каждый файл
            files.forEach(file => {

                // Проверка формата
                if (!allowedTypes.includes(file.type)) {

                    toastr.warning(`Файл ${file.name} недопустимого формата`);
                    return;
                }

                // Проверка размера
                if (file.size > (maxSizeMB * 1024 * 1024)) {

                    toastr.warning(`Файл ${file.name} превышает ${maxSizeMB} МБ`);
                    return;
                }
                // Узнаём индекс и сразу увеличиваем
                const index = photoIndex++;


                // Создаём скрытый input под этот файл
                const $singleInput = $('<input type="file" name="PhotoFiles" style="display:none;">');
                $singleInput[0].files = createFileList(file);
                $singleInput.attr("data-index", index);

                $hiddenInputs.append($singleInput);

                // Создание превью
                const reader = new FileReader();
                reader.onload = (e) => {

                    const $preview = $(`
                        <div class="photo-preview" data-index="${index}">
                            <button type="button" class="remove-photo btn btn-sm btn-danger mt-1">&times;</button>
                            <img src="${e.target.result}">
                        </div>
                    `);

                    $photoContainer.append($preview);

                    toastr.success('Фото добавлено');
                };

                reader.readAsDataURL(file);
            });
        });

        // Автоматически открываем диалог выбора файла
        $multyInput[0].click();
    });

    // Обработка удаления фото
    $photoContainer.on("click", ".remove-photo", function () {
        const index = $(this).parent().data("index");


        $(this).parent().fadeOut(250, function () {
            $(this).remove(); // удаляем preview
        });

        $(`input[data-index='${index}']`).remove(); // удаляем скрытый input
    });
    function createFileList(file) {
        const dt = new DataTransfer();
        dt.items.add(file);
        return dt.files;
    }
});

    
