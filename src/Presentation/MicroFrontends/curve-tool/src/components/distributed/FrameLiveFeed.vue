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
        conn.on("insert", (propertyName, obj) => this.insert(obj, propertyName));
        conn.on("update", (propertyName, obj) => this.update(obj, propertyName));
        await conn.start();
      }
    },
    update (obj, propertyName = "entities") {
      const prop = this.props[propertyName];
      if (prop && obj && Array.isArray(prop) && obj.id) {

        this.props[propertyName] = prop.map((x) => {
          if (x.id === obj.id) return obj;
          return x;

        });
      }
    },
    insert (obj, propertyName = "entities") {
      const prop = this.props[propertyName];
      if (prop && obj && Array.isArray(prop) && obj.id) {

        if (prop.find(x => x.id === obj.id)) return;
        this.props[propertyName] = [obj, ...prop];

      }
    },

  }
};
</script>
