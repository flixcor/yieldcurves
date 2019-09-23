<template>
  <md-field>
    <label :for="id">{{label}}</label>
    <md-input :id="id"
      v-bind:value="value"
      v-on:input="$emit('input', $event)"
      v-on:blur="$emit('blur', $event)"
      v-on:change="$emit('change', $event)"
      :type="type"
      :min="min"
      :max="max"
      :step="step"
    ></md-input>
  </md-field>
</template>

<script>
export default {
  props: ['label', 'id', 'value', 'type', 'min', 'max', 'step'],
};
</script>
