// Script to handle video hero section optimizations
document.addEventListener('DOMContentLoaded', function() {
    // Get the video element
    const videoElement = document.getElementById('hero-video');
    
    if (!videoElement) return;
    
    // Ensure video fills container on load
    function ensureVideoFill() {
        const container = videoElement.closest('.video-background');
        if (container) {
            const containerRect = container.getBoundingClientRect();
            const videoRect = videoElement.getBoundingClientRect();
            
            // If video doesn't cover the full container, adjust
            if (videoRect.width < containerRect.width || videoRect.height < containerRect.height) {
                videoElement.style.width = '100vw';
                videoElement.style.height = '100vh';
                videoElement.style.minWidth = '100vw';
                videoElement.style.minHeight = '100vh';
            }
        }
    }
    
    // Check if user prefers reduced motion
    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    
    // Only process video if reduced motion is not preferred
    if (!prefersReducedMotion) {
        // Function to handle video loading
        function handleVideoLoad() {
            // Add loaded class to fade in the video
            videoElement.classList.add('loaded');
            
            // Ensure video covers full area
            ensureVideoFill();
            
            // Log success
            console.log('Video loaded successfully');
        }
        
        // Handle video load event
        videoElement.addEventListener('loadeddata', handleVideoLoad);
        videoElement.addEventListener('canplay', handleVideoLoad);
        
        // Fallback if video doesn't load within 3 seconds
        setTimeout(function() {
            if (!videoElement.classList.contains('loaded')) {
                handleVideoLoad();
                console.log('Video load fallback triggered');
            }
        }, 3000);
        
        // Handle window resize to maintain video coverage
        let resizeTimeout;
        window.addEventListener('resize', function() {
            if (resizeTimeout) clearTimeout(resizeTimeout);
            resizeTimeout = setTimeout(ensureVideoFill, 200);
        });
        
        // Optimize video playback
        function optimizeVideoPlayback() {
            // Check if video is in viewport
            const rect = videoElement.getBoundingClientRect();
            const isVisible = (
                rect.top >= -rect.height && 
                rect.bottom <= window.innerHeight + rect.height
            );
            
            // Pause video if not visible to save resources
            if (!isVisible && !videoElement.paused) {
                videoElement.pause();
            } else if (isVisible && videoElement.paused) {
                // Play if visible and currently paused
                // Using a promise with catch to handle autoplay restrictions
                const playPromise = videoElement.play();
                
                if (playPromise !== undefined) {
                    playPromise.catch(error => {
                        console.log('Autoplay prevented:', error);
                    });
                }
            }
        }
        
        // Check video visibility on scroll (with throttling)
        let scrollTimeout;
        window.addEventListener('scroll', function() {
            if (scrollTimeout) clearTimeout(scrollTimeout);
            scrollTimeout = setTimeout(optimizeVideoPlayback, 200);
        });
        
        // Initial optimization
        optimizeVideoPlayback();
        
        // Handle page visibility changes
        document.addEventListener('visibilitychange', function() {
            if (document.hidden) {
                videoElement.pause();
            } else {
                optimizeVideoPlayback();
            }
        });
    }
    
    // Handle network condition changes
    if ('connection' in navigator) {
        const connection = navigator.connection;
        
        function handleNetworkChange() {
            // If on slow connection or data saver is on, pause video
            if (connection.saveData || 
                connection.effectiveType === 'slow-2g' || 
                connection.effectiveType === '2g') {
                
                videoElement.pause();
                videoElement.removeAttribute('autoplay');
                videoElement.style.display = 'none';
                console.log('Video disabled due to slow connection');
            }
        }
        
        // Initial check
        handleNetworkChange();
        
        // Listen for changes
        if (connection.addEventListener) {
            connection.addEventListener('change', handleNetworkChange);
        }
    }
});