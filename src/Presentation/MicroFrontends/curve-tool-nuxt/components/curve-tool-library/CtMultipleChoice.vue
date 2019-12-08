<template>
  <component
    :is="computedComponent"
    v-bind="{options: simpleOptions, label, value: currentValue}"
    @input="onInput"
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
  data () {
    const currentValue = this.options && this.options.length === 1
      ? this.options[0]
      : this.value

    return {
      currentValue
    }
  },
  computed: {
    computedComponent () {
      const optionsCount = this.simpleOptions.length
      const totalLength = this.simpleOptions.map(x => x.toString()).join('').length
      const lengthArray = this.simpleOptions.map(x => x.length)
      const maxLength = Math.max(...lengthArray)

      if (optionsCount < 10 && totalLength < 100 && maxLength < 20) {
        return () => import('./CtToggleButtons.vue')
      }

      return () => import('./CtSelect.vue')
    },
    simpleOptions () {
      if (this.options && typeof this.options[0] === 'object') {
        return this.options.map(x => x.value)
      }

      return this.options
    }
  },
  updated () {
    this.currentValue = this.options && this.options.length === 1
      ? this.options[0]
      : this.value
  },
  methods: {
    onInput (e) {
      const val = this.options && typeof this.options[0] === 'object'
        ? this.options.find(x => x.value === e).key
        : e

      this.$emit('input', val)
    }
  }
}
</script>
