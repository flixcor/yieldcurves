<template>
  <div class="md-layout-item">
    <md-field>
      <label :for="id">{{ label }}</label>
      <md-select
        :id="id"
        ref="dropRef"
        v-model="internalValue"
        :name="id"
        @md-selected="$emit('input', $event)"
      >
        <md-option
          v-for="opt in options"
          :key="opt"
          :value="opt"
        >
          {{ opt }}
        </md-option>
      </md-select>
    </md-field>
  </div>
</template>

<script>
export default {
  name: 'MtSelect',
  props: ['label', 'id', 'options', 'value'],
  data() {
    return {
      internalValue: this.value,
    };
  },
};
</script>
