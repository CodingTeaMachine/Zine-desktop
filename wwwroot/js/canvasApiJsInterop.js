/**
 * @type {{
 * 		canvasId: null|string,
 * 		ctx: null|CanvasRenderingContext2D,
 * 		img: null|HTMLImageElement,
 * 		translatedMousePosition: null|{x: number, y: number, z:number},
 * 		scale: number,
 * 		isDragging: boolean,
 * 		dragStartPosition: {x: number, y: number}
 * 		}}
 */
const globalState = {
	canvasId: null,
	img: null,
	translatedMousePosition: null,

	//Zooming
	scale: 1,

	//Panning
	isDragging: false,
	dragStartPosition: { x: 0, y: 0 },
	previousX: 0,
	previousY: 0,
	viewportTransform: {
		x: 0,
		y: 0,
	}
};

/**
 * @type {CanvasRenderingContext2D}
 */
let ctx = null;

const ZOOM_SCALE = 0.1;
const MAX_ZOOM_LEVEL = 3.5;
const MIN_ZOOM_LEVEL = 0.1;


export function initCanvas(canvasId) {
	globalState.canvasId = canvasId;

	if (ctx === null) {
		ctx = getCanvas(globalState.canvasId).getContext('2d');
	}

	window.addEventListener('resize', resizeWindowEventListener);

	ctx.canvas.addEventListener('mousedown', mouseDownListener);

	ctx.canvas.addEventListener('mouseup', mouseUpListener);

	ctx.canvas.addEventListener('mousemove', mouseMoveListener);
}


/**
 *
 * @param {string} imageSrc
 */
export function drawImage(imageSrc) {
	globalState.scale = 1.0;

	const image = new Image();
	image.src = imageSrc;
	image.onload = () => drawImageActualSize(image);

	globalState.img = image;
}

/**
 * @param {boolean} zoomByMouse
 */
export function zoomIn(zoomByMouse = false) {

	if (globalState.scale >= MAX_ZOOM_LEVEL) {
		return;
	}

	globalState.scale += ZOOM_SCALE;

	drawImageActualSize(
		globalState.img,
		() => setZoom(zoomByMouse));
}

/**
 * @param {boolean} zoomByMouse
 */
export function zoomOut(zoomByMouse = false) {

	if (globalState.scale - ZOOM_SCALE <= MIN_ZOOM_LEVEL) {
		return;
	}

	globalState.scale -= ZOOM_SCALE;

	drawImageActualSize(
		globalState.img,
		() => setZoom(zoomByMouse));
}

function setZoom(scaleImageByMouse) {
	let zoomX;
	let zoomY;

	if (!scaleImageByMouse) {
		zoomX = ctx.canvas.width / 2;
		zoomY = ctx.canvas.height / 2;
	} else {
		zoomX = globalState.translatedMousePosition.x;
		zoomY = globalState.translatedMousePosition.y;
	}


	ctx.translate(zoomX, zoomY);
	ctx.scale(globalState.scale, globalState.scale);
	ctx.translate(-zoomX, -zoomY);

	ctx.fillStyle = "orange"
}

/**
 * @returns {number}
 */
export function getZoomScale() {
	return globalState.scale;
}

export function dispose() {
	window.removeEventListener('resize', resizeWindowEventListener);
}

/**
 * @param {HTMLImageElement} image
 * @param {(() => void)|null} transform
 */
function drawImageActualSize(image, transform = null) {
	const canvas = ctx.canvas;
	const canvasContainer = canvas.parentElement;

	canvas.width = canvasContainer.clientWidth;
	canvas.height = canvasContainer.clientHeight;

	const hRatio = canvas.width / image.width;
	const vRatio = canvas.height / image.height;
	const ratio = Math.min(hRatio, vRatio);

	const drawWidth = image.width * ratio;
	const drawHeight = image.height * ratio;


	if (transform !== null) {
		transform();
	}

	// Clear the previous img so no artifacts are left behind
	//ctx.save();
	ctx.setTransform(1, 0, 0, 1, 0, 0);
	ctx.clearRect(0, 0, canvas.width, canvas.height);
	ctx.setTransform(globalState.scale, 0, 0, globalState.scale, globalState.viewportTransform.x, globalState.viewportTransform.y);
	//ctx.restore();


	//Draw the img
	ctx.drawImage(
		image,
		0, 0,
		image.width, image.height,
		(canvas.width / 2) - (image.width * ratio / 2), 0,
		drawWidth, drawHeight);
}

function resizeWindowEventListener() {
	drawImageActualSize(globalState.img);
}

function mouseMoveListener(event) {
	//For zooming to the mouse position
	if (!globalState.isDragging) {
		globalState.translatedMousePosition = getTransformedCursorPosition(event.offsetX, event.offsetY);
		return;
	}

	const localX = event.clientX;
	const localY = event.clientY;

	globalState.viewportTransform.x = localX - globalState.previousX;
	globalState.viewportTransform.y = localY - globalState.previousY;

	globalState.previousX = localX;
	globalState.previousY = localY;

	drawImageActualSize(globalState.img);
}

function mouseDownListener(event) {
	globalState.previousX = event.clientX;
	globalState.previousY = event.clientY;
	globalState.isDragging = true;
}

function mouseUpListener() {
	globalState.isDragging = false;
}

function getTransformedCursorPosition(x, y) {
	const cursorLocationInRealSpace = new DOMPoint(x, y);
	return ctx.getTransform().invertSelf().transformPoint(cursorLocationInRealSpace);
}


/**
 *
 * @param {string|null} canvasId
 * @throws Error
 * @returns {HTMLCanvasElement}
 */
function getCanvas(canvasId) {

	if (canvasId === null) {
		throw new Error('Canvas not initialized');
	}

	const canvas = document.getElementById(canvasId);

	if (canvas === null) {
		throw `Could not find canvas #${canvasId}`
	}

	return canvas;
}
