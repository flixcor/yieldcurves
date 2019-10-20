<template>
  <md-card class="md-layout-item md-size-50 md-small-size-100">
    <md-card-header>
      <div class="md-title">
        Create new instrument
      </div>
    </md-card-header>
    <md-progress-bar
      v-if="loading"
      md-mode="indeterminate"
    />
    <md-card-content v-else>
      <mt-select
        id="vendorDropdown"
        v-model="regular.command.vendor"
        label="Vendor"
        :options="regular.vendors"
      />
      <bloomberg-instrument
        v-if="isBloomberg"
        v-model="bloomberg"
      />
      <regular-instrument
        v-else
        v-model="regular"
      />
      <ul v-if="errors.length">
        <li
          v-for="error in errors"
          :key="error"
        >
          {{ error }}
        </li>
      </ul>
      <md-button
        class="md-raised md-primary"
        @click="submit"
      >
        Submit
      </md-button>
    </md-card-content>
  </md-card>
</template>

<script>
import axios from 'axios';

import MtSelect from '../Common/Material/MtSelect.vue';

const endpoint = 'https://localhost:5011';

export default {
  components: {
    BloombergInstrument: () => import('../CreateBloombergInstrument/Component.vue'),
    RegularInstrument: () => import('../CreateRegularInstrument/Component.vue'),
    MtSelect,
  },
  props: ['regular', 'bloomberg'],
  data() {
    return {
      loading: false,
      errors: [],
    };
  },
  computed: {
    isBloomberg() {
      if (!this.regular) return false;
      return this.regular.command.vendor === 'Bloomberg';
    },
  },
  methods: {
    submit() {
      const isBB = () => this.regular.command.vendor === 'Bloomberg';

      const obj = isBB()
        ? this.bloomberg.command
        : this.regular.command;

      const route = isBB() ? '/features/create-bloomberg-instrument/' : '/features/create-regular-instrument';

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
