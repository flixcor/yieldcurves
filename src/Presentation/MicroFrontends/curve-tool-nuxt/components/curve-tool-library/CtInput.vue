<template>
  <v-text-field
    :value="value"
    :label="label"
    :min="min"
    :max="max"
    :type="type"
    v-on="$listeners"
  />
</template>

<script>
export default {
  props: {
    label: {
      type: String,
      required: true
    },
    value: {
      type: [String, Number, Object],
      default: null
    },
    min: {
      type: String,
      default: null
    },
    max: {
      type: String,
      default: null
    },
    type: {
      type: String,
      default: null
    }
  }
}
</script>
