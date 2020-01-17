<template>
  <component
    :is="computedComponent"
    v-model="simpleValue"
    :options="simpleOptions"
    :label="label"
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
      default: null
    }
  },
  data () {
    const currentValue = this.getInitialValue()

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
    },
    simpleValue: {
      get () {
        return this.currentValue != null && typeof this.options[0] === 'object'
          ? this.currentValue.value
          : this.currentValue
      },
      set (val) {
        const newVal = typeof this.options[0] === 'object'
          ? this.options.find(x => x.value === val).key
          : val

        this.currentValue = newVal
      }
    }
  },
  watch: {
    currentValue: {
      immediate: true,
      handler (newVal, oldVal) {
        if (newVal !== oldVal) {
          this.$emit('input', newVal)
        }
      }
    }
  },
  updated () {
    this.currentValue = this.getInitialValue()
  },
  methods: {
    getInitialValue () {
      const firstOption = typeof this.options[0] === 'object'
        ? this.options[0].key
        : this.options[0]

      return this && this.options && this.options.length === 1
        ? firstOption
        : this.value
    }
  }
}
</script>
