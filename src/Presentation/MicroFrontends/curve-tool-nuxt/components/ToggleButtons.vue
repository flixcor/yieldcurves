<template>
  <div>
    <p v-html="label" />
    <v-btn-toggle v-model="toggle" mandatory>

      <v-btn v-for="(option, index) in options" :key="index">
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
      type: String,
      required: true
    }
  },
  data () {
    return {
      toggle: 0
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
  }
}
</script>
