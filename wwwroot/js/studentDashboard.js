// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// User dropdown functionality
const userAvatar = document.getElementById('userAvatar');
const userDropdown = document.getElementById('userDropdown');

userAvatar.addEventListener('click', function (e) {
    e.stopPropagation();
    userDropdown.classList.toggle('show');
});

// Close dropdown when clicking outside
document.addEventListener('click', function (e) {
    if (!userDropdown.contains(e.target) && !userAvatar.contains(e.target)) {
        userDropdown.classList.remove('show');
    }
});

// Side menu functionality
const menuToggle = document.getElementById('menuToggle');
const sideMenu = document.getElementById('sideMenu');
const menuClose = document.getElementById('menuClose');
const overlay = document.getElementById('overlay');

function openSideMenu() {
    sideMenu.classList.add('show');
    overlay.classList.add('show');
    document.body.style.overflow = 'hidden';
}

function closeSideMenu() {
    sideMenu.classList.remove('show');
    overlay.classList.remove('show');
    document.body.style.overflow = 'auto';
}

menuToggle.addEventListener('click', openSideMenu);
menuClose.addEventListener('click', closeSideMenu);
overlay.addEventListener('click', closeSideMenu);

// Close side menu on escape key
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        closeSideMenu();
        userDropdown.classList.remove('show');
    }
});

// Notification bell animation
const notificationBell = document.querySelector('.notification-bell');
notificationBell.addEventListener('click', function () {
    this.style.animation = 'swing 0.5s ease-in-out';
    setTimeout(() => {
        this.style.animation = '';
    }, 500);
});