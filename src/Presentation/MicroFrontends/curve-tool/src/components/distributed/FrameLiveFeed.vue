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
  data () {
    return {
      props: {},
      comp: "",
      hub: null
    };
  },
  watch: {
    endpoint: function () {
      this.fetch();
    }
  },
  created () {
    // Fetch initial data.
    this.init();
  },
  methods: {
    async fetch () {
      const { data } = await axios.get(this.endpoint);
      var props = data.data;

      if (Array.isArray(props)) {
        props = { entities: props };
      }

      this.props = props;
      this.comp = data.url;
      this.hub = data.hub;
    },
    async init () {
      await this.fetch();
      if (this.hub && Array.isArray(this.props.entities)) {
        const conn = await getConnection(this.hub);
        conn.on("insert", obj => this.insert(obj));
        conn.on("update", obj => this.insert(obj));
        await conn.start();
      }
    },
    update (obj) {
      const { entities, id } = this.props;
      if (Array.isArray(entities)) {
        this.props.entities = entities.map((x) => {
          if (x.id === obj.id) return obj;
          return x;
        })
      }
      else if (id === obj.id) {
        this.props = obj;
      }
    },
    insert (obj) {
      const { entities } = this.props;
      if (Array.isArray(entities) && obj.id) {

        if (entities.find(x => x.id === obj.id)) return;
        this.props.entities = [obj, ...entities];

      }
    },
  }
};
</script>
