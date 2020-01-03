import Vue from 'vue'
const requireFromUrl = require('require-from-url/sync')

Vue.prototype.$requireFromUrl = url => Promise.resolve(requireFromUrl(url))
