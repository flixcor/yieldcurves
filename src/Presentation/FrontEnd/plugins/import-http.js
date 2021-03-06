// source: https://markus.oberlehner.net/blog/distributed-vue-applications-loading-components-via-http/
import Vue from 'vue'

const externalComponent = async (url) => {
  const name = url.split('/').reverse()[0].match(/^(.*?)\.umd/)[1]

  if (window[name]) { return window[name] }

  window[name] = await new Promise((resolve, reject) => {
    const script = document.createElement('script')
    script.async = true
    script.addEventListener('load', () => {
      resolve(window[name])
    })
    script.addEventListener('error', () => {
      reject(new Error(`Error loading ${url}`))
    })
    script.src = url
    document.head.appendChild(script)
  })

  return window[name]
}

Vue.prototype.$externalComponent = externalComponent
