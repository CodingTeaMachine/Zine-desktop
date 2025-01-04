export default class Transform {

	/** @type {CanvasRenderingContext2D} */
	ctx;
	/** @type {number} */
	s = 1;
	/** @type {number} */
	dx = 0;
	/** @type {number} */
	dy = 0;

	/**
	 * @param {CanvasRenderingContext2D} ctx
	 */
	constructor(ctx) {
		this.ctx = ctx;
	}

	scale(scale) {
		this.ctx.scale(scale, scale);

		this.s *= 1 / scale;
		this.dx *= 1 / scale;
		this.dy *= 1 / scale;
	}

	translate(dx, dy) {
		this.ctx.translate(dx, dy);

		this.dx -= dx;
		this.dy -= dy;
	}

	transform({ x, y }) {
		return {
			x: this.s * x + this.dx,
			y: this.s * y + this.dy,
		}
	}

	transformedMousePosition(offsetX, offsetY) {
		const cursorLocationInRealSpace = new DOMPoint(offsetX, offsetY);
		const { x, y } = this.ctx.getTransform().invertSelf().transformPoint(cursorLocationInRealSpace);
		return { x, y }
	}
}
