document.addEventListener("DOMContentLoaded", function () {

    const toggleBtn = document.getElementById("toggleCommercialBtn");
    const formContainer = document.getElementById("commercialFormContainer");

    const commercialForm = document.getElementById("commercialForm");
    const topForm = document.getElementById("topForm");


    $(".toggleCommercialBtn").on('click', function () {
        let formCss = $(this).closest(".toggleParent").find(".commercialFormContainer");
        formCss.css("display") === "none" ? formCss.css("display", "block") : formCss.css("display", "none");
    });

    if (toggleBtn) {
        toggleBtn.addEventListener("click", function () {
            formContainer.style.display =
                formContainer.style.display === "none" ? "block" : "none";
        });
    }

    if (commercialForm) {
        commercialForm.addEventListener("submit", function (e) {
            e.preventDefault();
            formManager(commercialForm);
        });
    }

    if (topForm) {
        topForm.addEventListener("submit", function (e) {
            e.preventDefault();
            formManager(topForm);
        });
    }
    const deactivateCommercialBtn = document.getElementById("deactivateCommercialBtn");
    const deactivateTopBtn = document.getElementById("deactivateTopBtn");
    const deactivateUserTopBtn = document.getElementById("deactivateUserTopBtn");

    if (deactivateCommercialBtn) {
        deactivateCommercialBtn.addEventListener("click", () => deactivatePromotion(deactivateCommercialBtn, "/Advertisement/DeactivateCommercial"));
    }

    if (deactivateTopBtn) {
        deactivateTopBtn.addEventListener("click", () => deactivatePromotion(deactivateTopBtn, "/Advertisement/DeactivateTop"));
    }

    if (deactivateUserTopBtn) {
        deactivateUserTopBtn.addEventListener("click", () => deactivatePromotion(deactivateUserTopBtn, "/User/DeactivateTop"));
    }

    let formManager = function (form) {
        const url = form.dataset.url;
        const formData = new FormData(form);

        fetch(url, {
            method: "POST",
            body: formData
        })
        .then(async response => {
            const text = await response.text();

            let data = {};
            if (text) {
                try {
                    data = JSON.parse(text);
                } catch { }
            }

            if (!response.ok) {
                throw new Error(data.message || `Ошибка ${response.status}`);
            }

            return data;
        })
        .then(() => {
            alert("Услуга активирована");
            location.reload();
        })
        .catch(err => {
            alert(err.message);
        });
    }

    let deactivatePromotion = function (btn, promotionUrl) {
        if (!confirm("Вы уверены, что хотите отключить услугу?")) return;

        const promoId = btn.dataset.id;

        console.log(promoId);

        const formData = new FormData();
        formData.append("promotionId", promoId);
        
        fetch(promotionUrl, {
            method: "POST",
            body: formData
        })
        .then(async r => {
            const text = await r.text();
            let data = {};
            if (text) {
                try { data = JSON.parse(text); } catch { }
            }

            if (!r.ok) {
                throw new Error(data.message || "Ошибка при отключении услуги");
            }

            location.reload();
        })
        .catch(err => alert(err.message));
    }

    document.querySelectorAll("form").forEach(form => {
        form.querySelectorAll("[data-days]").forEach(btn => {
            btn.addEventListener("click", () => {
                const input = form.querySelector("input[name='Days']");
                input.value = btn.dataset.days;
            });
        });
    });
});

