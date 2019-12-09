<template>
  <ct-card>
    <template v-slot:title>
      <span>Create new instrument</span>
    </template>

    <template v-slot:content>
      <ct-multiple-choice
        v-model="regular.command.vendor"
        label="Vendor"
        :options="regular.vendors"
      />
      <bloomberg-instrument
        v-if="isBloomberg"
        :datasource="bloomberg"
      />
      <regular-instrument
        v-else
        :datasource="regular"
      />
    </template>

    <template v-slot:actions>
      <ct-spacer />
      <ct-btn
        class="primary"
        fab
        @click="submit"
      >
        <v-icon>mdi-send</v-icon>
      </ct-btn>
    </template>
  </ct-card>
</template>

<script>
const endpoint = "https://localhost:5011";

export default {
  components: {
    BloombergInstrument: () =>
      import("../CreateBloombergInstrument/Component.vue"),
    RegularInstrument: () => import("../CreateRegularInstrument/Component.vue")
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
  data() {
    return {
      loading: false,
      errors: []
    };
  },
  computed: {
    isBloomberg() {
      return this.regular.command.vendor === "Bloomberg";
    }
  },
  methods: {
    submit() {
      var isBB = this.isBloomberg;

      const obj = isBB ? this.bloomberg.command : this.regular.command;

      const route = isBB
        ? "/features/create-bloomberg-instrument/"
        : "/features/create-regular-instrument";

      this.$axios
        .$post(endpoint + route, obj)
        .then(() => this.$emit("success"))
        .catch(e => {
          if (e.response.data && Array.isArray(e.response.data))
            this.errors = e.response.data;
        });
    }
  }
};
</script>
