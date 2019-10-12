<template>
  <md-card class="md-layout-item md-size-50 md-small-size-100">
    <md-card-header>
      <div class="md-title">Publish a price</div>
    </md-card-header>
    <md-progress-bar v-if="loading" md-mode="indeterminate"></md-progress-bar>
    <md-card-content v-else>
      <md-datepicker id="asOfDatePicker" v-model="asOfDate" md-immediately />
      <mt-select
        id="instrumentDropdown"
        v-model="command.instrumentId"
        label="Instrument"
        :options="instruments.map(x=> x.id)"
      />
      <mt-select
        id="priceTypeDropdown"
        v-model="command.priceType"
        label="Price type"
        :options="priceTypes"
        v-if="hasPriceType"
      />
      <text-box
        id="priceCurrencyBox"
        v-model="command.priceCurrency"
        label="Price currency (ISO3)"
      />
      <text-box
        id="priceAmountBox"
        v-model="command.priceAmount"
        label="Price amount"
      />
      <ul v-if="errors.length">
        <li v-for="error in errors" :key="error">
          {{error}}
        </li>
      </ul>
      <md-button v-on:click="this.submit" class="md-raised md-primary">Submit</md-button>
    </md-card-content>
  </md-card>
</template>

<script>
import axios from 'axios';

import MtSelect from '../Common/Material/MtSelect.vue';
import TextBox from '../Common/Material/TextBox.vue';

const endpoint = 'https://localhost:5013/api';

export default {
  components: {
    MtSelect,
    TextBox,
  },
  props: ['command', 'priceTypes', 'instruments'],
  data() {
    return {
      loading: false,
      errors: [],
    };
  },
  computed: {
    hasPriceType() {
      const match = this.instruments
        .find(x => x.id === this.command.instrumentId);

      const hasPriceType = match && match.hasPriceType;

      return hasPriceType;
    },
    asOfDate: {
      get() {
        return new Date(this.command.asOfDate).toISOString().split('T')[0];
      },
      set(newVal) {
        this.command.asOfDate = new Date(newVal).toJSON();
      },
    },
  },
  methods: {
    submit() {
      if (!this.hasPriceType) {
        this.command.priceType = null;
      }

      axios
        .post(endpoint, this.command)
        .then(() => this.$emit('success'))
        .catch((e) => {
          if (e.response.data && Array.isArray(e.response.data)) this.errors = e.response.data;
        });
    },
  },
};
</script>
