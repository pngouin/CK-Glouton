const webpack = require('webpack');
const path = require('path');
const webpackMerge = require('webpack-merge');
const CommonsChunkPlugin = require('webpack/lib/optimize/CommonsChunkPlugin');
const TsConfigPathsPlugin = require('awesome-typescript-loader').TsConfigPathsPlugin;
const CheckerPlugin = require('awesome-typescript-loader').CheckerPlugin;
const HtmlWebpackPlugin = require('html-webpack-plugin');
const ContextReplacementPlugin = require('webpack/lib/ContextReplacementPlugin');
const NormalModuleReplacementPlugin = require('webpack/lib/NormalModuleReplacementPlugin');
const LoaderOptionsPlugin = require('webpack/lib/LoaderOptionsPlugin');

module.exports = {



  resolve: {
    extensions: ['.ts', '.js', '.json'],
    modules: [path.resolve(__dirname, '../src'), path.resolve(__dirname, '../node_modules')],
    plugins: [
      new TsConfigPathsPlugin(/* { tsconfig, compiler } */)
    ]
  },
  module: {
    rules: [
      // .ts files for TypeScript
      {
        test: /\.ts$/,
        use: [
          { // MAKE SURE TO CHAIN VANILLA JS CODE, I.E. TS COMPILATION OUTPUT.
            loader: 'ng-router-loader',
            options: {
              loader: 'async-import',
              genDir: 'compiled',
            }
          },
          {
            loader: 'awesome-typescript-loader',
            // options: {
            //   configFileName: 'tsconfig.webpack.json'
            // }
          },
          {
            loader: 'angular2-template-loader'
          }
        ],
        exclude: [/\.(spec|e2e)\.ts$/]
      },
      {
        test: /\.css$/,
        use: ['to-string-loader', 'css-loader']
      },
      /*
      * to string and sass loader support for *.scss files (from Angular components)
      * Returns compiled css content as string
      *
      */
      {
        test: /\.scss$/,
        use: ['to-string-loader', 'css-loader', 'sass-loader'],
        exclude: [path.resolve(__dirname, '../src/styles')]
      },
      {
        test: /\.html$/,
        use: 'raw-loader',
        exclude: [path.resolve(__dirname, '../src/index.html')]
      },
      /* 
        * File loader for supporting images, for example, in CSS files.
        */
      {
        test: /\.(jpg|png|gif)$/,
        use: 'file-loader'
      },

      /* File loader for supporting fonts, for example, in CSS files.
      */
      { 
        test: /\.(eot|woff2?|svg|ttf)([\?]?.*)$/,
        use: 'file-loader'
      }
    ]
  },
  plugins: [
    /*
     * Plugin: CheckerPlugin
     * Description: Do type checking in a separate process, so webpack don't need to wait.
     *
     * See: https://github.com/s-panferov/awesome-typescript-loader#forkchecker-boolean-defaultfalse
     */
    new CheckerPlugin(),
    
      /*
       * Plugin: CommonsChunkPlugin
       * Description: Shares common code between the pages.
       * It identifies common modules and put them into a commons chunk.
       *
       * See: https://webpack.github.io/docs/list-of-plugins.html#commonschunkplugin
       * See: https://github.com/webpack/docs/wiki/optimization#multi-page-app
       */
      new CommonsChunkPlugin({
        name: 'polyfills',
        chunks: ['polyfills']
      }),
      // This enables tree shaking of the vendor modules
      new CommonsChunkPlugin({
        name: 'vendor',
        chunks: ['main'],
        minChunks: module => /node_modules/.test(module.resource)
      }),
    new CommonsChunkPlugin({
      name: ['polyfills', 'vendor'].reverse()
    }),
    new ContextReplacementPlugin(
      // The (\\|\/) piece accounts for path separators in *nix and Windows
      /angular(\\|\/)core(\\|\/)@angular/,
      path.resolve(__dirname, '../src'),
      {
        // your Angular Async Route paths relative to this root directory
      }),
    new LoaderOptionsPlugin({}),
    /*
     * Plugin: HtmlWebpackPlugin
     * Description: Simplifies creation of HTML files to serve your webpack bundles.
     * This is especially useful for webpack bundles that include a hash in the filename
     * which changes every compilation.
     *
     * See: https://github.com/ampedandwired/html-webpack-plugin
     */
    new HtmlWebpackPlugin({
      template: 'src/index.html',
      chunksSortMode: 'dependency'
    }),

     // Fix Angular 2
      new NormalModuleReplacementPlugin(
        /facade(\\|\/)async/,
        path.resolve(__dirname, '../node_modules/@angular/core/src/facade/async.js')
      ),
      new NormalModuleReplacementPlugin(
        /facade(\\|\/)collection/,
         path.resolve(__dirname, '../node_modules/@angular/core/src/facade/collection.js')
      ),
      new NormalModuleReplacementPlugin(
        /facade(\\|\/)errors/,
         path.resolve(__dirname, '../node_modules/@angular/core/src/facade/errors.js')
      ),
      new NormalModuleReplacementPlugin(
        /facade(\\|\/)lang/,
         path.resolve(__dirname, '../node_modules/@angular/core/src/facade/lang.js')
      ),
      new NormalModuleReplacementPlugin(
        /facade(\\|\/)math/,
         path.resolve(__dirname, '../node_modules/@angular/core/src/facade/math.js')
      ),
  ],
  node: {
    global: true,
    crypto: 'empty',
    __dirname: true,
    __filename: true,
    process: true,
    Buffer: false,
    clearImmediate: false,
    setImmediate: false
  }
};
