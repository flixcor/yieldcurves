
const preloadEvents = async ({ eventTypes, $axios }) => {
  const typesString = eventTypes
    ? '?eventTypes=' + eventTypes.join('&eventTypes=')
    : ''

  const url = 'http://localhost:65072' + typesString

  const { data } = await $axios.get(url)
  return data
}

export default ({ app, $axios }, inject) => {
  // Set the function directly on the context.app object
  app.preloadEvents = eventTypes => preloadEvents({ $axios, eventTypes })
}
