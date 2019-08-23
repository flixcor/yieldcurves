<template>
  <md-card class="md-layout-item md-size-50 md-small-size-100">
    <md-card-header>
      <div class="md-title">Create new instrument</div>
    </md-card-header>
    <md-progress-bar v-if="loading" md-mode="indeterminate"></md-progress-bar>
    <md-card-content v-else>
      <mt-select
        id="vendorDropdown"
        v-model="regular.command.vendor"
        label="Vendor"
        :options="regular.vendors"
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

const endpoint = 'https://localhost:5011';

export default {
  components: {
    MtSelect,
  },
  props: ['regular', 'bloomberg'],
  computed: {
    isBloomberg() {
      if (!this.regular) return false;
      return this.regular.command.vendor === 'Bloomberg';
    },
  },
  data() {
    return {
      loading: false,
      errors: [],
    };
  },
  methods: {
    submit() {
      const isBB = () => this.regular.command.vendor === 'Bloomberg';

      const obj = isBB()
        ? this.bloomberg.command
        : this.regular.command;

      const route = isBB() ? '/api/bloomberg' : '/api';

      axios
        .post(endpoint + route, obj)
        .then(() => this.$emit('success'))
        .catch((e) => {
          if (e.response.data && Array.isArray(e.response.data)) this.errors = e.response.data;
        });
    },
  },
};
</script>
