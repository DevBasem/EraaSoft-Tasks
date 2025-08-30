// Navbar scroll behavior
(function() {
    // Variables to track scroll position
    let lastScrollTop = 0;
    let scrollDelta = 10; // Minimum scroll amount before showing/hiding
    let navbarHeight = 64; // Default height, will be measured on load
    let isNavbarHidden = false;

    // Get navbar element
    const navbar = document.querySelector('header.fixed-top');
    
    // Set initial position
    document.addEventListener('DOMContentLoaded', function() {
        if (navbar) {
            navbarHeight = navbar.offsetHeight;
            
            // Ensure navbar is visible when page loads at the top
            if (window.scrollY <= navbarHeight) {
                showNavbar();
            }
        }
    });
    
    // Function to handle scroll events
    function handleScroll() {
        const currentScrollTop = window.scrollY;
        
        // Skip if we haven't scrolled enough
        if (Math.abs(lastScrollTop - currentScrollTop) <= scrollDelta) {
            return;
        }
        
        // If scrolled down and past the navbar height
        if (currentScrollTop > lastScrollTop && currentScrollTop > navbarHeight * 2) {
            // Scrolling DOWN - hide the navbar
            hideNavbar();
        } else if (currentScrollTop < lastScrollTop || currentScrollTop <= navbarHeight) {
            // Scrolling UP or at the top - show the navbar
            showNavbar();
        }
        
        lastScrollTop = currentScrollTop;
    }
    
    // Function to hide navbar
    function hideNavbar() {
        if (!isNavbarHidden && navbar) {
            navbar.classList.add('navbar-hidden');
            isNavbarHidden = true;
        }
    }
    
    // Function to show navbar
    function showNavbar() {
        if (isNavbarHidden && navbar) {
            navbar.classList.remove('navbar-hidden');
            isNavbarHidden = false;
        }
    }
    
    // Add scroll event listener with throttling for performance
    let isScrolling;
    window.addEventListener('scroll', function() {
        window.clearTimeout(isScrolling);
        isScrolling = setTimeout(handleScroll, 10);
    });
})();