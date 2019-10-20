<template>
  <md-card>
    <md-card-header>
      <div class="md-title">
        Add new curve point
      </div>
    </md-card-header>
    <md-card-content>
      <mt-select
        id="tenorDropdown"
        v-model="command.tenor"
        label="Tenor"
        :options="tenors"
      />
      <mt-select
        id="instrumentDropdown"
        v-model="command.instrumentId"
        label="Instrument"
        :options="instruments.map(x=> x.id)"
      />
      <text-box
        id="dateLagBox"
        v-model="command.dateLag"
        type="number"
        max="0"
        label="DateLag"
      />

      <md-checkbox
        v-model="command.isMandatory"
      >
        Mandatory
      </md-checkbox>

      <br>

      <mt-select
        v-if="hasPriceType"
        id="priceTypeDropdown"
        v-model="command.priceType"
        label="Price Type"
        :options="priceTypes"
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
import TextBox from '../Common/Material/TextBox.vue';

const endpoint = 'https://localhost:5001/features/add-curve-point';

export default {
  components: {
    MtSelect,
    TextBox,
  },
  props: ['command', 'priceTypes', 'instruments', 'tenors'],
  data() {
    return {
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

      axios.post(endpoint, this.command)
        .then(() => this.$emit('success'))
        .catch((e) => {
          if (e.response.data && Array.isArray(e.response.data)) this.errors = e.response.data;
        });
    },
  },
};
</script>
