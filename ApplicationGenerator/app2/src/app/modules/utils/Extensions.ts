declare global {

  export interface Object {
    isOneOf(...values) : boolean;
  }

  export interface Number {
    isOneOf(...values) : boolean;
  }
}

let isOneOf = function(...values) : boolean {

  let equals : boolean;

  values.forEach(v => {
    if (this == v) {
      equals = true;
    }
  });

  return equals;
}

// Object.prototype.isOneOf = isOneOf;
// Number.prototype.isOneOf = isOneOf;

export {};
