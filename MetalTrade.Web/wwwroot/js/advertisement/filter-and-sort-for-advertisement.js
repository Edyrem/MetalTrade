document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById('ads-filter-form');
    const grid = document.getElementById('ads-grid');
    const basePartialUrl = '/Advertisement/PartialList';

    if (!form || !grid) return;

    function buildQueryFromForm(formElement) {
        const params = new URLSearchParams();
        const fd = new FormData(formElement);

        for (const [k, v] of fd.entries()) {
            if (!v) continue;
            params.append(k, v);
        }

        params.set('page', '1');
        return params.toString();
    }

    async function loadPartialWithParams(queryString, pushUrl = true) {
        const url = basePartialUrl + (queryString ? '?' + queryString : '');

        const response = await fetch(url, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });

        const html = await response.text();
        grid.innerHTML = html;

        if (pushUrl) {
            const newUrl = window.location.pathname + (queryString ? '?' + queryString : '');
            history.pushState({}, '', newUrl);
        }
    }

    form.addEventListener('submit', async function (e) {
        e.preventDefault();
        const qs = buildQueryFromForm(form);
        await loadPartialWithParams(qs, true);

        const offcanvasEl = document.getElementById('offcanvasExample');
        const offcanvas = bootstrap.Offcanvas.getInstance(offcanvasEl)
            || new bootstrap.Offcanvas(offcanvasEl);

        offcanvas.hide();
    });

    const sortSelect = form.querySelector('select[name="sort"]');
    if (sortSelect) {
        sortSelect.addEventListener('change', async function () {
            const qs = buildQueryFromForm(form);
            await loadPartialWithParams(qs, true);

            const offcanvas = bootstrap.Offcanvas.getInstance(
                document.getElementById('offcanvasExample')
            );
            offcanvas?.hide();
        });
    }

    grid.addEventListener('click', async function (e) {
        const link = e.target.closest('a');
        if (!link) return;

        const url = new URL(link.href);
        if (!url.searchParams.has('page')) return;

        e.preventDefault();
        await loadPartialWithParams(url.search.replace('?', ''), true);
    });

    window.addEventListener('popstate', async function () {
        await loadPartialWithParams(location.search.replace('?', ''), false);
    });
});