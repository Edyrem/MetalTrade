const { chatId, currentUserName, otherUserId } = window.__chat;
const chatEnabled = chatId !== null;
const currentChatId = chatId;



const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();


const connectionPromise = connection.start()
    .then(() => {
        connection.invoke("JoinChatList").catch(console.error);
    })
    .catch(console.error);

connection.onclose(() => {
});




let isChatJoined = false;
let lastRealtimeDateLabel = null;
let readSent = false;
let chatHeaderInitialized = false;
let offlineTimer = null;

// задержка перед показом offline
const OFFLINE_GRACE = 5000;



function messagesEl() {
    return document.getElementById("messagesWrapper");
}

function inputEl() { return document.getElementById("messageInput"); }

// индикатор "печатает..."
function typingEl() { return document.getElementById("typing"); }

// ключ черновика сообщения в localStorage
function draftKey(chatId) {
    return `chat-draft-${chatId}`;
}

// показ пустого состояния чата
function showEmptyState(el) {
    const empty = document.createElement("div");
    empty.className = "empty-state centered";
    empty.textContent = "Напишите первое сообщение 👋";
    el.appendChild(empty);
}

// кнопка "новые сообщения"
function newMsgBtn() {
    return document.getElementById("newMessagesBtn");
}




const textarea = document.getElementById("messageInput");
const counter  = document.getElementById("charCounter");
const messages = document.getElementById("messagesWrapper");

const MIN_HEIGHT = 38;
const MAX_HEIGHT = 120;

let lastHeight = MIN_HEIGHT;

// авто-ресайз textarea + счетчик символов
if (textarea && counter && messages) {
    textarea.addEventListener("input", () => {
        const len = textarea.value.length;

        // жесткий лимит длины сообщения
        if (len > 5000) {
            textarea.value = textarea.value.slice(0, 5000);
            counter.textContent = "5000 / 5000";
            return;
        }

        counter.textContent = `${len} / 5000`;

        textarea.style.height = "auto";
        const newHeight = Math.min(textarea.scrollHeight, MAX_HEIGHT);
        const diff = newHeight - lastHeight;

        textarea.style.height = newHeight + "px";

        if (diff > 0) messages.scrollTop += diff;

        lastHeight = newHeight;
    });
}

// форматирует дату сообщения: сегодня / вчера / дата
function formatDateLabel(dateStr) {
    const d = new Date(dateStr);
    const today = new Date();
    const yesterday = new Date();
    yesterday.setDate(today.getDate() - 1);

    const sameDay = (a, b) =>
        a.getFullYear() === b.getFullYear() &&
        a.getMonth() === b.getMonth() &&
        a.getDate() === b.getDate();

    if (sameDay(d, today)) return "Сегодня";

    if (sameDay(d, yesterday)) return "Вчера";

    return d.toLocaleDateString("ru-RU", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric"
    });
}

// рендер временного "моего" сообщения (pending)
function renderMyMessage(text, tempId) {
    const el = messagesEl();
    const li = document.createElement("li");

    li.className = "msg me pending";
    li.dataset.tempId = tempId;

    li.innerHTML = `
        <div>${text}</div>
        <div class="time">...</div>
    `;

    el.appendChild(li);
    smartScroll(true);
}



// кнопка отправки сообщения
const sendBtn = document.getElementById("sendBtn");

// клик по кнопке → отправка сообщения
if (sendBtn) {
    sendBtn.addEventListener("click", () => {
        sendMessage(window.__chat.chatId);
    });
}



// проверка: пользователь почти внизу списка
function isNearBottom(el) {
    return el.scrollHeight - el.scrollTop - el.clientHeight < 120;
}

// умный скролл (только если нужно)
function smartScroll(force = false) {
    const el = messagesEl();
    if (!el) return;

    // либо принудительно, либо если пользователь внизу
    if (force || isNearBottom(el)) {
        el.scrollTop = el.scrollHeight;
    }
}


// получение нового сообщения в реальном времени
connection.on("ReceiveMessage", (userName, text, date) => {
    if (!chatEnabled) return;

    const el = messagesEl();

    // запоминаем: был ли пользователь внизу
    const stickToBottom = isNearBottom(el);

    // если сообщение мое — убираем pending / failed
    if (userName === currentUserName) {
        document
            .querySelectorAll(".msg.me.pending, .msg.me.failed")
            .forEach(m => m.remove());
    }

    // получаем текстовую метку даты
    const label = formatDateLabel(date);

    // добавляем разделитель даты, если она новая
    if (label !== lastRealtimeDateLabel) {
        const sep = document.createElement("div");
        sep.className = "date-separator";
        sep.textContent = label;
        el.appendChild(sep);
        lastRealtimeDateLabel = label;
    }

    // создаем элемент сообщения
    const li = document.createElement("li");
    li.className = `msg ${userName === currentUserName ? "me" : "other"}`;
    li.dataset.chat = chatId;

    // форматируем время
    const time = new Date(date).toLocaleTimeString([], {
        hour: "2-digit",
        minute: "2-digit"
    });

    li.innerHTML = `
        <div class="bubble">
            <span class="text">${text}</span>
            <span class="meta">
                <span class="time">${time}</span>
                ${userName === currentUserName ? `<span class="status">✓</span>` : ``}
            </span>
        </div>
    `;

    // убираем пустое состояние, если было
    const empty = el.querySelector(".empty-state");
    if (empty) empty.remove();

    // добавляем сообщение в DOM
    el.appendChild(li);

    // если пользователь был внизу — скроллим
    if (stickToBottom) {
        smartScroll(true);
    } else {
        // иначе показываем кнопку "новые сообщения"
        newMsgBtn().hidden = false;
    }

    // если сообщение не мое — сбрасываем флаг прочтения
    if (userName !== currentUserName && chatEnabled) {
        readSent = false;
        markChatAsReadOnce(chatId);
    }
});


// таймер для скрытия индикатора печати
let typingTimer = null;

// флаг: сейчас показывается "печатает"
let isTypingShown = false;

// событие: собеседник печатает
connection.on("Typing", userName => {
    if (!chatEnabled) return;
    if (userName === currentUserName) return;

    const el = typingEl();

    // показываем текст "печатает" один раз
    if (!isTypingShown) {
        el.textContent = `${userName} печатает…`;
        isTypingShown = true;
    }

    // сбрасываем предыдущий таймер
    clearTimeout(typingTimer);

    // скрываем индикатор через 2 секунды
    typingTimer = setTimeout(() => {
        el.textContent = "";
        isTypingShown = false;
    }, 2000);
});


// обновление чата в списке
connection.on("ChatUpdated", (chatId, userName, text) => {
    const chatEl = document.querySelector(`[data-chat-id="${chatId}"]`);
    if (!chatEl) {
        updateGlobalBadge(1);
        location.reload();
        return;
    }

    const last = chatEl.querySelector(".last-message");

    if (typeof currentChatId === "undefined" || chatId !== currentChatId) {
        let badge = chatEl.querySelector(".chat-badge");

        if (!badge) {
            badge = document.createElement("span");
            badge.className = "chat-badge";
            badge.textContent = "1";
            chatEl.appendChild(badge);
        } else {
            badge.textContent = parseInt(badge.textContent) + 1;
        }
    }

    if (last) last.textContent = `${userName}: ${text}`;

    if (chatEl.parentElement) chatEl.parentElement.prepend(chatEl);
});

// чат прочитан
connection.on("ChatRead", chatId => {
    const chatEl = document.querySelector(`[data-chat-id="${chatId}"]`);
    updateGlobalBadge(-1);
    if (!chatEl) return;

    const badge = chatEl.querySelector(".chat-badge");
    if (badge) badge.remove();
});



let isOfflineShown = false;

// собеседник онлайн
connection.on("UserOnline", userId => {
    if (userId !== otherUserId) return;

    isOfflineShown = false;
    clearTimeout(offlineTimer);

    setChatOnline(true);
});

// собеседник оффлайн (с задержкой)
connection.on("UserOffline", userId => {
    if (userId !== otherUserId) return;

    clearTimeout(offlineTimer);

    offlineTimer = setTimeout(() => {
        if (!isOfflineShown) {
            isOfflineShown = true;

            setChatOnline(false);
            setLastSeen();
        }
    }, OFFLINE_GRACE);
});




chatHeaderInitialized = false;

// старт чата (вход в комнату + загрузка истории)
async function startChat(chatId) {
    if (!chatEnabled) return;

    const badge = document.querySelector(
        `[data-chat-id="${chatId}"] .chat-badge`
    );
    if (badge) badge.remove();

    await connectionPromise;
    if (!chatHeaderInitialized && typeof otherUserId !== "undefined") {
        const onlineUsers = await connection.invoke("GetOnlineUsers");

        if (onlineUsers.includes(otherUserId)) {
            setChatOnline(true);
        } else {
            await setLastSeen();
        }

        chatHeaderInitialized = true;
    }

    if (!isChatJoined) {
        await connection.invoke("JoinChat", chatId);
        isChatJoined = true;
    }

    await loadHistory(chatId);
    markChatAsReadOnce(chatId);


    const draft = localStorage.getItem(draftKey(chatId));
    if (draft) {
        inputEl().value = draft;
        sendBtn.disabled = !draft.trim();
    }
    requestAnimationFrame(() => {
        smartScroll(true);
    });
}


// отправка сообщения
async function sendMessage(chatId) {
    if (!chatEnabled) return;

    await connectionPromise;

    if (!isChatJoined) {
        await connection.invoke("JoinChat", chatId);
        isChatJoined = true;
    }

    const text = inputEl().value.trim();

    if (!text || text.length > 5000) return;

    sendBtn.disabled = true;

    const tempId = crypto.randomUUID();

    renderMyMessage(text, tempId);

    const failTimer = setTimeout(() => {
        markMessageFailed(tempId);
    }, 5000);

    try {
        await connection.invoke("SendMessage", chatId, text);
        clearTimeout(failTimer);
    } catch {
        markMessageFailed(tempId);
    }

    textarea.value = "";
    textarea.style.height = MIN_HEIGHT + "px";
    lastHeight = MIN_HEIGHT;
    counter.textContent = "0 / 5000";

    localStorage.removeItem(draftKey(chatId));


    sendBtn.disabled = false;
}

// отправка сообщения по Enter (без Shift)
document.addEventListener("keydown", e => {
    if (!chatEnabled) return;
    if (document.activeElement !== inputEl()) return;

    if (e.key === "Enter" && !e.shiftKey) {
        e.preventDefault();
        sendMessage(window.__chat.chatId);
    }
});



// загрузка истории сообщений чата
async function loadHistory(chatId) {
    const res = await fetch(`/Chat/History?chatId=${chatId}`);
    if (!res.ok) return;

    const data = await res.json();
    const el = messagesEl();

    el.innerHTML = "";

    if (data.length === 0) {
        showEmptyState(el);
        return;
    }

    let lastDateLabel = null;

    data.forEach(m => {
        const label = formatDateLabel(m.createdAt);

        if (label !== lastDateLabel) {
            const sep = document.createElement("div");
            sep.className = "date-separator";
            sep.textContent = label;
            el.appendChild(sep);
            lastDateLabel = label;
        }

        const li = document.createElement("li");
        li.className = `msg ${m.userName === currentUserName ? "me" : "other"}`;

        if (m.userName === currentUserName && m.isRead) {
            li.classList.add("read");
        }

        li.dataset.chat = chatId;

        const time = new Date(m.createdAt).toLocaleTimeString([], {
            hour: "2-digit",
            minute: "2-digit"
        });

        li.innerHTML = `
            <div class="bubble">
                <span class="text">${m.text}</span>
                <span class="meta">
                    <span class="time">${time}</span>
                    ${
            m.userName === currentUserName
                ? `<span class="status">${m.isRead ? "✓✓" : "✓"}</span>`
                : ``
        }
                </span>
            </div>
        `;

        el.appendChild(li);
    });
    lastRealtimeDateLabel = lastDateLabel;
    
}

let readTimeout = null;

function markChatAsReadOnce(chatId) {
    if (readSent) return;

    clearTimeout(readTimeout);
    readTimeout = setTimeout(() => {
        readSent = true;
        connection.invoke("MarkAsRead", chatId).catch(console.error);
    }, 300);
}




// состояние переподключения SignalR
function setReconnectState(isReconnecting) {
    const status = document.getElementById("onlineStatus");
    if (!status) return;

    if (isReconnecting) {
        status.textContent = "переподключение…";
        status.className = "offline";
        inputEl().disabled = true;
        sendBtn.disabled = true;
    } else {
        // возвращаем ввод
        inputEl().disabled = false;
        sendBtn.disabled = !inputEl().value.trim();
    }
}


// установка статуса online
function setChatOnline(isOnline) {
    const el = document.getElementById("onlineStatus");
    if (!el) return;

    if (isOnline) {
        el.textContent = "online";
        el.className = "online";
    }
    // offline обрабатывается отдельно
}


// установка online/offline индикатора в списке чатов
function setUserOnline(userId, isOnline) {
    const chatEl = document.querySelector(`[data-user-id="${userId}"]`);
    if (!chatEl) return;

    const dot = chatEl.querySelector(".status-dot");
    if (!dot) return;

    dot.classList.toggle("online", isOnline);
    dot.classList.toggle("offline", !isOnline);
}



// форматирует "был(а) ..." по времени последней активности
function formatLastSeen(utc) {
    const last = new Date(utc);
    const now = new Date();


    const diffMs = now - last;
    const min = Math.floor(diffMs / 60000);
    const h = Math.floor(min / 60);
    const d = Math.floor(h / 24);

    if (min < 5) return "был(а) недавно";

    if (last.toDateString() === now.toDateString()) {
        return `был(а) сегодня в ${last.toLocaleTimeString([], {
            hour: "2-digit",
            minute: "2-digit"
        })}`;
    }

    const y = new Date(now);
    y.setDate(now.getDate() - 1);
    if (last.toDateString() === y.toDateString()) {
        return "был(а) вчера";
    }

    if (d < 7) {
        return `был(а) ${d} ${plural(d, "день")} назад`;
    }

    return `был(а) ${last.toLocaleDateString("ru-RU")}`;
}


function plural(n, word) {
    if (word === "день") {
        if (n % 10 === 1 && n % 100 !== 11) return "день";
        if ([2,3,4].includes(n % 10) && ![12,13,14].includes(n % 100)) return "дня";
        return "дней";
    }

    if (word === "час") {
        if (n % 10 === 1 && n % 100 !== 11) return "час";
        if ([2,3,4].includes(n % 10) && ![12,13,14].includes(n % 100)) return "часа";
        return "часов";
    }
}


// получает и показывает last seen пользователя
async function setLastSeen() {
    if (typeof otherUserId === "undefined") return;

    const last = await connection.invoke("GetLastSeen", otherUserId);
    const el = document.getElementById("onlineStatus");
    if (!el) return;

    // если сервер не дал дату
    if (!last) {
        el.textContent = "был(а) недавно";
    } else {
        el.textContent = formatLastSeen(last);
    }

    el.className = "offline";
    window.lastSeenUtc = last;
}


// помечает сообщение как неотправленное
function markMessageFailed(tempId) {
    const msg = document.querySelector(`.msg.me.pending[data-temp-id="${tempId}"]`);
    if (!msg) return;

    msg.classList.remove("pending");
    msg.classList.add("failed");
    msg.innerHTML += `<div class="retry">не отправлено · нажми</div>`;
}


// пометка сообщений прочитанными при прокрутке вниз
document.addEventListener("DOMContentLoaded", () => {
    const el = messagesEl();
    if (el && chatEnabled) {
        el.addEventListener("scroll", () => {
            if (isNearBottom(el)) {
                newMsgBtn().hidden = true;
                markChatAsReadOnce(chatId);
                
            }
        });
    }
});


// повторная отправка failed-сообщения по клику
messagesEl()?.addEventListener("click", e => {
    const msg = e.target.closest(".msg.me.failed");
    if (!msg) return;

    const text = msg.querySelector("div")?.textContent;
    msg.remove();

    inputEl().value = text;
    sendMessage(window.__chat.chatId);
});


// обновление статуса ✓✓ при прочтении
connection.on("MessagesRead", chatId => {
    document
        .querySelectorAll(`.msg.me[data-chat="${chatId}"]`)
        .forEach(m => {
            m.classList.add("read");
            const status = m.querySelector(".status");
            if (status) status.textContent = "✓✓";
        });
});


// SignalR начал переподключение
connection.onreconnecting(() => {
    setReconnectState(true);
});

// SignalR восстановил соединение
connection.onreconnected(() => {
    setReconnectState(false);
});


// кнопка "новые сообщения"
newMsgBtn()?.addEventListener("click", () => {
    smartScroll(true);
    newMsgBtn().hidden = true;
});


// обновление last seen раз в минуту
setInterval(() => {
    const el = document.getElementById("onlineStatus");
    if (!el || el.classList.contains("online")) return;

    if (window.lastSeenUtc) {
        el.textContent = formatLastSeen(window.lastSeenUtc);
    }
}, 60_000);


// при возврате на вкладку — помечаем как прочитанное
document.addEventListener("visibilitychange", () => {
    if (document.visibilityState === "visible" && chatEnabled)
    {
        markChatAsReadOnce(chatId);
    }
});


// старт чата после загрузки страницы
if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", () => {
        if (chatEnabled) startChat(chatId);
    });
} else {
    if (chatEnabled) startChat(chatId);
}



// обработка меню чата и удаления
document.addEventListener("click", e => {
    const menuBtn = e.target.closest(".chat-menu-btn");
    if (menuBtn) {
        e.stopPropagation();

        const chatItem = menuBtn.closest(".chat-item");
        const menu = chatItem.querySelector(".chat-menu");

        document.querySelectorAll(".chat-menu").forEach(m => {
            if (m !== menu) m.hidden = true;
        });

        menu.hidden = !menu.hidden;
        return;
    }

    const deleteBtn = e.target.closest(".chat-delete-btn");
    if (deleteBtn) {
        e.stopPropagation();

        const chatId = deleteBtn.dataset.chatId;
        if (!confirm("Удалить чат?")) return;

        fetch("/Chat/Delete", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ chatId })
        }).then(res => {
            if (res.ok) {
                document
                    .querySelector(`.chat-item[data-chat-id="${chatId}"]`)
                    ?.remove();
            }
        });

        return;
    }

    // клик вне меню — закрываем всё
    document.querySelectorAll(".chat-menu").forEach(m => m.hidden = true);
});



const globalBadge = document.getElementById("chatGlobalBadge");

function updateGlobalBadge(delta) {
    if (!globalBadge) return;

    const current = parseInt(globalBadge.textContent || "0");
    const next = current + delta;

    if (next > 0) {
        globalBadge.textContent = next;
        globalBadge.hidden = false;
    } else {
        globalBadge.textContent = "0";
        globalBadge.hidden = true;
    }
}