// vue.config.js
module.exports = {
  outputDir: './wwwroot/',
  filenameHashing: false,
  css: {
    loaderOptions: {
      sass: {
        includePaths: ['./node_modules', './node_modules/@material'],
      },
    },
  },
};
