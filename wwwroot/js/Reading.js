function scrollElementIntoView(elementId) {
	const element = document.getElementById(elementId);
	if(elementId === null) {
		console.error(`Could not find element with id: ${elementId}`);
		return;
	}

	/** @type {ScrollIntoViewOptions} */
	const scrollArguments = {
		behavior: "auto",
		block: "center",
	};

	element.scrollIntoView(scrollArguments);
}
