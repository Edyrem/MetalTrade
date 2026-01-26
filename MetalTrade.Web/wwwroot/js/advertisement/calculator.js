document.addEventListener('DOMContentLoaded', () => {

    const price = parseFloat(
        document.getElementById('calc-price').dataset.price.replace(',', '.')
    );

    const input = document.getElementById('calc-quantity');
    const total = document.getElementById('calc-total');
    const error = document.getElementById('calc-error');

    const validNumberRegex = /^(?:0|[1-9]\d*)(?:\.\d+)?$/;

    input.addEventListener('input', () => {

        input.value = input.value
            .replace(',', '.')
            .replace(/[^0-9.]/g, '')
            .replace(/(\..*)\./g, '$1');

        const value = input.value.trim();

        if (!value || !validNumberRegex.test(value)) {
            showError();
            return;
        }

        const quantity = parseFloat(value);

        if (quantity <= 0) {
            showError();
            return;
        }

        hideError();
        total.textContent = (price * quantity).toLocaleString('ru-RU', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
    });

    function showError() {
        error.classList.remove('d-none');
        total.textContent = '0';
    }

    function hideError() {
        error.classList.add('d-none');
    }
    
});