window.initializeResizeHandler = (dotNetHelper) => {
    const updateDimensions = () => {
        dotNetHelper.invokeMethodAsync('UpdateWindowDimensions', window.innerWidth, window.innerHeight);
    };

    window.addEventListener('resize', updateDimensions);
    updateDimensions(); // Call initially
};