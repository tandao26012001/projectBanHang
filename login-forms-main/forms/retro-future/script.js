// Retro Future Login Form JavaScript
class RetroFutureLoginForm {
    constructor() {
        this.form = document.getElementById('loginForm');
        this.emailInput = document.getElementById('email');
        this.passwordInput = document.getElementById('password');
        this.passwordToggle = document.getElementById('passwordToggle');
        this.submitButton = this.form.querySelector('.retro-button');
        this.successMessage = document.getElementById('successMessage');
        this.socialButtons = document.querySelectorAll('.social-retro');
        
        this.init();
    }
    
    init() {
        this.bindEvents();
        this.setupPasswordToggle();
        this.setupSocialButtons();
        this.setupRetroEffects();
    }
    
    bindEvents() {
        this.form.addEventListener('submit', (e) => this.handleSubmit(e));
        this.emailInput.addEventListener('blur', () => this.validateEmail());
        this.passwordInput.addEventListener('blur', () => this.validatePassword());
        this.emailInput.addEventListener('input', () => this.clearError('email'));
        this.passwordInput.addEventListener('input', () => this.clearError('password'));
        
        // Add placeholder for label animations
        this.emailInput.setAttribute('placeholder', ' ');
        this.passwordInput.setAttribute('placeholder', ' ');
    }
    
    setupPasswordToggle() {
        this.passwordToggle.addEventListener('click', () => {
            const type = this.passwordInput.type === 'password' ? 'text' : 'password';
            this.passwordInput.type = type;
            
            this.passwordToggle.classList.toggle('toggle-active', type === 'text');
            
            // Add retro hologram effect
            this.triggerHologramEffect(this.passwordToggle);
        });
    }
    
    setupSocialButtons() {
        this.socialButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                const provider = button.querySelector('span').textContent.trim();
                this.handleSocialLogin(provider, button);
            });
        });
    }
    
    setupRetroEffects() {
        // Add hologram effects on input interactions
        [this.emailInput, this.passwordInput].forEach(input => {
            input.addEventListener('focus', (e) => {
                this.activateHologram(e.target.closest('.field-chrome'));
            });
            
            input.addEventListener('blur', (e) => {
                this.deactivateHologram(e.target.closest('.field-chrome'));
            });
        });
        
        // Add chrome effects to interactive elements
        this.addChromeEffects();
        
        // Start background matrix effects
        this.startMatrixEffects();
    }
    
    activateHologram(container) {
        // Add hologram activation effect
        container.style.transform = 'translateZ(10px)';
        container.style.boxShadow = '0 0 30px rgba(0, 255, 255, 0.4)';
    }
    
    deactivateHologram(container) {
        // Remove hologram effects
        container.style.transform = 'translateZ(0)';
        container.style.boxShadow = '';
    }
    
    triggerHologramEffect(element) {
        // Create hologram ripple effect
        element.style.filter = 'hue-rotate(180deg) brightness(1.3)';
        element.style.transform = 'scale(1.1)';
        
        setTimeout(() => {
            element.style.filter = '';
            element.style.transform = 'scale(1)';
        }, 200);
    }
    
    addChromeEffects() {
        // Add chrome interaction effects to all buttons
        const chromeElements = document.querySelectorAll('.retro-button, .social-retro, .retro-checkbox');
        
        chromeElements.forEach(element => {
            element.addEventListener('mousedown', () => {
                element.style.filter = 'brightness(1.2) contrast(1.1)';
            });
            
            element.addEventListener('mouseup', () => {
                element.style.filter = '';
            });
            
            element.addEventListener('mouseleave', () => {
                element.style.filter = '';
            });
        });
    }
    
    startMatrixEffects() {
        // Add random matrix glitch effects
        setInterval(() => {
            if (Math.random() < 0.15) { // 15% chance every interval
                this.triggerMatrixGlitch();
            }
        }, 3000);
    }
    
    triggerMatrixGlitch() {
        // Create matrix-style glitch effect on the card
        const card = document.querySelector('.future-card');
        card.style.filter = 'hue-rotate(90deg) saturate(1.5)';
        card.style.transform = 'translate(1px, -1px)';
        
        setTimeout(() => {
            card.style.filter = '';
            card.style.transform = '';
        }, 100);
    }
    
    validateEmail() {
        const email = this.emailInput.value.trim();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        
        if (!email) {
            this.showError('email', 'Email Required');
            return false;
        }
        
        if (!emailRegex.test(email)) {
            this.showError('email', 'Invalid Format');
            return false;
        }
        
        this.clearError('email');
        return true;
    }
    
    validatePassword() {
        const password = this.passwordInput.value;
        
        if (!password) {
            this.showError('password', 'Access Code Required');
            return false;
        }
        
        if (password.length < 6) {
            this.showError('password', 'Code Too Short');
            return false;
        }
        
        this.clearError('password');
        return true;
    }
    
    showError(field, message) {
        const retroField = document.getElementById(field).closest('.retro-field');
        const errorElement = document.getElementById(`${field}Error`);
        
        retroField.classList.add('error');
        errorElement.textContent = message;
        errorElement.classList.add('show');
        
        // Add error glitch effect
        this.triggerErrorGlitch(retroField);
    }
    
    clearError(field) {
        const retroField = document.getElementById(field).closest('.retro-field');
        const errorElement = document.getElementById(`${field}Error`);
        
        retroField.classList.remove('error');
        errorElement.classList.remove('show');
        setTimeout(() => {
            errorElement.textContent = '';
        }, 300);
    }
    
    triggerErrorGlitch(element) {
        // Error glitch animation
        element.style.animation = 'none';
        element.style.transform = 'translateX(3px)';
        element.style.filter = 'hue-rotate(300deg)';
        
        setTimeout(() => {
            element.style.transform = 'translateX(-3px)';
        }, 100);
        
        setTimeout(() => {
            element.style.transform = 'translateX(0)';
            element.style.filter = '';
            element.style.animation = '';
        }, 200);
    }
    
    async handleSubmit(e) {
        e.preventDefault();
        
        const isEmailValid = this.validateEmail();
        const isPasswordValid = this.validatePassword();
        
        if (!isEmailValid || !isPasswordValid) {
            // Add system error effect
            this.triggerSystemError();
            return;
        }
        
        this.setLoading(true);
        
        try {
            // Simulate retro authentication process
            await new Promise(resolve => setTimeout(resolve, 3500));
            
            // Show portal success
            this.showPortalSuccess();
        } catch (error) {
            this.showError('password', 'Connection Failed');
        } finally {
            this.setLoading(false);
        }
    }
    
    async handleSocialLogin(provider, button) {
        console.log(`Connecting to ${provider} via quantum link...`);
        
        // Retro loading state
        const originalHTML = button.innerHTML;
        button.style.pointerEvents = 'none';
        button.style.opacity = '0.8';
        
        const loadingHTML = `
            <div class="social-chrome"></div>
            <div class="y2k-spinner">
                <div class="spinner-ring ring-1"></div>
                <div class="spinner-ring ring-2"></div>
            </div>
            <span>LINKING...</span>
            <div class="social-hologram"></div>
        `;
        
        button.innerHTML = loadingHTML;
        
        // Add loading glitch effects
        const glitchInterval = setInterval(() => {
            button.style.filter = 'hue-rotate(180deg)';
            setTimeout(() => {
                button.style.filter = '';
            }, 50);
        }, 300);
        
        try {
            await new Promise(resolve => setTimeout(resolve, 2800));
            console.log(`Quantum tunnel established to ${provider}...`);
            // window.location.href = `/auth/${provider.toLowerCase()}`;
        } catch (error) {
            console.error(`${provider} quantum link error:`, error.message);
        } finally {
            clearInterval(glitchInterval);
            button.style.pointerEvents = 'auto';
            button.style.opacity = '1';
            button.style.filter = '';
            button.innerHTML = originalHTML;
        }
    }
    
    setLoading(loading) {
        this.submitButton.classList.toggle('loading', loading);
        this.submitButton.disabled = loading;
        
        // Disable social buttons during loading
        this.socialButtons.forEach(button => {
            button.style.pointerEvents = loading ? 'none' : 'auto';
            button.style.opacity = loading ? '0.5' : '1';
        });
        
        if (loading) {
            // Add loading matrix effects
            this.startLoadingMatrix();
        }
    }
    
    startLoadingMatrix() {
        // Matrix loading effects
        const matrixInterval = setInterval(() => {
            if (!this.submitButton.classList.contains('loading')) {
                clearInterval(matrixInterval);
                return;
            }
            
            const card = document.querySelector('.future-card');
            card.style.filter = 'hue-rotate(120deg) brightness(1.1)';
            setTimeout(() => {
                card.style.filter = '';
            }, 100);
        }, 400);
    }
    
    triggerSystemError() {
        // System-wide error effect
        const card = document.querySelector('.future-card');
        card.style.animation = 'none';
        card.style.transform = 'translate(2px, -2px)';
        card.style.filter = 'hue-rotate(300deg) contrast(1.2)';
        
        setTimeout(() => {
            card.style.transform = 'translate(-2px, 2px)';
        }, 100);
        
        setTimeout(() => {
            card.style.transform = '';
            card.style.filter = '';
            card.style.animation = '';
        }, 200);
    }
    
    showPortalSuccess() {
        // Hide form with retro transition
        this.form.style.transform = 'scale(0.9) rotateX(15deg)';
        this.form.style.opacity = '0';
        this.form.style.filter = 'blur(3px)';
        
        setTimeout(() => {
            this.form.style.display = 'none';
            document.querySelector('.future-social').style.display = 'none';
            document.querySelector('.future-signup').style.display = 'none';
            document.querySelector('.retro-divider').style.display = 'none';
            
            // Show portal success
            this.successMessage.classList.add('show');
            
            // Add success portal effects
            this.triggerPortalEffects();
            
        }, 300);
        
        // Redirect after portal activation
        setTimeout(() => {
            console.log('Portal activated! Welcome to the future...');
            // window.location.href = '/future-dashboard';
        }, 4500);
    }
    
    triggerPortalEffects() {
        // Portal activation effects
        const card = document.querySelector('.future-card');
        
        // Create portal glow
        setTimeout(() => {
            card.style.boxShadow = `
                0 25px 50px rgba(0, 255, 255, 0.3),
                0 10px 30px rgba(255, 0, 255, 0.2),
                inset 0 1px 0 rgba(255, 255, 255, 0.3)
            `;
        }, 1000);
        
        // Add success glitch
        setTimeout(() => {
            const successTitle = document.querySelector('.success-title');
            successTitle.style.filter = 'hue-rotate(180deg)';
            setTimeout(() => {
                successTitle.style.filter = '';
            }, 200);
        }, 2000);
        
        // Reset card glow
        setTimeout(() => {
            card.style.boxShadow = `
                0 25px 50px rgba(0, 0, 0, 0.3),
                0 10px 30px rgba(255, 0, 255, 0.1),
                inset 0 1px 0 rgba(255, 255, 255, 0.2)
            `;
        }, 3000);
    }
}

// Initialize the retro form when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    new RetroFutureLoginForm();
});