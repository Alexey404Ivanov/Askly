document.addEventListener('DOMContentLoaded',(function () {
    const form = document.getElementById('registerForm');
    const toastContainer = document.getElementById('toastContainer');

    if (!form) return;

    function showToast(message, timeout = 4000) {
        if (!toastContainer) return;
        const toast = document.createElement('div');
        toast.className = 'toast';
        toast.innerHTML = '<div class="toast-text"></div>';
        toast.querySelector('.toast-text').textContent = message;

        const closeBtn = document.createElement('button');
        closeBtn.className = 'toast-close';
        closeBtn.setAttribute('aria-label', 'Закрыть');
        closeBtn.innerHTML = '&times;';
        closeBtn.addEventListener('click', () => hideToast(toast));
        toast.appendChild(closeBtn);

        toastContainer.prepend(toast);
        requestAnimationFrame(() => requestAnimationFrame(() => toast.classList.add('show')));

        const timer = setTimeout(() => hideToast(toast), timeout);
        function hideToast(node) {
            clearTimeout(timer);
            node.classList.remove('show');
            node.classList.add('hide');
            node.addEventListener('transitionend', () => node.parentNode && node.parentNode.removeChild(node), { once: true });
        }
    }

    form.addEventListener('submit', async function (e) {
        e.preventDefault(); 

        const userName = document.getElementById('regName').value.trim();
        const email = document.getElementById('regEmail').value.trim();
        const password = document.getElementById('regPassword').value.trim();

        if (!userName || !email || !password) {
            showToast('Не все поля заполнены');
            return;
        }
        
        if (userName.length < 2) {
            showToast('Имя должно содержать не менее 2 символов');
            return;
        }
        
        if (userName.length > 30) {
            showToast('Имя должно содержать не более 30 символов');
            return;
        }
        if (password.length < 5) {
            showToast('Новый пароль должен содержать не менее 5 символов');
            return;
        }

        if (password.length > 30) {
            showToast('Новый пароль должен содержать не более 30 символов');
            return;
        }
        
        const standardEmailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        function validateEmail(email) {
            return standardEmailRegex.test(email);
        }

        if (!validateEmail(email)) {
            showToast('Некорректный формат email');
            return;
        }
        try {
            await fetch("/api/users/register", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ userName, email, password })
            });

            await fetch("/api/users/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email, password }),
                credentials: "include" // важно для cookie
            });

            window.location.href = "/polls";
        }
        catch (err) {
            console.error(err);
            showToast("Ошибка регистрации или входа");
        }
    });

}));