const webpack = require('webpack');
const path = require('path');
const webpackMerge = require('webpack-merge');
const webpackMergeDll = webpackMerge.strategy({plugins: 'replace'});
const commonConfig = require('./webpack.common.config.js');
const DefinePlugin = require('webpack/lib/DefinePlugin');
const DllBundlesPlugin = require('webpack-dll-bundles-plugin').DllBundlesPlugin;
const AddAssetHtmlPlugin = require('add-asset-html-webpack-plugin');
const ENV = process.env.ENV = process.env.NODE_ENV = 'development';
const HOST = process.env.HOST || 'localhost';
const PORT = process.env.PORT || 3000;

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
  devtool: 'cheap-module-source-map',
  output: {
    path: path.resolve(__dirname, '../../wwwroot'),
    filename: '[name].bundle.js',
    sourceMapFilename: '[file].map',
    chunkFilename: '[id].chunk.js',
    library: 'ac_[name]',
    libraryTarget: 'var',
  },
  plugins: [
    /**
     * DefinePlugin: generates a global object with compile time values.
     */
      new DefinePlugin( {
        'ENV': JSON.stringify(ENV),
        webpack:{enableProdMode:false}
      } ),
      new DllBundlesPlugin({
        bundles: {
          polyfills: [
            'core-js',
            {
              name: 'zone.js',
              path: 'zone.js/dist/zone.js'
            },
            {
              name: 'zone.js',
              path: 'zone.js/dist/long-stack-trace-zone.js'
            },
          ],
          vendor: [
            '@angular/platform-browser',
            '@angular/platform-browser-dynamic',
            '@angular/core',
            '@angular/common',
            '@angular/forms',
            '@angular/http',
            '@angular/router',
            'rxjs',
          ]
        },
        dllDir: path.resolve(__dirname, '../dll'),
        webpackConfig: webpackMergeDll(commonConfig, {
          devtool: 'cheap-module-source-map',
          plugins: []
        })
      }),

      /**
       * Plugin: AddAssetHtmlPlugin
       * Description: Adds the given JS or CSS file to the files
       * Webpack knows about, and put it into the list of assets
       * html-webpack-plugin injects into the generated html.
       *
       * See: https://github.com/SimenB/add-asset-html-webpack-plugin
       */
      new AddAssetHtmlPlugin([
        { filepath: path.resolve(__dirname, `../dll/${DllBundlesPlugin.resolveFile('polyfills')}`) },
        { filepath:  path.resolve(__dirname, `../dll/${DllBundlesPlugin.resolveFile('vendor')}`) }
      ]),
  ],

  devServer: {
    port: PORT,
    host: HOST,
    historyApiFallback: true,
    watchOptions: { aggregateTimeout: 300, poll: 1000 }
  },
  node: {
      global: true,
      crypto: 'empty',
      process: true,
      module: false,
      clearImmediate: false,
      setImmediate: false
    }
};

module.exports = webpackMerge(commonConfig, webpackConfig);
