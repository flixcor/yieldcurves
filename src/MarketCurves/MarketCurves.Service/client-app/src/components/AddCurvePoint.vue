<template>
  <md-card>
    <md-card-header>
      <div class="md-title">Add new curve point</div>
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
        type="number"
        max="0"
        v-model="this.command.dateLag"
        label="DateLag"
        id="dateLagBox"
      ></text-box>

      <md-checkbox
        v-model="command.isMandatory"
        >Mandatory
      </md-checkbox>

      <br/>

      <mt-select
        id="priceTypeDropdown"
        v-model="command.priceType"
        label="Price Type"
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
import TextBox from './Material/TextBox.vue';

const endpoint = 'https://localhost:5001/api/curvepoint';

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

      axios.post(endpoint, this.command).catch((e) => {
        if (e.response.data && Array.isArray(e.response.data)) this.errors = e.response.data;
      });
    },
  },
};
</script>
