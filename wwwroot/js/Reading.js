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

function convertStreamToBase64(fileStream){
	return new Promise((resolve, reject) => {
		const reader = new FileReader();
		reader.onloadend = () => resolve(reader.result);
		reader.onerror = reject;
		reader.readAsDataURL(fileStream);
	});
}
