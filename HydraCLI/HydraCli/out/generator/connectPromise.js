"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ConnectPromise = void 0;
class ConnectPromise {
    resolve(value) {
        this.value = value;
        if (this.successFunction) {
            this.successFunction(value);
        }
    }
    reject(error) {
        this.error = error;
        if (this.failFunction) {
            this.failFunction(error);
        }
    }
    then(onfulfilled, onrejected = null) {
        if (this.value) {
            onfulfilled(this.value);
            return;
        }
        if (this.error) {
            onrejected(this.error);
            return;
        }
        this.successFunction = onfulfilled;
        this.failFunction = onrejected;
        return this;
    }
    catch(onrejected) {
        this.failFunction = onrejected;
        return this;
    }
}
exports.ConnectPromise = ConnectPromise;
//# sourceMappingURL=connectPromise.js.map