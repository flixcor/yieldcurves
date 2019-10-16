<template>
  <dynamic-component
    :component="comp"
    :props="props"
    v-on="$listeners"
  />
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
  watch: {
    endpoint: function() {
      this.fetch();
    }
  },
  created() {
    // Fetch initial data.
    this.init();
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
      if (this.hub && Array.isArray(this.props.entities)) {
        const conn = await getConnection(this.hub);
        conn.on("insert", e => this.insert(e));
        conn.on("update", e => this.update(e));
        await conn.start();
      }
    },
    update(e){
      this.props.entities = this.props.entities.map((x) => {
        if (x.id === e.id) return e;
        return x;
      });
    },
    insert(e){
      if (this.props.entities.find(x => x.id === e.id)) return;
      this.props.entities = [e, ...this.props.entities];
    },
  }
};
</script>
