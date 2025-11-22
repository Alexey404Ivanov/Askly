document.addEventListener('DOMContentLoaded', function () {
    const voteBtn = document.getElementById('voteBtn');
    const toastContainer = document.getElementById('toastContainer');

    if (!voteBtn) return;

    function showToast(message, timeout = 4000) {
        if (!toastContainer) return () => {};

        const toast = document.createElement('div');
        toast.className = 'toast';
        toast.innerHTML = '<div class="toast-text"></div>';

        const text = toast.querySelector('.toast-text');
        text.textContent = message;

        const closeBtn = document.createElement('button');
        closeBtn.className = 'toast-close';
        closeBtn.setAttribute('aria-label', 'Закрыть');
        closeBtn.innerHTML = '&times;';

        closeBtn.addEventListener('click', () => hideToast(toast));
        toast.appendChild(closeBtn);

        // Добавляем в контейнер
        toastContainer.prepend(toast);

        // Делаем ререндер и включаем класс show для плавного появления
        requestAnimationFrame(() => {
            // небольшой следующий кадр для корректного запуска transition
            requestAnimationFrame(() => toast.classList.add('show'));
        });

        // Автоудаление
        const removeTimer = setTimeout(() => hideToast(toast), timeout);

        function hideToast(node) {
            clearTimeout(removeTimer);
            // убираем класс show и добавляем hide — CSS обработает анимацию
            node.classList.remove('show');
            node.classList.add('hide');
            node.addEventListener('transitionend', () => {
                if (node.parentNode) node.parentNode.removeChild(node);
            }, { once: true });
        }

        return () => hideToast(toast);
    }

    function createCancelButton(original) {
        const btn = document.createElement('button');
        btn.type = 'button';
        btn.id = 'cancelBtn';
        btn.className = original.className + ' cancel-button';
        btn.textContent = 'Отменить голос';

        btn.addEventListener('click', function () {
            btn.replaceWith(original);
            showToast('Голос отменен');
        });

        return btn;
    }

    voteBtn.addEventListener('click', function (e) {
        e.preventDefault();
        const cancel = createCancelButton(voteBtn);
        voteBtn.replaceWith(cancel);
        showToast('Голос принят');
    });
})