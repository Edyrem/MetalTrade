$(function () {
    const LOCAL_LEN = 9;
    const FULL_PREFIX = "+996";

    $("#PhoneNumber").attr("maxlength", LOCAL_LEN);
    $("#WhatsAppNumber").attr("maxlength", LOCAL_LEN);

    let waUserEdited = false;
    $("#PhoneNumber").on("input", function () {
        let v = $(this).val().replace(/\D/g, "");
        if (v.length > LOCAL_LEN) v = v.slice(0, LOCAL_LEN);
        $(this).val(v);
        
        if (!waUserEdited) {
            $("#WhatsAppNumber").val(v);
        }
    });
    
    $("#WhatsAppNumber").on("input", function () {
        waUserEdited = true;

        let digits = this.value.replace(/\D/g, "");
        if (digits.length > LOCAL_LEN) digits = digits.slice(0, LOCAL_LEN);
        this.value = digits;
    });
    
    $("#waSyncRestore").on("click", function () {
        const phone = $("#PhoneNumber").val().replace(/\D/g, "");
        $("#WhatsAppNumber").val(phone);
        waUserEdited = false;
    });
    
    
    $("#Photo").on("change", function () {
        const file = this.files && this.files[0];
        if (!file) return;

        if (!file.type.startsWith("image/")) {
            alert("Выберите изображение");
            return;
        }

        const reader = new FileReader();
        reader.onload = (e) => {
            $("#photoPreview").attr("src", e.target.result).show();
        };
        reader.readAsDataURL(file);

        const fd = new FormData();
        fd.append("photo", file);

        $.ajax({
            url: "/Account/UploadPhoto",
            type: "POST",
            data: fd,
            contentType: false,
            processData: false,
            success: function (resp) {
                if (resp && resp.url) {
                    $("#photoPreview").attr("src", resp.url);
                    $("#PhotoUrl").val(resp.url);
                }
            },
            error: function () {
                alert("Ошибка загрузки изображения.");
            }
        });
    });
});
