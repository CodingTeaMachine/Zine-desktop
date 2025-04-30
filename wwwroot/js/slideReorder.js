window.reorder = {
	/**
	 * Animates swapping positions of two elements
	 * @param {HTMLElement} element1
	 * @param {HTMLElement} element2
	 * @param {int} swapDuration
	 */
	animateSwapTwoImages: function(element1, element2, swapDuration) {
		console.log("Starting swap animation");

		if (!element1 || !element2) {
			console.error("Invalid elements provided for swap");
			return;
		}

		// Get initial positions of both elements
		const rect1 = element1.getBoundingClientRect();
		const rect2 = element2.getBoundingClientRect();

		// Calculate the distances to move each element
		const xDistance = rect2.left - rect1.left;
		const yDistance = rect2.top - rect1.top;

		// Apply higher z-index to elements being animated
		element2.style.zIndex = "1";
		element1.style.zIndex = (Number(element2.style.zIndex) + 1).toString();

		// Apply transitions
		element1.style.transition = `transform ${swapDuration}ms ease`;
		element2.style.transition = `transform ${swapDuration}ms ease`;

		// Move elements
		element1.style.transform = `translate(${xDistance}px, ${yDistance}px)`;
		element2.style.transform = `translate(${-xDistance}px, ${-yDistance}px)`;

		// Reset after animation completes
		setTimeout(() => {
			// Reset transforms and transitions
			element1.style.transition = "";
			element2.style.transition = "";
			element1.style.transform = "";
			element2.style.transform = "";
			element1.style.zIndex = "";
			element2.style.zIndex = "";

			console.log("Swap animation completed");
		}, swapDuration);
	}
};
