import { newSpecPage } from '@stencil/core/testing';
import { Carousel } from './carousel';

describe('carousel', () => {
  it('renders', async () => {
    const { root } = await newSpecPage({
      components: [Carousel],
      html: '<carousel-component></carousel-component>',
    });
    expect(root).toEqualHtml(`
      <carousel-component>
        <mock:shadow-root>
          <div>
            Hello, World! I'm
          </div>
        </mock:shadow-root>
      </carousel-component
    `);
  });

  it('renders with values', async () => {
    const { root } = await newSpecPage({
      components: [Carousel],
      html: `<carousel first="Stencil" last="'Don't call me a framework' JS"></carousel-component>`,
    });
    expect(root).toEqualHtml(`
      <carousel first="Stencil" last="'Don't call me a framework' JS">
        <mock:shadow-root>
          <div>
            Hello, World! I'm Stencil 'Don't call me a framework' JS
          </div>
        </mock:shadow-root>
      </carousel-component>
    `);
  });
});
