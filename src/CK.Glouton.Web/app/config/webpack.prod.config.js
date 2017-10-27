const webpack = require('webpack');
const path = require('path');
const webpackMerge = require('webpack-merge');
const commonConfig = require('./webpack.common.config.js');
const WebpackMd5Hash = require('webpack-md5-hash');
const UglifyJsPlugin = require('webpack/lib/optimize/UglifyJsPlugin');
const DefinePlugin = require('webpack/lib/DefinePlugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const ENV = process.env.NODE_ENV = process.env.ENV = 'production';
// Webpack Config
const webpackConfig = {
  entry: {
    'polyfills': './src/polyfills.browser.ts',
    'main': './src/main.browser.ts',
  },
   /**
     * Developer tool to enhance debugging
     *
     * See: http://webpack.github.io/docs/configuration.html#devtool
     * See: https://github.com/webpack/docs/wiki/build-performance#sourcemaps
     */
  devtool: 'source-map',

  output: {
      path: path.resolve(__dirname, '../../wwwroot'),
      filename: '[name].[chunkhash].bundle.js',
      sourceMapFilename: '[name].[chunkhash].map',
      chunkFilename: '[id].[chunkhash].chunk.js'
  },
  module: {
    rules: [
     
     /**
      * Extract CSS files from .src/styles directory to external CSS file
      */
      {
        test: /\.css$/,
        loader: ExtractTextPlugin.extract({
          fallback: 'style-loader',
          use: 'css-loader'
        }),
        include: [path.resolve(__dirname, '../src', 'styles')]
      },

       /**
        * Extract and compile SCSS files from .src/styles directory to external CSS file
        */
      {
        test: /\.scss$/,
        loader: ExtractTextPlugin.extract({
          fallback: 'style-loader',
          use: 'css-loader!sass-loader'
        }),
        include: [path.resolve(__dirname, '../src', 'styles')]
      }
    ]
  },
  plugins: [
    /**
     * DefinePlugin: generates a global object with compile time values.
     */
      new DefinePlugin( {
        'ENV': JSON.stringify(ENV),
        webpack:{enableProdMode:true}
      } ),
     /**
       * Plugin: WebpackMd5Hash
       * Description: Plugin to replace a standard webpack chunkhash with md5.
       *
       * See: https://www.npmjs.com/package/webpack-md5-hash
       */
      new WebpackMd5Hash(),
    /**
     * Plugin: UglifyJsPlugin
     * Description: Minimize all JavaScript output of chunks.
     * Loaders are switched into minimizing mode.
     *
     * See: https://webpack.github.io/docs/list-of-plugins.html#uglifyjsplugin
     */
    // NOTE: To debug prod builds uncomment //debug lines and comment //prod lines
     new UglifyJsPlugin({
        beautify: false, //prod
        output: {
          comments: false
        }, //prod
        mangle: {
          screw_ie8: true
        }, //prod
        compress: {
          screw_ie8: true,
          warnings: false,
          conditionals: true,
          unused: true,
          comparisons: true,
          sequences: true,
          dead_code: true,
          evaluate: true,
          if_return: true,
          join_vars: true,
          negate_iife: false // we need this for lazy v8
        },
      }),
  ],
};

module.exports = webpackMerge(commonConfig, webpackConfig);
