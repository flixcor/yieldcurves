import Vue from 'vue'
const requireFromUrl = require('require-from-url/sync')

Vue.prototype.$externalComponent = url => Promise.resolve(requireFromUrl(url))
