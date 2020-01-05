<template>
  <component
    :is="computedComponent"
    ref="ref"
    v-bind="props"
    v-on="$listeners"
  />
</template>

<script>
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
        this.computedComponent = () => this.$externalComponent(this.component)
      }
    }
  }
}
</script>
