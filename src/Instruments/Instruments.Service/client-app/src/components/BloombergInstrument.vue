<template>
  <div>
    <h5>Bloomberg instrument</h5>

    <mt-select
      id="pricingSourceSelect"
      ref="pricingSourceSelect"
      v-bind:value="dataSource.command.pricingSource"
      label="Pricing Source"
      :options="dataSource.pricingSources"
      v-on:input="emitChange()"
    />

    <mt-select
      id="yellowKeySelect"
      ref="yellowKeySelect"
      v-bind:value="dataSource.command.yellowKey"
      label="Yellow Key"
      :options="dataSource.yellowKeys"
      v-on:input="emitChange()"
    />

    <text-box
      v-bind:value="dataSource.command.ticker"
      id="tickerField"
      ref="tickerField"
      label="Ticker"
      v-on:input="emitChange()"
    />
  </div>
</template>


<script>
import TextBox from './Material/TextBox.vue';
import MtSelect from './Material/MtSelect.vue';

export default {
  components: {
    TextBox,
    MtSelect,
  },
  data() {
    return {
      dataSource: this.value,
    };
  },
  props: ['value'],
  methods: {
    emitChange() {
      this.$emit('input', this.dataSource);
    },
  },
};
</script>
