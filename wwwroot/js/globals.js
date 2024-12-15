/**
 * @typedef {{Direction: number, Modifier: number}} ScrollEvent
 */


window.JsFunctions = {

	/**
	 *  Call the HandleClickOutside function on the razor page, when a user either left or right clicks outside the element with elementId
	 *
	 * @param elementId
	 * @param dotNetHelper
	 * @returns {{dispose: *}}
	 */
	registerClickOutsideHandler: (elementId, dotNetHelper) => {
		const element = document.getElementById(elementId);
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
				document.removeEventListener('contextmenu', handler);
			}
		}
	},

	registerKeyDownEventListener: (dotNetHelper) => {
		let serializeEvent = function (e) {
			if (e) {
				return {
					key: e.key,
					code: e.keyCode.toString(),
					location: e.location,
					repeat: e.repeat,
					ctrlKey: e.ctrlKey,
					shiftKey: e.shiftKey,
					altKey: e.altKey,
					metaKey: e.metaKey,
					type: e.type
				};
			}
		};

		window.document.addEventListener('keydown', (e) =>
			dotNetHelper.invokeMethodAsync('JsOnKeyDown', serializeEvent(e))
		);

		return {
			dispose: () => {
				document.removeEventListener('keydown', serializeEvent);
			}
		}
	},

	registerOnScrollListener: (elementId, dotNetHelper, isAsyncHandler = false) => {
		const element = document.getElementById(elementId);
		if (!element) {
			console.error(`Could not find element with id: ${elementId}`)
			return;
		}

		/**
		 * @param {WheelEvent} event
		 */
		const invokeScrollMethod = (event) => {

			/** @type ScrollEvent */
			const eventSettings = {
				Direction: event.deltaY > 0 ? scrollDirection.Down : scrollDirection.Up,
				Modifier: getModifier(event)
			}

			const handlerName = isAsyncHandler
				? 'ElementScrolledAsync'
				: 'ElementScrolled';

			dotNetHelper.invokeMethodAsync(handlerName, eventSettings);
		}

		element.addEventListener("wheel", invokeScrollMethod);

		return {
			dispose: () => {
				element.removeEventListener('wheel', invokeScrollMethod);
			}
		}
	}
};

const scrollDirection = {
	Down: 0,
	Up: 1,
}

const keyModifier = {
	Ctrl: 0,
	Alt: 1,
	Shift: 2,
	None: 3
}

/**
 * @param {WheelEvent} event
 */
function getModifier(event) {
	if(event.altKey) return keyModifier.Alt;
	else if(event.shiftKey) return keyModifier.Shift;
	else if(event.ctrlKey) return keyModifier.Ctrl;
	else return keyModifier.None;
}
