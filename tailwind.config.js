/** @type {import('tailwindcss').Config} */
module.exports = {
	content: ["./**/*.{razor,cshtml,html,css}"],
	theme: {
		extend: {
			height: {
				'group-card': '150px'
			},
			colors: {
				primary: '#776be7' // mud-theme-primary
			}
		},
	},
	plugins: [],
}
