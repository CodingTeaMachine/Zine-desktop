import Transform from "./Transform.js";

const ORIGINAL_ZOOM_LEVEL = 100;
const MAX_ZOOM_LEVEL = 300;
const MIN_ZOOM_LEVEL = 50;

export default class PanAndZoom {

	/** @type {CanvasRenderingContext2D} */
	ctx;
	/**
	 * Set from outside in interop.js / setDotnetHelper
	 * @type {DotnetHelper}
	 * */
	dotnetHelper;
	/**
	 * @type {(ctx: CanvasRenderingContext2D) => void}
	 */
	onDraw;
	/** @type {Transform} */
	transform;
	/** @type {boolean} */
	isDragging = false;

	display = {
		zoomScale: ORIGINAL_ZOOM_LEVEL
	}

	dragStart = {
		x: 0,
		y: 0,
	}

	/**
	 * @param {CanvasRenderingContext2D} ctx
	 * @param {(ctx: CanvasRenderingContext2D) => void} drawFunction
	 */
	constructor(ctx, drawFunction) {
		this.ctx = ctx;
		this.onDraw = drawFunction;
		this.transform = new Transform(this.ctx);

		this.registerEventListeners();
	}

	draw() {
		this.onDraw(this.ctx);
	}

	/**
	 * @param {WheelEvent} e
	 */
	onWheel(e) {
		e.preventDefault();
		e.stopPropagation();

		if (!e.ctrlKey)
			return;

		const isZoomIn = Math.sign(e.deltaY) < 0;

		this.zoom(e.offsetX,e.offsetY, isZoomIn);
	}

	/**
	 * @param {MouseEvent} e
	 */
	onMouseDown(e) {
		e.preventDefault();
		e.stopPropagation();

		const offset = this.mouseOffset(e);

		this.dragStart = this.transform.getTransformPoint(offset);
		this.isDragging = true;
	}

	/**
	 * @param {MouseEvent} e
	 */
	onMouseUp(e) {
		e.preventDefault();
		e.stopPropagation();

		this.isDragging = false;
	}

	/**
	 * @param {MouseEvent} e
	 */
	onMouseMove(e) {
		e.preventDefault();
		e.stopPropagation();

		if (!this.isDragging) {
			return;
		}

		const offset = this.mouseOffset(e);
		const dragEnd = this.transform.getTransformPoint(offset);
		const dx = dragEnd.x - this.dragStart.x;
		const dy = dragEnd.y - this.dragStart.y;

		this.transform.translate(dx, dy);
		this.draw();

		this.dragStart = this.transform.getTransformPoint(offset);
	}

	/**
	 * @param {boolean} isZoomIn
	 */
	zoomToCenter(isZoomIn) {
		const centerX = this.ctx.canvas.width / 2;
		const centerY = this.ctx.canvas.height / 2;

		this.zoom(centerX, centerY, isZoomIn);
	}

	/**
	 *
	 * @param {number} x
	 * @param {number} y
	 * @param {boolean} isZoomIn
	 */
	zoom(x, y, isZoomIn) {

		if (
			(isZoomIn && this.display.zoomScale + 10 > MAX_ZOOM_LEVEL) // MAX zoom level reached
			|| (!isZoomIn && this.display.zoomScale - 10 < MIN_ZOOM_LEVEL) //MIN zoom level reached
		) {
			return;
		}


		this.display.zoomScale += isZoomIn ? 10 : -10;

		const transform = this.transform.transformedMousePosition(x, y);
		this.transform.translate(transform.x, transform.y);

		const factor = isZoomIn ? 1.1 : 0.9;
		this.transform.scale(factor);

		this.transform.translate(-transform.x, -transform.y);

		this.draw();

		this.dotnetHelper.invokeMethodAsync("JS_UpdatePageZoomDisplay");
	}

	/**
	 * @param {MouseEvent|WheelEvent} e
	 */
	mouseOffset(e) {
		return {
			x: e.pageX - this.ctx.canvas.offsetLeft,
			y: e.pageY - this.ctx.canvas.offsetTop,
		}
	}

	registerEventListeners() {
		this.ctx.canvas.addEventListener("wheel", e => this.onWheel(e));
		this.ctx.canvas.addEventListener("mousemove", e => this.onMouseMove(e));
		this.ctx.canvas.addEventListener("mousedown", e => this.onMouseDown(e));

		// When the mouse is out of the
		document.addEventListener("mouseup", e => this.onMouseUp(e));
	}

	resetZoom() {
		this.display.zoomScale = ORIGINAL_ZOOM_LEVEL;
	}

}
