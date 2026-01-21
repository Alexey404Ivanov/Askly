document.addEventListener('DOMContentLoaded',(function () {
    const profileForm = document.getElementById('profileForm');
    const changePasswordForm = document.getElementById('changePasswordForm');
    const toastContainer = document.getElementById('toastContainer');
    
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

    profileForm.addEventListener('submit', async function (e) {
        e.preventDefault();

        const name = document.getElementById('profileName').value.trim();
        const email = document.getElementById('profileEmail').value.trim();
        
        if (!name || !email) {
            showToast('Не все поля заполнены');
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
        
        if (name.length < 2)
        {
            showToast('Имя должно содержать не менее 2 символов');
            return;
        }
        
        if (name.length > 30)
        {
            showToast('Имя должно содержать не более 30 символов');
            return;
        }
        
        try {
            await fetch("/api/users/me/info", {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ name: name, email: email }),
                credentials: "include" // важно для cookie
            });
            showToast("Данные профиля обновлены");
        }
        catch (err) {
            console.error(err);
            showToast("Не удалось обновить данные профиля");
        }
    });

    changePasswordForm.addEventListener('submit', async function (e) {
        e.preventDefault();

        const currentPassword = document.getElementById('currentPassword').value.trim();
        const newPassword = document.getElementById('newPassword').value.trim();
        const confirmedPassword = document.getElementById('confirmedPassword').value.trim();
        
        if (!currentPassword || !newPassword || !confirmedPassword) {
            showToast('Не все поля заполнены');
            return;
        }
        
        if (newPassword.length < 5) {
            showToast('Новый пароль должен содержать не менее 5 символов');
            return;
        }
        
        if (newPassword.length > 30) {
            showToast('Новый пароль должен содержать не более 30 символов');
            return;
        }
        
        if (newPassword !== confirmedPassword) {
            showToast('Новые пароли не совпадают');
            return;
        }

        if (currentPassword === newPassword) {
            showToast('Новый пароль должен отличаться от текущего');
            return;
        }
        
        try {
            await fetch("/api/users/me/password", {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ currentPassword: currentPassword, newPassword: newPassword }),
                credentials: "include" // важно для cookie
            }).then(response => {
                if (!response.ok) {
                    if (response.status === 400) {
                        throw new Error("Введен неверный текущий пароль");
                    } else {
                        throw new Error("Не удалось обновить пароль");
                    }
                }
            })
            changePasswordForm.reset()
            showToast("Пароль обновлен");
        }
        catch (err) {
            console.error(err);
            showToast(err.message);
        }
    });

}));