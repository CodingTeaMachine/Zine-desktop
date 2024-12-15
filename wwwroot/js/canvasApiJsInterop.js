
const globalState = {
	canvasId: null,
	context: null,
	imgSrc: null,
	scale: 1
};

const ZOOM_SCALE = 0.1;
const MAX_ZOOM_LEVEL = 3.0;
const MIN_ZOOM_LEVEL = 0.1;


export function initCanvas(canvasId) {
	globalState.canvasId = canvasId;
}


export function drawImage(imageSrc) {
	globalState.imgSrc = imageSrc;
	const ctx = getContextByCanvasId(globalState.canvasId);

	redrawImage(ctx, imageSrc);

	window.addEventListener('resize', resizeWindowEventListener);
}

export function zoomInImage() {

	if(globalState.scale >= MAX_ZOOM_LEVEL) {
		return;
	}

	const ctx = getContextByCanvasId(globalState.canvasId);
	globalState.scale += ZOOM_SCALE;
	redrawImage(ctx);
}

export function zoomOutImage() {

	if(globalState.scale - ZOOM_SCALE <= MIN_ZOOM_LEVEL) {
		return;
	}

	const ctx = getContextByCanvasId(globalState.canvasId);
	globalState.scale -= ZOOM_SCALE;
	redrawImage(ctx);
}

export function dispose() {
	window.removeEventListener('resize', resizeWindowEventListener);
}

/**
 * @returns {number}
 */
export function getZoomScale() {
	return globalState.scale;
}


/**
 *
 * @param {CanvasRenderingContext2D} ctx
 * @param {HTMLImageElement} image
 */
function drawImageActualSize( ctx, image) {
	const canvas = ctx.canvas;
	const canvasContainer = canvas.parentElement;

	canvas.width = canvasContainer.clientWidth;
	canvas.height = canvasContainer.clientHeight;

	const imgAspectRatio = image.width / image.height;
	const canvasAspectRatio = canvas.width / canvas.height;

	let drawWidth;
	let drawHeight;

	if(imgAspectRatio > canvasAspectRatio) {
		drawWidth = canvas.width;
		drawHeight = canvas.width / imgAspectRatio;
	} else {
		drawHeight = canvas.height;
		drawWidth = canvas.height * imgAspectRatio;
	}

	drawWidth = drawWidth * globalState.scale;
	drawHeight = drawHeight * globalState.scale;

	//Center the image on the canvas
	const offsetX = (canvas.width - drawWidth) / 2;
	const offsetY = (canvas.height - drawHeight) / 2;

	//Clear the previous img so no artifacts are left behind
	ctx.clearRect(0, 0, canvas.width, canvas.height);

	//Draw the img
	ctx.drawImage(image, offsetX, offsetY, drawWidth, drawHeight);
}

function redrawImage(ctx) {
	const image = new Image();
	image.src = globalState.imgSrc;

	image.onload = () => drawImageActualSize(ctx, image);
}

function resizeWindowEventListener(event) {
	const ctx = getContextByCanvasId(globalState.canvasId);
	const imageSrc = globalState.imgSrc;

	redrawImage(ctx, imageSrc);
}


/**
 *
 * @param {string} canvasId
 * @returns {CanvasRenderingContext2D}
 */
function getContextByCanvasId(canvasId) {
	const canvas = getCanvas(canvasId);
	return canvas.getContext('2d');
}


/**
 *
 * @param {string|null} canvasId
 * @throws Error
 * @returns {HTMLCanvasElement}
 */
function getCanvas(canvasId) {

	if(canvasId === null) {
		throw new Error('Canvas not initialized');
	}

	const canvas = document.getElementById(canvasId);

	if(canvas === null) {
		throw `Could not find canvas #${canvasId}`
	}

	return canvas;
}
