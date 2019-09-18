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
      this.$refs.compRef.insert(e);
    },
    update(e){
      this.$refs.compRef.update(e);
    }
  },
};
</script>