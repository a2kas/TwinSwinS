window.initializeResizeHandler = (dotNetHelper) => {
    const updateDimensions = () => {
        dotNetHelper.invokeMethodAsync('UpdateWindowDimensions', window.innerWidth, window.innerHeight);
    };

    window.addEventListener('resize', updateDimensions);
    updateDimensions(); // Call initially
};

window.openInNewWindow = function (url) {
    var newWindow = window.open(url, '_blank');
    if (!newWindow || newWindow.closed || typeof newWindow.closed == 'undefined') {
        alert('The new window could not be opened. Please check your browser settings or disable your ad blocker.');
    }
};

window.loadImages = function (imageUrls, fallbackUrl) {
    console.log(imageUrls);
    return new Promise((resolve, reject) => {
        let loadedCount = 0;
        const totalImages = imageUrls.length;

        imageUrls.forEach(url => {
            const img = new Image();
            img.src = url;
            img.onload = () => {
                loadedCount++;
                if (loadedCount === totalImages) {
                    resolve();
                }
            };
            img.onerror = () => {
                console.warn(`Failed to load image: ${url}. Replacing with fallback image.`);
                img.src = fallbackUrl;
                img.onload = () => {
                    loadedCount++;
                    if (loadedCount === totalImages) {
                        resolve();
                    }
                };
                img.onerror = () => {
                    console.error(`Failed to load fallback image: ${fallbackUrl}`);
                    loadedCount++;
                    if (loadedCount === totalImages) {
                        resolve();
                    }
                };
            };
        });
    });
}