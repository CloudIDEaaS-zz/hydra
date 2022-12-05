import { Component, Prop, h } from '@stencil/core';
import { v4 as uuidv4 } from 'uuid';

@Component({
  tag: 'carousel-component',
  styleUrl: 'carousel.css',
  shadow: true,
})
export class Carousel {

  @Prop({ mutable: true, reflect: true } ) public slidecount: number;
  public set SlideCount(count: number) { this.slidecount = count }
  public get SlideCount() : number { return this.slidecount; }

  @Prop({ mutable: true, reflect: true } ) public containerClassName: string;
  public set ContainerClassName(name: string) { this.containerClassName = name }
  public get ContainerClassName() : string { return this.containerClassName; }

  @Prop({ mutable: true, reflect: true } ) public slideClassName: string;
  public set SlideClassName(name: string) { this.slideClassName = name }
  public get SlideClassName() : string { return this.slideClassName; }

  constructor() {
    this.slidecount = 3;
    this.containerClassName = "carousel";
    this.slideClassName = "carouselSlide";
  }

  getSlides() : Array<string> {

    let slides = [];
    let length = this.slidecount;

    for (var x = 1; x < length + 1; x++) {
      slides.push(x.toString());
    }

    return slides;
  }

  private goBack = () => {
    alert('Received the go back click!');
  }

  private goForward = () => {
    alert('Received the go forward click!');
  }

  render() {
    return  (
      <div id="carouselContainer" class="{ this.containerClassName }, carouselContainer" >
        {this.getSlides().map(s => {
          return (
            <div class="{ this.slideClassName }, carouselSlide">{s}</div>
          );
        })}
        <div class="buttonPanel">
          <a onClick={this.goBack} data-rpc="onclick" data-rpc-id={ uuidv4() } href='#'>Back</a>
          <a onClick={this.goForward}  data-rpc="onclick" data-rpc-id={ uuidv4() } href='#'>Forward</a>
        </div>
      </div>
    );
  }
}
