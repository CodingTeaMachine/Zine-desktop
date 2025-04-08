window.reorder = {

	swapDuration: 300,

	/**
	 * 
	 * @param {HTMLElement} element1 
	 * @param {HTMLElement} element2 
	 */
	animateSwapTwoImages: function(element1, element2) {

		element1.style.zIndex = 2;
		element2.style.zIndex = 1;

		this.animateSwap(element1)
				.then(() => this.animateSwap(element2))
				.then(() => {
					element1.style.zIndex = 0;
					element2.style.zIndex = 0;
				});
	},

	/**
	 * @return {Promise}
	 * @param {HTMLElement} element 
	 */
	animateSwap: function (element) {
		return new Promise(resolve => {
			const firstRect = element.getBoundingClientRect();

			requestAnimationFrame(() => {
				const lastRect = element.getBoundingClientRect();
				const dx = firstRect.left - lastRect.left;
				const dy = firstRect.top - lastRect.top;
	
				if (dx !== 0 || dy !== 0) {
					element.style.transform = `translate(${dx}px, ${dy}px)`;
					element.style.transition = 'transform 0s';
	
					requestAnimationFrame(() => {
						element.style.transform = '';
						element.style.transition = `transform ${this.swapDuration}ms ease`;

						setTimeout(() => {
							resolve();
						}, this.swapDuration);
					});
				}else {
					resolve();
				}
			});
		});
	}
};
