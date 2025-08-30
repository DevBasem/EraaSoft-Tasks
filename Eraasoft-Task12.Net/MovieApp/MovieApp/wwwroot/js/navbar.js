// Navbar scroll behavior
(function() {
    // Variables to track scroll position
    let lastScrollTop = 0;
    let scrollDelta = 10; // Minimum scroll amount before showing/hiding
    let navbarHeight = 64; // Default height, will be measured on load
    let isNavbarHidden = false;
    let isScrolling = false;

    // Get navbar elements
    const navbar = document.querySelector('header.fixed-top');
    const navbarCollapse = document.querySelector('.navbar-collapse');
    const navbarToggler = document.querySelector('.navbar-toggler');
    
    // Set initial position
    document.addEventListener('DOMContentLoaded', function() {
        if (navbar) {
            navbarHeight = navbar.offsetHeight;
            
            // Ensure navbar is visible when page loads at the top
            if (window.scrollY <= navbarHeight) {
                showNavbar();
            }
        }
        
        // Improve navbar collapse behavior
        if (navbarCollapse && navbarToggler) {
            // Let Bootstrap handle the collapse animations completely
            // Remove our custom transition interference
            
            // Close navbar when clicking outside on mobile
            document.addEventListener('click', function(event) {
                const isClickInsideNav = navbar.contains(event.target);
                const isNavOpen = navbarCollapse.classList.contains('show');
                
                if (!isClickInsideNav && isNavOpen && window.innerWidth < 992) {
                    // Use Bootstrap's method to close
                    bootstrap.Collapse.getOrCreateInstance(navbarCollapse).hide();
                }
            });
            
            // Close navbar when clicking on nav links (mobile)
            const navLinks = document.querySelectorAll('.navbar-nav .nav-link');
            navLinks.forEach(link => {
                link.addEventListener('click', function() {
                    const isNavOpen = navbarCollapse.classList.contains('show');
                    if (isNavOpen && window.innerWidth < 992) {
                        // Small delay for visual feedback, then close
                        setTimeout(() => {
                            bootstrap.Collapse.getOrCreateInstance(navbarCollapse).hide();
                        }, 100);
                    }
                });
            });
            
            // Handle collapse events properly
            navbarCollapse.addEventListener('show.bs.collapse', function() {
                // Ensure navbar stays visible when menu opens
                if (navbar) {
                    navbar.style.transform = 'translateY(0)';
                    navbar.classList.remove('navbar-hidden');
                    isNavbarHidden = false;
                }
            });
            
            navbarCollapse.addEventListener('shown.bs.collapse', function() {
                // Menu is fully open - keep navbar visible
                if (navbar) {
                    navbar.style.transform = 'translateY(0)';
                }
            });
            
            navbarCollapse.addEventListener('hide.bs.collapse', function() {
                // Menu is starting to close - let it finish
            });
            
            navbarCollapse.addEventListener('hidden.bs.collapse', function() {
                // Menu is fully closed - restore normal scroll behavior
                if (navbar) {
                    navbar.style.transform = ''; // Remove inline style
                    // Check current scroll position and hide if needed
                    const currentScrollTop = window.scrollY;
                    if (currentScrollTop > navbarHeight * 2 && currentScrollTop > lastScrollTop) {
                        hideNavbar();
                    }
                }
            });
        }
    });
    
    // Function to handle scroll events with improved performance
    function handleScroll() {
        if (isScrolling) return;
        
        isScrolling = true;
        
        // Use requestAnimationFrame for smooth performance
        requestAnimationFrame(() => {
            const currentScrollTop = window.scrollY;
            
            // Skip if we haven't scrolled enough
            if (Math.abs(lastScrollTop - currentScrollTop) <= scrollDelta) {
                isScrolling = false;
                return;
            }
            
            // Don't hide navbar if mobile menu is open
            const isNavOpen = navbarCollapse && navbarCollapse.classList.contains('show');
            if (isNavOpen) {
                isScrolling = false;
                return;
            }
            
            // Don't interfere if navbar has inline transform (during menu operations)
            if (navbar && navbar.style.transform) {
                isScrolling = false;
                return;
            }
            
            handleScrollDirection(currentScrollTop);
            
            lastScrollTop = currentScrollTop;
            isScrolling = false;
        });
    }
    
    // Separate function to handle scroll direction logic
    function handleScrollDirection(currentScrollTop) {
        // If scrolled down and past the navbar height
        if (currentScrollTop > lastScrollTop && currentScrollTop > navbarHeight * 2) {
            // Scrolling DOWN - hide the navbar
            hideNavbar();
        } else if (currentScrollTop < lastScrollTop || currentScrollTop <= navbarHeight) {
            // Scrolling UP or at the top - show the navbar
            showNavbar();
        }
    }
    
    // Function to hide navbar with improved animation
    function hideNavbar() {
        if (!isNavbarHidden && navbar && !navbar.style.transform) {
            navbar.classList.add('navbar-hidden');
            isNavbarHidden = true;
        }
    }
    
    // Function to show navbar with improved animation
    function showNavbar() {
        if (navbar && (isNavbarHidden || navbar.classList.contains('navbar-hidden'))) {
            navbar.classList.remove('navbar-hidden');
            navbar.style.transform = ''; // Clear any inline styles
            isNavbarHidden = false;
        }
    }
    
    // Optimized scroll event listener with passive option for better performance
    let scrollTimeout;
    window.addEventListener('scroll', function() {
        // Clear existing timeout
        if (scrollTimeout) {
            clearTimeout(scrollTimeout);
        }
        
        // Throttle scroll events
        scrollTimeout = setTimeout(handleScroll, 8);
    }, { passive: true });
    
    // Handle window resize to adjust navbar behavior
    window.addEventListener('resize', function() {
        // Recalculate navbar height on resize
        if (navbar) {
            navbarHeight = navbar.offsetHeight;
            // Clear any inline styles on resize
            navbar.style.transform = '';
        }
        
        // Close mobile menu on resize to desktop to prevent layout issues
        if (navbarCollapse && window.innerWidth >= 992) {
            const collapseInstance = bootstrap.Collapse.getInstance(navbarCollapse);
            if (collapseInstance) {
                collapseInstance.hide();
            }
        }
    });
})();