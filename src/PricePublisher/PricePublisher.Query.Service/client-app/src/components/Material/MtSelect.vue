<template>
  <div class="md-layout-item">
    <md-field>
      <label :for="id">{{label}}</label>
      <md-select
        ref="dropRef"
        v-on:md-selected="$emit('input', $event)"
        v-model="internalValue"
        :name="id"
        :id="id"
      >
        <md-option v-for="opt in options" :value="opt" :key="opt">{{opt}}</md-option>
      </md-select>
    </md-field>
  </div>
</template>

<script>
export default {
  data() {
    return {
      internalValue: this.value,
    };
  },
  name: 'mt-select',
  props: ['label', 'id', 'options', 'value'],
};
</script>
