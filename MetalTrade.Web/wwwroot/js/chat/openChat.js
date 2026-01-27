
// Открывает/создаёт один личный чат с пользователем
document.addEventListener("click", async e => {

    const userBtn = e.target.closest(".open-chat-user");
    if (userBtn) {
        const userId = userBtn.dataset.userId;

        const res = await fetch("/Chat/GetPrivateChatId", {
            method: "POST",
            body: new URLSearchParams({
                userId: userId
            })
        });


        if (!res.ok) return;

        const chatId = await res.json();
        window.location.href = `/Chat/Index?chatId=${chatId}`;
    }

});