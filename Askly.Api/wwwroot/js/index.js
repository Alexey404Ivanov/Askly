// `Askly.Api/wwwroot/js/index.js`

const modalBackdrop = document.getElementById("modalBackdrop");
const btnAdd = document.getElementById("btnAdd");
const modalClose = document.getElementById("modalClose");
const pollCreate = document.getElementById("pollCreate");
const pollContainer = document.getElementById("answers");
const toastContainer = document.getElementById('toastContainer');
let answerCount = 0;
const maxAnswers = 9;

const QUESTION_MIN_LEN = 10;
const QUESTION_MAX_LEN = 75;
const ANSWER_MAX_LEN = 50;

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

function ensureWarningElement() {
    let warn = document.getElementById("formWarning");
    if (!warn) {
        warn = document.createElement("div");
        warn.id = "formWarning";
        warn.style.color = "red";
        warn.style.marginTop = "8px";
        warn.style.fontSize = "0.95rem";
        warn.style.display = "none";
        if (pollContainer && pollContainer.parentElement) {
            pollContainer.parentElement.insertBefore(warn, pollContainer.nextSibling);
        } else {
            document.body.appendChild(warn);
        }
    }
    return warn;
}

function showWarning(text) {
    const warn = ensureWarningElement();
    warn.innerText = text;
    warn.style.display = "block";
}

function hideWarning() {
    const warn = document.getElementById("formWarning");
    if (warn) warn.style.display = "none";
}

function markInputError(input) {
    input.style.borderColor = "red";
    input.style.outline = "none";
}

function clearInputError(input) {
    input.style.borderColor = "";
    input.style.outline = "";
}

function addInputListeners(input) {
    input.addEventListener("input", () => {
        const v = input.value.trim();
        if (v !== "") {
            clearInputError(input);

            const anyAnswerError = Array.from(document.querySelectorAll("#answers .answer-row input"))
                .some(i => {
                    const tv = i.value.trim();
                    return tv === "" || tv.length > ANSWER_MAX_LEN;
                });

            const qv = document.getElementById("questionInput")?.value.trim() ?? "";
            const questionError = qv.length < QUESTION_MIN_LEN || qv.length > QUESTION_MAX_LEN;

            if (!anyAnswerError && !questionError) hideWarning();
        }
    });
}

function addAnswer() {
    if (answerCount >= maxAnswers) return;

    answerCount++;

    const container = document.getElementById("answers");

    const row = document.createElement("div");
    row.className = "answer-row";
    row.dataset.index = answerCount;

    row.innerHTML = `
        <input type="text" placeholder="Вариант ответа..." />
        <span class="remove-btn" onclick="removeAnswer(this)">✕</span>
    `;

    container.appendChild(row);

    const input = row.querySelector("input");
    addInputListeners(input);

    const hint = document.getElementById("hint");
    if (answerCount === maxAnswers)
        hint.innerText = `Достигнуто максимальное количество вариантов ответа.`;
    else
        hint.innerText = `Можно добавить ещё ${maxAnswers - answerCount} вариантов ответа.`;
}

function removeAnswer(btn) {
    const row = btn.parentElement;
    row.remove();
    answerCount--;
    const hint = document.getElementById("hint");
    hint.innerText = `Можно добавить ещё ${maxAnswers - answerCount} вариантов ответа.`;
}

btnAdd.addEventListener("click", () => {
    if (!window.isAuthenticated) {
        showToast("Только авторизованные пользователи могут создавать опросы.");
        return;
    }
    modalBackdrop.style.display = "flex";
    if (answerCount === 0) {
        addAnswer();
        addAnswer();
    }
});

modalClose.addEventListener("click", () => {
    modalBackdrop.style.display = "none";
});

window.addEventListener("click", (e) => {
    if (e.target === modalBackdrop) {
        modalBackdrop.style.display = "none";
    }
});

function openProfile() {
    window.location.href = "/me"
}

function logout() {
    fetch("/api/users/logout", {
        method: "POST",
        credentials: "include"
    }).then(() => {
        window.location.href = "/polls";
    });
}

function switchToRegistration() {
    window.location.href = "/register";
}

function switchToAuthorization() {
    window.location.href = "/login";
}

function openPoll(id) {
    if(!window.isAuthenticated) {
        showToast("Только авторизованные пользователи могут переходить к опросу");
        return;
    }
    window.location.href = `/polls/${id}`;
}

pollCreate.addEventListener("click", function() {
    hideWarning();

    const questionInput = document.getElementById("questionInput");
    const question = questionInput?.value.trim() ?? "";

    const answerInputs = document.querySelectorAll("#answers .answer-row input");
    const answers = [];
    answerInputs.forEach(input => {
        const value = input.value.trim();
        if (value) {
            answers.push({ text: value });
        }
    });

    let hasError = false;

    // Валидация вопроса: не пустой + длина 10..75
    if (!question) {
        showWarning("Поле вопроса не должно быть пустым.");
        if (questionInput) markInputError(questionInput);
        hasError = true;
    } else if (question.length < QUESTION_MIN_LEN || question.length > QUESTION_MAX_LEN) {
        showWarning(`Вопрос должен быть от ${QUESTION_MIN_LEN} до ${QUESTION_MAX_LEN} символов.`);
        if (questionInput) markInputError(questionInput);
        hasError = true;
    } else if (questionInput) {
        clearInputError(questionInput);
    }

    if (answerInputs.length === 0) {
        showWarning("Нет доступных вариантов ответа");
        hasError = true;
        return;
    }

    // Валидация вариантов: заполнены + длина <= 50
    let anyEmpty = false;
    let anyTooLong = false;

    answerInputs.forEach(input => {
        const v = input.value.trim();

        if (v === "") {
            anyEmpty = true;
            markInputError(input);
        } else if (v.length > ANSWER_MAX_LEN) {
            anyTooLong = true;
            markInputError(input);
        } else {
            clearInputError(input);
        }

        addInputListeners(input);
    });

    if (anyTooLong) {
        showWarning(`Вариант ответа не должен превышать ${ANSWER_MAX_LEN} символов.`);
        hasError = true;
    } else if (anyEmpty) {
        if (hasError) showWarning("Поля вопроса и ответов не заполнены");
        else showWarning("Не все варианты заполнены");
        hasError = true;
    }

    if (hasError) return;

    const IsMultipleChoice = document.getElementById("multi")?.checked;

    const questionObj = {
        title: question,
        options: answers,
        IsMultipleChoice: IsMultipleChoice
    };

    const json = JSON.stringify(questionObj, null, 2);

    fetch("/api/polls", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: json
    })
        .then(r => {
            if (!r.ok) throw new Error("Ошибка создания опроса");
            return r.json();
        })
        .then(id => {
            window.location.href = `/polls/${id}`;
        })
        .catch(err => {
            showWarning(err.message);
        });

    if (response.status === 'Error') {

    }
    else {
        modalBackdrop.style.display = "none";
    }
});
