window.scrollElementIntoView = function (elementId) {
	const element = document.getElementById(elementId);
	if(elementId === null) {
		console.error(`Could not find element with id: ${elementId}`);
		return;
	}

	/** @type {ScrollIntoViewOptions} */
	const scrollArguments = {
		behavior: "smooth",
		block: "center",

	};

	element.scrollIntoView(scrollArguments);
}

function setupMouseIdleWatcher (dotNetRef, idleTimeoutMs) {
	let idleTimerId;
	let isIdle = false;

	function handleIdle() {
		if (!isIdle) {
			isIdle = true;
			dotNetRef.invokeMethodAsync('JS_OnUserIdle');
		}
	}

	function resetIdleTimer() {
		clearTimeout(idleTimerId);

		if (isIdle) {
			// Transitioning from idle to active
			isIdle = false;
			dotNetRef.invokeMethodAsync('JS_OnUserActive');
		}

		// Only restart the timer — don't change state here unless going active
		idleTimerId = setTimeout(() => {
			handleIdle();
		}, idleTimeoutMs);
	}

	// Attach multiple relevant events to capture any activity
	['mousemove', 'mousedown', 'keydown', 'scroll', 'touchstart'].forEach(event => {
		window.addEventListener(event, resetIdleTimer);
	});

	resetIdleTimer(); // Start the timer initially
};
