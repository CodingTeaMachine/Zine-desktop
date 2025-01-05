import PanAndZoom from "./PanAndZoom.js";

/** @type {CanvasRenderingContext2D} */
let context;
/** @type {PanAndZoom} */
let panAndZoom;
/** @type {string} */
let currentImageSrc;
/** @type {HTMLImageElement} */
let currentImage;

const currentDraw = {
	ratio: 0,
	drawWidth: 0,
	drawHeight: 0,
}

// For some reason this only works here, not in the init function
window.addEventListener("resize", setCanvasSize);

// noinspection JSUnusedGlobalSymbols Used ub CanvasHandler.cs
/**
 * @param {string} canvasId
 */
export function init(canvasId) {
	const canvas = document.getElementById(canvasId);
	context = canvas.getContext("2d");
	panAndZoom = new PanAndZoom(context, drawImage);

	setCanvasSize()
}

// noinspection JSUnusedGlobalSymbols Used ub CanvasHandler.cs
/**
 * @param {string} imageSrc
 */
export function showComicPage(imageSrc) {
	resetZoom();
	clearCanvas(context);

	currentImageSrc = imageSrc;

	const image = new Image();
	image.src = imageSrc;

	image.onload = () => {
		const hRatio = context.canvas.clientWidth / image.width;
		const vRatio = context.canvas.clientHeight / image.height;
		currentDraw.ratio = Math.min(hRatio, vRatio);

		currentDraw.drawWidth = image.width * currentDraw.ratio;
		currentDraw.drawHeight = image.height * currentDraw.ratio;

		currentImage = image;

		__drawImage(context, image);
	};
}

// noinspection JSUnusedGlobalSymbols Used ub CanvasHandler.cs
/**
 * @param {boolean} isZoomIn
 */
export function zoom(isZoomIn) {
	panAndZoom.zoomToCenter(isZoomIn);
}


// noinspection JSUnusedGlobalSymbols Used ub CanvasHandler.cs
export function getZoomScale() {
	return panAndZoom.display.zoomScale;
}

// noinspection JSUnusedGlobalSymbols Used ub CanvasHandler.cs
/** @param {DotnetHelper} dotnetHelper */
export function setDotnetHelper(dotnetHelper) {
	panAndZoom.dotnetHelper = dotnetHelper;
}

export function dispose() {
	window.removeEventListener("resize", setCanvasSize);
}


/**
 * @param {CanvasRenderingContext2D} ctx
 */
function drawImage(ctx) {
	clearCanvas(ctx);

	__drawImage(ctx, currentImage);
}

/**
 * @param {CanvasRenderingContext2D} ctx
 */
function clearCanvas(ctx) {
	ctx.save();
	ctx.setTransform(1, 0, 0, 1, 0, 0);
	ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
	ctx.restore();
}

function __drawImage(ctx, image) {
	ctx.drawImage(
		image,
		0, 0,
		image.width, image.height,
		(ctx.canvas.width / 2) - (image.width * currentDraw.ratio / 2), 0,
		currentDraw.drawWidth, currentDraw.drawHeight);
}

function resetZoom() {
	panAndZoom.resetZoom();
	context.setTransform(1, 0, 0, 1, 0, 0);
}

function setCanvasSize() {

	if(context === null)
		return;

	const canvasContainer = context.canvas.parentElement;
	context.canvas.width = canvasContainer.clientWidth;
	context.canvas.height = canvasContainer.clientHeight;

	clearCanvas(context);
	showComicPage(currentImageSrc)
}
