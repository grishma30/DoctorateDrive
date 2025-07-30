// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

    document.addEventListener('DOMContentLoaded', function() {
            // Add smooth scrolling and entrance animations
            const cards = document.querySelectorAll('.welcome-card, .info-card');

    const observerOptions = {
        threshold: 0.1,
    rootMargin: '0px 0px -50px 0px'
            };

    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
            }, observerOptions);
            
            cards.forEach(card => {
        card.style.opacity = '0';
    card.style.transform = 'translateY(30px)';
    card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
    observer.observe(card);
            });

    // Register button click handler
    document.getElementById('registerBtn').addEventListener('click', function(e) {
        e.preventDefault();
    // You can replace this with your actual registration page URL
    window.location.href = '/registration';
                // Or for demonstration, you could use:
                // alert('Redirecting to registration page...');
            });

    // Add ripple effect to register button
    const registerBtn = document.getElementById('registerBtn');
    registerBtn.addEventListener('click', function(e) {
                const ripple = document.createElement('span');
    const rect = this.getBoundingClientRect();
    const size = Math.max(rect.width, rect.height);
    const x = e.clientX - rect.left - size / 2;
    const y = e.clientY - rect.top - size / 2;

    ripple.style.width = ripple.style.height = size + 'px';
    ripple.style.left = x + 'px';
    ripple.style.top = y + 'px';
    ripple.classList.add('ripple');

    this.appendChild(ripple);
                
                setTimeout(() => {
        ripple.remove();
                }, 600);
            });
        });

    // Add CSS for ripple effect
    const style = document.createElement('style');
    style.textContent = `
    .register-btn {
        position: relative;
    overflow: hidden;
            }

    .ripple {
        position: absolute;
    border-radius: 50%;
    background: rgba(255,255,255,0.3);
    transform: scale(0);
    animation: rippleAnimation 0.6s linear;
    pointer-events: none;
            }

    @keyframes rippleAnimation {
        to {
        transform: scale(4);
    opacity: 0;
                }
            }
    `;
    document.head.appendChild(style);


document.addEventListener('DOMContentLoaded', function () {
    // Animate steps on scroll
    const stepItems = document.querySelectorAll('.step-item');
    const progressBar = document.getElementById('progressBar');

    const observerOptions = {
        threshold: 0.3,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver(function (entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const delay = parseInt(entry.target.dataset.step) * 200;
                setTimeout(() => {
                    entry.target.classList.add('animate');
                }, delay);
            }
        });
    }, observerOptions);

    stepItems.forEach(item => {
        observer.observe(item);
    });

    // Progress bar animation
    function updateProgressBar() {
        const windowHeight = window.innerHeight;
        const documentHeight = document.documentElement.scrollHeight;
        const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
        const progress = (scrollTop / (documentHeight - windowHeight)) * 100;

        progressBar.style.width = Math.min(progress, 100) + '%';
    }

    window.addEventListener('scroll', updateProgressBar);

    // Start application button handler
    document.getElementById('startApplicationBtn').addEventListener('click', function (e) {
        e.preventDefault();
        // Replace with your actual application URL
        window.location.href = '/application-form';
        // Or for demonstration:
        // alert('Redirecting to application form...');
    });

    // Add smooth scrolling for better UX
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });

    // Add hover effects for step items
    stepItems.forEach(item => {
        item.addEventListener('mouseenter', function () {
            this.style.transform = 'scale(1.02)';
            this.style.transition = 'transform 0.3s ease';
        });

        item.addEventListener('mouseleave', function () {
            this.style.transform = 'scale(1)';
        });
    });

    // Add ripple effect to buttons
    const buttons = document.querySelectorAll('.start-application-btn');
    buttons.forEach(button => {
        button.addEventListener('click', function (e) {
            const ripple = document.createElement('span');
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            ripple.style.position = 'absolute';
            ripple.style.borderRadius = '50%';
            ripple.style.background = 'rgba(255,255,255,0.3)';
            ripple.style.transform = 'scale(0)';
            ripple.style.animation = 'ripple 0.6s linear';
            ripple.style.pointerEvents = 'none';

            this.appendChild(ripple);

            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    });
});

// Add CSS animation for ripple effect
const style = document.createElement('style');
style.textContent = `
            @keyframes ripple {
                to {
                    transform: scale(4);
                    opacity: 0;
                }
            }
        `;
document.head.appendChild(style);