/** @type {import('tailwindcss').Config} */
module.exports = {
	content: [
		"./**/*.{razor,cshtml,html,css,razor.css}",
		"./wwwroot/**/*.css"
	],
	theme: {
		extend: {
			height: {
				'group-card': '150px'
			},
			colors: {
				primary: '#776be7', // mud-theme-primary
				light: '#383843', // mud-theme-light
				dark: '#27272f', //mud-theme-dark
			}
		},
	},
	plugins: [],
}
