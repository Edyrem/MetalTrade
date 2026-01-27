let currentPage = 1;
async function loadUsers(page = 1) {
    currentPage = page;

    const params = new URLSearchParams({
        userName: document.getElementById("userName").value,
        email: document.getElementById("email").value,
        phoneNumber: document.getElementById("phoneNumber").value,
        sort: document.getElementById("sort").value,
        page: page
    });

    const response = await fetch(`/User/Filter?${params}`);
    const html = await response.text();
    document.getElementById("usersContainer").innerHTML = html;
}
async function resetFilters() {
    document.getElementById("userName").value = "";
    document.getElementById("email").value = "";
    document.getElementById("phoneNumber").value = "";
    document.getElementById("sort").value = "date_desc";
    loadUsers();
}

async function changeRole(userId, role, isAdd) {
    const token = document.querySelector('#antiForgeryForm input[name="__RequestVerificationToken"]').value;

    await fetch('/User/ChangeRoleAjax', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify({ userId, role, isAdd })
    });

    loadUsers(currentPage);
}

document.addEventListener("DOMContentLoaded", loadUsers);