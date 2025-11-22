// Файл: `VoitingApp.Api/wwwroot/js/index.js`
const modalBackdrop = document.getElementById("modalBackdrop");
const modal = document.getElementById("modal");
const btnAdd = document.getElementById("btnAdd");
const modalClose = document.getElementById("modalClose");
const poleCreate = document.getElementById("poleCreate");
const poleContainer = document.getElementById("answers");
let answerCount = 0;
const maxAnswers = 10;

function ensureWarningElement() {
    let warn = document.getElementById("formWarning");
    if (!warn) {
        warn = document.createElement("div");
        warn.id = "formWarning";
        warn.style.color = "red";
        warn.style.marginTop = "8px";
        warn.style.fontSize = "0.95rem";
        warn.style.display = "none";
        // вставляем предупреждение под контейнер с вариантами, если он есть
        if (poleContainer && poleContainer.parentElement) {
            poleContainer.parentElement.insertBefore(warn, poleContainer.nextSibling);
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
        if (input.value.trim() !== "") {
            clearInputError(input);
            // если больше никаких ошибок — скрыть предупреждение
            const anyError = Array.from(document.querySelectorAll("#answers .answer-row input"))
                .some(i => i.value.trim() === "");
            const questionEmpty = document.getElementById("questionInput")?.value.trim() === "";
            if (!anyError && !questionEmpty) hideWarning();
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
    modalBackdrop.style.display = "flex";
    if (answerCount === 0) {
        addAnswer();
        addAnswer();
    }
});

modalClose.addEventListener("click", () => {
    modalBackdrop.style.display = "none";
});

// Закрытие при клике по фону
window.addEventListener("click", (e) => {
    if (e.target === modalBackdrop) {
        modalBackdrop.style.display = "none";
    }
});

function openPoll(id) {
    window.location.href = `/poles/${id}`;
}

poleCreate.addEventListener("click", function() {
    hideWarning();

    // 1. Получаем текст вопроса
    const questionInput = document.getElementById("questionInput");
    const question = questionInput?.value.trim() ?? "";

    // 2. Получаем все варианты ответа
    const answerInputs = document.querySelectorAll("#answers .answer-row input");
    const answers = [];
    answerInputs.forEach(input => {
        const value = input.value.trim();
        if (value) {
            answers.push({ text: value });
        }
    });

    // Валидация: вопрос не пустой
    let hasError = false;
    if (!question) {
        showWarning("Поле вопроса не должно быть пустым.");
        if (questionInput) markInputError(questionInput);
        hasError = true;
    } else if (questionInput) {
        clearInputError(questionInput);
    }
    
    if (answerInputs.length === 0) {
        showWarning("Нет доступных вариантов ответа")
        hasError = true;
        return;
    }

    // Валидация: все варианты не пустые и их должно быть хотя бы 1
    if (answers.length < answerInputs.length) {
        if (hasError) showWarning("Поля вопроса и ответов не заполнены")
        else showWarning("Не все варианты заполнены");
        
        hasError = true;
    } else {
        // подсветка пустых
        answerInputs.forEach(input => {
            if (input.value.trim() === "") {
                markInputError(input);
                hasError = true;
            } else {
                clearInputError(input);
            }
            // добавляем слушатель на ввод для очистки ошибки
            addInputListeners(input);
        });
        if (hasError && !document.getElementById("formWarning")?.innerText) {
            showWarning("Пожалуйста, заполните все варианты ответа.");
        }
    }

    if (hasError) return; // отменяем отправку

    // 3. Получаем флаг "несколько ответов"
    const multipleAnswers = document.getElementById("multi")?.checked;

    // 4. Собираем объект
    const questionObj = {
        question: question,
        options: answers,
        // multipleAnswers: multipleAnswers
    };

    // 5. Сериализуем в JSON
    const json = JSON.stringify(questionObj, null, 2);

    // 6. Отправка на сервер
    fetch("/api/poles", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: json
    });

    // Можно закрыть модальное окно
    modalBackdrop.style.display = "none";
});
