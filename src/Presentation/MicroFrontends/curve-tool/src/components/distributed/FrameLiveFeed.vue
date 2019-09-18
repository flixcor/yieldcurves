<template>
  <dynamic-component ref="compRef" :component="comp" :props="props" v-on="$listeners" />
</template>

<script>
import axios from "axios";
import DynamicComponent from "./DynamicComponent.vue";
import getConnection from "../../utils/websockets.js";

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
      hub: null
    };
  },
  created() {
    // Fetch initial data.
    this.init();
  },
  watch: {
    endpoint: function() {
      this.fetch();
    }
  },
  methods: {
    async fetch() {
      const { data } = await axios.get(this.endpoint);
      this.props = data.data;
      this.comp = data.url;
      this.hub = data.hub;
    },
    async init(){
      await this.fetch();
      if (this.hub) {
        const conn = await getConnection(this.hub);
        conn.on("insert", e => this.$refs.compRef.insert(e));
        conn.on("update", e => this.$refs.compRef.update(e));
        await conn.start();
      }
    },
  }
};
</script>
