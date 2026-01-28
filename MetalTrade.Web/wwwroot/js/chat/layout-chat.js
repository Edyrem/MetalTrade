(() => {
    const globalBadge = document.getElementById("chatGlobalBadge");
    if (!globalBadge) return;

    const layoutConnection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .withAutomaticReconnect()
        .build();

    layoutConnection.start()
        .then(() => layoutConnection.invoke("JoinChatList"))
        .catch(console.error);

    layoutConnection.on("ChatUpdated", (chatId, userName) => {
        if (userName === window.__layout?.currentUserName) return;

        const next = getBadgeValue() + 1;
        globalBadge.textContent = next;
        globalBadge.hidden = false;
    });



    layoutConnection.on("ChatRead", () => {
        const current = Math.max(0, getBadgeValue() - 1);

        if (current === 0) {
            globalBadge.textContent = "0";
            globalBadge.hidden = true;
        } else {
            globalBadge.textContent = current;
        }
    });


})();