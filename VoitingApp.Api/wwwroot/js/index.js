const modalBackdrop = document.getElementById("modalBackdrop");
const modal = document.getElementById("modal");
const btnAdd = document.getElementById("btnAdd");
const modalClose = document.getElementById("modalClose");
const poleCreate = document.getElementById("poleCreate");

let answerCount = 0;
const maxAnswers = 10;


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
    const hint = document.getElementById("hint");
    hint.innerText = `Можно добавить ещё ${maxAnswers - answerCount} вариантов ответа.`;
}

function removeAnswer(btn) {
    const row = btn.parentElement;
    row.remove();
    answerCount--;
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
    // modal.style.display = "none";
});

// Закрытие при клике по фону
window.addEventListener("click", (e) => {
    if (e.target === modal) {
        modal.display = "none";
    }
});

function openPoll(id) {
    window.location.href = `/poles/${id}`;
}

poleCreate.addEventListener("click", function() {
    // 1. Получаем текст вопроса
    const question = document.getElementById("questionInput").value;

    // 2. Получаем все варианты ответа
    const answerInputs = document.querySelectorAll("#answers .answer-row input");
    const answers = [];
    answerInputs.forEach(input => {
        const value = input.value.trim();
        if (value) {
            answers.push({ text: value }); // ← завернули в объект
        }
    });
    
    console.log(question, answers);
    // 3. Получаем флаг "несколько ответов"
    const multipleAnswers = document.getElementById("multi").checked;

    // 4. Собираем объект
    const questionObj = {
        question: question,
        options: answers,
        // multipleAnswers: multipleAnswers
    };

    // 5. Сериализуем в JSON
    const json = JSON.stringify(questionObj, null, 2); // красиво с отступами

    // 6. Можно проверить результат в консоли или отправить на сервер
    console.log(json);

    // Например, отправка на сервер через fetch:
    fetch("/api/poles", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: json
    });

    // Можно закрыть модальное окно
    modalBackdrop.style.display = "none";
});
