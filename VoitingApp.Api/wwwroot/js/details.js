document.addEventListener('DOMContentLoaded', function () {
    const voteBtn = document.getElementById('voteBtn');
    const toastContainer = document.getElementById('toastContainer');

    if (!voteBtn) return;

    // Показывает тост с сообщением; возвращает функцию для немедленного удаления
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

        // Добавляем в начало контейнера, чтобы стековать сверху вниз
        toastContainer.prepend(toast);

        // Автоудаление
        const removeTimer = setTimeout(() => hideToast(toast), timeout);

        function hideToast(node) {
            clearTimeout(removeTimer);
            node.classList.add('hide');
            node.addEventListener('transitionend', () => {
                if (node.parentNode) node.parentNode.removeChild(node);
            }, { once: true });
        }

        // Возвращаем функцию, которая немедленно удалит тост
        return () => hideToast(toast);
    }

    // Функция создаёт кнопку "Отменить голос"
    function createCancelButton(original) {
        const btn = document.createElement('button');
        btn.type = 'button';
        btn.id = 'cancelBtn';
        // сохраняем классы, чтобы размер/отступы совпадали
        btn.className = original.className + ' cancel-button';
        btn.textContent = 'Отменить голос';

        // при нажатии отменяем — возвращаем исходную кнопку и показываем тост
        btn.addEventListener('click', function () {
            // заменяем обратно на оригинал
            btn.replaceWith(original);
            // показываем уведомление
            showToast('Голос отменен');
        });

        return btn;
    }

    voteBtn.addEventListener('click', function (e) {
        // предотвращаем немедленную отправку формы — только визуальная замена
        e.preventDefault();

        // создаём кнопку отмены и меняем местами
        const cancel = createCancelButton(voteBtn);
        voteBtn.replaceWith(cancel);

        // показываем уведомление
        showToast('Голос принят');
    });
});
