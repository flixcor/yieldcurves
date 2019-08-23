import Vue from 'vue';
import wrap from '@vue/web-component-wrapper';
import CreateInstrument from './components/CreateInstrument.vue';

const WrappedElement = wrap(Vue, CreateInstrument);

window.customElements.define('my-web-component', WrappedElement);
