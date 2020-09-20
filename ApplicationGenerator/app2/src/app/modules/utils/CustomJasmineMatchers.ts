import { } from 'jasmine';

const customJasmineMatchers : jasmine.CustomMatcherFactories = {

  any : (util : jasmine.MatchersUtil , customEqualityTesters : Array<jasmine.CustomEqualityTester>) => {
    return {
      compare: (actual : any, expected: any) : jasmine.CustomMatcherResult => {
        
        let result : any = {};
        let callback : (T) => boolean = expected;
        let array : any[] = actual;
        let pass: boolean = false;
        let failedItem: any;

        array.forEach(i => {

          if (callback(i)) {
            pass = true;
          }

        });

        result.pass = pass;

        if (pass) {
          result.message = "array passed as expected";
        }
        else {
          result.message = "no items passed expectation";
        }

        return result;
      }
    }
  },
  all : (util : jasmine.MatchersUtil , customEqualityTesters : Array<jasmine.CustomEqualityTester>) => {
    return {
      compare: (actual : any, expected: any) : jasmine.CustomMatcherResult => {

        let result : any = {};
        let callback : (T) => boolean = expected;
        let array : any[] = actual;
        let pass: boolean = true;
        let failedItem: object;

        array.forEach(i => {

          if (!callback(i)) {

            if (!failedItem) {
              failedItem = i;
              pass = false;
            }
          }

        });

        result.pass = pass;

        if (pass) {
          result.message = "array passed as expected";
        }
        else {
          result.message = "expectation failed with item: " + failedItem;
        }

        return result;
      }
    }
  }


}

export { customJasmineMatchers };
