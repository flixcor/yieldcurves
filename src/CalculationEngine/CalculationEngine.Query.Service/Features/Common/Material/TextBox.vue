<template>
  <md-field>
    <label :for="id">{{ label }}</label>
    <md-input
      :id="id"
      :value="value"
      :type="type"
      :min="min"
      :max="max"
      :step="step"
      @input="$emit('input', $event)"
      @blur="$emit('blur', $event)"
      @change="$emit('change', $event)"
    />
  </md-field>
</template>

<script>
export default {
  props: ['label', 'id', 'value', 'type', 'min', 'max', 'step'],
};
</script>
