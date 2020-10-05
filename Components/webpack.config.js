const path = require('path');

module.exports = {
	entry: {
		site: './resources/styles/index.js',
		currentTime: './Components/CurrentTime/currenttime.js'
	},
	module: {
		rules: [
			{
				test: /\.(sa|sc|c)ss$/,
				use: [
					'style-loader',
					'css-loader',
					{
						loader: 'sass-loader'
					}
				]
			}
		]
	},
	resolve: {
		modules: [path.resolve(__dirname, 'node_modules')]
	},
	devServer: {
		proxy: {
			"*": {
				target: 'http://localhost:5000',
				changeOrigin: true,
			}
		}
	},
	output: {
		filename: '[name].js',
		path: path.resolve(__dirname, './resources/dist/'),
	}
}
