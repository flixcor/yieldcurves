<template>
  <md-card class="md-layout-item md-size-50 md-small-size-100">
    <md-card-header>
      <div class="md-title">Create new instrument</div>
    </md-card-header>
    <md-progress-bar v-if="loading" md-mode="indeterminate"></md-progress-bar>
    <md-card-content v-else>
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

import MtSelect from './Material/MtSelect.vue';

const endpoint = 'https://localhost:5013/api';

export default {
  components: {
    MtSelect,
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
