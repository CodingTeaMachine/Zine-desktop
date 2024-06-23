/** @type {import('tailwindcss').Config} */
module.exports = {
	content: ["./**/*.{razor,cshtml,html}"],
	theme: {
		extend: {
			height: {
				'group-card': '150px'
			},
			color: {
				primary: '#776be7' // mud-theme-primary
			}
		},
	},
	plugins: [],
}
