import { } from 'jasmine';
import { async, TestBed } from '@angular/core/testing';
import { IonicModule } from 'ionic-angular';
import { StatusBar } from '@ionic-native/status-bar';
import { SplashScreen } from '@ionic-native/splash-screen';
import { AppModule } from '../../../app/app.module';
import { customJasmineMatchers } from '../../utils/CustomJasmineMatchers';
import { BindingsParser, TreeView, BindingTransform } from './';

const compiler = require("@angular/compiler");
const jsonpath : Function = require("JSONPath");
const app = AppModule.GetApp();

describe('Component: TestTree Component', () => {
  let fixture;
  let component;

  beforeEach(async(() => {

    TestBed.configureTestingModule({
      declarations: [
        app,
        TreeView
      ],
      imports: [
        IonicModule.forRoot(app)
      ],
      providers: [
        StatusBar,
        SplashScreen
      ]
    })
  }));

  beforeEach(() => {

    try {

      jasmine.addMatchers(customJasmineMatchers);

      fixture = TestBed.createComponent(app);
      component = fixture.componentInstance;

    } catch (error) {
      console.log(error);
    }

  });

  it('should parse bindings', () => {

    let bindingsParser = new BindingsParser(null);
    let result = bindingsParser.parse("childNodesMember", "systemDate | async | date :'short'");

    expect(result.propertyName).toEqual("systemDate");
    expect(result.bindingTransforms.length).toBe(2);
    expect(result.bindingTransforms).all((t : BindingTransform) => {

      switch (t.pipeName) {
        case "async": {
            return true;
          }
        case "date": {

            if (t.args.length == 1) {

              if (t.args[0] == "short") {
                return true;
              }
              else {
                return false;
              }

            }
            else {
              return false;
            }

          }
        default: {
          return false;
        }
      }
    });

  });

  it('should handle JSONPath expressions', () => {

    let node = {
      title: 'Users', icon: 'contacts',
      sub: [
        {
          title: 'Jon', icon: 'contact', sub: [
            { title: 'Email', icon: 'mail' },
            { title: 'Street', icon: 'home' },
            { title: 'Phone', icon: 'call' }
          ]
        },
        { title: 'Sergio', icon: 'contact' },
        {
          title: 'Winnie', icon: 'contact', sub: [
            { title: 'Street', icon: 'home' },
            {
              title: 'Email', icon: 'mail', sub: [
                { title: 'sss@hotmail.com', icon: 'at' },
                { title: 'sss@gmail.com', icon: 'at' },
                { title: 'aaa@yahoo.com', icon: 'at' }
              ]
            }
          ]
        }
      ]
    };

    let nodes = {
      nodes: node
    };

    let expression = "$[?(@.title == 'Users')]"
    let result = jsonpath({json: nodes, path: expression});

    expect(result.length).toBe(1);
    expect(result[0].title).toBe("Users");

  });
});
