document.addEventListener("DOMContentLoaded", function () {

    const toggleBtn = document.getElementById("toggleCommercialBtn");
    const formContainer = document.getElementById("commercialFormContainer");
    const form = document.getElementById("commercialForm");

    if (toggleBtn) {
        toggleBtn.addEventListener("click", function () {
            formContainer.style.display =
                formContainer.style.display === "none" ? "block" : "none";
        });
    }

    if (form) {
        form.addEventListener("submit", function (e) {
            e.preventDefault();

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
                        } catch {}
                    }

                    if (!response.ok) {
                        throw new Error(data.message || `Ошибка ${response.status}`);
                    }

                    return data;
                })
                .then(() => {
                    alert("Реклама активирована");
                    location.reload();
                })
                .catch(err => {
                    alert(err.message);
                });

        });
    }
    const deactivateBtn = document.getElementById("deactivateCommercialBtn");

    if (deactivateBtn) {
        deactivateBtn.addEventListener("click", () => {
            if (!confirm("Вы уверены, что хотите отключить рекламу?")) return;

            const adId = deactivateBtn.dataset.id;

            const formData = new FormData();
            formData.append("advertisementId", adId);

            fetch("/Advertisement/DeactivateCommercial", {
                method: "POST",
                body: formData
            })
                .then(async r => {
                    const text = await r.text();
                    let data = {};
                    if (text) {
                        try { data = JSON.parse(text); } catch {}
                    }

                    if (!r.ok) {
                        throw new Error(data.message || "Ошибка при отключении рекламы");
                    }

                    location.reload();
                })
                .catch(err => alert(err.message));
        });
    }


    document.querySelectorAll("[data-days]").forEach(btn => {
        btn.addEventListener("click", () => {
            const input = document.querySelector("input[name='Days']");
            input.value = btn.dataset.days;
        });
    });




});

