/**
 *  Call the HandleClickOutside function on the razor page, when a user either left or right clicks outside the element with elementId
 *
 * @param elementId
 * @param dotNetHelper
 * @returns {{dispose: *}}
 */
window.registerClickOutsideHandler = (elementId, dotNetHelper) => {
	const element = document.getElementById(elementId);
	console.log(element, elementId)
	const handler = (event) => {
		if (!element?.contains(event.target)) {
			dotNetHelper.invokeMethodAsync('HandleClickOutside');
		}
	};

	document.addEventListener('click', handler);
	document.addEventListener('contextmenu', handler);

	return {
		dispose: () => {
			document.removeEventListener('click', handler);
			document.addEventListener('contextmenu', handler);
		}
	};
};
