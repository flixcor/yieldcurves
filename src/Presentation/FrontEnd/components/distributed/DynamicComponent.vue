<template>
  <component
    :is="computedComponent"
    ref="ref"
    v-bind="props"
    v-on="$listeners"
  />
</template>

<script>
import externalComponent from '../../utils/external-component'
export default {
  name: 'DynamicComponent',
  props: {
    component: {
      required: true,
      type: String
    },
    props: {
      default: () => ({}),
      type: Object
    }
  },
  data () {
    return {
      computedComponent: null
    }
  },
  watch: {
    component: {
      immediate: true,
      handler (newComponent, prevComponent = '') {
        if (newComponent === prevComponent) { return }

        this.computedComponent = () => {
          if (process.server && !this.component.includes('.umd')) {
            return this.$requireFromUrl(`${this.component}.ssr.js`)
          }
          if (!this.component.includes('.umd')) {
            return import(/* webpackIgnore: true */ `${this.component}.esm.js`)
          }
          return externalComponent(this.component)
        }
      }
    }
  },
  methods: {
    passAlong (methodName, data) {
      const method = this.$refs.ref[methodName]
      if (typeof method === 'function') {
        method(data)
      }
    }
  }
}
</script>
