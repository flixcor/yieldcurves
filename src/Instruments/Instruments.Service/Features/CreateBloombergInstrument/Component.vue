<template>
  <div>
    <h5>Bloomberg instrument</h5>

    <mt-select
      id="pricingSourceSelect"
      v-model="datasource.command.pricingSource"
      label="Pricing Source"
      :options="datasource.pricingSources"
      v-on:input="emitChange()"
    />

    <mt-select
      id="yellowKeySelect"
      v-model="datasource.command.yellowKey"
      label="Yellow Key"
      :options="datasource.yellowKeys"
      v-on:input="emitChange()"
    />

    <text-box
      v-model="datasource.command.ticker"
      id="tickerField"
      label="Ticker"
      v-on:input="emitChange()"
    />
  </div>
</template>


<script>
import TextBox from '../Common/Material/TextBox.vue';
import MtSelect from '../Common/Material/MtSelect.vue';

export default {
  components: {
    TextBox,
    MtSelect,
  },
  data() {
    return {
      datasource: this.value,
    };
  },
  props: ['value'],
  methods: {
    emitChange() {
      this.$emit('input', this.datasource);
    },
  },
};
</script>
