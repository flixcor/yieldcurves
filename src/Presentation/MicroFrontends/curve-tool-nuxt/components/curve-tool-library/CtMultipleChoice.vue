<template>
  <component
    :is="computedComponent"
    v-bind="{options, label, value}"
    v-on="$listeners"
  />
</template>

<script>
export default {
  props: {
    options: {
      type: Array,
      required: true
    },
    label: {
      type: String,
      required: true
    },
    value: {
      type: [String, Number, Object],
      required: true
    }
  },
  computed: {
    computedComponent () {
      const optionsCount = this.options.length
      const totalLength = this.options.map(x => x.toString()).join('').length
      const lengthArray = this.options.map(x => x.length)
      const maxLength = Math.max(...lengthArray)

      if (optionsCount < 10 && totalLength < 100 && maxLength < 20) {
        return () => import('./CtToggleButtons.vue')
      }

      return () => import('./CtSelect.vue')
    }
  }
}
</script>
