// javascript
document.addEventListener('DOMContentLoaded', function () {
    const voteBtn = document.getElementById('voteBtn');
    const deletePoll = document.getElementById('deletePoll');
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

    function setInputsDisabled(formScope, disabled) {
        const inputs = formScope.querySelectorAll('input[type="radio"], input[type="checkbox"]');
        inputs.forEach(i => i.disabled = !!disabled);

        const wrappers = formScope.querySelectorAll('.option-item');
        wrappers.forEach(w => {
            if (disabled) w.classList.add('option-disabled');
            else w.classList.remove('option-disabled');
        });
    }

    function applyResultsToOptions(results, formScope) {
        if (!formScope || !Array.isArray(results)) return;

        const votesMap = new Map();
        results.forEach(o => {
            const id = String(o.optionId ?? '');
            if (!id) return;
            const votes = Number(o.votesCount ?? 0);
            const ratio = Number(o.ratio ?? 0);
            votesMap.set(id, { votes, ratio });
        });

        const optionInputs = formScope.querySelectorAll('input[type="radio"], input[type="checkbox"]');

        optionInputs.forEach(input => {
            const optionId = String(input.value);
            const data = votesMap.get(optionId) || { votes: 0, ratio: 0 };
            const percent = data.ratio; // уже посчитано api, например 12.3

            const wrapper = input.closest('.option-item') || input.parentElement;
            if (!wrapper) return;

            wrapper.classList.add('option-with-results');

            let resultSpan = wrapper.querySelector('.option-result');
            if (!resultSpan) {
                resultSpan = document.createElement('span');
                resultSpan.className = 'option-result';
                wrapper.appendChild(resultSpan);
            }

            resultSpan.textContent = data.votes + ' (' + percent + '%)';
        });
    }

    function createCancelButton(original, selectedIdsAtVote, pollId, formScope) {
        const btn = document.createElement('button');
        btn.type = 'button';
        btn.id = 'cancelBtn';
        btn.className = original.className + ' cancel-button';
        btn.textContent = 'Отменить голос';

        btn.addEventListener('click', function () {
            fetch(`/api/polls/${pollId}/vote`, {
                method: "DELETE",
                headers: { "Content-Type": "application/json" },
                // body: JSON.stringify(selectedIdsAtVote)
            })
                .then(r => {
                    if (!r.ok) throw new Error("Ошибка голосования");
                    showToast('Голос отменен');

                    // убираем результаты из интерфейса
                    try {
                        const resultSpans = formScope.querySelectorAll('.option-result');
                        resultSpans.forEach(s => s.remove());

                        const withResults = formScope.querySelectorAll('.option-with-results');
                        withResults.forEach(w => w.classList.remove('option-with-results'));
                    } catch (e) {
                        console.error('Ошибка при очистке результатов:', e);
                    }

                    setInputsDisabled(formScope, false);
                    btn.replaceWith(original);
                })
                .catch(err => {
                    console.error(err);
                    showToast('Ошибка при отмене голоса');
                });
        });

        return btn;
    }

    deletePoll.addEventListener('click', async function (e) {
        e.preventDefault();
        const formScope = voteBtn.closest('form') || document;
        const pollInput = formScope.querySelector('input[name="Id"], input[id="Id"], input[type="hidden"]');
        const pollId = pollInput ? String(pollInput.value) : '';
        try {
            await fetch(`/api/polls/${pollId}`, {
                method: "DELETE",
                credentials: "include" // важно для cookie
            });
            window.location.href = "/polls";
        }
        catch (err) {
            console.error(err);
            showToast("Не удалось удалить опрос");
        }
    })
    
    // обработчик клика по "Голосовать"
    voteBtn.addEventListener('click', function (e) {
        e.preventDefault();

        const formScope = voteBtn.closest('form') || document;
        const anyChecked = formScope.querySelector('input[type="radio"]:checked, input[type="checkbox"]:checked');

        if (!anyChecked) {
            showToast('Выберите хотя бы один вариант ответа');
            return;
        }

        const pollInput = formScope.querySelector('input[name="Id"], input[id="Id"], input[type="hidden"]');
        const pollId = pollInput ? String(pollInput.value) : '';

        let optionsIds = [];
        const checkedCheckboxes = Array.from(formScope.querySelectorAll('input.option-checkbox:checked'));
        if (checkedCheckboxes.length > 0) {
            optionsIds = checkedCheckboxes.map(i => String(i.value));
        } else {
            const checkedRadio = formScope.querySelector('input[type="radio"]:checked');
            if (checkedRadio) optionsIds.push(String(checkedRadio.value));
        }

        const selectedAtVote = optionsIds.slice();

        setInputsDisabled(formScope, true);

        const cancelBtn = createCancelButton(voteBtn, selectedAtVote, pollId, formScope);
        voteBtn.replaceWith(cancelBtn);
        showToast('Голос принят');

        fetch(`/api/polls/${pollId}/vote`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(optionsIds)
        })
            .then(r => {
                if (!r.ok) throw new Error("Ошибка голосования");
                return fetch(`/api/polls/${pollId}/results`, {
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
                try {
                    const currentCancel = formScope.querySelector('#cancelBtn');
                    if (currentCancel && currentCancel.parentNode) currentCancel.replaceWith(voteBtn);
                } finally {
                    setInputsDisabled(formScope, false);
                }
            });
    });

    // --- ИНИЦИАЛИЗАЦИЯ СОСТОЯНИЯ, ЕСЛИ ПОЛЬЗОВАТЕЛЬ УЖЕ ГОЛОСОВАЛ ---

    (function initFromModel() {
        const pollData = JSON.parse(
            document.getElementById('poll-data').textContent
        );



        const formScope = document.getElementById('pollForm') || document;

        // предполагаем, что сервер кладёт сюда Id выбранных опций
        const selectedIds = Array.isArray(pollData.UserVotes)
            ? pollData.UserVotes.map(x => String(x))
            : [];

        if (!selectedIds.length) return; // пользователь ещё не голосовал

        // отмечаем выбранные варианты
        const optionInputs = formScope.querySelectorAll('input[type="radio"], input[type="checkbox"]');
        optionInputs.forEach(inp => {
            const val = String(inp.value);
            if (selectedIds.includes(val)) {
                inp.checked = true;
            }
        });

        // блокируем инпуты
        setInputsDisabled(formScope, true);

        // подменяем кнопку "Голосовать" на "Отменить голос"
        const pollInput = formScope.querySelector('input[name="Id"], input[id="Id"], input[type="hidden"]');
        const pollId = pollInput ? String(pollInput.value) : '';
        const cancelBtn = createCancelButton(voteBtn, selectedIds, pollId, formScope);
        voteBtn.replaceWith(cancelBtn);

        // подтягиваем и отображаем результаты
        fetch(`/api/polls/${pollId}/results`, {
            method: "GET",
            headers: { "Content-Type": "application/json" }
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
                showToast('Не удалось загрузить результаты голосования');
            });
    })();
});
