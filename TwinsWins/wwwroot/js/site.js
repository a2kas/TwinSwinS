window.initializeResizeHandler = (dotNetHelper) => {
    const updateDimensions = () => {
        dotNetHelper.invokeMethodAsync('UpdateWindowDimensions', window.innerWidth, window.innerHeight);
    };

    window.addEventListener('resize', updateDimensions);
    updateDimensions(); // Call initially
};

async function connectMetaMask() {
    if (typeof window.ethereum !== 'undefined') {
        try {
            const accounts = await window.ethereum.request({ method: 'eth_requestAccounts' });
            return accounts[0];
        } catch (error) {
            console.error(error);
            return null;
        }
    } else {
        alert('MetaMask is not installed. Please install it to use this feature.');
        return null;
    }
}