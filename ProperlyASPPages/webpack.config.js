const path = require('path');

module.exports = {
  entry: {
    management: './wwwroot/src/management/index.tsx'
  },
  output: {
    path: path.resolve(__dirname, 'wwwroot/dist'),
    filename: '[name].bundle.js',
    clean: true
  },
  resolve: {
    extensions: ['.ts', '.tsx', '.js', '.jsx']
  },
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        use: 'ts-loader',
        exclude: /node_modules/
      }
    ]
  },
  devtool: 'source-map'
};
