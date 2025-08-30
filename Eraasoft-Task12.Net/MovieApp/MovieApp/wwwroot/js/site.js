// MovieApp Custom JavaScript

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    initializeImageLazyLoading();
    initializeTooltips();
    initializeSearchEnhancements();
    initializeAOSHelpers();
    initializeGlassmorphismEffects();
    initializeNavbarScroll();
});

// Navbar scroll effect
function initializeNavbarScroll() {
    const navbar = document.querySelector('.navbar');
    if (!navbar) return;
    
    window.addEventListener('scroll', function() {
        if (window.scrollY > 50) {
            navbar.classList.add('navbar-scrolled');
        } else {
            navbar.classList.remove('navbar-scrolled');
        }
    });
    
    // Initialize on page load too
    if (window.scrollY > 50) {
        navbar.classList.add('navbar-scrolled');
    }
}

// Glassmorphism interactive effects
function initializeGlassmorphismEffects() {
    const movieCards = document.querySelectorAll('.movie-card');
    
    movieCards.forEach(card => {
        card.addEventListener('mousemove', function(e) {
            const rect = this.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;
            
            // Create a subtle shine effect that follows cursor
            this.style.background = `
                radial-gradient(
                    circle at ${x}px ${y}px, 
                    rgba(255,255,255,0.1), 
                    rgba(30,30,30,0.75) 40%
                )
            `;
        });
        
        card.addEventListener('mouseleave', function() {
            // Reset background when mouse leaves
            this.style.background = '';
        });
    });
    
    // Add interactive hover effect for buttons
    const glassButtons = document.querySelectorAll('.btn-primary-custom, .btn-outline-primary-custom');
    
    glassButtons.forEach(button => {
        button.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-2px)';
        });
        
        button.addEventListener('mouseleave', function() {
            this.style.transform = '';
        });
    });
}

// AOS Helper functions
function initializeAOSHelpers() {
    // Check if screen is mobile (under 992px)
    const isMobile = () => window.innerWidth < 992;
    
    // Only initialize AOS observers and helpers on desktop
    if (!isMobile()) {
        // Refresh AOS on dynamic content changes
        const observer = new MutationObserver(function(mutations) {
            let shouldRefresh = false;
            mutations.forEach(function(mutation) {
                if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
                    mutation.addedNodes.forEach(function(node) {
                        if (node.nodeType === 1 && (node.hasAttribute('data-aos') || node.querySelector('[data-aos]'))) {
                            shouldRefresh = true;
                        }
                    });
                }
            });
            if (shouldRefresh && typeof AOS !== 'undefined') {
                AOS.refresh();
            }
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
        
        // Ensure AOS triggers again when tab becomes visible
        document.addEventListener('visibilitychange', function() {
            if (!document.hidden && typeof AOS !== 'undefined') {
                AOS.refresh();
            }
        });
    }
}

// Lazy loading for images
function initializeImageLazyLoading() {
    if ('IntersectionObserver' in window) {
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.src = img.dataset.src || img.src;
                    img.classList.remove('lazy');
                    
                    // Add subtle fade-in effect
                    img.style.opacity = 0;
                    setTimeout(() => {
                        img.style.transition = 'opacity 0.5s ease';
                        img.style.opacity = 1;
                    }, 50);
                    
                    imageObserver.unobserve(img);
                }
            });
        });

        document.querySelectorAll('img[data-src], img.lazy').forEach(img => {
            img.style.opacity = 0;
            imageObserver.observe(img);
        });
    }
}

// Initialize Bootstrap tooltips if available
function initializeTooltips() {
    if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
}

// Enhanced search functionality
function initializeSearchEnhancements() {
    // Real-time search with debounce
    const searchInputs = document.querySelectorAll('input[type="search"], .search-input');
    
    searchInputs.forEach(input => {
        let timeout;
        input.addEventListener('input', function() {
            clearTimeout(timeout);
            timeout = setTimeout(() => {
                triggerSearch(this);
            }, 300);
        });
    });
}

function triggerSearch(input) {
    const searchTerm = input.value.toLowerCase().trim();
    const targetGrid = input.closest('.container').querySelector('.movie-grid, .actor-grid, .category-grid, .actors-list');
    
    if (!targetGrid) return;
    
    const items = targetGrid.children;
    let visibleCount = 0;
    
    Array.from(items).forEach((item, index) => {
        const searchText = getSearchableText(item).toLowerCase();
        const isVisible = searchText.includes(searchTerm);
        
        if (isVisible) {
            item.style.display = item.classList.contains('actor-item') ? 'flex' : 'block';
            visibleCount++;
            
            // Re-trigger AOS animation for filtered items only on desktop
            if (window.innerWidth >= 992 && typeof AOS !== 'undefined' && item.hasAttribute('data-aos')) {
                item.classList.remove('aos-animate');
                setTimeout(() => {
                    item.classList.add('aos-animate');
                }, index * 50); // Stagger the re-animation
            }
        } else {
            item.style.display = 'none';
        }
    });
    
    // Show/hide no results message
    updateNoResultsMessage(targetGrid.parentElement, visibleCount === 0 && searchTerm !== '');
    
    // Refresh AOS after search only on desktop
    if (window.innerWidth >= 992 && typeof AOS !== 'undefined') {
        setTimeout(() => {
            AOS.refresh();
        }, 100);
    }
}

function getSearchableText(element) {
    // Extract searchable text from various elements
    const titleElement = element.querySelector('.movie-title, .actor-name, .category-name');
    const descElement = element.querySelector('.movie-meta, .actor-bio, .actor-bio-short, .category-description');
    
    let text = '';
    if (titleElement) text += titleElement.textContent + ' ';
    if (descElement) text += descElement.textContent + ' ';
    
    return text;
}

function updateNoResultsMessage(container, show) {
    let noResultsMsg = container.querySelector('.no-results-message');
    
    if (show && !noResultsMsg) {
        noResultsMsg = document.createElement('div');
        noResultsMsg.className = 'empty-state no-results-message';
        
        // Only add AOS attributes on desktop
        if (window.innerWidth >= 992) {
            noResultsMsg.setAttribute('data-aos', 'fade-in');
            noResultsMsg.setAttribute('data-aos-duration', '600');
        }
        
        noResultsMsg.innerHTML = `
            <i class="fas fa-search"></i>
            <h4>No results found</h4>
            <p>Try adjusting your search terms</p>
        `;
        container.appendChild(noResultsMsg);
        
        // Trigger AOS animation for the new element only on desktop
        if (window.innerWidth >= 992 && typeof AOS !== 'undefined') {
            AOS.refresh();
        }
    } else if (!show && noResultsMsg) {
        noResultsMsg.remove();
    }
}

// Utility functions
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Error handling for missing images
document.addEventListener('error', function(e) {
    if (e.target.tagName === 'IMG') {
        e.target.src = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMzAwIiBoZWlnaHQ9IjIwMCIgZmlsbD0iI2Y4ZjlmYSIvPjx0ZXh0IHg9IjUwJSIgeT0iNTAlIiBkb21pbmFudC1iYXNlbGluZT0ibWlkZGxlIiB0ZXh0LWFuY2hvcj0ibWlkZGxlIiBmb250LWZhbWlseT0ic2Fucy1zZXJpZiIgZm9udC1zaXplPSIxOHB4IiBmaWxsPSIjNmM3NTdkIj7wn5O94pm7IEltYWdlPC90ZXh0Pjwvc3ZnPg==';
        e.target.alt = 'Image not available';
    }
}, true);

// Add to favorites functionality (placeholder)
function toggleFavorite(button, itemId, itemType) {
    button.classList.toggle('favorited');
    const icon = button.querySelector('i');
    
    if (button.classList.contains('favorited')) {
        icon.className = 'fas fa-heart';
        button.style.color = '#ff453a';
        
        // Add a little pop animation
        button.animate([
            { transform: 'scale(1)' },
            { transform: 'scale(1.3)' },
            { transform: 'scale(1)' }
        ], {
            duration: 300,
            easing: 'ease'
        });
    } else {
        icon.className = 'far fa-heart';
        button.style.color = '';
    }
    
    // Here you would typically send an AJAX request to save the favorite
    console.log(`Toggled favorite for ${itemType} ${itemId}`);
}

// Enhanced card interactions
document.addEventListener('click', function(e) {
    // Handle card clicks for navigation
    if (e.target.closest('.movie-card, .actor-card, .category-card, .actor-item')) {
        const card = e.target.closest('.movie-card, .actor-card, .category-card, .actor-item');
        const link = card.querySelector('a[asp-action="Details"], a[href*="Details"], .btn-view-profile');
        
        if (link && !e.target.closest('a, button')) {
            window.location.href = link.href;
        }
    }
});

// Refresh AOS on window resize with mobile check
window.addEventListener('resize', debounce(function() {
    // Only refresh AOS on desktop
    if (window.innerWidth >= 992 && typeof AOS !== 'undefined') {
        AOS.refresh();
    }
}, 250));

// Export functions for use in other scripts
window.MovieApp = {
    toggleFavorite,
    debounce,
    refreshAOS: function() {
        // Only refresh AOS on desktop
        if (window.innerWidth >= 992 && typeof AOS !== 'undefined') {
            AOS.refresh();
        }
    }
};

// Add this class to CSS to support navbar scroll effect
document.head.insertAdjacentHTML('beforeend', `
<style>
    .navbar {
        transition: padding 0.3s ease, background 0.3s ease;
    }
    
    .navbar-scrolled {
        background: rgba(10,10,10,0.95) !important;
        padding-top: 0.5rem;
        padding-bottom: 0.5rem;
        box-shadow: 0 5px 20px rgba(0,0,0,0.3);
    }
    
    @media (prefers-reduced-motion: reduce) {
        .navbar, .navbar-scrolled {
            transition: none;
        }
    }
    
    /* Mobile optimizations - remove AOS-related transitions on mobile */
    @media (max-width: 991.98px) {
        [data-aos] {
            opacity: 1 !important;
            transform: none !important;
            transition: none !important;
        }
    }
</style>
`);
