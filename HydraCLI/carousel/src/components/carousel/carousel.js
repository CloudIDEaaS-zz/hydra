var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { Component, Prop, h } from '@stencil/core';
import { v4 as uuidv4 } from 'uuid';
let Carousel = class Carousel {
    constructor() {
        this.goBack = () => {
            alert('Received the go back click!');
        };
        this.goForward = () => {
            alert('Received the go forward click!');
        };
        this.slidecount = 3;
        this.containerClassName = "carousel";
        this.slideClassName = "carouselSlide";
    }
    set SlideCount(count) { this.slidecount = count; }
    get SlideCount() { return this.slidecount; }
    set ContainerClassName(name) { this.containerClassName = name; }
    get ContainerClassName() { return this.containerClassName; }
    set SlideClassName(name) { this.slideClassName = name; }
    get SlideClassName() { return this.slideClassName; }
    getSlides() {
        let slides = [];
        let length = this.slidecount;
        for (var x = 1; x < length + 1; x++) {
            slides.push(x.toString());
        }
        return slides;
    }
    render() {
        return (h("div", { id: "carouselContainer", class: "{ this.containerClassName }, carouselContainer" },
            this.getSlides().map(s => {
                return (h("div", { class: "{ this.slideClassName }, carouselSlide" }, s));
            }),
            h("div", { class: "buttonPanel" },
                h("a", { onClick: this.goBack, "data-rpc": "onclick", "data-rpc-id": uuidv4(), href: '#' }, "Back"),
                h("a", { onClick: this.goForward, "data-rpc": "onclick", "data-rpc-id": uuidv4(), href: '#' }, "Forward"))));
    }
};
__decorate([
    Prop({ mutable: true, reflect: true })
], Carousel.prototype, "slidecount", void 0);
__decorate([
    Prop({ mutable: true, reflect: true })
], Carousel.prototype, "containerClassName", void 0);
__decorate([
    Prop({ mutable: true, reflect: true })
], Carousel.prototype, "slideClassName", void 0);
Carousel = __decorate([
    Component({
        tag: 'carousel-component',
        styleUrl: 'carousel.css',
        shadow: true,
    })
], Carousel);
export { Carousel };
