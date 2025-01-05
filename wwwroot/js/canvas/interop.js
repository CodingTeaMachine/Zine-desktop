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
	rotationDegree: 0,
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
		currentImage = image;

		setDrawSize();

		__drawImage(context, image);
	};
}

// noinspection JSUnusedGlobalSymbols Used ub CanvasHandler.cs
/**
 * @param {"right"|"left"} direction
 */
export function rotate(direction) {

	currentDraw.rotationDegree = currentDraw.rotationDegree +
		(direction === "right" ? 90 : -90);

	if(Math.abs(currentDraw.rotationDegree) === 360) {
		currentDraw.rotationDegree = 0;
	}

	setDrawSize();

	drawImage(context);
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

// noinspection JSUnusedGlobalSymbols Used ub CanvasHandler.cs
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
	const rotation = currentDraw.rotationDegree;
	const drawOrigin = getDrawOrigin(ctx, image);

	ctx.save();

	if(Math.abs(rotation) !== 0) {
		ctx.translate(context.canvas.width / 2, context.canvas.height / 2)
		ctx.rotate(rotation * Math.PI / 180);
	}

	ctx.drawImage(
		image,
		0, 0,
		image.width, image.height,
		drawOrigin.x, drawOrigin.y,
		currentDraw.drawWidth, currentDraw.drawHeight
	);

	ctx.restore();
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

function setDrawSize() {
	const isImageOnItsSide = [90, 270].includes(Math.abs(currentDraw.rotationDegree));

	const hRatio = isImageOnItsSide
		? context.canvas.clientWidth / currentImage.height
		: context.canvas.clientWidth / currentImage.width;

	const vRatio = isImageOnItsSide
		? context.canvas.clientHeight / currentImage.width
		: context.canvas.clientHeight / currentImage.height;

	currentDraw.ratio = Math.min(hRatio, vRatio);

	currentDraw.drawWidth = currentImage.width * currentDraw.ratio;
	currentDraw.drawHeight = currentImage.height * currentDraw.ratio;
}

/**
 * @param {CanvasRenderingContext2D} ctx
 * @param {HTMLImageElement} image
 * @returns {{x: number, y: number}}
 */
function getDrawOrigin(ctx, image) {
	const rotation = Math.abs(currentDraw.rotationDegree);

	switch (rotation) {
		case 0: return ({
			x: (ctx.canvas.width / 2) - (image.width * currentDraw.ratio / 2),
			y: (ctx.canvas.height / 2) - (image.height * currentDraw.ratio / 2),
		})
		case 90:
		case 180:
		case 270: return ({
			x: -currentDraw.drawWidth / 2,
			y: -currentDraw.drawHeight / 2,
		})
	}
}
