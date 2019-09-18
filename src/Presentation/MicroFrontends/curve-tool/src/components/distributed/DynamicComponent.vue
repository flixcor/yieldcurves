<template>
  <Component
    :is="computedComponent"
    v-bind="props"
    ref="compRef"
    v-on="$listeners"
  />
</template>

<script>
import externalComponent from '../../utils/external-component';
export default {
  name: `DynamicComponent`,
  props: {
    component: {
      required: true,
      type: String,
    },
    props: {
      default: () => ({}),
      type: Object,
    },
  },
  data() {
    return {
      computedComponent: null,
    };
  },
  watch: {
    component: {
      immediate: true,
      handler(newComponent, prevComponent = ``) {
        if (newComponent === prevComponent) return;
        this.computedComponent = () => externalComponent(this.component);
      },
    },
  },
  methods: {
    insert(e){
      const comp = this.$refs.compRef;
      if (typeof comp.insert === "function") {
        comp.insert(e); 
      }
    },
    update(e){
      const comp = this.$refs.compRef;
      if (typeof comp.update === "function") {
        comp.update(e); 
      }
    }
  },
};
</script>