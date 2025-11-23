// javascript
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

    function applyResultsToOptions(results, formScope) {
        if (!results || !Array.isArray(results.options)) return;

        // общее количество голосов
        const totalVotes = results.options.reduce(
            (sum, o) => sum + (o.votesCount || 0),
            0
        );

        // если нет голосов — считаем total = 1 чтобы корректно показать 0%
        const safeTotal = totalVotes === 0 ? 1 : totalVotes;
        results.options.forEach(option => {
            const optionId = option.id;
            const votes = option.votesCount || 0;
            const percent = Math.round((votes / safeTotal) * 100);

            // ищем input по value == id
            const input = formScope.querySelector('input[value="' + optionId + '"]');
            if (!input) return;

            // получаем wrapper (label.option-item)
            const wrapper = input.closest('.option-item') || input.parentElement;
            if (!wrapper) return;
            wrapper.classList.add('option-with-results');

            // если результат уже добавлен, обновим, иначе создадим и поместим в конец
            let resultSpan = wrapper.querySelector('.option-result');
            if (!resultSpan) {
                resultSpan = document.createElement('span');
                resultSpan.className = 'option-result';
                // вставляем в конец, чтобы всегда быть справа
                wrapper.appendChild(resultSpan);
            }

            resultSpan.textContent = votes + ' (' + percent + '%)';
        });
    }
    
    voteBtn.addEventListener('click', function (e) {
        e.preventDefault();

        const formScope = voteBtn.closest('form') || document;
        const anyChecked = formScope.querySelector('input[type="radio"]:checked, input[type="checkbox"]:checked');

        if (!anyChecked) {
            showToast('Выберите хотя бы один вариант ответа');
            return;
        }

        const poleInput = formScope.querySelector('input[name="Id"], input[id="Id"], input[type="hidden"]');
        const poleId = poleInput ? String(poleInput.value) : '';

        let optionsIds = [];
        const checkedCheckboxes = Array.from(formScope.querySelectorAll('input.option-checkbox:checked'));
        if (checkedCheckboxes.length > 0) {
            optionsIds = checkedCheckboxes.map(i => String(i.value));
        } else {
            const checkedRadio = formScope.querySelector('input[type="radio"]:checked');
            if (checkedRadio) optionsIds.push(String(checkedRadio.value));
        }
        
        
        console.log(`/api/poles/${poleId}/vote`);

        fetch(`/api/poles/${poleId}/vote`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(optionsIds)
        })
            .then(r => {
                if (!r.ok) throw new Error("Ошибка голосования");
                return fetch(`/api/poles/${poleId}/results`, {
                    method: "GET",
                    headers: { "Content-Type": "application/json" }
                });
            })
            .then(r => {
                if (!r.ok) throw new Error("Ошибка получения результатов");
                return r.json();
            })
            .then(results => {
                applyResultsToOptions(results, formScope);
            })
            .catch(err => {
                console.error(err);
                showToast('Произошла ошибка при голосовании');
            });

        const cancel = createCancelButton(voteBtn);
        voteBtn.replaceWith(cancel);
        showToast('Голос принят');
    });
});
