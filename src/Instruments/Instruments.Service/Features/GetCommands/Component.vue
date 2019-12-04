<template>
  <v-card class="md-layout-item md-size-50 md-small-size-100">
    <v-card-title>
      <span>
        Create new instrument
      </span>
    </v-card-title>
    <v-card-text>
      <p>Vendor</p>
      <v-btn-toggle
        v-model="toggle"
        mandatory
      >
        <v-btn
          v-for="(vendor, index) in regular.vendors"
          :key="index"
        >
          {{ vendor }}
        </v-btn>
      </v-btn-toggle>
      <bloomberg-instrument
        v-if="isBloomberg"
        :datasource="bloomberg"
      />
      <regular-instrument
        v-else
        :datasource="regular"
      />
      <ul v-if="errors.length">
        <li
          v-for="error in errors"
          :key="error"
        >
          {{ error }}
        </li>
      </ul>
    </v-card-text>
    <v-card-actions>
      <v-spacer />
      <v-btn
        fab
        @click="submit"
      >
        Submit
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
import axios from 'axios';
const endpoint = 'https://localhost:5011';

export default {
  components: {
    BloombergInstrument: () => import('../CreateBloombergInstrument/Component.vue'),
    RegularInstrument: () => import('../CreateRegularInstrument/Component.vue')
  },
  props: {
    regular: {
      type: Object,
      required: true
    },
    bloomberg: {
      type: Object,
      required: true
    }
  },
  data () {
    return {
      loading: false,
      errors: [],
      toggle: 0
    };
  },
  computed: {
    isBloomberg () {
      return this.currentVendor === 'Bloomberg';
    },
    currentVendor () {
      return this.regular && this.regular.vendors[this.toggle]
    }
  },
  watch: {
    toggle () {
      this.regular.command.vendor = this.currentVendor
    }
  },
  methods: {
    submit () {
      var isBB = this.isBloomberg

      const obj = isBB
        ? this.bloomberg.command
        : this.regular.command

      const route = isBB
        ? '/features/create-bloomberg-instrument/'
        : '/features/create-regular-instrument'

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
