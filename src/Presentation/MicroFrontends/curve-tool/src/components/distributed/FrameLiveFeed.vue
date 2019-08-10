<template>
  <dynamic-component :component="comp" :props="props" v-on="$listeners"/>
</template>

<script>
import axios from "axios";
import DynamicComponent from "./DynamicComponent.vue";

export default {
  name: "FrameLiveFeed",
  components: { DynamicComponent },
  props: {
    endpoint: {
      required: true,
      type: String
    }
  },
  data() {
    return {
      props: {},
      comp: "",
    };
  },
  created() {
    // Fetch initial data.
    this.fetch();
  },
  methods: {
    async fetch() {
      const { data } = await axios.get(this.endpoint);
      this.props = data.data;
      this.comp = data.url;
    }
  }
};
</script>
