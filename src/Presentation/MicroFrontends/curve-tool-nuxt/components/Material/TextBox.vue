<template>
  <md-field>
    <label :for="id">{{ label }}</label>
    <md-input
      :id="id"
      :value="value"
      @input="emitChange($event)"
    />
  </md-field>
</template>

<script>
export default {
  props: ['label', 'id', 'value'],
  methods: {
    emitChange (e) {
      this.$emit('input', e)
    }
  }
}
</script>
