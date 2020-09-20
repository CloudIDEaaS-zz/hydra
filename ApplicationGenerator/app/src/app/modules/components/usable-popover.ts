import { Component } from "@angular/core";
import { Validators, FormGroup, FormControl, AbstractControl, ValidatorFn } from "@angular/forms";
import { IonicPage, NavController, NavParams, ViewController, ToastController } from "ionic-angular";
import { Observable } from 'rxjs/Rx';
import * as $ from 'jquery';
const Resizable = require("resizable");
const jQueryUtils = require("../utils/jQueryUtils.js");
const queryString = require('query-string');

export abstract class UsablePopover {

    resizable: any;

    ionViewDidLoad() {

        let popover = $(".popover-content");
        let content = popover.find(".scroll-content");
        let debugging = false;
        const queryParms = queryString.parse(location.search);
    
        popover.css("overflow", "hidden");
    
        if (queryParms.debugging == true) {
        
            // debugging
    
            popover.css({"border-color": "red", 
            "border-width":"1px", 
            "border-style":"solid"});
        
            content.css({"border-color": "black", 
            "border-width":"1px", 
            "border-style":"solid"});
        }
        
        this.resizable = new Resizable(popover[0], {
            within: 'document',
            handles: 's, se, e',
            threshold: 10,
            draggable: true
        });

        this.resizable.on('resize', function(){
          content.resizeRelativeTo(popover);
        });    
      }    
}