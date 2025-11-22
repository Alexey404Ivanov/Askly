// File: VoitingApp.Api/wwwroot/js/details.js
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

        toastContainer.prepend(toast);

        requestAnimationFrame(() => {
            requestAnimationFrame(() => toast.classList.add('show'));
        });

        const removeTimer = setTimeout(() => hideToast(toast), timeout);

        function hideToast(node) {
            clearTimeout(removeTimer);
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

        const formScope = voteBtn.closest('form') || document;
        const anyChecked = formScope.querySelector('input[type="radio"]:checked, input[type="checkbox"]:checked');

        if (!anyChecked) {
            showToast('Выберите хотя бы один вариант ответа');
            return;
        }

        // Собираем poleId из скрытого поля формы (asp-for="Id")
        const poleInput = formScope.querySelector('input[name="Id"], input[id="Id"], input[type="hidden"]');
        const poleId = poleInput ? String(poleInput.value) : '';

        // Собираем выбранные optionsId
        let optionsId = [];
        const checkedCheckboxes = Array.from(formScope.querySelectorAll('input.option-checkbox:checked'));
        if (checkedCheckboxes.length > 0) {
            optionsId = checkedCheckboxes.map(i => String(i.value));
        } else {
            const checkedRadio = formScope.querySelector('input[type="radio"]:checked');
            if (checkedRadio) optionsId.push(String(checkedRadio.value));
        }

        const payload = {
            poleId: poleId,
            optionsId: optionsId
        };

        fetch(`/api/poles/${poleId}/vote`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        })
        .then(r => {
            if (!r.ok) throw new Error("Ошибка голосования");
        })
        
        
        fetch(`/api/poles/${poleId}/results`, {
            method: "GET",
            headers: { "Content-Type": "application/json" }
        })
        .then(r => {
            if (!r.ok) throw new Error("Ошибка получения результатов");
            return r.json();
        })
        
        const cancel = createCancelButton(voteBtn);
        voteBtn.replaceWith(cancel);
        showToast('Голос принят');
    });
})
