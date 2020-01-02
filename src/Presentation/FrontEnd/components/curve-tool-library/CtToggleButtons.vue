<template>
  <div>
    <p>{{ label }}</p>
    <v-btn-toggle v-model="toggle" mandatory>
      <v-btn v-for="(option, index) in options" :key="index" class="tertiary">
        {{ option }}
      </v-btn>
    </v-btn-toggle>
  </div>
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
    const toggle =
      this.options && this.value ? this.options.indexOf(this.value) : 0
    return {
      toggle
    }
  },
  computed: {
    currentValue () {
      return this.options[this.toggle]
    }
  },
  watch: {
    toggle () {
      this.$emit('input', this.currentValue)
    }
  },
  created () {}
}
</script>
